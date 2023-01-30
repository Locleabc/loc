using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.Models
{
    public class CPoint2f : PropertyChangedNotifier
    {
        public static explicit operator CPoint(CPoint2f point2f) => new CPoint(point2f.X, point2f.Y);
        public static explicit operator CPoint2f(CPoint point) => new CPoint2f(point.X, point.Y);

        #region Properties
        public float X
        {
            get { return _X; }
            set
            {
                _X = value;
                OnPropertyChanged();
            }
        }

        public float Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public OpenCvSharp.Point2f OCvSPoint2f
        {
            get { return new OpenCvSharp.Point2f(X, Y); }
        }
        #endregion

        #region Constructors
        public CPoint2f()
            : this(0, 0)
        {
        }

        public CPoint2f(float x, float y)
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Privates
        private float _X;
        private float _Y;
        #endregion
    }
}
