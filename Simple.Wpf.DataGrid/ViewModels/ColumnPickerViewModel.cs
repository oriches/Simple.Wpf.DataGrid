// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Simple.Wpf.DataGrid.Collections;
using Simple.Wpf.DataGrid.Commands;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Helpers;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.ViewModels
{
    public sealed class ColumnPickerViewModel : BaseViewModel, IColumnPickerViewModel
    {
        private readonly IColumnsService _columnsService;

        private readonly string _identifier;
        private readonly RangeObservableCollection<ColumnPickerItemViewModel> _left;
        private readonly RangeObservableCollection<ColumnPickerItemViewModel> _right;
        private readonly ISchedulerService _schedulerService;

        public ColumnPickerViewModel(string identifier, IColumnsService columnsService,
            ISchedulerService schedulerService)
        {
            _identifier = identifier;
            _columnsService = columnsService;
            _schedulerService = schedulerService;

            _left = new RangeObservableCollection<ColumnPickerItemViewModel>();
            _right = new RangeObservableCollection<ColumnPickerItemViewModel>();

            _columnsService.Initialised
                .ObserveOn(schedulerService.TaskPool)
                .StartWith(identifier)
                .Where(x => x == identifier)
                .Select(x =>
                {
                    var visible = columnsService.VisibleColumns(identifier)
                        .Select(y => new ColumnPickerItemViewModel(y, ColumnHelper.DisplayName(y)))
                        .ToArray();

                    var hidden = columnsService.HiddenColumns(identifier)
                        .Select(y => new ColumnPickerItemViewModel(y, ColumnHelper.DisplayName(y)))
                        .ToArray();

                    return new VisibleColumns(visible, hidden);
                })
                .ObserveOn(schedulerService.Dispatcher)
                .Subscribe(x =>
                {
                    _left.Clear();
                    _right.Clear();

                    _left.AddRange(x.Hidden);
                    _right.AddRange(x.Visible);
                })
                .DisposeWith(this);

            AddCommand = ReactiveCommand.Create(CanAddObservable())
                .DisposeWith(this);

            RemoveCommand = ReactiveCommand.Create(CanRemoveObservable())
                .DisposeWith(this);

            MoveupCommand = ReactiveCommand.Create(CanMoveupObservable())
                .DisposeWith(this);

            MovedownCommand = ReactiveCommand.Create(CanMovedownObservable())
                .DisposeWith(this);

            AddCommand.ActivateGestures()
                .Subscribe(x => Add())
                .DisposeWith(this);

            RemoveCommand.ActivateGestures()
                .Subscribe(x => Remove())
                .DisposeWith(this);

            MoveupCommand.ActivateGestures()
                .Subscribe(x => Moveup())
                .DisposeWith(this);

            MovedownCommand.ActivateGestures()
                .Subscribe(x => Movedown())
                .DisposeWith(this);
        }

        public IEnumerable<ColumnPickerItemViewModel> Left => _left;

        public IEnumerable<ColumnPickerItemViewModel> Right => _right;

        public ReactiveCommand<object> AddCommand { get; }

        public ReactiveCommand<object> RemoveCommand { get; }

        public ReactiveCommand<object> MoveupCommand { get; }

        public ReactiveCommand<object> MovedownCommand { get; }

        private IObservable<bool> CanAddObservable()
        {
            return _left.ObserveCollectionChanged()
                .SelectMany(x => ObserveColumnProperties(_left))
                .Select(x => _left.Any(y => y.IsSelected));
        }

        private IObservable<bool> CanRemoveObservable()
        {
            return _right.ObserveCollectionChanged()
                .SelectMany(ObserveColumnProperties(_right))
                .Select(x => _right.Any(y => y.IsSelected));
        }

        private IObservable<bool> CanMoveupObservable()
        {
            return _right.ObserveCollectionChanged()
                .SelectMany(ObserveColumnProperties(_right))
                .Select(x =>
                {
                    var selectedItems = _right.Where(y => y.IsSelected)
                        .ToArray();

                    if (selectedItems.Length == 1)
                    {
                        var selectedIndex = _right.IndexOf(selectedItems[0]);
                        return selectedIndex != 0;
                    }

                    return false;
                });
        }

        private IObservable<bool> CanMovedownObservable()
        {
            return _right.ObserveCollectionChanged()
                .SelectMany(ObserveColumnProperties(_right))
                .Select(x =>
                {
                    var selectedItems = _right.Where(y => y.IsSelected)
                        .ToArray();

                    if (selectedItems.Length == 1)
                    {
                        var selectedItem = selectedItems[0];
                        var selectedIndex = _right.IndexOf(selectedItem);
                        return selectedIndex != _right.Count - 1;
                    }

                    return false;
                });
        }

        private IObservable<Unit> ObserveColumnProperties(IEnumerable<ColumnPickerItemViewModel> columns)
        {
            return columns.Select(y => y.ObservePropertyChanged())
                .Merge(_schedulerService.Dispatcher)
                .AsUnit()
                .StartWith(Constants.StartsWith.Unit.Default);
        }

        private void Add()
        {
            var selectedLeft = _left.Where(x => x.IsSelected)
                .ToArray();

            var visibleColumns = _columnsService.VisibleColumns(_identifier)
                .Concat(selectedLeft.Select(x => x.Name))
                .ToArray();

            _columnsService.ShowColumns(_identifier, visibleColumns);

            selectedLeft.ForEach(x =>
            {
                _left.Remove(x);
                _right.Add(x);

                x.IsSelected = false;
            });
        }

        private void Remove()
        {
            var selectedRight = _right.Where(x => x.IsSelected)
                .ToArray();

            var hiddenColumns = _columnsService.HiddenColumns(_identifier)
                .Concat(selectedRight.Select(x => x.Name))
                .ToArray();

            _columnsService.HideColumns(_identifier, hiddenColumns);

            selectedRight.ForEach(x =>
            {
                _right.Remove(x);
                _left.Add(x);

                x.IsSelected = false;
            });
        }

        private void Moveup()
        {
            var selectedItem = _right.First(x => x.IsSelected);
            var selectedIndex = _right.IndexOf(selectedItem);

            _right.Remove(selectedItem);
            _right.Insert(--selectedIndex, selectedItem);

            _columnsService.ShowColumns(_identifier, _right.Select(x => x.Name));
        }

        private void Movedown()
        {
            var selectedItem = _right.First(x => x.IsSelected);
            var selectedIndex = _right.IndexOf(selectedItem);

            _right.Remove(selectedItem);
            _right.Insert(++selectedIndex, selectedItem);

            _columnsService.ShowColumns(_identifier, _right.Select(x => x.Name));
        }

        private sealed class VisibleColumns
        {
            public VisibleColumns(IEnumerable<ColumnPickerItemViewModel> visible,
                IEnumerable<ColumnPickerItemViewModel> hidden)
            {
                Visible = visible;
                Hidden = hidden;
            }

            public IEnumerable<ColumnPickerItemViewModel> Visible { get; }

            public IEnumerable<ColumnPickerItemViewModel> Hidden { get; }
        }
    }
}