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

    public sealed class ClearColumnSortBehavior : Behavior<MenuItem>
    {
        private sealed class GridAndColumn
        {
            public DataGrid Grid { get; private set; }

            public DataGridColumn Column { get; private set; }

            public GridAndColumn(DataGrid grid, DataGridColumn column)
            {
                Grid = grid;
                Column = column;
            }
        }

        private sealed class ColumnAndCollectionView
        {
            public DataGridColumn Column { get; private set; }

            public ICollectionView View { get; private set; }

            public ColumnAndCollectionView(DataGridColumn column, ICollectionView view)
            {
                Column = column;
                View = view;
            }

            public bool HasSortDescriptions { get { return View != null && View.SortDescriptions != null; } }
        }

        private SerialDisposable _disposable;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += HandleLoaded;
            AssociatedObject.Unloaded += HandleUnloaded;
        }
        
        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            _disposable = new SerialDisposable();
            
            AssociatedObject.IsEnabled = AssociatedObject.GetHeader().SortDirection != null;
            AssociatedObject.Click += HandleClick;
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Click -= HandleClick;

            _disposable.Dispose();
        }

        private void HandleClick(object sender, RoutedEventArgs e)
        {
            _disposable.Disposable = Observable.Return(new GridAndColumn(AssociatedObject.GetDataGrid(), AssociatedObject.GetColumn()))
                .Select(x => new ColumnAndCollectionView(x.Column, CollectionViewSource.GetDefaultView(x.Grid.ItemsSource)))
                .Where(x => x.HasSortDescriptions)
                .ActivateGestures()
                .Subscribe(x =>
                {
                    x.Column.SortDirection = null;
                    x.View.SortDescriptions.Clear();
                });
        }
    }
}
