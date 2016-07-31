namespace Simple.Wpf.DataGrid.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using NLog;

    public static class LogHelper
    {
        private static readonly IEnumerable<LogLevel> AllLevels = new[]
                                                                  {
                                                                      LogLevel.Trace,
                                                                      LogLevel.Debug,
                                                                      LogLevel.Info,
                                                                      LogLevel.Warn,
                                                                      LogLevel.Error,
                                                                      LogLevel.Fatal
                                                                  };

        public static void ReconfigureLoggerToLevel(LogLevel level)
        {
            var disableLevels = AllLevels.Where(x => x < level)
                .ToArray();

            var enableLevels = AllLevels.Where(x => x >= level)
                .ToArray();

            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                var localRule = rule;

                disableLevels.ForEach(x => localRule.DisableLoggingForLevel(x));
                enableLevels.ForEach(x => localRule.EnableLoggingForLevel(x));
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
}