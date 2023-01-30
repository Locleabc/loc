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
using TopCom.Define;
using TopCom.Mqtt;
using TopVision.Grabbers;
using TopVision.Lights;
using TopVision.Models;
using PLV_BracketAssemble.Define;
using PLV_BracketAssemble.MVVM.Views;
using PLV_BracketAssemble.Processing;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class InitViewModel : PropertyChangedNotifier
    {
        #region Properties
        public event EventHandler InitStartedEvent;

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

        public string InitReturnMessage
        {
            get { return _InitReturnMessage; }
            set
            {
                if (string.IsNullOrEmpty(_InitReturnMessage))

                    _InitReturnMessage = value;
            }
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

            Task initTask = new Task(async () =>
            {
                TopCom.LOG.UILog.Info($"-----------------------------------------------------VERSION INFOMATION-----------------------------------------------------");
                TopCom.LOG.UILog.Info($"-----[Machine    ] {MachineInfor.MachineName,-100}-----");
                TopCom.LOG.UILog.Info($"-----[Version    ] {MachineInfor.SoftwareVersion,-100}-----");
                TopCom.LOG.UILog.Info($"-----[Description] {MachineInfor.VersionDescription,-100}-----");
                TopCom.LOG.UILog.Info($"-----------------------------------------------------VERSION INFOMATION-----------------------------------------------------");
                InitStatus = "Initialization Started.";
                Thread.Sleep(1000);

                InitGlobalFolders();
                Thread.Sleep(200);
                InitProcessing();
                Thread.Sleep(200);
                InitRecipe();
                Thread.Sleep(200);
                InitMotion();
                Thread.Sleep(200);
                InitIO();
                Thread.Sleep(200);
                InitVision();
                Thread.Sleep(200);
                InitVisionVM();
                Thread.Sleep(200);
                InitWorkData();
                Thread.Sleep(200);
                //InitMES();
                InitMqtt();
                Thread.Sleep(200);

                InitStatus = "Initialization Done.";
                Thread.Sleep(300);

                CDef.IO.Output.TowerLamp_Idle = true;

                InitCompleted = true;

                if (string.IsNullOrEmpty(InitReturnMessage) == false)
                {
                    CDef.MessageViewModel.Show(InitReturnMessage + "\nCheck Log for more infomation!",
                                               isAlarm: true,
                                               caption: "ERROR");
                }
                else
                {
                    TopCom.LOG.UILog.Info("Machine turned on successed");
                }

                CDef.MainViewModel.SwitchToMainContent();
            });

            initTask.Start();
        }

        public void InitMqtt()
        {
            InitStatus = "Initializing Mqtt...";

            MqttGlobal.Client.Initialize();
        }

        public void InitMotion()
        {
            InitStatus = "Initializing motion...";

            CDef.AllAxis = new CAllAxis();
            CDef.AllAxis.Load();
            CDef.AllAxis.Save();

            foreach (var motion in CDef.AllAxis.AxisList)
            {
                InitStatusDetail = $"Initializing axis {motion.AxisName}";
                if (motion.Initialization() != TopCom.Define.EMotionRtnCode.RTN_OK)
                {
                    InitReturnMessage = $"Problem detected while initializing motion {motion.AxisName}.";
                }

#if SIMULATION
                Thread.Sleep(200);
#endif
            }
        }

        public void InitIO()
        {
            InitStatus = "Initializing IO Signal...";

            // Connect to COMPORT 2, so that the COMPORT 2's IO can be Read / Write
#if !SIMULATION
            if (FASTECH.EziMOTIONPlusRLib.FAS_Connect(2, 115200) == 0)
            {
                InitReturnMessage = "COM2 Connect Error";
            }
#endif
        }

        public void InitProcessing()
        {
            InitStatus = "Initializing process...";

            CDef.RootProcess.InitProcess();
        }

        public void InitRecipe()
        {
            InitStatus = "Initializing Recipe...";
            CDef.GlobalRecipe = new CGlobalRecipe().Load<CGlobalRecipe>();
            CDef.CommonRecipe = new CCommonRecipe().Load<CCommonRecipe>();
            
            CDef.TrayRecipe = new CTrayRecipe().Load<CTrayRecipe>();
            CDef.HeadRecipe = new CHeadRecipe().Load<CHeadRecipe>();
            CDef.UnderVisionRecipe = new CUnderVisionRecipe().Load<CUnderVisionRecipe>();
        }

        public async void InitVision()
        {
            InitStatus = "Initializing Vision...";

            CDef.BotCamera = new CameraBaslerGigE("BOTTOM")
            {
                SimulationImageDirectory = CSimulation.BotImagePath
            };

            CDef.LightController = new LightControllerDLS($"COM{CDef.UnderVisionRecipe.LightComport}", 9600, withConnect: false);

            bool botCamConnected = await CDef.BotCamera.ConnectAsync();
            if (botCamConnected == false)
            {
                InitReturnMessage = $"Camera {CDef.BotCamera.Name} connected failed.";
            }
            if (CDef.LightController.Open() == false)
            {
                InitReturnMessage = $"Connect to Light controller {CDef.LightController.Name} failed.";
            }
            else
            {
                CDef.LightController.SetLightStatus(1, false);
            }

            try
            {
                CDef.BotCamera.ExposureTime = CDef.UnderVisionRecipe.UnderCamera_ExposureTime;
            }
            catch (Exception ex)
            {
                InitReturnMessage = $"Setting camera exposure failed. {ex.Message}";
            }
        }

        public void InitVisionVM()
        {
            InitStatus = "Initializing Vision View...";
            CDef.MainViewModel.MainContentVM.VisionAutoVM.VisionAutoViewModelInit();
        }

        public void InitGlobalFolders()
        {
            InitStatus = "Initializing Program Folder...";
            GlobalFolders.Check();
            LocalFolders.Check();
        }

        public void InitWorkData()
        {
            InitStatus = "Initializing Workdata...";
            Datas.WorkData.Load();
        }

        public void InitMES()
        {
            InitStatus = "Initializing MES...";
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
        #endregion
        #endregion

        #region Privates
        private string _InitStatus;
        private string _InitStatusDetail;

        // Set InitComplete Default as true, for showing main content in design mode
        private bool _InitCompleted = false;
        private string _InitReturnMessage = "";
        #endregion
    }
}
