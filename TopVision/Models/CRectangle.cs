using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.Models
{
    public class CRectangle : PropertyChangedNotifier
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

        public int Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get { return _Height; }
            set
            {
                _Height = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public OpenCvSharp.Rect OCvSRect
        {
            get { return new OpenCvSharp.Rect(X, Y, Width, Height); }
        }
        #endregion

        #region Constructors
        public CRectangle()
            : this(0, 0, 100, 100)
        {
        }

        [JsonConstructor]
        public CRectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public CRectangle(OpenCvSharp.Point location, OpenCvSharp.Size size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }
        #endregion

        public CRectangle Resize(int Scale)
        {
            CRectangle ReSizeRect = new CRectangle();
            ReSizeRect.X = this.X / Scale;
            ReSizeRect.Y = this.Y / Scale;
            ReSizeRect.Width = this.Width / Scale;
            ReSizeRect.Height = this.Height / Scale;
            return ReSizeRect;
        }
        #region Privates
        private int _X;
        private int _Y;
        private int _Width;
        private int _Height;
        #endregion
    }
}
