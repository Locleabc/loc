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
using TOPV_Dispenser.Define;
using TOPV_Dispenser.MES;
using TOPV_Dispenser.MVVM.Views;
using TOPV_Dispenser.Processing;

namespace TOPV_Dispenser.MVVM.ViewModels
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

        public string InitReturnMessage
        {
            get { return _InitReturnMessage; }
            set
            {
                if (string.IsNullOrEmpty(_InitReturnMessage))
                {
                    _InitReturnMessage = value;
                }
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

            Task initTask = new Task(() =>
            {
                TopCom.LOG.UILog.Info($"-----VERSION INFOMATION {"",-95}-----");
                TopCom.LOG.UILog.Info($"-----[Machine    ] {MachineInfor.MachineName,-100}-----");
                TopCom.LOG.UILog.Info($"-----[Version    ] {MachineInfor.SoftwareVersion,-100}-----");
                TopCom.LOG.UILog.Info($"-----[Description] {MachineInfor.VersionDescription,-100}-----");
                TopCom.LOG.UILog.Info($"-----VERSION INFOMATION {"", -95}-----");
                InitStatus = "Initialization Started.";
                Thread.Sleep(300);

                InitGlobalFolders();
                InitProcessing();
                InitRecipe();
                InitMotion();
                InitVision();
                InitWorkData();
                //InitMES();
                InitMqtt();

                //20211109 Open LinkAgent.exe when Start PGM
                OpenAgentApp();

                InitStatus = "Initialization Done.";
                Thread.Sleep(300);

                CDef.IO.Output.TowerLamp_Idle = true;
                CDef.IO.Output.IonizerSol = true;

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

                if (InitCompletedEvent != null)
                {
                    InitCompletedEvent.Invoke(this, EventArgs.Empty);
                }
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
            InitStatus = "Initializing Ajin Board...";

            if (CDef.AjinBoard.Connect() == false)
            {
                InitReturnMessage = $"Problem detected while initializing Ajin Board.";

#if SIMULATION
                Thread.Sleep(100);
#endif
            }

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
#if USPCUTTING
            CDef.PressRecipe = new CPressRecipe().Load<CPressRecipe>();
#endif

            Thread.Sleep(200);
        }

        public async void InitVision()
        {
            InitStatus = "Initializing Vision...";

            CDef.TopCamera = new CameraBaslerGigE("TOP");
            CDef.BotCamera = new CameraBaslerGigE("BOTTOM")
            {
                SimulationImageDirectory = CSimulation.BotImagePath
            };

            CDef.LightController = new LightControllerDLS($"COM{CDef.CommonRecipe.LightComport}", 9600, withConnect: false);

            bool topCamConnected = await CDef.TopCamera.ConnectAsync();
            if (topCamConnected == false)
            {
                InitReturnMessage = $"Camera {CDef.TopCamera.Name} connected failed.";
            }
            bool botCamConnected = await CDef.BotCamera.ConnectAsync();
            if (botCamConnected == false)
            {
                InitReturnMessage = $"Camera {CDef.BotCamera.Name} connected failed.";
            }
            if (CDef.LightController.Open() == false)
            {
                InitReturnMessage = $"Connect to Light controller {CDef.LightController.Name} failed.";
            }

            try
            {
                //CDef.TopCamera.ExposureTime = CDef.UpperVisionRecipe.UpperCamera_ExposureTime;
                //CDef.BotCamera.ExposureTime = CDef.UnderVisionRecipe.UnderCamera_ExposureTime;
            }
            catch (Exception ex)
            {
                InitReturnMessage = $"Setting camera exposure failed. {ex.Message}";
            }

            Thread.Sleep(200);
        }

        public void InitGlobalFolders()
        {
            InitStatus = "Initializing Program Folder...";

            GlobalFolders.Check();
            LocalFolders.Check();

            Thread.Sleep(200);
        }

        public void InitWorkData()
        {
            InitStatus = "Initializing Workdata...";

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
                CDef.RootProcess.SetWarning($"{ex.Message} \"{CDef.GlobalRecipe.FileThatRunWithTheApplication}\"");
            }
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
