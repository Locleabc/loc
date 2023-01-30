namespace TopCom
{
    public class CObjectAlarmStatus : PropertyChangedNotifier
    {
        #region Properties
        public bool IsAlarm
        {
            get
            {
                return _IsAlarm;
            }
            set
            {
                _IsAlarm = value;
                OnPropertyChanged();
            }
        }

        public int AlarmCode
        {
            get { return _AlarmCode; }
            set
            {
                _AlarmCode = value;
                OnPropertyChanged();
            }
        }

        public string AlarmMessage
        {
            get { return _AlarmMessage; }
            set
            {
                _AlarmMessage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private bool _IsAlarm;
        private int _AlarmCode;
        private string _AlarmMessage;
        #endregion

        #region Contructor
        public CObjectAlarmStatus()
        {
            IsAlarm = false;
            AlarmCode = 0;
            AlarmMessage = "";
        }
        #endregion
    }
}
