using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.Models
{
    public class COutputMatOption : PropertyChangedNotifier
    {
        #region Properties
        public bool ShowROI
        {
            get { return _ShowROI; }
            set
            {
                _ShowROI = value;
                OnPropertyChanged();
            }
        }

        public bool ShowResultString
        {
            get { return _ShowResultString; }
            set
            {
                _ShowResultString = value;
                OnPropertyChanged();
            }
        }

        public bool ShowDetectedMask
        {
            get { return _ShowDetectedMask; }
            set
            {
                _ShowDetectedMask = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private bool _ShowROI;
        private bool _ShowResultString;
        private bool _ShowDetectedMask;
        #endregion
    }
}
