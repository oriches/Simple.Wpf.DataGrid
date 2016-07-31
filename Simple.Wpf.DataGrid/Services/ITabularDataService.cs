namespace Simple.Wpf.DataGrid.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using Models;

    public interface ITabularDataService : IService
    {
        IObservable<IEnumerable<DynamicData>> GetAsync(IScheduler scheduler);
    }
}