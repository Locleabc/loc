using System.Linq;
using TopCom;
using TopCom.Processing;
using VCM_CoilLoading.Define;

namespace VCM_CoilLoading.Processing
{
    public partial class CRootProcess : ProcessingBase
    {
        private void CheckAlarmStatus()
        {
            CAlarmStatus alarmStatus = AlarmStatus;
            alarmStatus.MotionAlarmStatus = CheckMotionAlarmStatus();
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
    }
}
