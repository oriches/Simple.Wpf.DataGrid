// ReSharper disable ConvertClosureToMethodGroup
namespace Simple.Wpf.DataGrid.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows.Data;
    using Collections;
    using Commands;
    using Extensions;
    using Models;
    using Services;

    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        private const string GridName = "Example.Grid";

        private sealed class DataSet
        {
            public DynamicData[] Updates { get; }

            public DynamicDataViewModel[] New { get; }

            public KeyValuePair<object, DynamicDataViewModel>[] NewAsKeyValuePairs { get; }

            public string[] ColumnNames { get; }

            public DataSet(DynamicDataViewModel[] @new, DynamicData[] updates)
            {
                Updates = updates;
                New = @new;
                
                if (@new.Any())
                {
                    NewAsKeyValuePairs = @new.Select(x => new KeyValuePair<object, DynamicDataViewModel>(x.Id, x))
                        .ToArray();

                    ColumnNames = @new
                        .First()
                        .GetProperties()
                        .Cast<PropertyDescriptor>()
                        .Select(x => x.Name)
                        .ToArray();
                }
            }
        }

        private readonly ITabularDataService _tabularDataService;
        private readonly ISchedulerService _schedulerService;
        private readonly IColumnsService _columnsService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ListCollectionView _collectionView;
        private readonly Dictionary<object, DynamicDataViewModel> _dataIds;
        private readonly CustomTypeRangeObservableCollection<DynamicDataViewModel> _data;
        private readonly SerialDisposable _dataDisposable;
        private readonly object _dataSync = new object();
        private readonly SerialDisposable _suspendNotifications;
        private readonly Subject<Unit> _filterRefresh;
        private readonly Dictionary<long, int> _updateStats;
        private readonly Subject<int> _updates;

        private string _filter;
        private string _filterLowercase;
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
            _dateTimeService = dateTimeService;
            Diagnostics = diagnosticsViewModel;

            _dataIds = new Dictionary<object, DynamicDataViewModel>(1000);
            _data = new CustomTypeRangeObservableCollection<DynamicDataViewModel>();
            _collectionView = new ListCollectionView(_data) { Filter = FilterData };
            
            _updateStats = new Dictionary<long, int>();

            _updates = new Subject<int>()
                .DisposeWith(this);

            var refreshEnabled = new BehaviorSubject<bool>(true)
                .DisposeWith(this);

            _filterRefresh = new Subject<Unit>()
                .DisposeWith(this);

            _dataDisposable = new SerialDisposable()
                .DisposeWith(this);

            RefreshCommand = ReactiveCommand.Create(refreshEnabled.DistinctUntilChanged())
               .DisposeWith(this);

            ClearCommand = ReactiveCommand.Create(refreshEnabled.DistinctUntilChanged())
               .DisposeWith(this);

            ColumnPickerCommand = ReactiveCommand.Create(_data.ObserveCollectionChanged().Select(x => _data.Any()))
                .DisposeWith(this);
            
            RefreshCommand.ActivateGestures()
                 .StartWith(Constants.StartsWith.Unit.DefaultBoxed)
                 .ResilentSubscribe(_ =>
                 {
                     refreshEnabled.OnNext(false);
                     DisposeOfData();
                     
                     _dataDisposable.Disposable = _tabularDataService.GetAsync(schedulerService.TaskPool)
                        .Select(x => ConvertToDataSet(x))
                        .ObserveOn(schedulerService.Dispatcher)
                        .ResilentSubscribe(x =>
                        {
                            ProcessDataSet(x);
                            refreshEnabled.OnNext(true);
                        }, schedulerService.Dispatcher);
                 }, schedulerService.Dispatcher)
                 .DisposeWith(this);
           
            ClearCommand.ActivateGestures()
                .ResilentSubscribe(x => DisposeOfData(), schedulerService.Dispatcher)
                .DisposeWith(this);
            
            ColumnPickerCommand.ActivateGestures()
                .ResilentSubscribe(x =>
                {
                    var viewModel = new ColumnPickerViewModel(GridName, _columnsService, schedulerService);
                    overlayService.Post("Column Picker", viewModel, viewModel);
                }, schedulerService.Dispatcher)
                .DisposeWith(this);

            Disposable.Create(() => DisposeOfData())
                .DisposeWith(this);

            _suspendNotifications = new SerialDisposable()
                .DisposeWith(this);

            _filterRefresh.Throttle(Constants.UI.Grids.FilterThrottle, schedulerService.TaskPool)
                .ObserveOn(schedulerService.Dispatcher)
                .ActivateGestures()
                .Subscribe(x =>
                {
                    using(Duration.Measure(Logger, "Refresh"))
                        _collectionView.Refresh();
                })
                .DisposeWith(this);

            columnsService.Changed
                .Where(x => x == GridName)
                .ObserveOn(schedulerService.Dispatcher)
                .ResilentSubscribe(x => OnPropertyChanged(() => VisibleColumns), schedulerService.Dispatcher)
                .DisposeWith(this);

            _updates.Buffer(Constants.UI.Grids.UpdatesInfoThrottle, schedulerService.TaskPool)
                .Where(x => x.Any())
                .Select(x => x.Last())
                .ObserveOn(schedulerService.Dispatcher)
                .Subscribe(x => UpdatesPerSecond = x)
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
            get { return _updatesPerSecond; }
            private set { SetPropertyAndNotify(ref _updatesPerSecond, value, () => UpdatesPerSecond); }
        }

        public IDiagnosticsViewModel Diagnostics { get; }

        public IEnumerable<string> AvailableCultures => CultureService.AvailableCultures;

        public string SelectedCulture
        {
            get
            {
                return CultureService.CultureChanged
                    .Take(1)
                    .Wait();
            }

            set
            {
                var currentCulture = CultureService.CultureChanged
                    .Take(1)
                    .Wait();

                if (!string.Equals(currentCulture, value))
                {
                    CultureService.SetCulture(value);
                }
            }
        }

        public bool SuspendUpdates
        {
            set
            {
                if (_suspendNotifications.Disposable == null && !value)
                {
                    return;
                }

                Debug.WriteLine(value ? "Updates: Suspended" : "Updates: Resumed");
                IEnumerable<IDisposable> suspendedNotifications;

                lock (_dataSync)
                    suspendedNotifications = _data.Select(x => x.SuspendNotifications());

                _suspendNotifications.Disposable = value ? new CompositeDisposable(suspendedNotifications) : null;
            }
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                if (SetPropertyAndNotify(ref _filter, value, "Filter"))
                {
                    _filterLowercase = _filter.ToLower();
                    _filterRefresh.OnNext(Unit.Default);
                }
            }
        }

        private void DisposeOfData()
        {
            _dataDisposable.Disposable = Disposable.Empty;

            DynamicDataViewModel[] data;
            lock (_dataSync)
            {
                data = _data.ToArray();

                _dataIds.Clear();
                _data.Clear();

                _updateStats.Clear();
            }

            // perf hack...
            DynamicDataViewModel.Reset();
            
            _columnsService.InitialiseColumns(GridName, Enumerable.Empty<string>());

            DisposeOfAsync(data, _schedulerService.TaskPool);
            
            OnPropertyChanged("HasData");
            OnPropertyChanged("VisibleColumns");
            OnPropertyChanged("TotalNumberOfRows");
            OnPropertyChanged("TotalNumberOfValues");
            OnPropertyChanged("UpdatesPerSecond");
        }
        
        private DataSet ConvertToDataSet(IEnumerable<DynamicData> data)
        {
            var array = data.ToArray();

            var updates = new List<DynamicData>(1000);
            var @new = new List<DynamicDataViewModel>(1000);

            // Avoiding LINQ here to improve performance - this method is called frequently when there are updates from the backend

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in array)
            {
                if (_dataIds.ContainsKey(item.Id))
                {
                    updates.Add(item);
                }
                else
                {
                    @new.Add(new DynamicDataViewModel(item, _dateTimeService));
                }
            }
                
            return new DataSet(@new.ToArray(), updates.ToArray());
        }

        private void ProcessDataSet(DataSet data)
        {
            if (data.New.Length != 0)
            {
               using (Duration.Measure(Logger, "ProcessDataSet - New"))
                {
                    lock (_dataSync)
                    {
                        _dataIds.AddRange(data.NewAsKeyValuePairs);
                        _data.AddRange(data.New);
                    }

                    _columnsService.InitialiseColumns(GridName, data.ColumnNames);

                    OnPropertyChanged("HasData");
                    OnPropertyChanged("TotalNumberOfRows");
                    OnPropertyChanged("TotalNumberOfValues");
                }
            }

            // Avoiding LINQ here to improve performance - this method is called frequently when there are updates from the backend

            // ReSharper disable once ForCanBeConvertedToForeach
            foreach(var update in data.Updates)
            {
                var viewModel = _dataIds[update.Id];
                viewModel.ProcessUpdate(update);
            }

            CalculateUpdateRates(data.Updates);
        }

        private void CalculateUpdateRates(IReadOnlyCollection<DynamicData> updates)
        {
            var now = _dateTimeService.Now;
            var previous = now.AddSeconds(-1);

            _updateStats[now.Ticks] = updates.Count;

            var oldItems = _updateStats.Keys
                .Where(x => x < previous.Ticks)
                .ToArray();

            foreach (var item in oldItems)
            {
                _updateStats.Remove(item);
            }

            _updates.OnNext((int) (Math.Round(_updateStats.Sum(x => x.Value) / 100d, 0) * 100));
        }

        private bool FilterData(object obj)
        {
            if (string.IsNullOrEmpty(_filter))
            {
                return true;
            }

            var data = obj as DynamicDataViewModel;
            if (data != null)
            {
                var propertyDescriptors = data.GetProperties()
                    .OfType<PropertyDescriptor>()
                    .ToArray();

                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < propertyDescriptors.Length; i++)
                {
                    var propertyDescriptor = propertyDescriptors[i];
                    if (data.GetValueAsString(propertyDescriptor.Name).Contains(_filterLowercase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}