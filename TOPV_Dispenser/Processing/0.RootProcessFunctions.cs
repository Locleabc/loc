using System.Linq;
using TopCom;
using TopCom.Processing;
using TOPV_Dispenser.Define;

namespace TOPV_Dispenser.Processing
{
    public partial class CRootProcess : ProcessingBase
    {
        private void CheckAlarmStatus()
        {
            CAlarmStatus alarmStatus = AlarmStatus;
            alarmStatus.MotionAlarmStatus = CheckMotionAlarmStatus();
            alarmStatus.UtilsAlarmStatus = CheckUtilsAlarmStatus();
            // RooProcess Mode will change to ToAlarm base on AlarmStatus property value
            AlarmStatus = alarmStatus;
        }

        private CObjectAlarmStatus CheckMotionAlarmStatus()
        {
            if (CDef.AllAxis.AxisList == null)
            {
                return null;
            }

            if (CDef.AllAxis.AxisList.Count(axis => axis.Status.AlarmStatus.IsAlarm == true) > 0)
            {
                IMotion alarmMotion = CDef.AllAxis.AxisList.First(axis => axis.Status.AlarmStatus.IsAlarm == true);
                CObjectAlarmStatus alarmStatus = alarmMotion.Status.AlarmStatus;

                Log.Error($"Motion alarm: {alarmMotion.AxisName} - [{alarmMotion.Status.AlarmStatus.AlarmCode}] \"{alarmMotion.Status.AlarmStatus.AlarmMessage}\"");

                return alarmStatus;
            }

            return null;
        }

        private CObjectAlarmStatus CheckUtilsAlarmStatus()
        {
            CObjectAlarmStatus utilsAlarmStatus = new CObjectAlarmStatus();
            if (CDef.IO.Input.MainPower == false)
            {
                utilsAlarmStatus = new CObjectAlarmStatus
                {
                    IsAlarm = true,
                    AlarmMessage = "Main Power is not suplied"
                };
            }

            if (CDef.IO.Input.EMS_SW == false)
            {
                utilsAlarmStatus = new CObjectAlarmStatus
                {
                    IsAlarm = true,
                    AlarmMessage = "Emergency Switch is pressed"
                };
            }

            if (CDef.IO.Input.MainCDA == false)
            {
                utilsAlarmStatus = new CObjectAlarmStatus
                {
                    IsAlarm = true,
                    AlarmMessage = "Main Air is not suplied"
                };
            }

            return utilsAlarmStatus;
        }
    }
}
