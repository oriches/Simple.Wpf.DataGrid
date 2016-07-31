namespace Simple.Wpf.DataGrid.Services
{
    using System;
    using System.Reactive.Subjects;
    using Extensions;
    using Models;
    using ViewModels;

    public sealed class OverlayService : DisposableObject, IOverlayService
    {
        private readonly Subject<OverlayViewModel> _show;

        public OverlayService()
        {
            using (Duration.Measure(Logger, "Constructor - " + GetType().Name))
            {
                _show = new Subject<OverlayViewModel>()
                    .DisposeWith(this);
            }
        }

        public void Post(string header, BaseViewModel viewModel, IDisposable lifetime)
        {
            _show.OnNext(new OverlayViewModel(header, viewModel, lifetime));
        }

        public IObservable<OverlayViewModel> Show => _show;
    }
}