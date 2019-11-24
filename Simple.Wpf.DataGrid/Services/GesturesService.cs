using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Services
{
    public sealed class GesturesService : DisposableObject, IGestureService
    {
        private readonly DispatcherTimer _timer;

        private bool _isBusy;

        public GesturesService()
        {
            using (Duration.Measure(Logger, "Constructor - " + GetType().Name))
            {
                _timer = new DispatcherTimer(TimeSpan.Zero, DispatcherPriority.ApplicationIdle, TimerCallback,
                    Application.Current.Dispatcher);
                _timer.Stop();
            }

            Disposable.Create(() => _timer.Stop())
                .DisposeWith(this);
        }

        public void SetBusy()
        {
            SetBusyState(true);
        }

        private void SetBusyState(bool busy)
        {
            if (busy != _isBusy)
            {
                _isBusy = busy;
                Mouse.OverrideCursor = busy ? Cursors.Wait : null;

                if (_isBusy) _timer.Start();
            }
        }

        private void TimerCallback(object sender, EventArgs e)
        {
            SetBusyState(false);
            _timer.Stop();
        }
    }
}