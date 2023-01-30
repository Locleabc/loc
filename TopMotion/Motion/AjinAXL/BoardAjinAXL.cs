using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopCom;
using TopCom.Models;
using log4net;
using TopCom.Define;

namespace TopMotion
{
    public class BoardAjinAXL : PropertyChangedNotifier, ILoggable
    {
        #region Properties
        public ILog Log { get; set; }

        public bool IsOpened
        {
            get
            {
#if SIMULATION
                return true;
#else
                return CAXL.AxlIsOpened() == 1;
#endif
            }
        }

        public int AxisCount
        {
            get
            {
#if SIMULATION
                return _AxisCount;
#else
                AXT_FUNC_RESULT returnCode = (AXT_FUNC_RESULT)CAXM.AxmInfoGetAxisCount(ref _AxisCount);

                if (returnCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    return _AxisCount;
                }
                else
                {
                    Log.Info("AxmInfoGetAxisCount failed with code " + returnCode);
                    return -1;
                }
#endif

            }
#if SIMULATION
            set
            {
                _AxisCount = value;
            }
#endif
        }

        public bool IsMotionModuleExist
        {
            get
            {
#if SIMULATION
                return true;
#else
                uint status = 0;
                AXT_FUNC_RESULT returnCode = (AXT_FUNC_RESULT)CAXM.AxmInfoIsMotionModule(ref status);

                if (returnCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    return (AXT_EXISTENCE)status == AXT_EXISTENCE.STATUS_EXIST;
                }
                else
                {
                    Log.Info("AxmInfoIsMotionModule failed with code " + returnCode);
                    return false;
                }
#endif
            }
        }

        public bool IsDIOModuleExist
        {
            get
            {
#if SIMULATION
                return true;
#else
                uint status = 0;
                AXT_FUNC_RESULT returnCode = (AXT_FUNC_RESULT)CAXD.AxdInfoIsDIOModule(ref status);

                if (returnCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    return (AXT_EXISTENCE)status == AXT_EXISTENCE.STATUS_EXIST;
                }
                else
                {
                    Log.Info("AxdInfoIsDIOModule failed with code " + returnCode);
                    return false;
                }
#endif
            }
        }

        public bool IsAIOModuleExist
        {
            get
            {
#if SIMULATION
                return true;
#else
                uint status = 0;
                AXT_FUNC_RESULT returnCode = (AXT_FUNC_RESULT)CAXA.AxaInfoIsAIOModule(ref status);

                if (returnCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    return (AXT_EXISTENCE)status == AXT_EXISTENCE.STATUS_EXIST;
                }
                else
                {
                    Log.Info("AxaInfoIsAIOModule failed with code " + returnCode);
                    return false;
                }
#endif
            }
        }
#endregion

        #region Constructor(s)
        public BoardAjinAXL()
        {
            Log = LogManager.GetLogger("MOTION_DLL");
            LogFactory.Configure();
        }
        #endregion

        #region Method(s)
        public bool Connect()
        {
#if SIMULATION
            Log.Debug("AXL Library Init success!");
            return true;
#else
            if (IsOpened)
            {
                Log.Debug("Library Init already");
                return true;
            }

            uint returnCode = CAXL.AxlOpen(7);

            if (returnCode == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                Log.Debug("AXL Library Init success!");
            }
            else
            {
                Log.Debug("AXL Library Init failed!! ErrorCode = " + (AXT_FUNC_RESULT)returnCode);
                return false;
            }

            return true;
#endif
        }

        public void Disconnect()
        {
#if SIMULATION
            Log.Debug("AXL Library Closed!");
#else
            if (!IsOpened)
            {
                Log.Debug("AXL Library is not opened.");
                return;
            }

            int returnCode = CAXL.AxlClose();
            if (returnCode == 1)
            {
                Log.Debug("AXL Library Closed.");
            }
            else
            {
                Log.Debug("AXL Library close failed!.");
            }
#endif
        }
        #endregion

        #region Private(s)
        private int _AxisCount = -1;
#endregion
    }
}
