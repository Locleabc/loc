using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TopCom;
using TopCom.LOG;
using TopVision.Grabbers;
using TopVision.Lights;
using TopVision.Models;
using VCM_FullAssy.Define;
using VCM_FullAssy.MES;
using VCM_FullAssy.MVVM.Views;
using VCM_FullAssy.Processing;

namespace VCM_FullAssy.MVVM.ViewModels
{
    public class InitViewModel : PropertyChangedNotifier
    {
        #region Properties
        public event EventHandler InitStartedEvent;
        public event EventHandler InitCompletedEvent;

        public string InitStatus
        {
            get { return _InitStatus; }
            set
            {
                if (_InitStatus == value) return;

                _InitStatus = value;
                InitStatusDetail = "";
                OnPropertyChanged();
            }
        }

        public string InitStatusDetail
        {
            get { return _InitStatusDetail; }
            set
            {
                _InitStatusDetail = value;
                OnPropertyChanged();
            }
        }

        public bool InitCompleted
        {
            get { return _InitCompleted; }
            set
            {
                _InitCompleted = value;
                OnPropertyChanged();
                OnPropertyChanged("NotInitCompleted");
            }
        }

        public bool NotInitCompleted
        {
            get { return !InitCompleted; }
        }
        #endregion

        #region Constructors
        public InitViewModel()
        {
        }
        #endregion

        #region Methods
        #region Init Methods
        public void Initialize()
        {
            InitCompleted = false;
            if (InitStartedEvent != null)
            {
                InitStartedEvent.Invoke(this, EventArgs.Empty);
            }

            Task initTask = new Task(() =>
            {
                InitStatus = "Initialization Started.";
                Thread.Sleep(300);

                InitProgramFolder();
                InitMotion();   // Motion Init before Process Init
                InitProcessing();
                InitRecipe();
                InitVision();
                InitWorkData();
                //InitMES();

                OpenAgentApp();

                InitStatus = "Initialization Done.";
                Thread.Sleep(1000);

                CDef.IO.Output.TowerLamp_Idle = true;

                InitCompleted = true;

                if (InitCompletedEvent != null)
                {
                    InitCompletedEvent.Invoke(this, EventArgs.Empty);
                }
            });

            //if (TopLisense.CryptoService.IsValid == true) // Uncomment this to enable License
            //{
            initTask.Start();
            //}
        }

        public void InitMotion()
        {
            InitStatus = "Initializing motion...";

            CDef.AllAxis = new CAllAxis();
            CDef.AllAxis.Load();

            foreach (var motion in CDef.AllAxis.AxisList)
            {
                InitStatusDetail = $"Initializing axis {motion.AxisName}";
                motion.Initialization();

                Thread.Sleep(200);
            }
        }

        public void InitProcessing()
        {
            InitStatus = "Initializing process...";

            CDef.RootProcess.InitProcess();

            Thread.Sleep(200);
        }

        public void InitRecipe()
        {
            InitStatus = "Initializing Recipe...";

            CDef.GlobalRecipe = new CGlobalRecipe().Load<CGlobalRecipe>();
            CDef.CommonRecipe = new CCommonRecipe().Load<CCommonRecipe>();
            CDef.LeftTrayRecipe = new CLeftTrayRecipe().Load<CLeftTrayRecipe>();
            CDef.RightTrayRecipe = new CRightTrayRecipe().Load<CRightTrayRecipe>();
            CDef.TransferRecipe = new CTransferRecipe().Load<CTransferRecipe>();
            CDef.HeadRecipe = new CHeadRecipe().Load<CHeadRecipe>();
            CDef.UnderVisionRecipe = new CUnderVisionRecipe().Load<CUnderVisionRecipe>();
            CDef.UpperVisionRecipe = new CUpperVisionRecipe().Load<CUpperVisionRecipe>();

            Thread.Sleep(200);
        }

        public void InitVision()
        {
            InitStatus = "Initializing Vision...";

            CDef.TopCamera = new CameraBaslerGigE("TOP");
            CDef.BotCamera = new CameraBaslerGigE("BOTTOM")
            {
                SimulationImageDirectory = CSimulation.BotImagePath
            };
            CDef.LightController = new LightControllerDLS("COM5", 9600);

            Thread.Sleep(200);
        }

        public void InitProgramFolder()
        {
            InitStatus = "Initializing Program Folder...";

            ProgramFolder.Check();

            Thread.Sleep(200);
        }

        public void InitWorkData()
        {
            InitStatus = "Initializing Program Folder...";

            Datas.WorkData.Load();

            Thread.Sleep(200);
        }

        public void InitMES()
        {
            InitStatus = "Initializing MES...";

            CDef.MES.InitMES();

            Thread.Sleep(200);
        }

        private bool ProgramIsRunning(string FullPath)
        {
            string FilePath = Path.GetDirectoryName(FullPath);
            string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
            bool isRunning = false;

            Process[] pList = Process.GetProcessesByName(FileName);

            foreach (Process p in pList)
            {
                if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    isRunning = true;
                    break;
                }
            }

            return isRunning;
        }
        private void OpenAgentApp()
        {
            if (CDef.GlobalRecipe.FileThatRunWithTheApplication == "" || CDef.GlobalRecipe.FileThatRunWithTheApplication == null)
            {
                return;
            }

            if (ProgramIsRunning(CDef.GlobalRecipe.FileThatRunWithTheApplication))
            {
                return;
            }

            ProcessStartInfo _OpenAgentApp = new ProcessStartInfo();

            _OpenAgentApp.FileName = CDef.GlobalRecipe.FileThatRunWithTheApplication;
            _OpenAgentApp.WindowStyle = ProcessWindowStyle.Maximized;

            Process OpenAgentApp = new Process();
            OpenAgentApp.StartInfo = _OpenAgentApp;

            try
            {

                OpenAgentApp.Start();
            }
            catch (Exception ex)
            {
                CDef.MessageViewModel.Show($"{ex.Message} \"{CDef.GlobalRecipe.FileThatRunWithTheApplication}\"");
                UILog.Warning($"{ex.Message} \"{CDef.GlobalRecipe.FileThatRunWithTheApplication}\"");
            }
        }
        #endregion
        #endregion

        #region Privates
        private string _InitStatus;
        private string _InitStatusDetail;

        #endregion// Set InitComplete Default as true, for showing main content in design mode
        private bool _InitCompleted = false;
    }
}
