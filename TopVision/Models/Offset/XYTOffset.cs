using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Models
{
    public class XYTOffset : XYOffset, IThetaOffset
    {
        private double _Theta = 0;
        public double Theta
        {
            get { return _Theta; }
            set
            {
                _Theta = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"dX={X:0.###},dY={Y:0.###},dT={Theta:0.####}";
        }
    }
}
