// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NLog;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Services
{
    public sealed class TabularDataService : ITabularDataService
    {
        private static readonly TimeSpan DataInterval = TimeSpan.FromMilliseconds(50);
        private static readonly TimeSpan DataDelay = TimeSpan.FromMilliseconds(567);
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISchedulerService _schedulerService;

        public TabularDataService(ISchedulerService schedulerService)
        {
            _schedulerService = schedulerService;
            using (Duration.Measure(Logger, "Constructor - " + GetType()
                .Name))
            {
            }
        }

        public IObservable<IEnumerable<DynamicData>> GetAsync()
        {
            return Observable.Create<IEnumerable<DynamicData>>(x =>
                {
                    var data = TabularDataGenerator.CreateInitialSnapshot()
                        .ToArray();

                    x.OnNext(data);

                    return Observable.Interval(DataInterval, _schedulerService.TaskPool)
                        .Finally(() => x.OnCompleted())
                        .DelaySubscription(DataDelay, _schedulerService.TaskPool)
                        .Synchronize(data)
                        .Subscribe(_ =>
                        {
                            var localCopy = data.Select(y => y.Clone())
                                .ToArray();
                            var updates = TabularDataGenerator.CreateUpdates(localCopy);

                            x.OnNext(updates);
                            data = localCopy;
                        });
                })
                .SubscribeOn(_schedulerService.TaskPool);
        }
    }
}