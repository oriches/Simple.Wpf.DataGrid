using System;
using System.Reactive;

namespace Simple.Wpf.DataGrid.Services
{
    public interface IIdleService : IService
    {
        IObservable<Unit> Idling { get; }
    }
}