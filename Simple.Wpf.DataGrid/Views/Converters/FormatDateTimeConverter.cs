using System;
using System.Globalization;
using System.Windows.Data;
using Simple.Wpf.DataGrid.Helpers;

namespace Simple.Wpf.DataGrid.Resources.Converters
{
    public sealed class FormatDateTimeConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new FormatDateTimeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            try
            {
                return string.Format(DateTimeHelper.DetermineFormat((DateTime) value), value);
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}