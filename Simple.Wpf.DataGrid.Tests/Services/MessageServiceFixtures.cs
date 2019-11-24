using System;
using NUnit.Framework;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.Services;
using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Tests.Services
{
    [TestFixture]
    public sealed class MessageServiceFixtures : BaseServiceFixtures
    {
        private sealed class TestClosableViewModel : CloseableViewModel
        {
        }

        [Test]
        public void posts_message_with_lifetime()
        {
            // ARRANGE
            var contentViewModel = new TestClosableViewModel();

            var service = new MessageService();

            Message message = null;
            service.Show.Subscribe(x => message = x);

            // ACT
            service.Post("header 1", contentViewModel);

            // ASSERT
            Assert.That(message.Header, Is.EqualTo("header 1"));
            Assert.That(message.ViewModel, Is.EqualTo(contentViewModel));
        }

        [Test]
        public void posts_overlay_without_lifetime()
        {
            // ARRANGE
            var contentViewModel = new TestClosableViewModel();

            var service = new MessageService();

            Message message = null;
            service.Show.Subscribe(x => message = x);

            // ACT
            service.Post("header 1", contentViewModel);

            // ASSERT
            Assert.That(message.Header, Is.EqualTo("header 1"));
            Assert.That(message.ViewModel, Is.EqualTo(contentViewModel));
        }
    }
}