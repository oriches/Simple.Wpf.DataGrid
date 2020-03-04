using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Wpf.DataGrid
{
    public static class Constants
    {
        public static class StartsWith
        {
            public static class Unit
            {
                public static IEnumerable<System.Reactive.Unit> Default = new[] {System.Reactive.Unit.Default}
                    .ToArray();

                public static IEnumerable<object> DefaultBoxed = new[] {System.Reactive.Unit.Default}
                    .Cast<object>()
                    .ToArray();
            }

            public static class Boolean
            {
                public static IEnumerable<bool> False = new[] {false}
                    .ToArray();

                public static IEnumerable<bool> True = new[] {true}
                    .ToArray();
            }
        }

        // ReSharper disable once InconsistentNaming
        public static class UI
        {
            public const string ExceptionTitle = "whoops - something's gone wrong!";

            public static readonly TimeSpan MessageDelay = TimeSpan.FromMilliseconds(250);

            public static class Diagnostics
            {
                public const string DefaultCpuString = "CPU: 00 %";
                public const string DefaultManagedMemoryString = "Managed Memory: 00 Mb";
                public const string DefaultTotalMemoryString = "Total Memory: 00 Mb";

                public static readonly TimeSpan Heartbeat = TimeSpan.FromSeconds(5);
                public static readonly TimeSpan UiFreeze = TimeSpan.FromMilliseconds(500);
                public static readonly TimeSpan UiFreezeTimer = TimeSpan.FromMilliseconds(333);

                public static readonly TimeSpan DiagnosticsLogInterval = TimeSpan.FromSeconds(1);
                public static readonly TimeSpan DiagnosticsIdleBuffer = TimeSpan.FromMilliseconds(666);
                public static readonly TimeSpan DiagnosticsCpuBuffer = TimeSpan.FromMilliseconds(666);
                public static readonly TimeSpan DiagnosticsSubscriptionDelay = TimeSpan.FromMilliseconds(1000);
            }

            public static class Grids
            {
                public const string DateFormat = "{0:d}";
                public const string DateTimeFormat = "{0:G}";

                public static readonly TimeSpan FilterThrottle = TimeSpan.FromMilliseconds(333);
                public static readonly TimeSpan ScrollingThrottle = TimeSpan.FromMilliseconds(250);
                public static readonly TimeSpan UpdatesInfoThrottle = TimeSpan.FromMilliseconds(500);

                public static readonly string ColumnNameSeparator = "_";
                public static readonly string ColumnNameDisplaySeparator = " ";

                public class SeparatorsFormat
                {
                    public bool? Separators { get; set; }

                    public int? Rounding { get; set; }

                    public string Format { get; set; }
                }

                public static class PredefinedColumns
                {
                    public static readonly string Id = "id";
                    public static readonly string CreatedOn = "created_on";
                    public static readonly string ModifiedOn = "modified_on";
                }

                public static class Transitions
                {
                    public static string Default = "Default";
                    public static string NewPositive = "NewPositive";
                    public static string NewNegative = "NewNegative";
                }
            }

            public static class Settings
            {
                public static class Names
                {
                    public const string Columns = "Columns";
                    public const string VisibleColumns = "VisibleColumns";
                }
            }
        }
    }
}