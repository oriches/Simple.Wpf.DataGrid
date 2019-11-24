using System;
using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Services
{
    public interface IOverlayService : IService
    {
        IObservable<OverlayViewModel> Show { get; }

        void Post(string header, BaseViewModel viewModel, IDisposable lifetime);
    }
}