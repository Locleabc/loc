using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopCom;

namespace TopMotion
{
    public class MotionFas16000Status : MotionStatusBase
    {
        #region Properties
        public uint InputStatus
        {
            get { return _InputStatus; }
            set
            {
                _InputStatus = value;
                OnPropertyChanged();
            }
        }

        public uint OutputStatus
        {
            get { return _OutputStatus; }
            set
            {
                _OutputStatus = value;
                OnPropertyChanged();
            }
        }

        public ushort PositionTableItem
        {
            get { return _PositionTableItem; }
            set
            {
                _PositionTableItem = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private uint _InputStatus;
        private uint _OutputStatus;
        private ushort _PositionTableItem;
        #endregion
    }
}
