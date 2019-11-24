using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Simple.Wpf.DataGrid.Extensions
{
    public static class SchedulerExtensions
    {
        public static IDisposable Schedule(this IScheduler scheduler, TimeSpan timeSpan, Action action)
        {
            return scheduler.Schedule(action, timeSpan, (s1, s2) =>
            {
                s2();
                return Disposable.Empty;
            });
        }

        public static IDisposable Schedule(this IScheduler scheduler, Action action)
        {
            return scheduler.Schedule(action, (s1, s2) =>
            {
                s2();
                return Disposable.Empty;
            });
        }
    }
}