using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Services
{
    public sealed class MessageService : DisposableObject, IMessageService
    {
        private readonly Subject<Message> _show;

        private readonly object _sync = new object();
        private readonly Queue<Message> _waitingMessages = new Queue<Message>();

        public MessageService()
        {
            _show = new Subject<Message>()
                .DisposeWith(this);

            Disposable.Create(() => _waitingMessages.Clear())
                .DisposeWith(this);
        }

        public void Post(string header, ICloseableViewModel viewModel)
        {
            var newMessage = new Message(header, viewModel);

            newMessage.ViewModel.Closed
                .Take(1)
                .Subscribe(x =>
                {
                    Message nextMessage = null;
                    lock (_sync)
                    {
                        _waitingMessages.Dequeue();

                        if (_waitingMessages.Any()) nextMessage = _waitingMessages.Peek();
                    }

                    if (nextMessage != null) _show.OnNext(nextMessage);

                    viewModel.Dispose();
                });

            bool show;
            lock (_sync)
            {
                _waitingMessages.Enqueue(newMessage);
                show = _waitingMessages.Count == 1;
            }

            if (show) _show.OnNext(newMessage);
        }

        public IObservable<Message> Show => _show;
    }
}