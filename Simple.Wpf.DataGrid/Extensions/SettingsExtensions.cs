using System;
using System.ComponentModel;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.Extensions
{
    public static class SettingsExtensions
    {
        public static T Get<T>(this ISettings settings, string name)
        {
            var value = settings[name];
            if (value == null) return default;

            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (converter.CanConvertFrom(value.GetType())) return (T) converter.ConvertFrom(value);

            try
            {
                var convertedValue = Convert.ChangeType(value, typeof(T));
                return (T) convertedValue;
            }
            catch (Exception)
            {
            }

            return (T) value;
        }
    }
}