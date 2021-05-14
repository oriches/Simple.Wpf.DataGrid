using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Simple.Wpf.DataGrid.Models;

namespace Simple.Wpf.DataGrid.Services
{
    public sealed class Settings : ISettings
    {
        private readonly IObserver<bool> _persist;
        private readonly List<Setting> _settings;

        public Settings(IEnumerable<Setting> settings, IObserver<bool> persist)
        {
            _persist = persist;
            _settings = settings.ToList();
        }

        public object this[string name]
        {
            get
            {
                var setting = FindFirst(name);
                return setting?.Value;
            }

            set
            {
                var setting = FindFirst(name);
                if (setting != null)
                {
                    var index = _settings.IndexOf(setting);
                    _settings[index] = new Setting(name, value);
                }
                else
                {
                    _settings.Add(new Setting(name, value));
                }

                _persist.OnNext(true);
            }
        }

        IEnumerator<Setting> IEnumerable<Setting>.GetEnumerator()
        {
            return new SettingsEnumerator(_settings);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SettingsEnumerator(_settings);
        }

        private Setting FindFirst(string name)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < _settings.Count; i++)
            {
                var setting = _settings[i];
                if (setting.Name == name) return setting;
            }

            return null;
        }

        internal sealed class SettingsEnumerator : IEnumerator<Setting>
        {
            private int _index;
            private List<Setting> _settings;

            public SettingsEnumerator(List<Setting> settings)
            {
                _settings = settings;
                Reset();
            }

            public void Reset()
            {
                _index = -1;
            }

            object IEnumerator.Current => ((IEnumerator<Setting>) this).Current;

            Setting IEnumerator<Setting>.Current => _settings[_index];

            public bool MoveNext()
            {
                ++_index;

                return _index < _settings.Count;
            }

            public void Dispose()
            {
                _settings = null;
                _index = -1;
            }
        }
    }
}