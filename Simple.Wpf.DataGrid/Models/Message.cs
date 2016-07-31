namespace Simple.Wpf.DataGrid.Models
{
    using ViewModels;

    public sealed class Message
    {
        public Message(string header, ICloseableViewModel viewModel)
        {
            Header = header;
            ViewModel = viewModel;
        }

        public string Header { get; private set; }

        public ICloseableViewModel ViewModel { get; private set; }
    }
}