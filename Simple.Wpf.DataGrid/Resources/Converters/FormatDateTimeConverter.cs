namespace Simple.Wpf.DataGrid.Resources.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Helpers;

    public sealed class FormatDateTimeConverter : IValueConverter
    {
        public string DateFormat { get; set; }

        public string DateTimeFormat { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

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