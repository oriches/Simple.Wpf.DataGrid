using System;
using System.Reactive.Disposables;
using Moq;
using NUnit.Framework;
using Simple.Wpf.DataGrid.Services;
using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Tests.Services
{
    [TestFixture]
    public sealed class OverlayServiceFixtures
    {
        [Test]
        public void posts_overlay_with_lifetime()
        {
            // ARRANGE
            var contentViewModel = new Mock<BaseViewModel>();
            var lifetime = Disposable.Empty;

            var service = new OverlayService();

            OverlayViewModel overlayViewModel = null;
            service.Show.Subscribe(x => overlayViewModel = x);

            // ACT
            service.Post("header 1", contentViewModel.Object, lifetime);

            // ASSERT
            Assert.That(overlayViewModel.HasLifetime, Is.True);
            Assert.That(overlayViewModel.Lifetime, Is.EqualTo(lifetime));
            Assert.That(overlayViewModel.Header, Is.EqualTo("header 1"));
            Assert.That(overlayViewModel.ViewModel, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void posts_overlay_without_lifetime()
        {
            // ARRANGE
            var contentViewModel = new Mock<BaseViewModel>();

            var service = new OverlayService();

            OverlayViewModel overlayViewModel = null;
            service.Show.Subscribe(x => overlayViewModel = x);

            // ACT
            service.Post("header 1", contentViewModel.Object, null);

            // ASSERT
            Assert.That(overlayViewModel.HasLifetime, Is.False);
            Assert.That(overlayViewModel.Lifetime, Is.Null);
            Assert.That(overlayViewModel.Header, Is.EqualTo("header 1"));
            Assert.That(overlayViewModel.ViewModel, Is.EqualTo(contentViewModel.Object));
        }
    }
}