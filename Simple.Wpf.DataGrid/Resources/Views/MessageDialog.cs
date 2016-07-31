namespace Simple.Wpf.DataGrid.Resources.Views
{
    using System.Windows.Markup;
    using MahApps.Metro.Controls.Dialogs;
    using Models;
    using ViewModels;

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