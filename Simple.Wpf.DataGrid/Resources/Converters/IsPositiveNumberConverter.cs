using System;
using System.Globalization;
using System.Windows.Data;

namespace Simple.Wpf.DataGrid.Resources.Converters
{
    public sealed class IsPositiveNumberConverter : IValueConverter
    {
        private static readonly object False = false;
        private static readonly object True = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is int) return (int) value > 0 ? True : False;

                if (value is double) return (double) value > 0 ? True : False;

                return False;
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