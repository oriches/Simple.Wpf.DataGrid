namespace Simple.Wpf.DataGrid.Tests
{
    using System;
    using DataGrid.Services;
    using Microsoft.Reactive.Testing;

    public sealed class MockDateTimeService : IDateTimeService
    {
        private readonly TestScheduler _testScheduler;

        public MockDateTimeService(TestScheduler testScheduler)
        {
            _testScheduler = testScheduler;
        }

        public DateTimeOffset Now => _testScheduler.Now;
    }
}