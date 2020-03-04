using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.ViewModels
{
    public abstract class BaseViewModel : DisposableObject, IViewModel
    {
        private static readonly PropertyChangedEventArgs EmptyChangeArgs = new PropertyChangedEventArgs(string.Empty);

        private static readonly IDictionary<string, PropertyChangedEventArgs> ChangedProperties =
            new Dictionary<string, PropertyChangedEventArgs>();

        private SuspendedNotifications _suspendedNotifications;

        protected BaseViewModel()
        {
            Observable.Return(this)
                .SelectMany(x => CultureService.CultureChanged.Skip(1), (x, y) => x)
                .ActivateGestures()
                .Subscribe(x => x.OnPropertyChanged())
                .DisposeWith(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IDisposable SuspendNotifications()
        {
            if (_suspendedNotifications == null) _suspendedNotifications = new SuspendedNotifications(this);

            return _suspendedNotifications.AddRef();
        }

        protected virtual void OnPropertyChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (_suspendedNotifications != null)
            {
                _suspendedNotifications.Add(propertyName);
            }
            else
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        handler(this, EmptyChangeArgs);
                    }
                    else
                    {
                        if (!ChangedProperties.TryGetValue(propertyName, out var args))
                        {
                            args = new PropertyChangedEventArgs(propertyName);
                            ChangedProperties.Add(propertyName, args);
                        }

                        handler(this, args);
                    }
                }
            }
        }

        protected virtual bool SetProperty<T>(ref T existingValue, T newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(existingValue, newValue)) return false;

            existingValue = newValue;
            OnPropertyChanged(propertyName);

            return true;
        }

        private sealed class SuspendedNotifications : IDisposable
        {
            private readonly Counter _counter;

            private readonly HashSet<string> _properties = new HashSet<string>();
            private readonly BaseViewModel _target;

            public SuspendedNotifications(BaseViewModel target)
            {
                _target = target;

                _counter = new Counter(Dispose);
            }

            public void Dispose()
            {
                _target._suspendedNotifications = null;

                foreach (var property in _properties) _target.OnPropertyChanged(property);
            }

            public void Add(string propertyName)
            {
                _properties.Add(propertyName);
            }

            public IDisposable AddRef()
            {
                _counter.Increment();

                return _counter;
            }
            // Using an internal class to avoid using closure which would cases the creation of a class
            // under the covers at runtime - gives better performance for this base class...

            private sealed class Counter : IDisposable
            {
                private readonly Action _dispose;
                private int _refCount;

                public Counter(Action dispose)
                {
                    _dispose = dispose;
                }

                public void Dispose()
                {
                    if (--_refCount == 0) _dispose();
                }

                public void Increment()
                {
                    ++_refCount;
                }
            }
        }
    }
}