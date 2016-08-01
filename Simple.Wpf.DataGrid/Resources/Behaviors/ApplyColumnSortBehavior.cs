namespace Simple.Wpf.DataGrid.Resources.Behaviors
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Interactivity;
    using Extensions;

    public sealed class ApplyColumnSortBehavior : Behavior<MenuItem>
    {
        private sealed class GriColumnAndDirection
        {
            public DataGrid Grid { get; private set; }

            public DataGridColumn Column { get; private set; }

            public ListSortDirection Direction { get; private set; }

            public GriColumnAndDirection(DataGrid grid, DataGridColumn column, ListSortDirection direction)
            {
                Grid = grid;
                Column = column;
                Direction = direction;
            }
        }

        private sealed class ColumnCollectionViewAndDirection
        {
            public DataGridColumn Column { get; private set; }

            public ICollectionView View { get; private set; }

            public ListSortDirection Direction { get; private set; }

            public ColumnCollectionViewAndDirection(DataGridColumn column, ICollectionView view, ListSortDirection direction)
            {
                Column = column;
                View = view;
                Direction = direction;
            }

            public bool HasSortDescriptions { get { return View != null && View.SortDescriptions != null; } }
        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", 
            typeof(ListSortDirection),
            typeof(ApplyColumnSortBehavior),
            new PropertyMetadata(default(ListSortDirection)));

        private SerialDisposable _disposable;

        public ListSortDirection Direction
        {
            get { return (ListSortDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
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
            _disposable.Disposable = Observable.Return(new GriColumnAndDirection(AssociatedObject.GetDataGrid(), AssociatedObject.GetColumn(), Direction))
                .Select(x => new ColumnCollectionViewAndDirection(x.Column, CollectionViewSource.GetDefaultView(x.Grid.ItemsSource), x.Direction))
                .Where(x => x.HasSortDescriptions)
                .ActivateGestures()
                .Subscribe(x =>
                {
                    x.Column.SortDirection = x.Direction;
                    x.View.SortDescriptions.Clear();
                    x.View.SortDescriptions.Add(new SortDescription(x.Column.SortMemberPath, x.Direction));
                });
        }
    }
}
