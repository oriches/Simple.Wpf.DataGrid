using System;
using System.Globalization;
using System.Windows.Data;

namespace Simple.Wpf.DataGrid.Resources.Converters
{
    public sealed class IsPositiveNumberConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new IsPositiveNumberConverter();

        private static readonly object False = false;
        private static readonly object True = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                switch (value)
                {
                    case int i:
                        return i > 0 ? True : False;
                    case double d:
                        return d > 0 ? True : False;
                    default:
                        return False;
                }
            }
            catch (Exception)
            {
                return False;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}