using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Services;
using Simple.Wpf.DataGrid.Tests.ViewModels;

namespace Simple.Wpf.DataGrid.Tests.Services
{
    [TestFixture]
    public sealed class CultureServiceFixtures : BaseServiceFixtures
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void culture_changes()
        {
            // ARRANGE
            var called = false;
            CultureService.CultureChanged
                .Subscribe(x => called = true);

            var newCulture = CultureService.AvailableCultures.Last();

            // ACT
            CultureService.SetCulture(newCulture);

            // ASSERT
            Assert.That(called, Is.True);
            Assert.That(CultureService.CurrentCulture, Is.EqualTo(newCulture));
        }

        [Test]
        public void properties_refreshed_after_culture_changes()
        {
            // ARRANGE
            var viewModel = new TestViewModel();

            var propertyNames = new List<string>();
            viewModel.ObservePropertyChanged()
                .Subscribe(x => propertyNames.Add(x.PropertyName));

            // ACT
            CultureService.SetCulture(CultureService.AvailableCultures.Skip(1).First());

            // ASSERT
            Assert.That(propertyNames.Count, Is.EqualTo(1));
            Assert.That(propertyNames.Contains(""), Is.True);
        }
    }
}