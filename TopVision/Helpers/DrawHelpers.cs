using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Models;

namespace TopVision.Helpers
{
    public static class DrawHelpers
    {
        public static void Draw(this Mat img, CArcRing Arc, int thinkness = 10)
        {
            img.Draw(Arc, new Scalar(0x00, 0xff, 0x00, 0xff), thinkness);
        }

        public static void Draw(this Mat img, CArcRing Arc, Scalar color, int thinkness = 10)
        {
            Cv2.Ellipse(img,
                        Arc.Center.OCvSPoint,
                        new Size(Arc.InnerRadius, Arc.InnerRadius),
                        0,
                        Arc.StartAngle,
                        Arc.EndAngle,
                        color,
                        thinkness);

            Cv2.Ellipse(img,
                        Arc.Center.OCvSPoint,
                        new Size((Arc.InnerRadius + Arc.OuterRadius) / 2, (Arc.InnerRadius + Arc.OuterRadius) / 2),
                        0,
                        Arc.StartAngle,
                        Arc.EndAngle,
                        color,
                        thinkness / 2);

            Cv2.Ellipse(img,
                        Arc.Center.OCvSPoint,
                        new Size(Arc.OuterRadius, Arc.OuterRadius),
                        0,
                        Arc.StartAngle,
                        Arc.EndAngle,
                        color,
                        thinkness);

            Point StartAnglePoint1 = new Point((int)(Arc.InnerRadius * Math.Cos(Arc.StartAngle * Math.PI / 180.0)), (int)(Arc.InnerRadius * Math.Sin(Arc.StartAngle * Math.PI / 180.0)));
            Point StartAnglePoint2 = new Point((int)(Arc.OuterRadius * Math.Cos(Arc.StartAngle * Math.PI / 180.0)), (int)(Arc.OuterRadius * Math.Sin(Arc.StartAngle * Math.PI / 180.0)));
            StartAnglePoint1.X += Arc.Center.X;
            StartAnglePoint1.Y += Arc.Center.Y;
            StartAnglePoint2.X += Arc.Center.X;
            StartAnglePoint2.Y += Arc.Center.Y;

            Point EndAnglePoint1 = new Point((int)(Arc.InnerRadius * Math.Cos(Arc.EndAngle * Math.PI / 180.0)), (int)(Arc.InnerRadius * Math.Sin(Arc.EndAngle * Math.PI / 180.0)));
            Point EndAnglePoint2 = new Point((int)(Arc.OuterRadius * Math.Cos(Arc.EndAngle * Math.PI / 180.0)), (int)(Arc.OuterRadius * Math.Sin(Arc.EndAngle * Math.PI / 180.0)));
            EndAnglePoint1.X += Arc.Center.X;
            EndAnglePoint1.Y += Arc.Center.Y;
            EndAnglePoint2.X += Arc.Center.X;
            EndAnglePoint2.Y += Arc.Center.Y;

            Cv2.Line(img,
                     StartAnglePoint1,
                     StartAnglePoint2,
                     color,
                     thinkness);
            Cv2.Line(img,
                     EndAnglePoint1,
                     EndAnglePoint2,
                     color,
                     thinkness);
        }

        public static void RotationRect(this Mat OutputImg, Point2f CenterPoint, Rect Rect, double Theta)
        {
            RotatedRect rotationRect = new RotatedRect(new Point2f(CenterPoint.X, CenterPoint.Y), new Size2f(Rect.Width, Rect.Height), -(float)Theta);
            Point2f[] vectorPoints;
            vectorPoints = rotationRect.Points();
            for (int i = 0; i < 4; i++)
                Cv2.Line(OutputImg, vectorPoints[i].ToPoint(), vectorPoints[(i + 1) % 4].ToPoint(), new Scalar(0, 255, 0, 255), 10);
        }
    }
}
