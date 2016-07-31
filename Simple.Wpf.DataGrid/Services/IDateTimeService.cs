namespace Simple.Wpf.DataGrid.Services
{
    using System;

    public interface IDateTimeService : IService
    {
        DateTimeOffset Now { get; }
    }
}