namespace Simple.Wpf.DataGrid.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DataGrid.Services;
    using Microsoft.Reactive.Testing;
    using Models;
    using NUnit.Framework;

    [TestFixture]
    public sealed class TabularDataServiceFixtures
    {
        private TestScheduler _testScheduler;
        private MockSchedulerService _schedulerService;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);
        }

        [Test]
        public void generates_data()
        {
            // ARRANGE
            var service = new TabularDataService();

            IEnumerable<DynamicData> data = null;

            // ACT
            service.GetAsync(_schedulerService.TaskPool)
                .Subscribe(x =>
                {
                    data = x;
                });

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(data, Is.Not.Empty);
        }
    }
}