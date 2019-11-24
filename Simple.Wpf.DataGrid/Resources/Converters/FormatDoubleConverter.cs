using System;
using System.Globalization;
using System.Windows.Data;

namespace Simple.Wpf.DataGrid.Resources.Converters
{
    public sealed class FormatDoubleConverter : IValueConverter
    {
        public string DateFormat { get; set; }

        public string DateTimeFormat { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            try
            {
                return $"{value:0.###}";
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