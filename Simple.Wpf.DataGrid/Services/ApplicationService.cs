using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace Simple.Wpf.DataGrid.Services
{
    public sealed class ApplicationService : IApplicationService
    {
        private string _logFolder;

        public string LogFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(_logFolder)) return _logFolder;

                _logFolder = GetLogFolder();
                return _logFolder;
            }
        }

        public void CopyToClipboard(string text)
        {
            Clipboard.SetText(text);
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void Restart()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public void OpenFolder(string folder)
        {
            Process.Start("explorer.exe", folder);
        }

        private static string GetLogFolder()
        {
            var logFile = LogManager.Configuration.AllTargets
                .OfType<FileTarget>()
                .Select(x => x.FileName as SimpleLayout)
                .Select(x => x.Text)
                .FirstOrDefault();

            return Path.GetDirectoryName(logFile);
        }
    }
}