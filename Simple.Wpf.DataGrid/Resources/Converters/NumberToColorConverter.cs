using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Simple.Wpf.DataGrid.Resources.Converters
{
    public sealed class NumberToColorConverter : IValueConverter
    {
        public Brush PositiveNumber { get; set; }

        public Brush NegativeNumber { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.Transparent;

            try
            {
                if (value is int) return (int) value < 0 ? NegativeNumber : PositiveNumber;

                if (value is double) return (double) value < 0 ? NegativeNumber : PositiveNumber;

                return Brushes.Transparent;
            }
            catch (Exception)
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}