using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.Models
{
    public class CArcRing : PropertyChangedNotifier
    {
        #region Properties
        public CPoint Center
        {
            get { return _Center; }
            set
            {
                _Center = value;
                OnPropertyChanged();
            }
        }

        public double StartAngle
        {
            get { return _StartAngle; }
            set
            {
                _StartAngle = value % (360 * 2 + 1);
                OnPropertyChanged();
            }
        }

        public double EndAngle
        {
            get { return _EndAngle; }
            set
            {
                _EndAngle = value % (360 * 2 + 1);
                OnPropertyChanged();
            }
        }

        public int InnerRadius
        {
            get { return _InnerRadius; }
            set
            {
                _InnerRadius = value;
                OnPropertyChanged();
            }
        }

        public int OuterRadius
        {
            get { return _OuterRadius; }
            set
            {
                _OuterRadius = value;
                OnPropertyChanged();
            }
        }

        public bool IsValid
        {
            get
            {
                if (InnerRadius >= OuterRadius)
                {
                    return false;
                }
                if (OuterRadius == 0)
                {
                    return false;
                }

                return true;
            }
        }
        #endregion

        #region Privates
        private CPoint _Center = new CPoint(0, 0);
        private double _StartAngle = 0;
        private double _EndAngle = 0;
        private int _InnerRadius = 0;
        private int _OuterRadius = 0;
        #endregion
    }
}
