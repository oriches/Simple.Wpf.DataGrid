// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.Resources.Views
{
    public partial class MainWindow
    {
        private readonly IDisposable _disposable;

        public MainWindow(IMessageService messageService, ISchedulerService schedulerService)
        {
            InitializeComponent();

            _disposable = messageService.Show
                // Delay to make sure there is time for the animations
                .Delay(Constants.UI.MessageDelay, schedulerService.TaskPool)
                .ObserveOn(schedulerService.Dispatcher)
                .Select(x => new MessageDialog(x))
                .SelectMany(x => ShowDialogAsync(x), (x, y) => x)
                .Subscribe();

            Closed += HandleClosed;
        }

        private void HandleClosed(object sender, EventArgs e)
        {
            _disposable.Dispose();
        }

        private IObservable<Unit> ShowDialogAsync(MessageDialog dialog)
        {
            var settings = new MetroDialogSettings
            {
                AnimateShow = true,
                AnimateHide = true,
                ColorScheme = MetroDialogColorScheme.Accented
            };

            return this.ShowMetroDialogAsync(dialog, settings)
                .ToObservable()
                .SelectMany(x => dialog.CloseableContent.Closed, (x, y) => x)
                .SelectMany(x => this.HideMetroDialogAsync(dialog).ToObservable(), (x, y) => x)
                .Take(1);
        }
    }
}