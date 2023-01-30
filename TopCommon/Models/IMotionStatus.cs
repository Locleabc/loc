using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopCom
{
    public interface IMotionStatus
    {
        bool IsConnected { get; set; }
        bool IsMotionOn { get; set; }
        bool IsHomeDone { get; set; }
        bool IsMotionDone { get; set; }
        CObjectAlarmStatus AlarmStatus { get; set; }

        uint AxisStatus { get; set; }

        double CommandPosition { get; set; }
        double ActualPosition { get; set; }
        double PositionError { get; set; }
        double ActualVelocity { get; set; }
    }
}
