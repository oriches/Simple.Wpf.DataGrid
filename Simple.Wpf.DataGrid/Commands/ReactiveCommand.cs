// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using NLog;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.Commands
{
    public sealed class ReactiveCommand : ReactiveCommand<object>
    {
        private ReactiveCommand(IObservable<bool> canExecute)
            : base(canExecute.StartWith(Constants.StartsWith.Boolean.False))
        {
        }

        public new static ReactiveCommand<object> Create()
        {
            return ReactiveCommand<object>.Create(Observable.Return(true));
        }

        public new static ReactiveCommand<object> Create(IObservable<bool> canExecute)
        {
            return ReactiveCommand<object>.Create(canExecute);
        }
    }

    public class ReactiveCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _canDisposable;
        private readonly List<EventHandler> _eventHandlers;
        private readonly Subject<T> _execute;

        private bool _currentCanExecute;

        protected ReactiveCommand(IObservable<bool> canExecute)
        {
            _eventHandlers = new List<EventHandler>(8);

            _canDisposable = canExecute.Subscribe(x =>
            {
                _currentCanExecute = x;
                CommandManager.InvalidateRequerySuggested();
            });

            _execute = new Subject<T>();
        }

        public virtual void Execute(object parameter)
        {
            var typedParameter = parameter is T o ? o : default;

            if (CanExecute(typedParameter)) _execute.OnNext(typedParameter);
        }

        public virtual bool CanExecute(object parameter)
        {
            return _currentCanExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                _eventHandlers.Add(value);
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                _eventHandlers.Remove(value);
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose - " + GetType().Name))
            {
                _eventHandlers.ForEach(x => CommandManager.RequerySuggested -= x);
                _eventHandlers.Clear();

                _canDisposable.Dispose();

                _execute.OnCompleted();
                _execute.Dispose();
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _execute.ActivateGestures().Subscribe(x => observer.OnNext(x),
                e => observer.OnError(e),
                () => observer.OnCompleted());
        }

        public static ReactiveCommand<T> Create()
        {
            return new ReactiveCommand<T>(Observable.Return(true));
        }

        public static ReactiveCommand<T> Create(IObservable<bool> canExecute)
        {
            return new ReactiveCommand<T>(canExecute);
        }
    }
}