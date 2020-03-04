namespace Simple.Wpf.DataGrid.Helpers
{
    public static class ColumnHelper
    {
        public static string DisplayName(string columnName)
        {
            return columnName.Replace(Constants.UI.Grids.ColumnNameSeparator,
                Constants.UI.Grids.ColumnNameDisplaySeparator);
        }
    }
}