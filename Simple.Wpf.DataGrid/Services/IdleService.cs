using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Services
{
    public sealed class IdleService : DisposableObject, IIdleService
    {
        private readonly IConnectableObservable<EventPattern<object>> _idleObservable;

        public IdleService(ISchedulerService schedulerService)
        {
            using (Duration.Measure(Logger, "Constructor - " + GetType()
                .Name))
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow == null) throw new Exception("Main window has not been created yet!");

                _idleObservable = Observable.FromEventPattern(h => mainWindow.Dispatcher.Hooks.DispatcherInactive += h,
                        h => mainWindow.Dispatcher.Hooks.DispatcherInactive -= h, schedulerService.TaskPool)
                    .Publish();

                _idleObservable.Connect()
                    .DisposeWith(this);
            }
        }

        public IObservable<Unit> Idling => _idleObservable.AsUnit();
    }
}