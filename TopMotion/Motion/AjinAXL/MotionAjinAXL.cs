using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Define;

namespace TopMotion
{
    public class MotionAjinAXL : MotionBase
    {
        #region Extra Properties
        public double Accel
        {
            get { return _Accel; }
            set
            {
                _Accel = value;
                OnPropertyChanged();
            }
        }

        public double Deccel
        {
            get { return _Deccel; }
            set
            {
                _Deccel = value;
                OnPropertyChanged();
            }
        }

        private double _Accel = 100.0;
        private double _Deccel = 100.0;
        #endregion

        public MotionAjinAXL(int index, string name)
        {
            Index = index;
            AxisName = name;
            Status = new MotionAjinAXLStatus();
        }

        public override EMotionRtnCode AlarmReset()
        {
#if SIMULATION
            return base.AlarmReset();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            uint retCode = CAXM.AxmSignalServoAlarmReset(Index, 1);

            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                Log.Debug($"{AxisName} alarm reset successed!");
                nRtn = EMotionRtnCode.RTN_OK;
            }
            else
            {
                Log.Debug($"{AxisName} alarm reset failed! ErrorCode = {(AXT_FUNC_RESULT)retCode}");
                nRtn = EMotionRtnCode.ERR_ALARM_RESET_FAIL;
            }

            return nRtn;
#endif
        }

        public override EMotionRtnCode Connect()
        {
            return base.Connect();
        }

        public override EMotionRtnCode Disconnect()
        {
            return base.Disconnect();
        }

        public override EMotionRtnCode EMGStop()
        {
#if SIMULATION
            return base.EMGStop();
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            uint retCode = CAXM.AxmMoveEStop(Index);

            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                nRtn = EMotionRtnCode.RTN_OK;
            }
            else
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

            uint retCode = CAXM.AxmHomeSetStart(Index);

            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                nRtn = EMotionRtnCode.RTN_OK;
            }
            else
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

            uint retCode = CAXM.AxmSignalServoOn(Index, 0);

            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                nRtn = EMotionRtnCode.RTN_OK;
            }
            else
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

            uint retCode = CAXM.AxmSignalServoOn(Index, 1);

            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                nRtn = EMotionRtnCode.RTN_OK;
            }
            else
            {
                nRtn = EMotionRtnCode.ERR_MOTION_OFF_FAIL;
            }

            return nRtn;
#endif
        }

        public override EMotionRtnCode MoveAbs(double dAbsPos, double dVelocity)
        {
#if SIMULATION
            return base.MoveAbs(dAbsPos, dVelocity);
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            uint uMode = 0;
            CAXM.AxmMotGetAbsRelMode(Index, ref uMode);
            if (uMode != (uint)AXT_MOTION_ABSREL.POS_ABS_MODE)
            {
                CAXM.AxmMotSetAbsRelMode(Index, (uint)AXT_MOTION_ABSREL.POS_ABS_MODE);
            }

            uint retCode = CAXM.AxmMoveStartPos(Index, dAbsPos, dVelocity, Accel, Deccel);

            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                nRtn = EMotionRtnCode.RTN_OK;
            }
            else
            {
                nRtn = EMotionRtnCode.ERR_MOVE_ABS_FAIL;
            }

            return nRtn;
#endif
        }

        public override EMotionRtnCode MoveInc(double dIncPos, double dVelocity)
        {
#if SIMULATION
            return base.MoveInc(dIncPos, dVelocity);
#else
            EMotionRtnCode nRtn = EMotionRtnCode.RTN_OK;

            uint uMode = 0;
            CAXM.AxmMotGetAbsRelMode(Index, ref uMode);
            if (uMode != (uint)AXT_MOTION_ABSREL.POS_REL_MODE)
            {
                CAXM.AxmMotSetAbsRelMode(Index, (uint)AXT_MOTION_ABSREL.POS_REL_MODE);
            }

            uint retCode = CAXM.AxmMoveStartPos(Index, dIncPos, dVelocity, Accel, Deccel);

            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                nRtn = EMotionRtnCode.RTN_OK;
            }
            else
            {
                nRtn = EMotionRtnCode.ERR_MOVE_ABS_FAIL;
            }

            return nRtn;
#endif
        }
    }
}
