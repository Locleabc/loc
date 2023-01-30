using LOC.Define;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LOC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex;

        [DllImport("user32.dll")]  //chưa hiểu
        [return: MarshalAs(UnmanagedType.Bool)] //chưa hiểu
        static extern bool SetForegroundWindow(IntPtr hWnd); //chưa hiểu
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string AssociatedFilePath = CheckAssociatedFile(e);
            PreventMultipleApplicationLaunch();

            Cdef.mainView.DataContext = Cdef.mainViewModel;
            Cdef.mainView.Show();

            Cdef.massageView.DataContext = Cdef.messageViewModel;
            Cdef.massageView.Show();
        }
        private string CheckAssociatedFile(StartupEventArgs e)
        {
            string AssociatedFileFullPath = null;

            if (e.Args.Length == 1)
            {
                if (Path.GetExtension(e.Args[0]).ToLower() == ".teq")
                {
                    AssociatedFileFullPath = e.Args[0];
                }
            }

            return AssociatedFileFullPath;
        }

        private void PreventMultipleApplicationLaunch()
        {
            bool createdNew;
            // Do not change the GUID inside Mutex
            _mutex = new Mutex(true, "35FDD248-462D-478F-877C-8AC1B39930DE", out createdNew);

            if (!createdNew)
            {
                // Bring other instance to front and exit.
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }

                MessageBox.Show("Application Already Launch!!");

                Application.Current.Shutdown();
            }
        }
    }
}
