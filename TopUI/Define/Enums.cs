using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace TopUI.Define
{
    public enum ECellStatus
    {
        Empty = 0,
        /// <summary>
        /// Cutting status, etc
        /// </summary>
        PrepareDone,
        Processing,
        NGVision,
        OKVision,
        NGPickOrPlace,
        OK,
        InspNG,
        InspOK,
        MAX
    }

    public static class CColors
    {
        public static Dictionary<string, SolidColorBrush> CellColor
        {
            get
            {
                return new Dictionary<string, SolidColorBrush>()
                {
                    { "Empty", new SolidColorBrush(Colors.Gray)},
                    { "PreDone", new SolidColorBrush(Colors.Silver)},
                    { "Processing", new SolidColorBrush(Colors.Orange)},
                    { "NG Vision", new SolidColorBrush(Colors.Red)},
                    { "OK Vision", new SolidColorBrush(Colors.Blue)},
                    { "NG P&P", new SolidColorBrush(Colors.DarkRed)},
                    { "OK", new SolidColorBrush(Colors.Green)},
                    { "Insp. NG", new SolidColorBrush(Colors.OrangeRed)},
                    { "Insp. OK", new SolidColorBrush(Colors.LightSeaGreen)},
                    { "Undefined", new SolidColorBrush(Colors.Black)},
                };
            }
        }
    }

    public enum ECellShape
    {
        Circle,
        Rectangle
    }
}
