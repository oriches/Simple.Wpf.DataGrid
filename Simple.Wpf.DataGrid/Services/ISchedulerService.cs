namespace Simple.Wpf.DataGrid.Services
{
    using System.Reactive.Concurrency;

    public interface ISchedulerService : IService
    {
        IScheduler Dispatcher { get; }

        IScheduler Current { get; }

        IScheduler TaskPool { get; }

        IScheduler EventLoop { get; }

        IScheduler NewThread { get; }

        IScheduler StaThread { get; }
    }
}