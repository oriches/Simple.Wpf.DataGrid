using System.Windows.Markup;
using MahApps.Metro.Controls.Dialogs;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Views.Views
{
    [ContentProperty("DialogBody")]
    public sealed class MessageDialog : BaseMetroDialog
    {
        private readonly Message _message;

        public MessageDialog(Message message)
        {
            _message = message;

            Title = _message.Header;
            Content = _message.ViewModel;
        }

        public ICloseableViewModel CloseableContent => _message.ViewModel;
    }
}