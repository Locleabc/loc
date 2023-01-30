using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom.Mqtt;
using TopCom;
using TopVision.Grabbers;
using TopVision.Lights;
using LOC.Define;
using System.IO;

namespace LOC.MVVM.ViewModels
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
                TopCom.LOG.UILog.Info($"-----[Machine    ] {MachineInfo.MachineName,-100}-----");
                TopCom.LOG.UILog.Info($"-----[Version    ] {MachineInfo.SoftwareVersion,-100}-----");
                TopCom.LOG.UILog.Info($"-----[Description] {MachineInfo.VersionDescription,-100}-----");
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

                //Cdef.IO.Output.TowerLamp_Idle = true; chú ý

                InitCompleted = true;

                if (string.IsNullOrEmpty(InitReturnMessage) == false)
                {
                    Cdef.messageViewModel.Show(InitReturnMessage + "\nCheck Log for more infomation!",
                                               isAlarm: true,
                                               caption: "ERROR");
                }
                else
                {
                    TopCom.LOG.UILog.Info("Machine turned on successed");
                }

                Cdef.mainViewModel.SwitchToMainTabControl();// cần test
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

            //Cdef.AllAxis = new CAllAxis(); chú ý
            //Cdef.AllAxis.Load();
            //Cdef.AllAxis.Save();

//            foreach (var motion in Cdef.AllAxis.AxisList)
//            {
//                InitStatusDetail = $"Initializing axis {motion.AxisName}";
//                if (motion.Initialization() != TopCom.Define.EMotionRtnCode.RTN_OK)
//                {
//                    InitReturnMessage = $"Problem detected while initializing motion {motion.AxisName}.";
//                }

//#if SIMULATION
//                Thread.Sleep(200);
//#endif
//            }
        }

        public void InitIO()
        {
            InitStatus = "Initializing IO Signal...";

            // Connect to COMPORT 2, so that the COMPORT 2's IO can be Read / Write
#if !SIMULATION
            //if (FASTECH.EziMOTIONPlusRLib.FAS_Connect(2, 115200) == 0)                                chú ý
            //{
            //    InitReturnMessage = "COM2 Connect Error";
            //}
#endif
        }

        public void InitProcessing()
        {
            InitStatus = "Initializing process...";

            //Cdef.RootProcess.InitProcess();                                   chú ý
        }

        public void InitRecipe()
        {
            InitStatus = "Initializing Recipe...";
            //Cdef.GlobalRecipe = new CGlobalRecipe().Load<CGlobalRecipe>();
            //Cdef.CommonRecipe = new CCommonRecipe().Load<CCommonRecipe>();

            //Cdef.TrayRecipe = new CTrayRecipe().Load<CTrayRecipe>();
            //Cdef.HeadRecipe = new CHeadRecipe().Load<CHeadRecipe>();
            //Cdef.UnderVisionRecipe = new CUnderVisionRecipe().Load<CUnderVisionRecipe>();     chú ý
        }

        public async void InitVision()
        {
            InitStatus = "Initializing Vision...";

            //Cdef.BotCamera = new CameraBaslerGigE("BOTTOM")                                           chú ý
            //{
            //    SimulationImageDirectory = CSimulation.BotImagePath
            //};

            //Cdef.LightController = new LightControllerDLS($"COM{Cdef.UnderVisionRecipe.LightComport}", 9600, withConnect: false);

            //bool botCamConnected = await Cdef.BotCamera.ConnectAsync();
            //if (botCamConnected == false)
            //{
            //    InitReturnMessage = $"Camera {Cdef.BotCamera.Name} connected failed.";
            //}
            //if (Cdef.LightController.Open() == false)
            //{
            //    InitReturnMessage = $"Connect to Light controller {Cdef.LightController.Name} failed.";
            //}
            //else
            //{
            //    Cdef.LightController.SetLightStatus(1, false);
            //}

            //try
            //{
            //    Cdef.BotCamera.ExposureTime = Cdef.UnderVisionRecipe.UnderCamera_ExposureTime;
            //}
            //catch (Exception ex)
            //{
            //    InitReturnMessage = $"Setting camera exposure failed. {ex.Message}";
            //}
        }

        public void InitVisionVM()
        {
            InitStatus = "Initializing Vision View...";
            //Cdef.mainViewModel.MainTabControlVM.VisionAutoVM.VisionAutoViewModelInit();           chú ý
        }

        public void InitGlobalFolders()                                      //cần test, đọc lại
        {
            InitStatus = "Initializing Program Folder...";
            GlobalFolders.Check();
            LocalFolders.Check();
        }

        public void InitWorkData()
        {
            InitStatus = "Initializing Workdata...";
            //Datas.WorkData.Load();                                            chú ý
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
