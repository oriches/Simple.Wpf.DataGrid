using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Simple.Wpf.DataGrid.Extensions;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Services
{
    public sealed class ColumnsService : DisposableObject, IColumnsService
    {
        private readonly Subject<string> _changed;
        private readonly Subject<string> _initialised;
        private readonly ISettingsService _settingsService;

        public ColumnsService(ISettingsService settingsService)
        {
            using (Duration.Measure(Logger, "Constructor - " + GetType().Name))
            {
                _settingsService = settingsService;

                _initialised = new Subject<string>()
                    .DisposeWith(this);

                _changed = new Subject<string>()
                    .DisposeWith(this);
            }
        }

        public IObservable<string> Initialised => _initialised;

        public IObservable<string> Changed => _changed;

        public IEnumerable<string> GetAllColumns(string identifier)
        {
            ISettings settings;
            if (_settingsService.TryGet(identifier, out settings))
            {
                var allColumns = settings.Get<string[]>(Constants.UI.Settings.Names.Columns);
                return allColumns;
            }

            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> VisibleColumns(string identifier)
        {
            ISettings settings;
            if (_settingsService.TryGet(identifier, out settings))
            {
                var allColumns = settings.Get<string[]>(Constants.UI.Settings.Names.Columns);
                var visibleColumns = settings.Get<string[]>(Constants.UI.Settings.Names.VisibleColumns);

                return visibleColumns.Intersect(allColumns)
                    .ToArray();
            }

            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> HiddenColumns(string identifier)
        {
            ISettings settings;
            if (_settingsService.TryGet(identifier, out settings))
            {
                var allColumns = settings.Get<string[]>(Constants.UI.Settings.Names.Columns);
                var visibleColumns = settings.Get<string[]>(Constants.UI.Settings.Names.VisibleColumns);

                return allColumns.Except(visibleColumns)
                    .ToArray();
            }

            return Enumerable.Empty<string>();
        }

        public void InitialiseColumns(string identifier, IEnumerable<string> columns)
        {
            var settings = _settingsService.CreateOrUpdate(identifier);

            settings[Constants.UI.Settings.Names.Columns] = columns ?? Enumerable.Empty<string>();
            settings[Constants.UI.Settings.Names.VisibleColumns] = columns;

            _initialised.OnNext(identifier);
        }

        public void HideColumns(string identifier, IEnumerable<string> columns)
        {
            ISettings settings;
            if (_settingsService.TryGet(identifier, out settings))
            {
                var allColumns = settings.Get<string[]>(Constants.UI.Settings.Names.Columns);
                settings[Constants.UI.Settings.Names.VisibleColumns] = allColumns.Except(columns).ToArray();

                _changed.OnNext(identifier);
            }
        }

        public void ShowColumns(string identifier, IEnumerable<string> columns)
        {
            ISettings settings;
            if (_settingsService.TryGet(identifier, out settings))
            {
                var allColumns = settings.Get<string[]>(Constants.UI.Settings.Names.Columns);
                settings[Constants.UI.Settings.Names.VisibleColumns] = columns.Intersect(allColumns).ToArray();

                _changed.OnNext(identifier);
            }
        }
    }
}