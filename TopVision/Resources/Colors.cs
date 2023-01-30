using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision
{
    public class Colors
    {
        public static Scalar ROI = new Scalar(0xff, 0x00, 0x00, 0xff);
        public static Scalar Point = new Scalar(0x00, 0x00, 0xff, 0xff);
        public static Scalar Text = new Scalar(0x00, 0xff, 0x00, 0xff);
        public static Scalar TextNegative = new Scalar(0x00, 0x00, 0xff, 0xff);
        public static Scalar TextPositive = new Scalar(0x00, 0xff, 0x00, 0xff);
        public static Scalar Box = new Scalar(0x3b, 0x55, 0xed, 0xff);
    }

    public class FontSizes
    {
        public static double ResulText = 3.5;
        public static double MarkingText = 3.0;
    }

    public class Thinknesses
    {
        public static int ResulText = 12;
        public static int MarkingText = 12;

        public static int DetectedRegion = 10;
        public static int ROI = 10;

        public static int Contour = 5;
        public static int CenterMark = 9;
    }
}
