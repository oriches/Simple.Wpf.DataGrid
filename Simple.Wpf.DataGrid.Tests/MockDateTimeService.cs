using System;
using Microsoft.Reactive.Testing;
using Simple.Wpf.DataGrid.Services;

namespace Simple.Wpf.DataGrid.Tests
{
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