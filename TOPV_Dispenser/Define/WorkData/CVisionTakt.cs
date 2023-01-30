using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TOPV_Dispenser.Define
{
    public class CVisionTakt : PropertyChangedNotifier
    {
        #region Properties
        public double ProcessTime
        {
            get { return _ProcessTime; }
            set
            {
                _ProcessTime = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _ProcessTime;
        #endregion
    }
}
