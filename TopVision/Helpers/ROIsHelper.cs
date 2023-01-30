using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Models;

namespace TopVision.Helpers
{
    public enum ROIDirect
    {
        Horizontal,
        Vertical,
        Square,
    }

    public static class ROIsHelper
    {
        public static Point[] ToPointArray(this Point2f[] source)
        {
            List<Point> result = new List<Point>();

            foreach (var point2f in source)
            {
                result.Add((Point)point2f);
            }

            return result.ToArray();
        }

        public static ROIDirect GetDirect(this CRectangle ROI)
        {
            if (ROI.Width > ROI.Height)
            {
                return ROIDirect.Horizontal;
            }
            else if (ROI.Width < ROI.Height)
            {
                return ROIDirect.Vertical;
            }
            else
            {
                return ROIDirect.Square;
            }
        }

        public static Point Center(this Rect rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }
}
