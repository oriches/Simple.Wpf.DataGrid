using System.Collections.Generic;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Services
{
    public interface ISettings : IEnumerable<Setting>
    {
        object this[string name] { get; set; }
    }
}