using System;

namespace Simple.Wpf.DataGrid.Services
{
    public interface IDateTimeService : IService
    {
        DateTimeOffset Now { get; }
    }
}