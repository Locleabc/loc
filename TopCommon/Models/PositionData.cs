using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopCom.Models
{
    public class PositionData : PropertyChangedNotifier
    {
        #region Properties
        public string AxisName
        {
            get { return _AxisName; }
            set
            {
                _AxisName = value;
                OnPropertyChanged();
            }
        }

        public string PositionName
        {
            get { return _PositionName; }
            set
            {
                _PositionName = value;
                OnPropertyChanged();
            }
        }

        public double OldValue
        {
            get { return _OldValue; }
            set
            {
                _OldValue = value;
                OnPropertyChanged();
            }
        }

        public double Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged();
            }
        }

        public double? MaxValue
        {
            get { return _MaxValue; }
            set
            {
                _MaxValue = value;
                OnPropertyChanged();
            }
        }

        public double? MinValue
        {
            get { return _MinValue; }
            set
            {
                _MinValue = value;
                OnPropertyChanged();
            }
        }

        public double CurrentValue
        {
            get { return _CurrentValue; }
            set
            {
                _CurrentValue = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private string _AxisName;
        private string _PositionName;
        private double _OldValue;
        private double? _MinValue;
        private double? _MaxValue;
        private double _Value;
        private double _CurrentValue;
        #endregion

        #region Contructor
        public PositionData()
            : this(true)
        {
        }

        public PositionData(bool isNotBlank)
        {
            if (isNotBlank)
            {
                AxisName = "Undefined";
                PositionName = "Undefined";
                MaxValue = int.MaxValue;
                MinValue = int.MinValue;
            }
            else
            {
                AxisName = "";
                PositionName = "";
                MaxValue = null;
                MinValue = null;
            }
        }
        #endregion
    }
}
