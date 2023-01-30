using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopCom
{
    public static class Unit
    {
        #region Length
        public const string mm = "mm";
        public const string Micrometer = "μm";
        #endregion

        #region Time
        public const string MicroSecond = "µs";
        public const string MilliSecond = "ms";
        public const string Second = "s";
        #endregion

        #region Temperature
        public const string Celsius = "°C";
        public const string Kelvin = "°K";
        public const string Fahrenheit = "°F";
        #endregion

        #region Angle
        public const string Degree = "°";
        public const string Radians = "rad";
        #endregion

        #region Speed & Acceleration
        public const string mmPerSecond = "mm/s";
        public const string mmPerSecondSquare = "mm/s²";
        #endregion

        #region Count
        public const string EA = "EA";
        #endregion

        #region Scale
        public const string Percentage = "%";
        #endregion

        public const string ETC = "";
    }
}
