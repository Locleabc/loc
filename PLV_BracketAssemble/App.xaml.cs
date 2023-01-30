﻿using PLV_BracketAssemble.Define;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace PLV_BracketAssemble
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string TopLangNamespaceName = "TopLang";
        private Mutex _mutex;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void SelectCulture(string culture)
        {
            if (String.IsNullOrEmpty(culture))
                return;

            //Copy all MergedDictionarys into a auxiliar list.
            var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();

            //Search for the specified culture.     
            string requestedCulture = $"pack://application:,,,/{TopLangNamespaceName};component/Generics/Language.{culture}.xaml";
            var resourceDictionary = dictionaryList.
                FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

            if (resourceDictionary == null)
            {
                //If not found, select our default language.             
                requestedCulture = $"pack://application:,,,/{TopLangNamespaceName};component/Generics/Language.xaml";
                resourceDictionary = dictionaryList.
                    FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
            }

            //If we have the requested resource, remove it from the list and place at the end.     
            //Then this language will be our string table to use.      
            if (resourceDictionary != null)
            {
                Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            //Inform the threads of the new culture.     
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string AssociatedFilePath = CheckAssociatedFile(e);
            PreventMultipleApplicationLaunch();

            CDef.MainView.DataContext = CDef.MainViewModel;
            CDef.MainView.Show();

            CDef.MessageView.DataContext = CDef.MessageViewModel;
            CDef.MessageView.Show();
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
