using log4net.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace PLV_BracketAssemble.Define.WorkData
{
    public class CTaktTime : PropertyChangedNotifier
    {
        #region Properties
        public double Total
        {
            get { return _Total; }
            set
            {
                if (_Total == value) return;

                _Total = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Average));
            }
        }

        public double Maximum
        {
            get { return _Maximum; }
            set
            {
                if (_Maximum == value) return;

                _Maximum = value;
                OnPropertyChanged();
            }
        }

        public double CycleCurrent
        {
            get { return _CycleCurrent; }
            set
            {
                if (_CycleCurrent == value) return;

                _CycleCurrent = value;
                OnPropertyChanged();
            }
        }

        public double Average
        {
            get
            {
                if (Datas.WorkData.CountData.Total == 0) return 0;
                return Total / Datas.WorkData.CountData.Total;
            }
        }
        #endregion Properties

        #region Privates
        private double _Total;
        private double _Maximum = 0;
        private double _CycleCurrent = 0;
        #endregion
    }
}
