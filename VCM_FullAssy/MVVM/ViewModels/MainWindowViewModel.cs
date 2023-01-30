using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TopCom;
using TopCom.Command;
using TopCom.Processing;
using TopCom.LOG;
using TopUI.Controls;
using TopUI.Models;
using VCM_FullAssy.Define;
using VCM_FullAssy.Processing;

namespace VCM_FullAssy.MVVM.ViewModels
{
    public class MainWindowViewModel : PropertyChangedNotifier
    {
        #region Properties
        #region Properties_ViewModel
        public LicenseActiveViewModel LicenseActiveVM
        {
            get
            {
                return _LicenseActiveVM ?? (_LicenseActiveVM = new LicenseActiveViewModel());
            }
        }

        public HeaderViewModel HeaderVM
        {
            get
            {
                return _HeaderVM ?? (_HeaderVM = new HeaderViewModel());
            }
        }

        public InitViewModel InitVM
        {
            get
            {
                return _InitVM ?? (_InitVM = new InitViewModel());
            }
        }

        public AutoViewModel AutoVM
        {
            get
            {
                return _AutoVM ?? (_AutoVM = new AutoViewModel());
            }
        }

        public ManualViewModel ManualVM
        {
            get
            {
                return _ManualVM ?? (_ManualVM = new ManualViewModel());
            }
        }

        public IOViewModel IOVM
        {
            get
            {
                return _IOVM ?? (_IOVM = new IOViewModel());
            }
        }

        public RecipeViewModel RecipeVM
        {
            get
            {
                return _RecipeVM ?? (_RecipeVM = new RecipeViewModel());
            }
        }

        public StatisticViewModel StatisticVM
        {
            get
            {
                return _StatisticVM ?? (_StatisticVM = new StatisticViewModel());
            }
        }

        public FooterViewModel FooterVM
        {
            get
            {
                return _FooterVM ?? (_FooterVM = new FooterViewModel());
            }
        }

        public VisionAutoViewModel VisionAutoVM
        {
            get
            {
                return _VisionAutoVM ?? (_VisionAutoVM = new VisionAutoViewModel());
            }
        }

        public MESPMStatusViewModel MESPMStatusVM
        {
            get
            {
                return _MESPMStatusVM ?? (_MESPMStatusVM = new MESPMStatusViewModel());
            }
        }
        #endregion
        #endregion

        #region Commands
        public RelayCommand WindowLoadDone
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    InitVM.Initialize();
                });
            }
        }

        #region Auto Commands
        public RelayCommand HomeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    UILog.Info("Home Button Clicked!");

                    if (CDef.RootProcess.Mode != ProcessingMode.ModeRun &&
                       CDef.RootProcess.Mode != ProcessingMode.ModeOrigin &&
                       (int)CDef.RootProcess.Mode % 2 != 1)
                    {
                        CDef.RootProcess.OperationCommand = OperatingMode.Origin;
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
        #endregion

        #region Privates
        private LicenseActiveViewModel _LicenseActiveVM;
        private HeaderViewModel _HeaderVM;
        private InitViewModel _InitVM;
        private AutoViewModel _AutoVM;
        private ManualViewModel _ManualVM;
        private IOViewModel _IOVM;
        private RecipeViewModel _RecipeVM;
        private StatisticViewModel _StatisticVM;
        private FooterViewModel _FooterVM;
        private VisionAutoViewModel _VisionAutoVM;
        private MESPMStatusViewModel _MESPMStatusVM;
        #endregion
    }
}
