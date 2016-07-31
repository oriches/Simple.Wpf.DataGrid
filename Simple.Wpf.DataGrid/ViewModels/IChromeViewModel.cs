namespace Simple.Wpf.DataGrid.ViewModels
{
    using Commands;

    public interface IChromeViewModel : IViewModel
    {
        IMainViewModel Main { get; }
        ReactiveCommand<object> CloseOverlayCommand { get; }
        bool HasOverlay { get; }
        string OverlayHeader { get; }
        BaseViewModel Overlay { get; }
    }
}