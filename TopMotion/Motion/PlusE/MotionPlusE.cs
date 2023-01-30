using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom;
using TopCom.Define;
using NativeLib = FASTECH.EziMOTIONPlusELib;

namespace TopMotion
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MotionPlusE : MotionBase
    {
        //private MotionPlusEStatus status;
        //new public MotionPlusEStatus Status
        //{
        //    get { return status; }
        //    set
        //    {
        //        status = value;
        //        OnPropertyChanged("Status");
        //    }
        //}

        #region Extra Properties
        public IPAddress IP
        {
            get { return _IP; }
            set
            {
                _IP = value;
                Index = IP.GetAddressBytes()[3];
                OnPropertyChanged();
            }
        }
        #endregion

        public MotionPlusE(IPAddress _ip, string _Name)
        {
            IP = _ip;
            AxisName = _Name;
            Status = new MotionPlusEStatus();
        }

        public MotionPlusE(int index, string _Name)
            : this(IPAddress.Parse("192.168.0." + index), _Name)
        {
        }

        public override EMotionRtnCode AlarmReset()
        {
#if SIMULATION
            return base.AlarmReset();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (Status.IsMotionOn == true)
            {
                MotionOff();
            }

            if (NativeLib.FAS_ServoAlarmReset(Index) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_ALARM_RESET_FAIL;
            }

            if (Status.IsMotionOn == false)
            {
                MotionOn();
            }

            return nRtn;
#endif
        }

        public override EMotionRtnCode Connect()
        {
#if SIMULATION
            return base.Connect();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_Connect(IP, Index) != true)
            {
                Log.Error($"{AxisName} connect error");
                nRtn = EMotionRtnCode.ERR_CONNECT_FAIL;
                Status.IsConnected = false;
            }
            else
            {
                Log.Debug($"{AxisName} connect success!");
                Status.IsConnected = true;
            }

            return nRtn;
#endif
        }

        public override EMotionRtnCode Disconnect()
        {
#if SIMULATION
            return base.Disconnect();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            NativeLib.FAS_Close(Index);
            Status.IsConnected = false;

            return nRtn;
#endif
        }

        public override EMotionRtnCode EMGStop()
        {
#if SIMULATION
            return base.EMGStop();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_EmergencyStop(Index) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_EMG_STOP_FAIL;
            }

            return nRtn;
#endif
        }

        public override EMotionRtnCode HomeSearch()
        {
#if SIMULATION
            return base.HomeSearch();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;
            this.Status.IsHomeDone = false;

            if (NativeLib.FAS_MoveOriginSingleAxis(Index) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_HOME_SEARCH_FAIL;
            }

            return nRtn;
#endif
        }

        public override EMotionRtnCode MotionOff()
        {
#if SIMULATION
            return base.MotionOff();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_ServoEnable(Index, 0) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_MOTION_OFF_FAIL;
            }

            return nRtn;
#endif
        }

        public override EMotionRtnCode MotionOn()
        {
#if SIMULATION
            return base.MotionOn();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_ServoEnable(Index, 1) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_MOTION_ON_FAIL;
            }

            return nRtn;
#endif
        }

        private EMotionRtnCode MoveAbs_Pulse(int nAbsPos, int nVelocity)
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_MoveSingleAxisAbsPos(Index, (int)nAbsPos, (uint)nVelocity) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_MOVE_ABS_FAIL;
            }

            return nRtn;
        }

        public override EMotionRtnCode MoveAbs(double dAbsPos, double dVelocity)
        {
#if SIMULATION
            return base.MoveAbs(dAbsPos, dVelocity);
#else
            Log.Debug($"{AxisName} MoveAbs from {Status.ActualPosition} to {dAbsPos} [mm]");
            return MoveAbs_Pulse((int)(dAbsPos * 1000 / GearRatio), (int)(dVelocity * 1000));
#endif
        }

        private EMotionRtnCode MoveInc_Pulse(int nIncPos, int nVelocity)
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_MoveSingleAxisIncPos(Index, (int)nIncPos, (uint)nVelocity) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_MOVE_INC_FAIL;
            }

            return nRtn;
        }

        public override EMotionRtnCode MoveInc(double dIncPos, double dVelocity)
        {
#if SIMULATION
            return base.MoveInc(dIncPos, dVelocity);
#else
            return MoveInc_Pulse((int)(dIncPos * 1000 / GearRatio), (int)(dVelocity * 1000));
#endif
        }

        private EMotionRtnCode MoveJog_PulsePerSecond(int nVelocity, bool isINC)
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_MoveVelocity(Index, (uint)nVelocity, isINC ? 1 : 0) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_MOVE_JOG_FAIL;
            }

            return nRtn;
        }

        public override EMotionRtnCode MoveJog(double dVelocity, bool isINC)
        {
#if SIMULATION
            return base.MoveJog(dVelocity, isINC);
#else
            return MoveJog_PulsePerSecond((int)(dVelocity * 1000 / GearRatio), isINC);
#endif
        }

        public override EMotionRtnCode SoftStop()
        {
#if SIMULATION
            return base.SoftStop();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_MoveStop(Index) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_SOFT_STOP_FAIL;
            }

            return nRtn;
#endif
        }

        public override EMotionRtnCode StatusUpdate()
        {
#if SIMULATION
            return base.StatusUpdate();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            MotionPlusEStatus tmpStatus = new MotionPlusEStatus();
            tmpStatus = NativeLib.FAS_GetAllStatus(Index);

            (Status as MotionPlusEStatus).InputStatus = tmpStatus.InputStatus;
            (Status as MotionPlusEStatus).OutputStatus = tmpStatus.OutputStatus;
            (Status as MotionPlusEStatus).PositionTableItem = tmpStatus.PositionTableItem;
            Status.AxisStatus = tmpStatus.AxisStatus;
            Status.CommandPosition = tmpStatus.CommandPosition / (1000 / GearRatio);  // Micron to Milimeter
            Status.ActualPosition = tmpStatus.ActualPosition / (1000 / GearRatio);  // Micron to Milimeter
            Status.PositionError = tmpStatus.PositionError / (1000 / GearRatio);  // Micron to Milimeter
            Status.ActualVelocity = tmpStatus.ActualPosition / (1000 / GearRatio);  // Micron per second to Milimeter per second
            
            if (((Status as MotionPlusEStatus).AxisStatus & NativeLib.FFLAG_ORIGINRETOK) > 0)
            {
                Status.IsHomeDone = true;
            }
            else
            {
                Status.IsHomeDone = false;
            }

            if (((Status as MotionPlusEStatus).AxisStatus & NativeLib.FFLAG_SERVOON) > 0)
            {
                Status.IsMotionOn = true;
            }
            else
            {
                Status.IsMotionOn = false;
            }

            if (((Status as MotionPlusEStatus).AxisStatus & NativeLib.FFLAG_MOTIONING) > 0)
            {
                Status.IsMotionDone = false;
            }
            else
            {
                Status.IsMotionDone = true;
            }

            #region Alarm Update
            byte bAlarmType = 0;
            NativeLib.FAS_GetAlarmType(Index, ref bAlarmType);

            Status.AlarmStatus.AlarmCode = bAlarmType;
            if (bAlarmType != 0)
            {
                Status.AlarmStatus.IsAlarm = true;
                Status.AlarmStatus.AlarmMessage = NativeLib.ALARM_DESCRIPTION[bAlarmType];
            }
            else
            {
                Status.AlarmStatus.IsAlarm = false;
            }

            if (Status.IsHomeDone)
            {
                if (((Status as MotionPlusEStatus).AxisStatus & NativeLib.FFLAG_HWNEGALMT) > 0)
                {
                    Status.AlarmStatus.IsAlarm = true;
                    Status.AlarmStatus.AlarmMessage = $"{AxisName} Hardware negative limit(-) detected";
                }
                if (((Status as MotionPlusEStatus).AxisStatus & NativeLib.FFLAG_HWPOSILMT) > 0)
                {
                    Status.AlarmStatus.IsAlarm = true;
                    Status.AlarmStatus.AlarmMessage = $"{AxisName} Hardware positive limit(+) detected";
                }

                if (((Status as MotionPlusEStatus).AxisStatus & NativeLib.FFLAG_SWNEGALMT) > 0)
                {
                    Status.AlarmStatus.IsAlarm = true;
                    Status.AlarmStatus.AlarmMessage = $"{AxisName} Software negative limit(-) detected";
                }
                if (((Status as MotionPlusEStatus).AxisStatus & NativeLib.FFLAG_SWPOGILMT) > 0)
                {
                    Status.AlarmStatus.IsAlarm = true;
                    Status.AlarmStatus.AlarmMessage = $"{AxisName} Software positive limit(+) detected";
                }
            }
            #endregion

            return nRtn;
#endif
        }

        public override EMotionRtnCode ClearPosition()
        {
#if SIMULATION
            return base.StatusUpdate();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            NativeLib.FAS_SetActualPos(Index, 0);
            NativeLib.FAS_SetCommandPos(Index, 0);

            return nRtn;
#endif
        }

        #region Privates
        private IPAddress _IP;
        #endregion
    }
}
