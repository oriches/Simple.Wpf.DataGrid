using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Simple.Wpf.DataGrid.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            var array = enumerable.ToArray();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < array.Length; i++)
            {
                var item = array[i];
                action(item);
            }

            return array;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var array = enumerable.ToArray();
            for (var i = 0; i < array.Length; i++) action(array[i], i);

            return array;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ObservableCollection<T>(enumerable);
        }
    }
}