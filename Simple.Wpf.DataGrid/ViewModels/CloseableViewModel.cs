namespace Simple.Wpf.DataGrid.ViewModels
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Commands;
    using Extensions;

    public abstract class CloseableViewModel : BaseViewModel, ICloseableViewModel
    {
        private readonly Subject<Unit> _closed;
        private readonly Subject<Unit> _confirmed;
        private readonly Subject<Unit> _denied;

        protected CloseableViewModel()
        {
            _closed = new Subject<Unit>()
                .DisposeWith(this);

            _denied = new Subject<Unit>()
                .DisposeWith(this);

            _confirmed = new Subject<Unit>()
                .DisposeWith(this);

            CancelCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            CancelCommand.ActivateGestures()
                .Subscribe(x => _closed.OnNext(Unit.Default))
                .DisposeWith(this);

            ConfirmCommand = ReactiveCommand.Create(InitialiseCanConfirm())
                .DisposeWith(this);

            ConfirmCommand.ActivateGestures()
                .Subscribe(x =>
                {
                    _confirmed.OnNext(Unit.Default);
                    _closed.OnNext(Unit.Default);
                })
                .DisposeWith(this);

            DenyCommand = ReactiveCommand.Create(InitialiseCanDeny())
                .DisposeWith(this);

            DenyCommand.ActivateGestures()
                .Subscribe(x =>
                {
                    _denied.OnNext(Unit.Default);
                    _closed.OnNext(Unit.Default);
                })
                .DisposeWith(this);
        }

        public ReactiveCommand<object> CancelCommand { get; }
        public ReactiveCommand<object> ConfirmCommand { get; protected set; }
        public ReactiveCommand<object> DenyCommand { get; protected set; }

        public IObservable<Unit> Closed => _closed;

        public IObservable<Unit> Denied => _denied;

        public IObservable<Unit> Confirmed => _confirmed;

        protected virtual IObservable<bool> InitialiseCanConfirm()
        {
            return Observable.Return(true);
        }

        protected virtual IObservable<bool> InitialiseCanDeny()
        {
            return Observable.Return(true);
        }
    }
}