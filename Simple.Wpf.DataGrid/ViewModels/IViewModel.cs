namespace Simple.Wpf.DataGrid.ViewModels
{
    using System;
    using System.ComponentModel;

    public interface IViewModel : INotifyPropertyChanged, IDisposable
    {
        IDisposable SuspendNotifications();
    }
}