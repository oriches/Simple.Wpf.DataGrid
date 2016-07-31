namespace Simple.Wpf.DataGrid.Services
{
    using System;

    public sealed class DateTimeService : IDateTimeService
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
