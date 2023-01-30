using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TopCom;
using TopCom.Define;
using TopCom.Models;

namespace TopMotion
{
    public class MotionBase : PropertyChangedNotifier, IMotion
    {
        #region Implementation Properties
        public int Index
        {
            get { return _Index; }
            set
            {
                _Index = value;
                OnPropertyChanged();
            }
        }

        public string AxisName
        {
            get { return _AxisName; }
            set
            {
                _AxisName = value;
                OnPropertyChanged();
            }
        }

        public IMotionStatus Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged();
            }
        }

        public bool EnableAutoUpdate
        {
            get { return _EnableAutoUpdate; }
            set
            {
                _EnableAutoUpdate = value;
                if (value)
                {
                    StatusUpdateTimer.Enabled = true;
                }
                else
                {
                    StatusUpdateTimer.Enabled = false;
                }
                OnPropertyChanged();
            }
        }

        public bool IsEncOnly
        {
            get { return _IsEncOnly; }
            set
            {
                _IsEncOnly = value;
                OnPropertyChanged();
            }
        }

        public double Speed
        {
            get { return _Speed; }
            set
            {
                _Speed = value;
                OnPropertyChanged();
            }
        }

        public double  GearRatio
        {
            get { return _GearRatio; }
            set
            {
                _GearRatio = value;
                OnPropertyChanged();
            }
        }

        public double AllowPositionDiff
        {
            get { return _AllowPositionDiff; }
            set
            {
                _AllowPositionDiff = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private System.Timers.Timer StatusUpdateTimer = new System.Timers.Timer(100);
        private bool Flag_StopSimulationMove { get; set; } = false;
        public ILog Log { get; protected set; }

#if SIMULATION
        public Random random = new Random();
#endif

        public MotionBase()
        {
            Log = LogManager.GetLogger("MOTION_DLL");
            LogFactory.Configure();

            StatusUpdateTimer.Elapsed += StatusUpdateTimer_Elapsed;
            StatusUpdateTimer.AutoReset = true;

            EnableAutoUpdate = true;
            IsEncOnly = false;
        }

        ~MotionBase()
        {
            StatusUpdateTimer.Stop();
        }

        public override string ToString()
        {
            return AxisName;
        }

        private void StatusUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StatusUpdate();
        }

        public virtual EMotionRtnCode AlarmReset()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Status.AlarmStatus.IsAlarm = false;
            Status.AlarmStatus.AlarmCode = 0;
            Status.AlarmStatus.AlarmMessage = "";
#endif

            return nRtn;
        }

        public virtual EMotionRtnCode Connect()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Status.IsConnected = true;
#endif

            return nRtn;
        }

        public virtual EMotionRtnCode Disconnect()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Status.IsConnected = false;
#endif

            return nRtn;
        }

        public virtual EMotionRtnCode HomeSearch()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Simuation_MoveTo(0, IsHomeSearch: true);
#endif

            return nRtn;
        }

        /// <summary>
        /// This function must be call mannually
        /// Disconnect -> Connect -> Motion On
        /// Must include base.Initialization() in case override this method on child class
        /// </summary>
        /// <returns></returns>
        public virtual EMotionRtnCode Initialization()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            Disconnect();
            nRtn = Connect();
            if (nRtn == EMotionRtnCode.RTN_OK)
            {
                MotionOn();
            }
#if SIMULATION
            // Add Simulation code here
#endif

            return nRtn;
        }

        public virtual EMotionRtnCode MotionOff()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Status.IsMotionOn = false;
#endif


            return nRtn;
        }

        public virtual EMotionRtnCode MotionOn()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Status.IsMotionOn = true;
#endif

            return nRtn;
        }

        public virtual EMotionRtnCode Simuation_MoveTo(double dTargetPos, bool IsHomeSearch = false)
        {
            return Simuation_MoveTo(dTargetPos, Speed, IsHomeSearch);
        }

        public virtual EMotionRtnCode Simuation_MoveTo(double dTargetPos, double dVelocity, bool IsHomeSearch = false)
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (dVelocity <= 0) dVelocity = 1;

            Flag_StopSimulationMove = false;
            Task task = new Task(() =>
            {
                const int updateFrequency = 100;  // Milisecond

                double distance = dTargetPos - Status.ActualPosition;
                double timeCost = Math.Abs(distance) /* mm */ * 1000 / dVelocity /* mm per second */;  // Milisecond

                int numberOfSteps = (int)Math.Floor(timeCost / updateFrequency);

                double moveStep = Math.Truncate((distance / numberOfSteps) * 1000) / 1000;

                for (int i = 0; i < numberOfSteps; i++)
                {
                    if (Flag_StopSimulationMove == true)
                    {
                        return;
                    }

                    Thread.Sleep(updateFrequency);
                    Status.ActualPosition += moveStep;
                }

                if (IsHomeSearch)
                {
                    Status.IsHomeDone = true;
                }

                Thread.Sleep((int)(timeCost - updateFrequency * numberOfSteps));  // Sleep remain time
                Status.ActualPosition = dTargetPos;
            });

            task.Start();
            //task.Wait();

            return nRtn;
        }

        public EMotionRtnCode MoveAbs(double dAbsPos)
        {
            return MoveAbs(dAbsPos, this.Speed);
        }


        public virtual EMotionRtnCode MoveAbs(double dAbsPos, double dVelocity)
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            Log.Debug($"{AxisName} MoveAbs from {Status.ActualPosition} to {dAbsPos} [mm]");
#if SIMULATION
            Simuation_MoveTo(dAbsPos, dVelocity);
#endif

            return nRtn;
        }

        public EMotionRtnCode MoveInc(double dAbsPos)
        {
            return MoveInc(dAbsPos, this.Speed);
        }

        public virtual EMotionRtnCode MoveInc(double dIncPos, double dVelocity)
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Simuation_MoveTo(Status.ActualPosition + dIncPos, dVelocity);
#endif

            return nRtn;
        }

        public virtual EMotionRtnCode MoveJog(double dVelocity, bool isINC)
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Status.ActualPosition += (int)(random.NextDouble() * dVelocity) * (isINC ? 1 : -1);
#endif

            return nRtn;
        }

        public virtual EMotionRtnCode EMGStop()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Flag_StopSimulationMove = true;
#endif
            return nRtn;
        }

        public virtual EMotionRtnCode SoftStop()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Flag_StopSimulationMove = true;
#endif

            return nRtn;
        }

        public virtual EMotionRtnCode StatusUpdate()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            // Add Simulation code here
#endif

            return nRtn;
        }

        public bool IsOnPosition(double dPos)
        {
            // Motor is on position but still running?
            return (Math.Abs(Status.ActualPosition - dPos) < AllowPositionDiff) & (Status.IsMotionDone);
        }

        public virtual EMotionRtnCode ClearPosition()
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

#if SIMULATION
            Status.ActualPosition = 0;
            Status.CommandPosition = 0;
#endif

            return nRtn;
        }

        #region Privates
        private int _Index;
        private string _AxisName;
        private IMotionStatus _Status;
        private bool _EnableAutoUpdate;
        private bool _IsEncOnly;
        private double _Speed = 100;
        private double _GearRatio = 1;
        private double _AllowPositionDiff = 0.01;
        #endregion
    }
}
