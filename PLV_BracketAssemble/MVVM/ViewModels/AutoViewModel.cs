using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TopCom;
using TopCom.Command;
using TopCom.Processing;
using TopCom.LOG;
using TopUI.Controls;
using TopUI.Models;
using PLV_BracketAssemble.Define;
using PLV_BracketAssemble.Processing;
using System.Windows.Media;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class AutoViewModel : PropertyChangedNotifier
    {
        #region Properties
        public WorkDataViewModel WorkDataVM
        {
            get
            {
                return _WorkDataVM ?? (_WorkDataVM = new WorkDataViewModel());
            }
        }

        public TrayViewModel InputTrayVM
        {
            get
            {
                return _InputTrayVM ?? (_InputTrayVM = new TrayViewModel("Input Tray") { BackgroundColor = Brushes.NavajoWhite, });
            }
        }

        public TrayViewModel OutputTrayVM
        {
            get
            {
                return _OutputTrayVM ?? (_OutputTrayVM = new TrayViewModel("Output Tray") { BackgroundColor = Brushes.LightSeaGreen, }) ;
            }
        }
        #endregion

        #region Commands
        #region Auto Commands
        public RelayCommand HomeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    UILog.Info("Home Button Clicked!");

                    CDef.MessageViewModel.ShowDialog("Do you want to HOME machine?");
                    if (CDef.MessageViewModel.Result == false)
                    {
                        UILog.Info("User denied HOME!");
                        return;
                    }

                    UILog.Info("User confirmed HOME!");

                    if (CDef.RootProcess.Mode != ProcessingMode.ModeRun &&
                        CDef.RootProcess.Mode != ProcessingMode.ModeOrigin &&
                        (int)CDef.RootProcess.Mode % 2 != 1)
                    {
                        CDef.RootProcess.OperationCommand = OperatingMode.Origin;
                    }
                });
            }
        }

        public RelayCommand ChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    UILog.Info("Change Button Clicked!");

                    if (CDef.RootProcess.Mode != ProcessingMode.ModeOrigin &&
                        (int)CDef.RootProcess.Mode % 2 != 1)
                    {
                        CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                        CDef.RootProcess.OperationCommand = OperatingMode.Run;
                    }
                });
            }
        }

        public RelayCommand StartCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    UILog.Info("Start Button Clicked!");

                    if (CDef.RootProcess.Mode != ProcessingMode.ModeRun &&
                       CDef.RootProcess.Mode != ProcessingMode.ModeOrigin &&
                       (int)CDef.RootProcess.Mode % 2 != 1)
                    {
                        CDef.RootProcess.RunMode = ERunMode.AutoRun;
                        CDef.RootProcess.OperationCommand = OperatingMode.Run;
                    }
                });
            }
        }

        public RelayCommand StopCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    UILog.Info("Stop Button Clicked!");

                    CDef.RootProcess.OperationCommand = OperatingMode.Stop;
                });
            }
        }
        #endregion
        #endregion

        #region Constructors
        public AutoViewModel()
        {
        }
        #endregion

        #region Methods
        #endregion

        #region Privates
        private WorkDataViewModel _WorkDataVM;
        private TrayViewModel _InputTrayVM;
        private TrayViewModel _OutputTrayVM;
        #endregion
    }
}
