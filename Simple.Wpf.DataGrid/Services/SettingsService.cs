namespace Simple.Wpf.DataGrid.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Extensions;
    using Models;
    using Newtonsoft.Json;

    public sealed class SettingsService : DisposableObject, ISettingsService
    {
        private readonly IDictionary<string, ISettings> _settings;
        private readonly Subject<bool> _persist;
        private readonly object _persistSync = new object();

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        };

        public SettingsService(ISchedulerService schedulerService)
        {
            using (Duration.Measure(Logger, "Constructor - " + GetType().Name))
            {
                _persist = new Subject<bool>()
                   .DisposeWith(this);

                _persist.ObserveOn(schedulerService.TaskPool)
                        .Synchronize(_persistSync)
                        .Subscribe(_ => Persist())
                        .DisposeWith(this);

                _settings = new Dictionary<string, ISettings>();

                var serializedSettings = Properties.Settings.Default.GlobalSettings;

                if (!string.IsNullOrEmpty(serializedSettings))
                {
                    JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<Dtos.Setting>>>(
                        serializedSettings, _serializerSettings)
                        .ForEach(y => _settings.Add(y.Key, CreateSettings(y.Value)));
                }
            }
        }

        public ISettings CreateOrUpdate(string name)
        {
            return CreateOrUpdate(name, Enumerable.Empty<Setting>());
        }

        public ISettings CreateOrUpdate(string name, IEnumerable<Setting> values)
        {
            var settings = new Settings(values, _persist);
            _settings[name] = settings;

            _persist.OnNext(true);

            return settings;
        }

        public bool TryGet(string name, out ISettings settings)
        {
            return _settings.TryGetValue(name, out settings);
        }

        public void Persist()
        {
            var globalSettings = new Dictionary<string, IEnumerable<Dtos.Setting>>(_settings.Count);


            globalSettings.AddRange(_settings.Select(x =>
            {
                var settings = x.Value.Select(z => new Dtos.Setting { Name = z.Name, Value = z.Value })
                    .ToArray();

                return new KeyValuePair<string, IEnumerable<Dtos.Setting>>(x.Key, settings);
            }));

            var serializedSettings = JsonConvert.SerializeObject(globalSettings, _serializerSettings);

            Properties.Settings.Default.GlobalSettings = serializedSettings;
            Properties.Settings.Default.Save();
        }

        private Settings CreateSettings(IEnumerable<Dtos.Setting> value)
        {
            var settings = value.Select(x => new Setting(x.Name, x.Value))
                .ToArray();

            return new Settings(settings, _persist);
        }
    }
}