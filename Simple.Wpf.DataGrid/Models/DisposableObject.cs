using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NLog;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.Models
{
    public abstract class DisposableObject : IDisposable
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CompositeDisposable _disposable;
        private readonly string _disposeMessage;

        protected DisposableObject()
        {
            _disposable = new CompositeDisposable();
            _disposeMessage = string.Intern("Dispose - " + GetType().Name);
        }

        protected bool SuppressDebugWriteline { get; set; }

        public virtual void Dispose()
        {
            if (SuppressDebugWriteline)
                _disposable.Dispose();
            else
                using (Duration.Measure(Logger, _disposeMessage))
                {
                    _disposable.Dispose();
                }
        }

        protected void DisposeOfAsync(IEnumerable<IDisposable> disposables, IScheduler scheduler)
        {
            Observable.Return(disposables, scheduler)
                .Subscribe(x => x.ForEach(y => y.Dispose()))
                .DisposeWith(this);
        }

        public static implicit operator CompositeDisposable(DisposableObject disposable)
        {
            return disposable._disposable;
        }
    }
}