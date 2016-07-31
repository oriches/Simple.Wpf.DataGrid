namespace Simple.Wpf.DataGrid.Extensions
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;

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