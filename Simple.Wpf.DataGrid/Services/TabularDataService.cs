// ReSharper disable ConvertClosureToMethodGroup
namespace Simple.Wpf.DataGrid.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Models;
    using NLog;

    public sealed class TabularDataService : ITabularDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TabularDataService()
        {
            using (Duration.Measure(Logger, "Constructor - " + GetType().Name))
            {
            }
        }
        
        public IObservable<IEnumerable<DynamicData>> GetAsync(IScheduler scheduler)
        {
            return Observable.Create<IEnumerable<DynamicData>>(x =>
            {
                var data = TabularDataGenerator.CreateInitialSnapshot()
                    .ToArray();

                x.OnNext(data);
                
                return Observable.Interval(TimeSpan.FromMilliseconds(50), scheduler)
                    .Finally(() => x.OnCompleted())
                    .DelaySubscription(TimeSpan.FromSeconds(3), scheduler)
                    .Synchronize(data)
                    .Subscribe(_ =>
                    {
                        var localCopy = data.Select(y => y.Clone()).ToArray();
                        var updates = TabularDataGenerator.CreateUpdates(localCopy);

                        x.OnNext(updates);
                        data = localCopy;
                    });

            })
            .SubscribeOn(scheduler);
        }
    }
}
