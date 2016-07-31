namespace Simple.Wpf.DataGrid.ViewModels
{
    public interface IMainViewModel : IViewModel
    {
        IDiagnosticsViewModel Diagnostics { get; }
    }
}