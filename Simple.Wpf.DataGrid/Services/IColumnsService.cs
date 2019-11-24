using System;
using System.Collections.Generic;

namespace Simple.Wpf.DataGrid.Services
{
    public interface IColumnsService : IService
    {
        IObservable<string> Initialised { get; }

        IObservable<string> Changed { get; }

        IEnumerable<string> GetAllColumns(string identifier);

        IEnumerable<string> VisibleColumns(string identifier);

        IEnumerable<string> HiddenColumns(string identifier);

        void InitialiseColumns(string identifier, IEnumerable<string> columnDefinitions);

        void HideColumns(string identifier, IEnumerable<string> columns);

        void ShowColumns(string identifier, IEnumerable<string> columns);
    }
}