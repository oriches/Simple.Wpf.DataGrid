using System;
using System.Globalization;
using System.Windows.Data;

namespace Simple.Wpf.DataGrid.Views.Converters
{
    public sealed class BooleanInvertConverter : IValueConverter
    {
        private static readonly object False = false;
        private static readonly object True = true;

        public static readonly IValueConverter Instance = new BooleanInvertConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return False;

                var boolValue = (bool) value;
                return boolValue ? False : True;
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