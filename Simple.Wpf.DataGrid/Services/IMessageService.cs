using System;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Services
{
    public interface IMessageService : IService
    {
        IObservable<Message> Show { get; }

        void Post(string header, ICloseableViewModel viewModel);
    }
}