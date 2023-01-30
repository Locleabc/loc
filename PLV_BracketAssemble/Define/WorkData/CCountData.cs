using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace PLV_BracketAssemble.Define.WorkData
{
    public class CCountData : PropertyChangedNotifier
    {
        #region Properties
        public uint Total
        { 
            get 
            {
                return OK + VisionNG;
            }
        }

        public uint OK
        {
            get { return _OK; }
            set
            {
                if (_OK == value) return;

                _OK = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public uint VisionNG
        {
            get { return _VisionNG; }
            set
            {
                if (_VisionNG == value) return;

                _VisionNG = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }
        #endregion

        #region Privates
        private uint _OK = 0;
        private uint _VisionNG = 0;
        #endregion
    }
}
