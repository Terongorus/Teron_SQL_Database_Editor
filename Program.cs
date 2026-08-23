using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Logic
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using Mutex mutex = new Mutex(true, "TeronSQLDatabaseEditor.SingleInstance", out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show($"{AppInfo.DisplayName} is already running.", AppInfo.DisplayName, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var app = new Application();
            app.DispatcherUnhandledException += (sender, e) =>
            {
                LogException(e.Exception);
                e.Handled = true;
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => LogException(e.ExceptionObject as Exception);
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                LogException(e.Exception);
                e.SetObserved();
            };

            app.Run(Globals.startup);
        }

        internal static void LogException(Exception? ex)
        {
            if (ex == null)
                return;

            try
            {
                Directory.CreateDirectory(Globals.AppDataRoot);
                string logPath = Path.Combine(Globals.AppDataRoot, "error.log");
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}{Environment.NewLine}{Environment.NewLine}");
            }
            catch
            {
                // Logging must never crash the app.
            }
        }
    }
}
