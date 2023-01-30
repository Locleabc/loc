using Newtonsoft.Json;
using TopCom.Define;
using TopCom.Models;

namespace TopCom
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IMotion : ILoggable
    {
        int Index { get; }
        string AxisName { get; set; }
        bool IsEncOnly { get; set; }
        IMotionStatus Status { get; }
        /// <summary>
        /// Default speed is 100 [pps]
        /// </summary>
        [JsonProperty]
        double Speed { get; set; }
        /// <summary>
        /// <strong>Unit per Pulse</strong><br/>
        /// <br/>
        /// Number of pulse for 1 rotation / Travel distance for 1 ratation<br/>
        /// <strong>Unit:</strong> pulse / μm<br/>
        /// <strong>Default:</strong> 1 [pulse / μm]
        /// </summary>
        double GearRatio { get; set; }
        /// <summary>
        /// Default AllowPositionDiff is 0.01 [mm]
        /// </summary>
        [JsonProperty]
        double AllowPositionDiff { get; set; }
        EMotionRtnCode Connect();
        EMotionRtnCode Disconnect();
        EMotionRtnCode Initialization();
        EMotionRtnCode HomeSearch();
        EMotionRtnCode MotionOn();
        EMotionRtnCode MotionOff();
        /// <summary>
        /// Move to absolute position
        /// </summary>
        /// <param name="dAbsPos">Absolute position (unit: mm)</param>
        /// <returns></returns>
        EMotionRtnCode MoveAbs(double dAbsPos);
        /// <summary>
        /// Move to absolute position
        /// </summary>
        /// <param name="dAbsPos">Absolute position (unit: mm)</param>
        /// <param name="dVelocity">Motion speed (unit: mm / s)</param>
        /// <returns></returns>
        EMotionRtnCode MoveAbs(double dAbsPos, double dVelocity);
        /// <summary>
        /// Move to relative position (increase or decrease)
        /// </summary>
        /// <param name="dIncPos">Relative position (unit: mm)</param>
        /// <returns></returns>
        EMotionRtnCode MoveInc(double dIncPos);
        /// <summary>
        /// Move to relative position (increase or decrease)
        /// </summary>
        /// <param name="dIncPos">Relative position (unit: mm)</param>
        /// <param name="dVelocity">Motion speed (unit: mm / s)</param>
        /// <returns></returns>
        EMotionRtnCode MoveInc(double dIncPos, double dVelocity);
        /// <summary>
        /// Velocity driving motion
        /// </summary>
        /// <param name="dVelocity">Speed (unit: mm / s)</param>
        /// <param name="isINC">Motion direction</param>
        /// <returns></returns>
        EMotionRtnCode MoveJog(double dVelocity, bool isINC);
        EMotionRtnCode SoftStop();
        EMotionRtnCode EMGStop();
        EMotionRtnCode AlarmReset();
        EMotionRtnCode StatusUpdate();
        EMotionRtnCode ClearPosition();
        bool IsOnPosition(double dPos);
    }
}
