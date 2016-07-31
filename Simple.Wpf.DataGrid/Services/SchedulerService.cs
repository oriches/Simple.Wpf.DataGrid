namespace Simple.Wpf.DataGrid.Services
{
    using System;
    using System.Reactive.Concurrency;
    using System.Threading;

    public sealed class SchedulerService : ISchedulerService
    {
        private readonly DispatcherScheduler _dispatcherScheduler;

        public SchedulerService()
        {
            _dispatcherScheduler = DispatcherScheduler.Current;
        }

        public IScheduler Dispatcher => _dispatcherScheduler;

        public IScheduler Current => CurrentThreadScheduler.Instance;

        public IScheduler TaskPool => TaskPoolScheduler.Default;

        public IScheduler EventLoop => new EventLoopScheduler();

        public IScheduler NewThread => NewThreadScheduler.Default;

        public IScheduler StaThread
        {
            get
            {
                Func<ThreadStart, Thread> func = x =>
                                                 {
                                                     var thread = new Thread(x) {IsBackground = true};
                                                     thread.SetApartmentState(ApartmentState.STA);

                                                     return thread;
                                                 };

                return new EventLoopScheduler(func);
            }
        }
    }
}