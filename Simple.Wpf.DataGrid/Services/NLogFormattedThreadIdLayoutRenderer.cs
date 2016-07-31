namespace Simple.Wpf.DataGrid.Services
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using NLog;
    using NLog.LayoutRenderers;

    [LayoutRenderer("formatted_threadid")]
    public sealed class NLogFormattedThreadIdLayoutRenderer : LayoutRenderer
    {
        private readonly Func<int> _threadIdFunc;

        public NLogFormattedThreadIdLayoutRenderer()
        {
            _threadIdFunc = () => Thread.CurrentThread.ManagedThreadId;
        }

        public NLogFormattedThreadIdLayoutRenderer(Func<int> threadIdFunc)
        {
            _threadIdFunc = threadIdFunc;
        }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var threadId = _threadIdFunc();
            if (threadId < 10)
            {
                builder.Append("0");
            }

            builder.Append(threadId.ToString(CultureInfo.InvariantCulture));
        }
    }
}