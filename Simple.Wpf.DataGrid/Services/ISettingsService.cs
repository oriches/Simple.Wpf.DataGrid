namespace Simple.Wpf.DataGrid.Services
{
    using System.Collections.Generic;
    using Models;

    public interface ISettingsService : IService
    {
        ISettings CreateOrUpdate(string name);

        ISettings CreateOrUpdate(string name, IEnumerable<Setting> values);

        bool TryGet(string name, out ISettings settings);

        void Persist();
    }
}