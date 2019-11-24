using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using Simple.Wpf.DataGrid.Extensions;

namespace Simple.Wpf.DataGrid.Resources.Behaviors
{
    public sealed class ApplyColumnSortBehavior : Behavior<MenuItem>
    {
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction",
            typeof(ListSortDirection),
            typeof(ApplyColumnSortBehavior),
            new PropertyMetadata(default(ListSortDirection)));

        private SerialDisposable _disposable;

        public ListSortDirection Direction
        {
            get => (ListSortDirection) GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += HandleLoaded;
            AssociatedObject.Unloaded += HandleUnloaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            _disposable = new SerialDisposable();

            AssociatedObject.IsEnabled = AssociatedObject.GetHeader().SortDirection != Direction;
            AssociatedObject.Click += HandleClick;
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Click -= HandleClick;

            _disposable.Dispose();
        }

        private void HandleClick(object sender, RoutedEventArgs e)
        {
            _disposable.Disposable = Observable.Return(new GriColumnAndDirection(AssociatedObject.GetDataGrid(),
                    AssociatedObject.GetColumn(), Direction))
                .Select(x => new ColumnCollectionViewAndDirection(x.Column,
                    CollectionViewSource.GetDefaultView(x.Grid.ItemsSource), x.Direction))
                .Where(x => x.HasSortDescriptions)
                .ActivateGestures()
                .Subscribe(x =>
                {
                    x.Column.SortDirection = x.Direction;
                    x.View.SortDescriptions.Clear();
                    x.View.SortDescriptions.Add(new SortDescription(x.Column.SortMemberPath, x.Direction));
                });
        }

        private sealed class GriColumnAndDirection
        {
            public GriColumnAndDirection(System.Windows.Controls.DataGrid grid, DataGridColumn column,
                ListSortDirection direction)
            {
                Grid = grid;
                Column = column;
                Direction = direction;
            }

            public System.Windows.Controls.DataGrid Grid { get; }

            public DataGridColumn Column { get; }

            public ListSortDirection Direction { get; }
        }

        private sealed class ColumnCollectionViewAndDirection
        {
            public ColumnCollectionViewAndDirection(DataGridColumn column, ICollectionView view,
                ListSortDirection direction)
            {
                Column = column;
                View = view;
                Direction = direction;
            }

            public DataGridColumn Column { get; }

            public ICollectionView View { get; }

            public ListSortDirection Direction { get; }

            public bool HasSortDescriptions => View != null && View.SortDescriptions != null;
        }
    }
}