// ReSharper disable ConvertClosureToMethodGroup

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;

namespace Simple.Wpf.DataGrid.Collections
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification;

        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (!_suppressNotification)
            {
                var handlers = CollectionChanged;
                if (handlers != null)
                    foreach (var handler in handlers.GetInvocationList().Cast<NotifyCollectionChangedEventHandler>())
                        if (handler.Target is CollectionView view)
                            view.Refresh();
                        else
                            handler(this, args);
            }
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            _suppressNotification = true;

            var array = items.ToArray();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < array.Length; i++)
            {
                var item = array[i];
                Add(item);
            }

            _suppressNotification = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array,
                array.Length));
        }
    }
}