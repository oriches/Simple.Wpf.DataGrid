namespace Simple.Wpf.DataGrid.ViewModels
{
    using System;

    public abstract class OverlayViewModel<T>
    {
        protected OverlayViewModel(string header, T viewModel, IDisposable lifetime)
        {
            Header = header;
            ViewModel = viewModel;
            Lifetime = lifetime;
        }

        public string Header { get; private set; }

        public T ViewModel { get; private set; }

        public IDisposable Lifetime { get; }

        public bool HasLifetime => Lifetime != null;
    }

    public sealed class OverlayViewModel : OverlayViewModel<BaseViewModel>
    {
        public OverlayViewModel(string header, BaseViewModel viewModel, IDisposable lifetime)
            : base(header, viewModel, lifetime)
        {
        }
    }
}