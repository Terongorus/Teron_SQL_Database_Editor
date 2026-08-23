using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                MessageBox.Show($"{AppInfo.DisplayName} is already running.", AppInfo.DisplayName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => LogException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => LogException(e.ExceptionObject as Exception);
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                LogException(e.Exception);
                e.SetObserved();
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Globals.startup);
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
