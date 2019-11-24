using System;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Simple.Wpf.DataGrid.Models;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.Tests.Services
{
    [TestFixture]
    public sealed class TabularDataServiceFixtures
    {
        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);
        }

        private TestScheduler _testScheduler;
        private MockSchedulerService _schedulerService;

        [Test]
        public void generates_data()
        {
            // ARRANGE
            var service = new TabularDataService();

            IEnumerable<DynamicData> data = null;

            // ACT
            service.GetAsync(_schedulerService.TaskPool)
                .Subscribe(x => { data = x; });

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(data, Is.Not.Empty);
        }
    }
}