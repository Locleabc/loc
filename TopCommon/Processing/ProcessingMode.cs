using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopCom.Processing
{
    public enum ProcessingMode
    {
        ModeNone = 0,
        ModeToAlarm = 1,
        ModeAlarm = 2,
        ModeToOrigin = 3,
        ModeOrigin = 4,
        ModeToStop = 5,
        ModeStop = 6,
        ModeToWarning = 7,
        ModeWarning = 8,
        ModeToRun = 9,
        ModeRun = 10
    }

    public enum EProcessingStatus
    {
        ToRunFail = -9,
        None = 0,
        ToAlarmDone = 1,
        ToOriginDone = 3,
        OriginDone,
        ToStopDone = 5,
        ToWarningDone = 7,
        ToRunDone = 9,
    }

    public enum OperatingMode
    {
        None = 0,
        Origin = 4,
        Stop = 6,
        Run = 10
    }
}
