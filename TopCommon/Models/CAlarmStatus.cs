using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopCom;

namespace TopCom
{
    public class CAlarmStatus : PropertyChangedNotifier
    {
        #region Properties
        public CObjectAlarmStatus MotionAlarmStatus
        {
            get { return motionAlarmStatus; }
            set
            {
                motionAlarmStatus = value;
                OnPropertyChanged();
            }
        }

        public CObjectAlarmStatus UtilsAlarmStatus
        {
            get { return _UtilsAlarmStatus; }
            set
            {
                _UtilsAlarmStatus = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public CAlarmStatus()
        {
            MotionAlarmStatus = new CObjectAlarmStatus();
            UtilsAlarmStatus = new CObjectAlarmStatus();
        }
        #endregion

        #region Privates
        private CObjectAlarmStatus motionAlarmStatus;
        private CObjectAlarmStatus _UtilsAlarmStatus;
        #endregion
    }
}
