using ProcessFocus.Services;

using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ProcessFocus
{
    public class ProcessFocusApplicationContext : ApplicationContext
    {
        #region ..Private Static
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int WAITTIME = int.Parse(ConfigurationManager.AppSettings["waitTime"]);
        private static string APPNAME = ConfigurationManager.AppSettings["appName"];
        #endregion

        #region ..Private
        private NotifyIcon trayIcon;
        #endregion

        #region ..Public Methods
        public ProcessFocusApplicationContext()
        {
            log.Info("init");
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = ProcessFocus.AppResources.MainIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Exit", Exit)
            }),
                Visible = true
            };


            ProcessWatcher procWatcher = new ProcessWatcher(APPNAME);
            procWatcher.ProcessCreated += ProcWatcher_ProcessCreated;
            procWatcher.ProcessDeleted += ProcWatcher_ProcessDeleted;
            procWatcher.Start();
        }
        #endregion
        
        #region ..Events
        private static void ProcWatcher_ProcessDeleted(Win32_Process proc)
        {
            log.Info($"PROCESS {APPNAME} Deleted");
        }

        private static void ProcWatcher_ProcessCreated(Win32_Process proc)
        {
            log.Info($"PROCESS {APPNAME} Created, waiting for {WAITTIME} to bring to front");
            Thread.Sleep(WAITTIME);
            log.Info($"    moving PROCESS {APPNAME} to front");

            var currentProcess = Process.GetProcessById(int.Parse(proc.Handle));
            WindowHelper.bringToFront(currentProcess.MainWindowHandle);
        }        

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Application.Exit();
        }
        #endregion
    }
}
