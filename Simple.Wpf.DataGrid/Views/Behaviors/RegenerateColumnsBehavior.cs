using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.Views.Behaviors
{
    public sealed class RegenerateColumnsBehavior : Behavior<System.Windows.Controls.DataGrid>
    {
        public static readonly DependencyProperty VisibleColumnsProperty = DependencyProperty.Register("VisibleColumns",
            typeof(IEnumerable<string>),
            typeof(RegenerateColumnsBehavior),
            new PropertyMetadata(Enumerable.Empty<string>(), HandleVisibleColumnsChanged));

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled",
            typeof(bool),
            typeof(RegenerateColumnsBehavior),
            new PropertyMetadata(default(bool), HandleIsEnabledChanged));

        private Dictionary<string, NamedDataGridTemplateColumn> _columns;
        private List<string> _columnsName;
        private DataTemplate _dateTimeTemplate;

        private CompositeDisposable _disposable;
        private DataTemplate _doubleTemplate;
        private DataTemplate _integerTemplate;
        private DataTemplate _stringTemplate;

        public IEnumerable<string> VisibleColumns
        {
            get => (IEnumerable<string>) GetValue(VisibleColumnsProperty);
            set => SetValue(VisibleColumnsProperty, value);
        }

        public bool IsEnabled
        {
            get => (bool) GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        private static void HandleVisibleColumnsChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue == args.NewValue) return;

            var behavior = (RegenerateColumnsBehavior) dependencyObject;
            behavior.HandleVisibleColumnsChanged();
        }

        private static void HandleIsEnabledChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue == args.NewValue) return;

            var behavior = (RegenerateColumnsBehavior) dependencyObject;
            behavior.HandleIsEnabledChanged();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AutoGeneratingColumn += HandleGeneratingColumn;
            AssociatedObject.AutoGeneratedColumns += HandleCoumnsGenerated;

            AssociatedObject.Loaded += HandleLoaded;
            AssociatedObject.Unloaded += HandleUnloaded;
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            _disposable.Dispose();
        }

        private void HandleLoaded(object sender, RoutedEventArgs args)
        {
            _columns = new Dictionary<string, NamedDataGridTemplateColumn>();
            _columnsName = new List<string>();

            _stringTemplate = Application.Current.Resources["DataGridStringCellTemplate"] as DataTemplate;
            _dateTimeTemplate = Application.Current.Resources["DataGridDateTimeCellTemplate"] as DataTemplate;
            _integerTemplate = Application.Current.Resources["DataGridIntegerCellTemplate"] as DataTemplate;
            _doubleTemplate = Application.Current.Resources["DataGridDoubleCellTemplate"] as DataTemplate;

            _disposable = new CompositeDisposable();

            Observable.Return(AssociatedObject)
                .SelectMany(x => CultureService.CultureChanged.Skip(1), (x, y) => x)
                .Subscribe(x => x.Items.Refresh())
                .DisposeWith(_disposable);

            Observable.Return(AssociatedObject)
                .SelectMany(x => Observable.FromEventPattern<DataGridSortingEventHandler, DataGridSortingEventArgs>(
                    h => x.Sorting += h,
                    h => x.Sorting -= h), (x, y) => y)
                .ActivateGestures()
                .Subscribe()
                .DisposeWith(_disposable);
        }

        private void HandleGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs args)
        {
            var propertyDescriptor = (PropertyDescriptor) args.PropertyDescriptor;
            var column = new NamedDataGridTemplateColumn(propertyDescriptor.Name);

            var displayName = GetPropertyDisplayName(propertyDescriptor);
            if (!string.IsNullOrEmpty(displayName)) column.Header = displayName;

            if (propertyDescriptor.PropertyType == typeof(string))
            {
                column.CellTemplate = _stringTemplate;
                column.SortMemberPath = propertyDescriptor.Name;
            }
            else if (propertyDescriptor.PropertyType == typeof(DateTime))
            {
                column.CellTemplate = _dateTimeTemplate;
                column.SortMemberPath = propertyDescriptor.Name;
            }
            else if (propertyDescriptor.PropertyType == typeof(int))
            {
                column.CellTemplate = _integerTemplate;
            }
            else if (propertyDescriptor.PropertyType == typeof(double))
            {
                column.CellTemplate = _doubleTemplate;
            }

            args.Column = column;
        }

        public static string GetPropertyDisplayName(PropertyDescriptor descriptor)
        {
            // Check for DisplayName attribute and set the column header accordingly
            var displayName = (DisplayNameAttribute) descriptor.Attributes[typeof(DisplayNameAttribute)];
            return !Equals(displayName, DisplayNameAttribute.Default) ? displayName.DisplayName : null;
        }

        private void HandleCoumnsGenerated(object sender, EventArgs args)
        {
            _columns.Clear();
            _columnsName.Clear();

            _columns.AddRange(_columns = AssociatedObject.Columns
                .OfType<NamedDataGridTemplateColumn>()
                .ToDictionary(x => x.ColumnName, x => x));

            _columnsName.AddRange(_columns.Keys);
        }

        private void HandleVisibleColumnsChanged()
        {
            if (AssociatedObject == null) return;

            if (_columns.Count == 0) return;

            // Not using LINQ to improve UI responsiveness...

            // ReSharper disable once ForCanBeConvertedToForeach
            foreach (var columnName in _columnsName)
                _columns[columnName]
                    .Visibility = Visibility.Hidden;

            var visibleColumns = VisibleColumns.ToArray();
            for (var i = 0; i < visibleColumns.Length; i++)
            {
                var column = _columns[visibleColumns[i]];
                var oldIndex = AssociatedObject.Columns.IndexOf(column);
                var newIndex = i;

                column.Visibility = Visibility.Visible;

                if (oldIndex != newIndex)
                {
                    AssociatedObject.Columns.RemoveAt(oldIndex);
                    AssociatedObject.Columns.Insert(newIndex, column);
                }
            }
        }

        private void HandleIsEnabledChanged()
        {
            if (AssociatedObject == null) return;

            AssociatedObject.AutoGenerateColumns = IsEnabled;
        }

        internal sealed class NamedDataGridTemplateColumn : DataGridTemplateColumn
        {
            public NamedDataGridTemplateColumn(string columnName)
            {
                ColumnName = columnName;
            }

            public string ColumnName { get; }

            protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
            {
                // The DataGridTemplateColumn uses ContentPresenter with your DataTemplate.
                var contentPresenter = (ContentPresenter) base.GenerateElement(cell, dataItem);

                // Reset the Binding to the specific column. The default binding is to the DataRowView.
                var binding = new Binding(ColumnName);

                BindingOperations.SetBinding(contentPresenter, ContentPresenter.ContentProperty, binding);
                return contentPresenter;
            }
        }
    }
}