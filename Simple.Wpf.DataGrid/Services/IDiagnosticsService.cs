namespace Simple.Wpf.DataGrid.Services
{
    using System;
    using Models;

    public interface IDiagnosticsService : IService
    {
        IObservable<Memory> Memory { get; }

        IObservable<int> Cpu { get; }
    }

}