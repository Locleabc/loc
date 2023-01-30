using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Define;
using NativeLib = FASTECH.EziMOTIONPlusRLib;

namespace TopMotion
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MotionPlusR : MotionBase
    {
        #region Properties
        public byte PortNumber
        {
            get
            {
                return _PortNumber;
            }
            internal set
            {
                if (_PortNumber == value) return;

                _PortNumber = value;
                OnPropertyChanged();
            }
        }

        public uint BaudRate
        {
            get { return _BaudRate; }
            internal set
            {
                if (_BaudRate == value) return;

                _BaudRate = value;
                OnPropertyChanged();
            }
        }

        public byte SlaveID
        {
            get { return _SlaveID; }
            set
            {
                if (_SlaveID == value) return;

                _SlaveID = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initialization Fastech motion PlusR
        /// </summary>
        /// <param name="portNumber">Com port number</param>
        /// <param name="slaveID">Slave index of the motion</param>
        /// <param name="baudRate">Speed of the motion</param>
        public MotionPlusR(byte portNumber, byte slaveID, string axisName, uint baudRate = 115200)
        {
            PortNumber = portNumber;
            SlaveID = slaveID;
            BaudRate = baudRate;
            AxisName = axisName;
            Status = new MotionPlusRStatus();
        }
        #endregion

        public override EMotionRtnCode AlarmReset()
        {
#if SIMULATION
            return base.AlarmReset();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;
            
            MotionOff();
            
            if (NativeLib.FAS_ServoAlarmReset(PortNumber, SlaveID) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_ALARM_RESET_FAIL;
            }

            MotionOn();
            
            return nRtn;
#endif
        }

        public override EMotionRtnCode Connect()
        {
#if SIMULATION
            return base.Connect();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_Connect(PortNumber, BaudRate) == 0)
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

            NativeLib.FAS_Close(PortNumber);
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

            if (NativeLib.FAS_EmergencyStop(PortNumber, SlaveID) != NativeLib.FMM_OK)
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

            if (NativeLib.FAS_MoveOriginSingleAxis(PortNumber, SlaveID) != NativeLib.FMM_OK)
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

            if (NativeLib.FAS_ServoEnable(PortNumber, SlaveID, 0) != NativeLib.FMM_OK)
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

            if (NativeLib.FAS_ServoEnable(PortNumber, SlaveID, 1) != NativeLib.FMM_OK)
            {
                nRtn = EMotionRtnCode.ERR_MOTION_ON_FAIL;
            }

            return nRtn;
#endif
        }

        private EMotionRtnCode MoveAbs_Pulse(int nAbsPos, int nVelocity)
        {
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            if (NativeLib.FAS_MoveSingleAxisAbsPos(PortNumber, SlaveID, (int)nAbsPos, (uint)nVelocity) != NativeLib.FMM_OK)
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

            if (NativeLib.FAS_MoveSingleAxisIncPos(PortNumber, SlaveID, (int)nIncPos, (uint)nVelocity) != NativeLib.FMM_OK)
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

            if (NativeLib.FAS_MoveVelocity(PortNumber, SlaveID, (uint)nVelocity, isINC ? 1 : 0) != NativeLib.FMM_OK)
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

            if (NativeLib.FAS_MoveStop(PortNumber, SlaveID) != NativeLib.FMM_OK)
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

            MotionPlusRStatus tmpStatus = new MotionPlusRStatus();
            tmpStatus = NativeLib.FAS_GetAllStatus(PortNumber, SlaveID);

            (Status as MotionPlusRStatus).InputStatus = tmpStatus.InputStatus;
            (Status as MotionPlusRStatus).OutputStatus = tmpStatus.OutputStatus;
            (Status as MotionPlusRStatus).PositionTableItem = tmpStatus.PositionTableItem;
            Status.AxisStatus = tmpStatus.AxisStatus;
            Status.CommandPosition = tmpStatus.CommandPosition / (1000 / GearRatio);  // Micron to Milimeter
            Status.ActualPosition = tmpStatus.ActualPosition / (1000 / GearRatio);  // Micron to Milimeter
            Status.PositionError = tmpStatus.PositionError / (1000 / GearRatio);  // Micron to Milimeter
            Status.ActualVelocity = tmpStatus.ActualPosition / (1000 / GearRatio);  // Micron per second to Milimeter per second

            if (((Status as MotionPlusRStatus).AxisStatus & NativeLib.FFLAG_ORIGINRETOK) > 0)
            {
                Status.IsHomeDone = true;
            }
            else
            {
                Status.IsHomeDone = false;
            }
            if (((Status as MotionPlusRStatus).AxisStatus & NativeLib.FFLAG_SERVOON) > 0)
            {
                Status.IsMotionOn = true;
            }
            else
            {
                Status.IsMotionOn = false;
            }
            if (((Status as MotionPlusRStatus).AxisStatus & NativeLib.FFLAG_MOTIONING) > 0)
            {
                Status.IsMotionDone = false;
            }
            else
            {
                Status.IsMotionDone = true;
            }

            #region Alarm Update
            byte bAlarmType = 0;
            NativeLib.FAS_GetAlarmType(PortNumber, SlaveID, ref bAlarmType);

            Status.AlarmStatus.AlarmCode = bAlarmType;
            if (bAlarmType != 0)
            {
                Status.AlarmStatus.IsAlarm = true;
            }
            else
            {
                Status.AlarmStatus.IsAlarm = false;
            }
            Status.AlarmStatus.AlarmMessage = NativeLib.ALARM_DESCRIPTION[bAlarmType];
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

            NativeLib.FAS_SetActualPos(PortNumber, SlaveID, 0);
            NativeLib.FAS_SetCommandPos(PortNumber, SlaveID, 0);

            return nRtn;
#endif
        }

        #region Privates
        private byte _PortNumber;
        private uint _BaudRate;
        private byte _SlaveID;
        #endregion
    }
}
