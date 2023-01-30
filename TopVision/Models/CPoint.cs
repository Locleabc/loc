using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.Models
{
    public class CPoint : PropertyChangedNotifier
    {
        #region Properties
        public int X
        {
            get { return _X; }
            set
            {
                _X = value;
                OnPropertyChanged();
            }
        }

        public int Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public OpenCvSharp.Point OCvSPoint
        {
            get { return new OpenCvSharp.Point(X, Y); }
        }

        [JsonIgnore]
        public System.Windows.Point SystemPoint
        {
            get { return new System.Windows.Point(X, Y); }
        }
        #endregion

        #region Constructors
        public CPoint()
            : this((int)0, (int)0)
        {
        }

        public CPoint(double x, double y)
            : this((int)x, (int)y)
        {
        }

        public CPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Privates
        private int _X;
        private int _Y;
        #endregion
    }
}
