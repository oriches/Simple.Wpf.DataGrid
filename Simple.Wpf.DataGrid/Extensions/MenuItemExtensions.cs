using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Simple.Wpf.DataGrid.Extensions
{
    public static class MenuItemExtensions
    {
        public static DataGridColumnHeader GetHeader(this MenuItem menuItem)
        {
            return ((ContextMenu) menuItem.Parent)
                .PlacementTarget
                .FindAncestor<DataGridColumnHeader>();
        }

        public static DataGridColumn GetColumn(this MenuItem menuItem)
        {
            return GetHeader(menuItem)
                .Column;
        }

        public static System.Windows.Controls.DataGrid GetDataGrid(this MenuItem menuItem)
        {
            var column = menuItem.GetColumn();

            return (System.Windows.Controls.DataGrid) column.GetType()
                .GetProperty("DataGridOwner", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(column, null);
        }
    }
}