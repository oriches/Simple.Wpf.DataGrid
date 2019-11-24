using System;
using System.ComponentModel;

namespace Simple.Wpf.DataGrid.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged, IDisposable
    {
        IDisposable SuspendNotifications();
    }
}