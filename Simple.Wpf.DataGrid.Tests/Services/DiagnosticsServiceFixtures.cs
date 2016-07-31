namespace Simple.Wpf.DataGrid.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Subjects;
    using DataGrid.Services;
    using Microsoft.Reactive.Testing;
    using Models;
    using Moq;
    using NUnit.Framework;

    [TestFixture, Ignore("Inconsistent when running through NCrunch")]
    public sealed class DiagnosticsServiceFixtures
    {
        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);

            _idleService = new Mock<IIdleService>();

            _idling = new Subject<Unit>();
            _idleService.Setup(x => x.Idling).Returns(_idling);
        }

        private TestScheduler _testScheduler;
        private ISchedulerService _schedulerService;
        private Mock<IIdleService> _idleService;
        private Subject<Unit> _idling;

        [Test]
        public void cpu_pumps_when_idling()
        {
            // ARRANGE
            var values = new List<int>();

            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.Cpu.Subscribe(x => values.Add(x));

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ACT
            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ASSERT
            Assert.That(values, Is.Not.Empty);
            Assert.That(values.Count, Is.EqualTo(2));
        }

        [Test]
        public void disposing_stops_streams_pumping()
        {
            // ARRANGE
            var called = false;
            var service = new DiagnosticsService(_idleService.Object, _schedulerService);

            service.Cpu.Subscribe(x => { called = true; });
            service.Memory.Subscribe(x => { called = true; });

            // ACT
            service.Dispose();

            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ASSERT
            Assert.That(called, Is.False);
        }
        
        [Test]
        public void memory_pumps_when_idling()
        {
            // ARRANGE
            var values = new List<Memory>();

            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.Memory.Subscribe(x => values.Add(x));

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ACT
            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ASSERT
            Assert.That(values, Is.Not.Empty);
            Assert.That(values.Count, Is.EqualTo(2));
        }
    }
}