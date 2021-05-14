using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Simple.Wpf.DataGrid.Views.Converters
{
    public sealed class NumberToColorConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new NumberToColorConverter();

        private readonly object _negativeNumber;
        private readonly object _positiveNumber;

        private NumberToColorConverter()
        {
            _positiveNumber = GetBrush("PositiveNumberBackgroundBrush");
            _negativeNumber = GetBrush("NegativeNumberBackgroundBrush");
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                switch (value)
                {
                    case int i:
                        return i < 0 ? _negativeNumber : _positiveNumber;
                    case double d:
                        return d < 0 ? _negativeNumber : _positiveNumber;
                    default:
                        return Brushes.Transparent;
                }
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

        private static SolidColorBrush GetBrush(string resource)
        {
            return (SolidColorBrush) Application.Current.Resources[resource];
        }
    }
}