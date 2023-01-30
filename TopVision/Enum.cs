using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision
{
    public enum ERotateDirect
    {
        /// <summary>
        /// Clockwise
        /// </summary>
        CW,
        /// <summary>
        /// Counter clockwise
        /// </summary>
        CCW,
    }

    public enum EDetectionDirection
    {
        /// <summary>
        /// Left to Right
        /// </summary>
        Left2Right,
        /// <summary>
        /// Right to Left
        /// </summary>
        Right2Left,
        /// <summary>
        /// Top to Bottom
        /// </summary>
        Top2Bottom,
        /// <summary>
        /// Bottom to Top
        /// </summary>
        Bottom2Top
    }
    public enum ESearchDirect
    {
        InToOut,
        OutToIn,
    }

    public enum EGrabRtnCode
    {
        GRAB_FAIL = -100,
        OK,
    }

    public enum EVisionRtnCode
    {
        FAIL = -1,
        OK
    }

    public enum EVisionJudge
    {
        NG = -1,
        OK = 1
    }

    public enum EVisionProcessStatus
    {
        IDLE,
        IN_PROCESSING,
        PROCESS_DONE
    }
}
