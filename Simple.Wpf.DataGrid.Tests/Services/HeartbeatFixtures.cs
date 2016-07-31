namespace Simple.Wpf.DataGrid.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using DataGrid.Services;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class HeartbeatFixtures
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
        public void beats_regularly()
        {
            // ARRANGE
            var hearbeat = new HeartbeatService(TimeSpan.FromMilliseconds(200), _schedulerService);

            // ACT
            var beats = new List<Unit>();
            hearbeat.Listen.Subscribe(x => beats.Add(x));

            _testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(450));

            // ASSERT
            Assert.That(beats, Is.Not.Empty);
            Assert.That(beats.Count, Is.EqualTo(2));
        }

        [Test]
        public void disposing_stops_the_heart_beating()
        {
            // ARRANGE
            var hearbeat = new HeartbeatService(TimeSpan.FromMilliseconds(200), _schedulerService);

            // ACT
            var beats = new List<Unit>();
            hearbeat.Listen.Subscribe(x => beats.Add(x));

            hearbeat.Dispose();

            _testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(450));

            // ASSERT
            Assert.That(beats, Is.Empty);
        }
    }
}