using System;

namespace Simple.Wpf.DataGrid.Services
{
    public sealed class DateTimeService : IDateTimeService
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}