using System;
using System.Collections.Generic;
using System.Text;

public class MotionDef
{
    public static bool ON = true;
    public static bool OFF = false;
    public static bool INC = true;
    public static bool DEC = true;
}

namespace TopCom.Define
{
    public enum EIORtnCode
    {
        RTN_OK = 0
    }

    public enum EMotionRtnCode
    {
        ERR_UNDEFINED = -9999,
        ERR_IP_PARSE_FAIL,
        ERR_UNKNOW_MOTION,

        ERR_CONNECT_FAIL,

        ERR_HOME_SEARCH_FAIL,

        ERR_MOTION_ON_FAIL,
        ERR_MOTION_OFF_FAIL,

        ERR_MOVE_ABS_FAIL,
        ERR_MOVE_INC_FAIL,
        ERR_MOVE_JOG_FAIL,

        ERR_EMG_STOP_FAIL,
        ERR_SOFT_STOP_FAIL,

        ERR_ALARM_RESET_FAIL,

        /// <summary>
        /// ActualPosition and Position to comfirm miss match
        /// </summary>
        ERR_POSITION_MISS_MATCH,

        RTN_OK = 0,
    }

    enum EMotionType
    {
        // MOTIONBOARD/CONNECTTYPE_DRIVER_MOTOR_NOTE
        UNDEFINED = -1,
        UNKOWN = 0,
        FAS_AKRIBIS_AKRIBIS_FasFM16000 = 1,
        ETHERNET_FAS_FAS_EziPlusE = 2,
        RSxxx_FAS_FAS_EziPlusR = 3,
        AJIN_AKRIBIS_AKRIBIS_AjinAXM = 10,
        AJIN_PANA_PANA_AjinAXM = 11,

        MAX_MOTION_TYPE,
    }
}
