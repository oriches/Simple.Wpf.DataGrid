using System;
using System.Reactive;

namespace Simple.Wpf.DataGrid.Services
{
    public interface IHeartbeatService : IService
    {
        IObservable<Unit> Listen { get; }
    }
}