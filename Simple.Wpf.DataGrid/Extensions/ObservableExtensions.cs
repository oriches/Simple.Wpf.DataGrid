using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Simple.Wpf.DataGrid.Commands;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.Extensions
{
    public static class ObservableExtensions
    {
        public static IGestureService GestureService;

        public static IObservable<Unit> AsUnit<T>(this IObservable<T> observable)
        {
            return observable.Select(x => Unit.Default);
        }

        public static ReactiveCommand<object> ToCommand(this IObservable<bool> canExecute)
        {
            return ReactiveCommand.Create(canExecute);
        }

        public static IObservable<T> ActivateGestures<T>(this IObservable<T> observable)
        {
            if (GestureService == null) throw new Exception("GestureService has not been initialised");

            return observable.Do(x => GestureService.SetBusy());
        }

        public static IDisposable ResilentSubscribe<T>(this IObservable<T> observable, Action<T> onNext,
            Action<Exception> onError, IScheduler scheduler)
        {
            return observable.Subscribe(x => OnNextInvoke(onNext, x, scheduler), onError);
        }

        public static IDisposable ResilentSubscribe<T>(this IObservable<T> observable, IScheduler scheduler)
        {
            return observable.Subscribe(x => OnNextInvoke(y => { }, x, scheduler));
        }

        public static IDisposable ResilentSubscribe<T>(this IObservable<T> observable, Action<T> onNext,
            Action onCompleted,
            IScheduler scheduler)
        {
            return observable.Subscribe(x => OnNextInvoke(onNext, x, scheduler), onCompleted);
        }

        public static IDisposable ResilentSubscribe<T>(this IObservable<T> observable, Action<T> onNext,
            IScheduler scheduler)
        {
            return observable.Subscribe(x => OnNextInvoke(onNext, x, scheduler));
        }

        public static void ResilentSubscribe<T>(this IObservable<T> observable, Action<T> onNext, Action onCompleted,
            CancellationToken token, IScheduler scheduler)
        {
            observable.Subscribe(x => OnNextInvoke(onNext, x, scheduler), onCompleted, token);
        }

        public static void ResilentSubscribe<T>(this IObservable<T> observable, Action<T> onNext,
            Action<Exception> onError,
            Action onCompleted, CancellationToken token, IScheduler scheduler)
        {
            observable.Subscribe(x => OnNextInvoke(onNext, x, scheduler), onError, onCompleted, token);
        }

        private static void OnNextInvoke<T>(Action<T> onNext, T instance, IScheduler scheduler)
        {
            try
            {
                onNext(instance);
            }
            catch (Exception exn)
            {
                scheduler.Schedule(exn, (s1, s2) => { throw s2; });
            }
        }
    }
}