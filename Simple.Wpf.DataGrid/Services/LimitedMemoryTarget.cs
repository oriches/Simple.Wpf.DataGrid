using System.Collections.Generic;
using System.ComponentModel;
using NLog;
using NLog.Targets;

namespace Simple.Wpf.DataGrid.Services
{
    [Target("LimitedMemory")]
    public sealed class LimitedMemoryTarget : TargetWithLayout
    {
        private readonly Queue<string> _logs = new Queue<string>();

        public LimitedMemoryTarget()
        {
            Limit = 1000;
        }

        public IEnumerable<string> Logs => _logs;

        [DefaultValue(1000)] public int Limit { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            var msg = Layout.Render(logEvent);

            _logs.Enqueue(msg);
            if (_logs.Count > Limit) _logs.Dequeue();
        }
    }
}