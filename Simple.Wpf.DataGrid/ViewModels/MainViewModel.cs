// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Data;
using Simple.Wpf.DataGrid.Collections;
using Simple.Wpf.DataGrid.Commands;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.ViewModels
{
    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        private const string GridName = "Example.Grid";
        private readonly ListCollectionView _collectionView;
        private readonly IColumnsService _columnsService;
        private readonly CustomTypeRangeObservableCollection<DynamicDataViewModel> _data;
        private readonly Dictionary<object, DynamicDataViewModel> _dataIds;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOverlayService _overlayService;
        private readonly ISchedulerService _schedulerService;
        private readonly SerialDisposable _suspendNotifications;
        private readonly ITabularDataService _tabularDataService;

        private readonly Dictionary<long, int> _updateStats;
        private IDisposable _dataStream;

        private string _filter;
        private int _updatesPerSecond;

        public MainViewModel(IDiagnosticsViewModel diagnosticsViewModel,
            ITabularDataService tabularDataService,
            IColumnsService columnsService,
            IOverlayService overlayService,
            IDateTimeService dateTimeService,
            ISchedulerService schedulerService)
        {
            _tabularDataService = tabularDataService;
            _schedulerService = schedulerService;
            _columnsService = columnsService;
            _overlayService = overlayService;
            _dateTimeService = dateTimeService;
            Diagnostics = diagnosticsViewModel;

            _dataIds = new Dictionary<object, DynamicDataViewModel>(1000);
            _data = new CustomTypeRangeObservableCollection<DynamicDataViewModel>();
            _collectionView = new ListCollectionView(_data) {Filter = FilterData};

            _updateStats = new Dictionary<long, int>();

            RefreshCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            ClearCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            ColumnPickerCommand = ReactiveCommand.Create(HasDataChanged)
                .DisposeWith(this);

            _dataStream = InitialiseAndProcessDataAsync()
                .DisposeWith(this);

            RefreshCommand.Subscribe(x => Refresh())
                .DisposeWith(this);

            ClearCommand.Subscribe(x => Clear())
                .DisposeWith(this);

            ColumnPickerCommand.Subscribe(x => ShowColumnPicker())
                .DisposeWith(this);

            _suspendNotifications = new SerialDisposable()
                .DisposeWith(this);

            FilterChanged.Subscribe(x => _collectionView.Refresh())
                .DisposeWith(this);

            ColumnsChanged.ObserveOn(schedulerService.Dispatcher)
                .Subscribe(x => OnPropertyChanged(nameof(VisibleColumns)))
                .DisposeWith(this);

            Disposable.Create(() => Clear())
                .DisposeWith(this);
        }

        public ReactiveCommand<object> RefreshCommand { get; }

        public ReactiveCommand<object> ClearCommand { get; }

        public ReactiveCommand<object> ColumnPickerCommand { get; }

        public IEnumerable Data => _collectionView;

        public bool HasData => _dataIds.Any();

        public IEnumerable<string> VisibleColumns => _columnsService.VisibleColumns(GridName);

        public int TotalNumberOfRows => _dataIds.Count;

        public int TotalNumberOfColumns => _dataIds.Any() ? _dataIds.First().Value.GetProperties().Count : 0;

        public int TotalNumberOfValues => TotalNumberOfRows * TotalNumberOfColumns;

        public int UpdatesPerSecond
        {
            get => _updatesPerSecond;
            private set => SetProperty(ref _updatesPerSecond, value);
        }

        public IEnumerable<string> AvailableCultures => CultureService.AvailableCultures;

        public string SelectedCulture
        {
            get =>
                CultureService.CultureChanged
                    .Take(1)
                    .Wait();

            set
            {
                var currentCulture = CultureService.CultureChanged
                    .Take(1)
                    .Wait();

                if (!string.Equals(currentCulture, value)) CultureService.SetCulture(value);
            }
        }

        public bool SuspendUpdates
        {
            set
            {
                if (_suspendNotifications.Disposable == null && !value) return;

                Debug.WriteLine(value ? "Updates: Suspended" : "Updates: Resumed");

                var suspendedNotifications = _data.Select(x => x.SuspendNotifications());
                _suspendNotifications.Disposable = value ? new CompositeDisposable(suspendedNotifications) : null;
            }
        }

        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }

        private IObservable<string> FilterChanged =>
            this.ObservePropertyChanged(nameof(Filter)).Select(x => Filter)
                .DistinctUntilChanged()
                .Select(x => x?.ToLower())
                .Throttle(Constants.UI.Grids.FilterThrottle, _schedulerService.Dispatcher)
                .ActivateGestures();

        private IObservable<Unit> ColumnsChanged =>
            _columnsService.Changed.Where(x => x == GridName).AsUnit();

        private IObservable<bool> HasDataChanged =>
            _data.ObserveCollectionChanged().Select(x => _data.Any());

        public IDiagnosticsViewModel Diagnostics { get; }

        private IObservable<DataSet> InitialiseDataAsync()
        {
            return _tabularDataService.GetAsync()
                .Select(x => Convert(x))
                .Catch<DataSet, Exception>(e =>
                {
                    Logger.Error(e, "Failed to get any data!");
                    return InitialiseDataAsync();
                });
        }

        private IDisposable InitialiseAndProcessDataAsync()
        {
            return InitialiseDataAsync()
                .Publish()
                .RefCount()
                .ObserveOn(_schedulerService.Dispatcher)
                .Subscribe(x => UpdatesPerSecond = Process(x));
        }

        private void Refresh()
        {
            Clear();

            _dataStream = InitialiseAndProcessDataAsync()
                .DisposeWith(this);
        }

        private void Clear()
        {
            _dataStream.Dispose();

            var data = _data.ToArray();

            _dataIds.Clear();
            _data.Clear();

            _updateStats.Clear();

            // perf hack...
            DynamicDataViewModel.Reset();

            _columnsService.InitialiseColumns(GridName, Enumerable.Empty<string>());

            DisposeOfAsync(data, _schedulerService.TaskPool);

            OnPropertyChanged(nameof(HasData));
            OnPropertyChanged(nameof(VisibleColumns));
            OnPropertyChanged(nameof(TotalNumberOfRows));
            OnPropertyChanged(nameof(TotalNumberOfValues));
            OnPropertyChanged(nameof(UpdatesPerSecond));
        }

        private DataSet Convert(IEnumerable<DynamicData> data)
        {
            var array = data.ToArray();

            var updates = new List<DynamicData>(1000);
            var @new = new List<DynamicDataViewModel>(1000);

            // Avoiding LINQ here to improve performance - this method is called frequently when there are updates from the backend

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in array)
                if (_dataIds.ContainsKey(item.Id))
                    updates.Add(item);
                else
                    @new.Add(new DynamicDataViewModel(item, _dateTimeService));

            return new DataSet(@new.ToArray(), updates.ToArray());
        }

        private int Process(DataSet data)
        {
            if (data.New.Length != 0)
                using (Duration.Measure(Logger, "Process - New"))
                {
                    data.New.ForEach(x => { _dataIds[x.Id] = x; });
                    _data.AddRange(data.New);

                    _columnsService.InitialiseColumns(GridName, data.ColumnNames);

                    OnPropertyChanged(nameof(HasData));
                    OnPropertyChanged(nameof(TotalNumberOfRows));
                    OnPropertyChanged(nameof(TotalNumberOfValues));
                }

            // Avoiding LINQ here to improve performance - this method is called frequently when there are updates from the backend

            // ReSharper disable once ForCanBeConvertedToForeach
            foreach (var update in data.Updates)
            {
                var viewModel = _dataIds[update.Id];
                viewModel.Update(update);
            }

            return CalculateUpdateRates(data.Updates);
        }

        private int CalculateUpdateRates(IReadOnlyCollection<DynamicData> updates)
        {
            var now = _dateTimeService.Now;
            var previous = now.AddSeconds(-1);

            _updateStats[now.Ticks] = updates.Count;

            var oldItems = _updateStats.Keys
                .Where(x => x < previous.Ticks)
                .ToArray();

            foreach (var item in oldItems) _updateStats.Remove(item);

            return System.Convert.ToInt32(Math.Round(_updateStats.Sum(x => x.Value) / 100d, 0) * 100);
        }

        private bool FilterData(object obj)
        {
            if (string.IsNullOrEmpty(_filter)) return true;

            if (!(obj is DynamicDataViewModel data)) return false;

            var propertyDescriptors = data.GetProperties()
                .OfType<PropertyDescriptor>()
                .ToArray();

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < propertyDescriptors.Length; i++)
            {
                var propertyDescriptor = propertyDescriptors[i];
                var valueAsString = data.GetValueAsString(propertyDescriptor.Name);
                return valueAsString.IndexOf(_filter, StringComparison.InvariantCultureIgnoreCase) != -1;
            }

            return false;
        }

        private void ShowColumnPicker()
        {
            var viewModel = new ColumnPickerViewModel(GridName, _columnsService, _schedulerService);
            _overlayService.Post("Column Picker", viewModel, viewModel);
        }

        private sealed class DataSet
        {
            public DataSet(DynamicDataViewModel[] @new, DynamicData[] updates)
            {
                Updates = updates;
                New = @new;

                if (@new.Any())
                    ColumnNames = @new
                        .First()
                        .GetProperties()
                        .Cast<PropertyDescriptor>()
                        .Select(x => x.Name)
                        .ToArray();
            }

            public DynamicData[] Updates { get; }

            public DynamicDataViewModel[] New { get; }

            public string[] ColumnNames { get; }
        }
    }
}