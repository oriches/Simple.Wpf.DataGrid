using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Services
{
    public interface ITabularDataService : IService
    {
        IObservable<IEnumerable<DynamicData>> GetAsync(IScheduler scheduler);
    }
}