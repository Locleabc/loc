using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TopCom;
using TopCom.Processing;
using TopCom.MES;
using VCM_CoilLoading.Define;

namespace VCM_CoilLoading.Processing
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

        public CPickerProcess PickerProcess { get; set; }
        public CUpperVisionProcess UpperVisionProcess { get; set; }
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

        #region Constructors
        public CRootProcess(string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }

        public void InitProcess()
        {
            this.Mode = ProcessingMode.ModeNone;

            PickerProcess = new CPickerProcess(this, "Picker", MessageCode + 2000, IntervalTime = this.IntervalTime + 5);
            UpperVisionProcess = new CUpperVisionProcess(this, "UpperVision", MessageCode + 2500, IntervalTime = this.IntervalTime + 5);

            Childs = new ObservableCollection<IProcessing>();

            this.Childs.Add(PickerProcess);
            this.Childs.Add(UpperVisionProcess);
        }
        #endregion

        #region Methods
        public override PRtnCode PreProcess()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (Mode != ProcessingMode.ModeToAlarm && Mode != ProcessingMode.ModeAlarm)
            {
                if (CDef.MainViewModel.InitVM.InitCompleted)
                {
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

            if (CDef.IO.Input.StopSW == true)
            {
                Log.Info("Stop Button Pressed!");

                CDef.RootProcess.OperationCommand = OperatingMode.Stop;
            }

            if (CDef.IO.Input.HomeSW == true)
            {
                Log.Info("Home Button Pressed!");

                if (CDef.RootProcess.Mode != ProcessingMode.ModeRun &&
                   CDef.RootProcess.Mode != ProcessingMode.ModeOrigin &&
                   (int)CDef.RootProcess.Mode % 2 != 1)
                {
                    CDef.RootProcess.OperationCommand = OperatingMode.Origin;
                }
            }

            switch (OperationCommand)
            {
                case OperatingMode.Origin:
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
                    if (!HomeStatus.IsAllAxisHomeDone)
                    {
                        CDef.MessageViewModel.Show("HOME FIRST");
                    }
                    else
                    {
                        foreach (IProcessing processing in Childs)
                        {
                            processing.Step.ToRunStep = 0;
                        }

                        CDef.MainViewModel.MESPMStatusVM.ToChangePMStatus = EPMStatus.NONE;

                        Mode = ProcessingMode.ModeToRun;
                    }

                    OperationCommand = OperatingMode.None;
                    break;
                case OperatingMode.Stop:
                    if (Mode == ProcessingMode.ModeOrigin || Mode == ProcessingMode.ModeToOrigin)
                    {
                        CDef.MessageViewModel.Show("Wait Home done first");
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
                CDef.MES.Send_EquipStatus(EMESEqpStatus.IDLE);

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
                CDef.MessageViewModel.Show("HOME DONE");

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
                CDef.MES.Send_EquipStatus(EMESEqpStatus.RUN);

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

                CDef.MES.Send_EquipStatus(EMESEqpStatus.IDLE);

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

                CDef.MES.Send_EquipStatus(EMESEqpStatus.DOWN);

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

            CDef.MES.Send_EquipStatus(EMESEqpStatus.DOWN);

            CDef.IO.Output.TowerLamp_Alarm = true;
            Mode = ProcessingMode.ModeAlarm;

            return RtnCode;
        }
        #endregion

        #region Privates
        private CAlarmStatus _AlarmStatus = new CAlarmStatus();
        private ERunMode _RunMode;
        private OperatingMode _OperationCommand;
        #endregion
    }
}
