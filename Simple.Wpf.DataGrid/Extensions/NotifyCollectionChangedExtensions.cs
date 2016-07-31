namespace Simple.Wpf.DataGrid.Extensions
{
    using System;
    using System.Collections.Specialized;
    using System.Reactive.Linq;

    public static class NotifyCollectionChangedExtensions
    {
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChanged(
            this INotifyCollectionChanged source)
        {
            return Observable.Return(source)
                .SelectMany(x => Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    h => x.CollectionChanged += h,
                    h => x.CollectionChanged -= h),
                    (x, y) => y)
                .Select(x => x.EventArgs);
        }
    }
}