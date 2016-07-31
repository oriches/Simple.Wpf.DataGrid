namespace Simple.Wpf.DataGrid.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using NLog;

    public static class CultureService
    {
        private static readonly IDictionary<string, CultureInfo> Cultures = new Dictionary<string, CultureInfo>()
        {
            {"English (UK)", new CultureInfo("en-GB")},
            {"English (USA)", new CultureInfo("en-US")},
            {"French (FRA)", new CultureInfo("fr-FR")},
            {"German (DEU)", new CultureInfo("de-DE")},
            {"Chinese (SGP)", new CultureInfo("zh-SG")}
        };

        private static readonly BehaviorSubject<string> Changed;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static CultureService()
        {
            using (Duration.Measure(Logger, "Constructor - " + typeof(CultureService).Name))
            {
                Thread.CurrentThread.CurrentCulture = Cultures.First().Value;
                Thread.CurrentThread.CurrentUICulture = Cultures.First().Value;

                Changed = new BehaviorSubject<string>(Cultures.First().Key);
            }
        }

        public static IEnumerable<string> AvailableCultures => Cultures.Keys;

        public static IObservable<string> CultureChanged => Changed;

        public static string CurrentCulture => CultureChanged
            .Take(1)
            .Wait();

        public static void SetCulture(string cultureName)
        {
            CultureInfo culture;
            if (Cultures.TryGetValue(cultureName, out culture))
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Changed.OnNext(cultureName);
            }
        }
    }
}