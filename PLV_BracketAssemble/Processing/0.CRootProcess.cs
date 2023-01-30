using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TopCom;
using TopCom.Processing;
using TopCom.MES;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using TopCom.Models;
using PLV_BracketAssemble.Define;

namespace PLV_BracketAssemble.Processing
{
    public partial class CRootProcess : ProcessingBase
    {
        #region Properties
        public CAlarmStatus AlarmStatus
        {
            get { return _AlarmStatus; }
            set
            {
                _AlarmStatus = value;
                if (value.MotionAlarmStatus != null)
                {
                    if (value.MotionAlarmStatus.IsAlarm)
                    {
                        //TODO: HANDLER ALARM EVENT
                        SetAlarm(value.MotionAlarmStatus.AlarmMessage);
                        Mode = ProcessingMode.ModeToAlarm;
                    }
                }

                if (value.UtilsAlarmStatus != null)
                {
                    if (value.UtilsAlarmStatus.IsAlarm)
                    {
                        //TODO: HANDLER ALARM EVENT
                        SetAlarm(value.UtilsAlarmStatus.AlarmMessage);
                        Mode = ProcessingMode.ModeToAlarm;
                    }
                }

                OnPropertyChanged("AlarmStatus");
            }
        }

        public bool IsMachineNotRunning
        {
            get
            {
                bool result;
                switch (Mode)
                {
                    case ProcessingMode.ModeToOrigin:
                    case ProcessingMode.ModeOrigin:
                    case ProcessingMode.ModeToRun:
                    case ProcessingMode.ModeRun:
                        result = false;
                        break;

                    case ProcessingMode.ModeNone:
                    case ProcessingMode.ModeToAlarm:
                    case ProcessingMode.ModeAlarm:
                    case ProcessingMode.ModeToStop:
                    case ProcessingMode.ModeStop:
                    case ProcessingMode.ModeToWarning:
                    case ProcessingMode.ModeWarning:
                        result = true;
                        break;

                    default:
                        result = false;
                        break;
                }

                return result;
            }
        }

        public CTrayProcess TrayProcess { get; set; }
        public CHeadProcess HeadProcess { get; set; }
        public CUnderVisionProcess UnderVisionProcess { get; set; }
        #endregion

        public OperatingMode OperationCommand
        {
            get
            {
                return _OperationCommand;
            }
            set
            {
                _OperationCommand = value;
                OnPropertyChanged();
            }
        }

        public ERunMode RunMode
        {
            get
            {
                return _RunMode;
            }
            set
            {
                if (value == _RunMode)
                {
                    return;
                }

                _RunMode = value;
                if (value != ERunMode.Stop)
                {
                    OperationCommand = OperatingMode.Run;
                }
                OnPropertyChanged("RunMode");
            }
        }

        public void SetAlarm(string message)
        {
            lock (setEventLockObject)
            {
                // Change RootProcess Mode
                Mode = ProcessingMode.ModeToAlarm;

                Log.Error(message);
                CDef.MainViewModel.MainContentVM.StatisticVM.StatisticHistory.AddRecord(
                    CDef.MainViewModel.MainContentVM.StatisticVM.StatisticHistory.AlarmRecords,
                    new CEventRecord()
                    {
                        Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Description = message,
                    }
                );
                // TODO: Update MES Status
                CDef.MessageViewModel.Show(message, isAlarm: true, caption: "ALARM");
            }
        }

        public void SetWarning(string message)
        {
            lock (setEventLockObject)
            {
                // Change RootProcess Mode
                Mode = ProcessingMode.ModeToWarning;

                Log.Warn(message);
                CDef.MainViewModel.MainContentVM.StatisticVM.StatisticHistory.AddRecord(
                    CDef.MainViewModel.MainContentVM.StatisticVM.StatisticHistory.WarningRecords,
                    new CEventRecord()
                    {
                        Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Description = message,
                    }
                );
                // TODO: Update MES Status
                CDef.MessageViewModel.Show(message, isAlarm: true, caption: "WARNING");
            }
        }

        #region Constructors
        public CRootProcess(string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
            Thread ImageDeleteThread = new Thread(ImageDeleteHandler);
            ImageDeleteThread.IsBackground = true;
            ImageDeleteThread.Start();
        }

        private void ImageDeleteHandler()
        {
            // Make sure all component is initialization done
            while(true)
            {
                Thread.Sleep(60000 * 5);

                // TODO: Update ImageSaveDay
                if (false/*CDef.GlobalRecipe.ImageSaveDay <= 0*/)
                {
                    continue;
                }

                if (IsMachineNotRunning == false)
                {
                    Thread.Sleep(60000 * 5);
                }
                else
                {
                    try
                    {
                        Regex reg = new Regex(@"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$");

                        DirectoryInfo imageRootFolder = new DirectoryInfo(@"D:\TOP\TOPVEQ\Images");
                        DirectoryInfo[] dayImageFolders = imageRootFolder.GetDirectories();

                        for (int i = 0; i < dayImageFolders.Count(); i++)
                        {
                            if (reg.IsMatch(dayImageFolders[i].Name))
                            {
                                string[] datetimeSegements = dayImageFolders[i].Name.Split('-');
                                int year = int.Parse(datetimeSegements[0]);
                                int month = int.Parse(datetimeSegements[1]);
                                int day = int.Parse(datetimeSegements[2]);

                                DateTime folderDateTime = new DateTime(year, month, day);
                                DateTime todayDateTime = DateTime.Today;

                                int diffDay = (todayDateTime.Date - folderDateTime.Date).Days;
                                // TODO: Update ImageSaveDay
                                if (diffDay > 5/*CDef.GlobalRecipe.ImageSaveDay*/)
                                {
                                    dayImageFolders[i].Delete(true);

                                    if (IsMachineNotRunning == false)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        public void InitProcess()
        {
            this.Mode = ProcessingMode.ModeNone;

            TrayProcess = new CTrayProcess(this, "Tray", MessageCode + 2000, IntervalTime = this.IntervalTime + 5);
            HeadProcess = new CHeadProcess(this, "Head", MessageCode + 4000, IntervalTime = this.IntervalTime + 3);
            UnderVisionProcess = new CUnderVisionProcess(this, "Under", MessageCode + 5500, IntervalTime = this.IntervalTime + 5);

            Childs = new ObservableCollection<IProcessing>();

            this.Childs.Add(TrayProcess);
            this.Childs.Add(HeadProcess);
            this.Childs.Add(UnderVisionProcess);
        }
        #endregion

        private bool licenseValid = false;

        #region Methods
        public override PRtnCode PreProcess()
        {
            if (licenseValid == false)
            {
                licenseValid = TopLicense.CryptoService.IsValid;
                Sleep(1000);
                return PRtnCode.RtnOk;
            }

            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (Mode != ProcessingMode.ModeToAlarm && Mode != ProcessingMode.ModeAlarm)
            {
                if (CDef.MainViewModel.InitVM.InitCompleted)
                {
                    // Checking alarm status after machine turn on
                    CheckAlarmStatus();
                }
            }

            if (CDef.IO.Input.StartSW == true)
            {
                if (CDef.RootProcess.Mode != ProcessingMode.ModeRun &&
                    CDef.RootProcess.Mode != ProcessingMode.ModeOrigin &&
                    (int)CDef.RootProcess.Mode % 2 != 1)
                {
                    Log.Info("Start Button Pressed!");

                    CDef.RootProcess.RunMode = ERunMode.AutoRun;
                    CDef.RootProcess.OperationCommand = OperatingMode.Run;
                }
            }

            if (CDef.IO.Input.ChangeSW == true)
            {
                if (CDef.RootProcess.Mode != ProcessingMode.ModeOrigin &&
                    (int)CDef.RootProcess.Mode % 2 != 1)
                {
                    Log.Info("Change Button Pressed!");

                    CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                    CDef.RootProcess.OperationCommand = OperatingMode.Run;
                }
            }

            switch (OperationCommand)
            {
                case OperatingMode.Origin:
                    // Check motion on before origin
                    if (CDef.AllAxis.AxisList.Count(axis => !axis.IsEncOnly && !axis.Status.IsMotionOn) > 0)
                    {
                        CDef.MessageViewModel.Show(this.ProcessName + " " + "MOTION ON first");
                    }
                    else
                    {
                        HomeStatus.Clear();

                        foreach (IMotion axis in CDef.AllAxis.AxisList)
                        {
                            axis.Status.IsHomeDone = false;
                        }

                        Mode = ProcessingMode.ModeToOrigin;
                    }

                    OperationCommand = OperatingMode.None;
                    break;
                case OperatingMode.Run:
                    // Check alarm status on before running
                    if (Mode == ProcessingMode.ModeToAlarm || Mode == ProcessingMode.ModeAlarm)
                    {
                        CDef.MessageViewModel.Show("Clear Alarm and HOME machine then try again.");
                    }
                    // Check home done before running
                    else if (!HomeStatus.IsAllAxisHomeDone)
                    {
                        CDef.MessageViewModel.Show("Home machine before start");
                    }
                    // Check motion on before running
                    else if (CDef.AllAxis.AxisList.Count(axis => !axis.IsEncOnly && !axis.Status.IsMotionOn) > 0)
                    {
                        CDef.MessageViewModel.Show(this.ProcessName + " " + "MOTION ON first");
                    }
                    
                    else
                    {
                        // Reset all children process' step
                        foreach (IProcessing processing in Childs)
                        {
                            processing.Step.ToRunStep = 0;
                        }

                        // TODO: Check and clear tray if tray is null (work done)
                        

                        // TODO: Turn off live mode
                        // if (CDef.TopCamera.IsLive) CDef.TopCamera.Stop();

                        Mode = ProcessingMode.ModeToRun;
                    }

                    OperationCommand = OperatingMode.None;
                    break;
                case OperatingMode.Stop:
                    if (Mode == ProcessingMode.ModeOrigin || Mode == ProcessingMode.ModeToOrigin)
                    {
                        CDef.MessageViewModel.Show((string)Application.Current.FindResource("str_HomeWaring"));
                    }
                    else if (Mode == ProcessingMode.ModeNone)
                    {
                        Mode = ProcessingMode.ModeNone;
                    }
                    else
                    {
                        foreach (IMotion motion in CDef.AllAxis.AxisList)
                        {
                            //motion.SoftStop();
                        }

                        if (Mode != ProcessingMode.ModeToAlarm && Mode != ProcessingMode.ModeAlarm)
                        {
                            Mode = ProcessingMode.ModeToStop;
                        }
                    }

                    OperationCommand = OperatingMode.None;
                    break;
                default:
                    break;
            }

            return RtnCode;
        }

        public override PRtnCode ProcessToOrigin()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (Childs.Count(child => child.ProcessingStatus != EProcessingStatus.ToOriginDone) == 0)
            {
                Flags.HeadHomeDone = false;
                // TODO: Update MES Status IDLE

                CDef.IO.Output.TowerLamp_Home = true;
                Mode = ProcessingMode.ModeOrigin;
            }
            else
            {
                Sleep(10);
            }

            return RtnCode;
        }

        public override PRtnCode ProcessOrigin()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (Childs.Count(child => child.ProcessingStatus != EProcessingStatus.OriginDone) == 0)
            {
                HomeStatus.IsAllAxisHomeDone = true;

                Log.Debug("Machine home done");
                CDef.MessageViewModel.Show((string)Application.Current.FindResource("str_HomeDoneNotification"));

                CDef.AllAxis.AxisList.ToList().ForEach(motion => motion.ClearPosition());

                CDef.IO.Output.TowerLamp_Stop = true;
                Mode = ProcessingMode.ModeToStop;
            }
            else
            {
                Sleep(50);
            }

            return RtnCode;
        }

        public override PRtnCode ProcessToRun()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (Childs.Count(child => (int)child.ProcessingStatus < 0) > 0)
            {
                CDef.IO.Output.TowerLamp_Stop = true;
                Mode = ProcessingMode.ModeToStop;
            }
            else if (Childs.Count(child => child.ProcessingStatus != EProcessingStatus.ToRunDone) == 0)
            {
                // TODO : Update MES STATUS RUN

                CDef.IO.Output.AllLightOff();

                CDef.IO.Output.TowerLamp_Run = true;
                Mode = ProcessingMode.ModeRun;

                Log.Debug("ToRun Done, Running");
            }
            else
            {
                Sleep(10);
            }

            return RtnCode;
        }

        public override PRtnCode ProcessToStop()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (Childs.Count(child => child.ProcessingStatus != EProcessingStatus.ToStopDone) == 0)
            {
                foreach (IMotion axis in CDef.AllAxis.AxisList)
                {
                    axis.SoftStop();
                }

                // TODO : UPDATE MES STATUS IDLE

                CDef.IO.Output.TowerLamp_Stop = true;
                Mode = ProcessingMode.ModeStop;
            }
            else
            {
                Sleep(10);
            }

            return RtnCode;
        }

        public override PRtnCode ProcessToWarning()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (Childs.Count(child => child.ProcessingStatus != EProcessingStatus.ToWarningDone) == 0)
            {
                foreach (IMotion axis in CDef.AllAxis.AxisList)
                {
                    axis.SoftStop();
                }

                // TODO: UPDATE MES STATUS DOWN

                CDef.IO.Output.TowerLamp_Warning = true;
                Mode = ProcessingMode.ModeWarning;
            }
            else
            {
                Sleep(10);
            }

            return RtnCode;
        }

        public override PRtnCode ProcessToAlarm()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            foreach (IMotion motion in CDef.AllAxis.AxisList)
            {
                motion.EMGStop();
            }

            HomeStatus.Clear();

            // TODO: CHANGE MES STATUS TO DOWN

            CDef.IO.Output.TowerLamp_Alarm = true;
            Mode = ProcessingMode.ModeAlarm;

            return RtnCode;
        }
        #endregion

        #region Privates
        private CAlarmStatus _AlarmStatus = new CAlarmStatus();
        private ERunMode _RunMode;
        private OperatingMode _OperationCommand;

        private readonly object setEventLockObject = new object();
        #endregion
    }
}
