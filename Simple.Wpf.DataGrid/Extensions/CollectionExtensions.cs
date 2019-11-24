using System.Collections.Generic;
using System.Linq;

namespace Simple.Wpf.DataGrid.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            var array = enumerable.ToArray();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < array.Length; i++) collection.Add(array[i]);
        }
    }
}