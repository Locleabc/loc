using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.Models
{
    public class XYOffset : PropertyChangedNotifier, IXYOffset
    {
        private double _X = 0;

        public double X
        {
            get { return _X; }
            set
            {
                _X = value;
                OnPropertyChanged();
            }
        }

        private double _Y = 0;

        public double Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"dX={X:0.###},dY={Y:0.###}";
        }
    }
}
