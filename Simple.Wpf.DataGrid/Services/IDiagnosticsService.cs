using System;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Services
{
    public interface IDiagnosticsService : IService
    {
        IObservable<Memory> Memory { get; }

        IObservable<int> Cpu { get; }
    }
}