using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom.Define;
using TopCom.Models;
using TopCom.Mqtt;

namespace TopCom.Processing
{
    public abstract class ProcessingBase : CommomProcessBase, IProcessing
    {
        #region Properties
        public IProcessing Parent { get; protected set; }

        public ObservableCollection<IProcessing> Childs
        {
            get
            {
                return _Childs;
            }
            set
            {
                _Childs = value;
                OnPropertyChanged("Childs");
            }
        }

        public int MessageCode { get; protected set; }

        public ProcessingMode Mode
        {
            get { return _Mode; }
            set
            {
                if (_Mode == value) return;

                _Mode = value;
                OnPropertyChanged("Mode");
                OnPropertyChanged("ModeToString");
                OnPropertyChanged("IsMachineNotRunning");

                if (_Childs != null)
                {
                    foreach (IProcessing child in _Childs)
                    {
                        child.ProcessingStatus = EProcessingStatus.None;
                    }
                }
            }
        }

        public string ModeDetail { get; set; }

        public EProcessingStatus ProcessingStatus
        {
            get { return _ProcessingStatus; }
            set
            {
                if (_ProcessingStatus != value)
                {
                    _ProcessingStatus = value;
                    OnPropertyChanged("ProcessingStatus");
                }
            }
        }

        public string ModeToString
        {
            get
            {
                switch (Mode)
                {
                    case ProcessingMode.ModeNone:
                        return "None";
                    case ProcessingMode.ModeToAlarm:
                        return "ToAlarm";
                    case ProcessingMode.ModeAlarm:
                        return "Alarm";
                    case ProcessingMode.ModeToOrigin:
                        return "ToOrigin";
                    case ProcessingMode.ModeOrigin:
                        return "Origin";
                    case ProcessingMode.ModeToStop:
                        return "ToStop";
                    case ProcessingMode.ModeStop:
                        return "Stop";
                    case ProcessingMode.ModeToWarning:
                        return "ToWarning";
                    case ProcessingMode.ModeWarning:
                        return "Warning";
                    case ProcessingMode.ModeToRun:
                        return "ToRun";
                    case ProcessingMode.ModeRun:
                        return "Run";
                    default:
                        return "Status";
                }
            }
        }
        #endregion

        #region Constructors and Decstructor
        public ProcessingBase(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
        {
            LogFactory.Configure();
            Log = log4net.LogManager.GetLogger(_Name);

            Step = new CStep();
            Step.RunStepChangeHandler = delegate { PTimer.StepTimeoutWatcher = PTimer.Now; };
            Step.ToRunStepChangeHandler = delegate { PTimer.StepTimeoutWatcher = PTimer.Now; };
            Step.HomeStepChangeHandler = delegate { PTimer.StepTimeoutWatcher = PTimer.Now; };
            Step.SubStepChangeHandler = delegate { PTimer.StepTimeoutWatcher = PTimer.Now; };

            ProcessName = _Name;
            MessageCode = _MessageCodeStartIndex;
            IntervalTime = _IntervalTimeMs;

            if (_Parent != null)
            {
                Parent = _Parent;
            }
            else
            {
                this.Parent = this;
            }

            IsAlive = true;
            ProcessStart();
        }

        public ProcessingBase(string _Name, int _MessageCodeStartIndex, int _IntervalTime)
            : this(null, _Name, _MessageCodeStartIndex, _IntervalTime)
        {
        }

        ~ProcessingBase()
        {
            IsAlive = false;
            try
            {
                ThisThread.Abort();
            }
            catch { }
        }
        #endregion

        #region Methods
        public override void ProcessWorker()
        {
            try
            {
                PreProcess();

                switch (Parent.Mode)
                {
                    case ProcessingMode.ModeNone:
                        ProcessNone();
                        break;
                    case ProcessingMode.ModeToAlarm:
                        ProcessToAlarm();
                        break;
                    case ProcessingMode.ModeAlarm:
                        ProcessAlarm();
                        break;
                    case ProcessingMode.ModeToOrigin:
                        ProcessToOrigin();
                        break;
                    case ProcessingMode.ModeOrigin:
                        ProcessOrigin();
                        break;
                    case ProcessingMode.ModeToStop:
                        ProcessToStop();
                        break;
                    case ProcessingMode.ModeStop:
                        ProcessStop();
                        break;
                    case ProcessingMode.ModeToWarning:
                        ProcessToWarning();
                        break;
                    case ProcessingMode.ModeWarning:
                        ProcessWarning();
                        break;
                    case ProcessingMode.ModeToRun:
                        ProcessToRun();
                        break;
                    case ProcessingMode.ModeRun:
                        ProcessRun();
                        break;
                    default:
                        break;
                }

                PostProcess();
            }
            catch (Exception ex)
            {
                var v = MqttGlobal.Client.PublishAsync(Topics.EquipException,
                    new ExceptionMessage
                    {
                        Message = ex.Message,
                        Note = $"[{ProcessName}][Mode]{Parent.Mode}[Detal]{ModeDetail}[STEP]{Step}"
                    });
#if SIMULATION
                // throw exception on developer mode
                throw ex;
#endif
            }
        }

        public virtual PRtnCode PreProcess()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            return nRtn;
        }

        public virtual PRtnCode PostProcess()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            return nRtn;
        }

        public virtual PRtnCode ProcessToAlarm()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            Step.RunStep = 0;
            ProcessingStatus = EProcessingStatus.ToAlarmDone;
            return nRtn;
        }

        public virtual PRtnCode ProcessAlarm()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            return nRtn;
        }

        public virtual PRtnCode ProcessToOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            Step.HomeStep = 0;
            ProcessingStatus = EProcessingStatus.ToOriginDone;
            return nRtn;
        }

        public virtual PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            ProcessingStatus = EProcessingStatus.OriginDone;
            return nRtn;
        }

        public virtual PRtnCode ProcessToStop()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            Step.RunStep = 0;
            ProcessingStatus = EProcessingStatus.ToStopDone;
            return nRtn;
        }

        public virtual PRtnCode ProcessStop()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            return nRtn;
        }

        public virtual PRtnCode ProcessToWarning()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            Step.RunStep = 0;
            ProcessingStatus = EProcessingStatus.ToWarningDone;
            return nRtn;
        }

        public virtual PRtnCode ProcessWarning()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            return nRtn;
        }

        /// <summary>
        /// 1. Clear RunStep to 0
        /// <br/>
        /// 2. Set ProcessingStatus to ToRunDone
        /// </summary>
        /// <returns>Process Return code</returns>
        public virtual PRtnCode ProcessToRun()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            Step.RunStep = 0;
            ProcessingStatus = EProcessingStatus.ToRunDone;
            return nRtn;
        }

        public virtual PRtnCode ProcessRun()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            return nRtn;
        }

        public virtual PRtnCode ProcessNone()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            return nRtn;
        }
        #endregion

        #region Privates
        private ObservableCollection<IProcessing> _Childs;
        private ProcessingMode _Mode;
        private EProcessingStatus _ProcessingStatus;
        #endregion
    }
}
