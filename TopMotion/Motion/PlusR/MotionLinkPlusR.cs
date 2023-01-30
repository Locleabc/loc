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
    public class MotionLinkPlusR : MotionPlusR
    {
        #region Constructors
        /// <summary>
        /// Initialization Fastech motion PlusR
        /// </summary>
        /// <param name="portNumber">Com port number</param>
        /// <param name="slaveID">Slave index of the motion</param>
        /// <param name="baudRate">Speed of the motion</param>
        public MotionLinkPlusR(byte portNumber, byte slaveID, string axisName, uint baudRate = 115200)
            : base(portNumber, slaveID, axisName, baudRate)
        {
        }
        #endregion

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
            if (Status.IsHomeDone == false) return nRtn;

            foreach (MotionLinkErrors error in Enum.GetValues(typeof(MotionLinkErrors)))
            {
                if (((Status as MotionPlusRStatus).AxisStatus & (1 << (int)error)) > 0)
                {
                    Status.AlarmStatus.AlarmMessage = $"{AxisName} {ErrorMessages[(int)error]}";
                    Status.AlarmStatus.IsAlarm = true;
                    return nRtn;
                }
            }

            Status.AlarmStatus.AlarmMessage = "";
            Status.AlarmStatus.IsAlarm = false;
            #endregion

            return nRtn;
#endif
        }

        private enum MotionLinkErrors
        {
            MotionAlarm = 0,
            HardwarePositiveLimit,
            HardwareNegativeLimit,
            SoftwarePositiveLimit,
            SoftwareNegativeLimit,
            //MotionAlarm = (int)NativeLib.FFLAG_ERRORALL,
            //HardwarePositiveLimit = (int)NativeLib.FFLAG_HWPOSILMT,
            //HardwareNegativeLimit = (int)NativeLib.FFLAG_HWNEGALMT,
            //SoftwarePositiveLimit = (int)NativeLib.FFLAG_SWPOGILMT,
            //SoftwareNegativeLimit = (int)NativeLib.FFLAG_SWNEGALMT,
        }

        private readonly string[] ErrorMessages = new string[]
        {
            "Motion Alarm (Error All)",
            "Hardware Positive Limit (+)",
            "Hardware Negative Limit (-)",
            "Software Positive Limit (+)",
            "Software Negative Limit (-)",
        };
    }
}
