namespace Simple.Wpf.DataGrid.Extensions
{
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public static class MenuItemExtensions
    {
        public static DataGridColumnHeader GetHeader(this MenuItem menuItem)
        {
            return ((ContextMenu)menuItem.Parent)
                .PlacementTarget
                .FindAncestor<DataGridColumnHeader>();
        }

        public static DataGridColumn GetColumn(this MenuItem menuItem)
        {
            return GetHeader(menuItem)
                .Column;
        }

        public static DataGrid GetDataGrid(this MenuItem menuItem)
        {
            var column = menuItem.GetColumn();

            return (DataGrid)column.GetType()
                .GetProperty("DataGridOwner", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(column, null);
        }
    }
}
