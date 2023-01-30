using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopCom;

namespace TopMotion
{
    public class MotionPlusRStatus : MotionStatusBase
    {
        private uint inputStatus;
        public uint InputStatus
        {
            get { return inputStatus; }
            set
            {
                inputStatus = value;
                OnPropertyChanged("InputStatus");
            }
        }

        private uint outputStatus;
        public uint OutputStatus
        {
            get { return outputStatus; }
            set
            {
                outputStatus = value;
                OnPropertyChanged("OutputStatus");
            }
        }

        private ushort positionTableItem;
        public ushort PositionTableItem
        {
            get { return positionTableItem; }
            set
            {
                positionTableItem = value;
                OnPropertyChanged("PositionTableItem");
            }
        }
    }
}
