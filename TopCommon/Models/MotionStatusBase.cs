using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopCom
{
    public class MotionStatusBase : PropertyChangedNotifier, IMotionStatus
    {
        #region Properties
        public bool IsConnected
        {
            get { return _IsConnected; }
            set
            {
                _IsConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }

        public bool IsMotionOn
        {
            get { return _IsMotionOn; }
            set
            {
                _IsMotionOn = value;
                OnPropertyChanged("IsMotionOn");
            }
        }

        public bool IsHomeDone
        {
            get { return _IsHomeDone; }
            set
            {
                if (_IsHomeDone == value) return;

                _IsHomeDone = value;
                OnPropertyChanged("IsHomeDone");
            }
        }

        public bool IsMotionDone
        {
            get { return _IsMotionDone; }
            set
            {
                _IsMotionDone = value;
                OnPropertyChanged("IsMotionDone");
            }
        }

        public CObjectAlarmStatus AlarmStatus
        {
            get { return _AlarmStatus; }
            set
            {
                _AlarmStatus = value;
                OnPropertyChanged("AlarmStatus");
            }
        }

        public uint AxisStatus
        {
            get { return _AxisStatus; }
            set
            {
                _AxisStatus = value;
                OnPropertyChanged("AxisStatus");
            }
        }

        public double CommandPosition
        {
            get { return _CommandPosition; }
            set
            {
                _CommandPosition = value;
                OnPropertyChanged("CommandPosition");
            }
        }


        public double ActualPosition
        {
            get { return _ActualPosition; }
            set
            {
                _ActualPosition = value;
                OnPropertyChanged("ActualPosition");
            }
        }

        public double PositionError
        {
            get { return _PositionError; }
            set
            {
                _PositionError = value;
                OnPropertyChanged("PositionError");
            }
        }

        public double ActualVelocity
        {
            get { return _ActualVelocity; }
            set
            {
                _ActualVelocity = value;
                OnPropertyChanged("ActualVelocity");
            }
        }
        #endregion

        #region Privates
        private bool _IsConnected;
        private bool _IsMotionOn;
        private bool _IsHomeDone;
        private bool _IsMotionDone;
        private CObjectAlarmStatus _AlarmStatus;
        private uint _AxisStatus;
        private double _CommandPosition;
        private double _PositionError;
        private double _ActualPosition;
        private double _ActualVelocity;
        #endregion

        #region Contructor
        public MotionStatusBase()
        {
            IsConnected = false;
            IsMotionOn = false;
            IsHomeDone = false;
            IsMotionDone = true;
            AlarmStatus = new CObjectAlarmStatus();
            CommandPosition = 0.0;
            ActualPosition = 0.0;
            PositionError = 0.0;
            ActualVelocity = 0.0;
        }
        #endregion
    }
}
