
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.Models
{
    public class CCircle : PropertyChangedNotifier
    {
        #region Properties
        public CPoint2f Center
        {
            get { return _Center; }
            set
            {
                _Center = value;
                OnPropertyChanged();
            }
        }

        public float Radius
        {
            get { return _Radius; }
            set
            {
                _Radius = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public OpenCvSharp.CircleSegment OCvSCircle
        {
            get { return new OpenCvSharp.CircleSegment(Center.OCvSPoint2f, Radius); }
        }
        #endregion

        #region Constructors
        public CCircle()
            : this(new CPoint2f(), 0)

        {
        }

        public CCircle(float x, float y, float radius)
            : this(new CPoint2f(x, y), radius)
        {
        }

        public CCircle(CPoint2f center, float radius)
        {
            Center = center;
            Radius = radius;
        }
        #endregion

        public override bool Equals(object obj)
        {
            return this.Equals(obj as CCircle);
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode() + Radius.GetHashCode();
        }

        public bool Equals(CCircle circle)
        {
            if (circle == null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, circle))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != circle.GetType())
            {
                return false;
            }

            return (Center.X == circle.Center.X) && (Center.Y == circle.Center.Y) && (Radius == circle.Radius);
        }

        public override string ToString()
        {
            return $"X: {Center.X}, Y: {Center.Y}, R: {Radius}";
        }

        #region Privates
        private CPoint2f _Center = new CPoint2f(0, 0);
        private float _Radius;
        #endregion
    }
}
