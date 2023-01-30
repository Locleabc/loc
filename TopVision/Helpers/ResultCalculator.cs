using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Models;

namespace TopVision.Helpers
{
    public static class Calculation
    {
        public static Scalar MSSIM(Mat i1, Mat i2)
        {
            const double C1 = 6.5025, C2 = 58.5225;
            /***************************** INITS **********************************/
            MatType d = MatType.CV_32F;

            Mat I1 = new Mat(), I2 = new Mat();
            i1.ConvertTo(I1, d);           // cannot calculate on one byte large values
            i2.ConvertTo(I2, d);

            Mat I2_2 = I2.Mul(I2);        // I2^2
            Mat I1_2 = I1.Mul(I1);        // I1^2
            Mat I1_I2 = I1.Mul(I2);        // I1 * I2
            /***********************PRELIMINARY COMPUTING ******************************/

            Mat mu1 = new Mat(), mu2 = new Mat();   //
            Cv2.GaussianBlur(I1, mu1, new OpenCvSharp.Size(11, 11), 1.5);
            Cv2.GaussianBlur(I2, mu2, new OpenCvSharp.Size(11, 11), 1.5);

            Mat mu1_2 = mu1.Mul(mu1);
            Mat mu2_2 = mu2.Mul(mu2);
            Mat mu1_mu2 = mu1.Mul(mu2);

            Mat sigma1_2 = new Mat(), sigma2_2 = new Mat(), sigma12 = new Mat();

            Cv2.GaussianBlur(I1_2, sigma1_2, new OpenCvSharp.Size(11, 11), 1.5);
            sigma1_2 -= mu1_2;

            Cv2.GaussianBlur(I2_2, sigma2_2, new OpenCvSharp.Size(11, 11), 1.5);
            sigma2_2 -= mu2_2;

            Cv2.GaussianBlur(I1_I2, sigma12, new OpenCvSharp.Size(11, 11), 1.5);
            sigma12 -= mu1_mu2;

            ///////////////////////////////// FORMULA ////////////////////////////////
            Mat t1, t2, t3;

            t1 = 2 * mu1_mu2 + C1;
            t2 = 2 * sigma12 + C2;
            t3 = t1.Mul(t2);              // t3 = ((2*mu1_mu2 + C1).*(2*sigma12 + C2))

            t1 = mu1_2 + mu2_2 + C1;
            t2 = sigma1_2 + sigma2_2 + C2;
            t1 = t1.Mul(t2);               // t1 =((mu1_2 + mu2_2 + C1).*(sigma1_2 + sigma2_2 + C2))

            Mat ssim_map = new Mat();
            Cv2.Divide(t3, t1, ssim_map);      // ssim_map =  t3./t1;

            Scalar mssim = Cv2.Mean(ssim_map);// mssim = average of ssim map

            return mssim;
        }
    }

    public static class ResultCalculator
    {
        /// <summary>
        /// Calculate X, Y, T offset base on some point locations
        /// </summary>
        /// <param name="points">Input points list</param>
        /// <param name="center">Base point center</param>
        /// <returns></returns>
        public static XYTOffset GetXYTOffset(this List<CPoint> points, CPoint center)
        {
            XYTOffset offset = new XYTOffset();
            CPoint calculatedCenter = new CPoint();

            if (points.Count != 4)
            {
                if (points.Count <= 0)
                {
                    Console.WriteLine("Input of point list must have more than 0 points.");
                    return offset;
                }

                foreach (CPoint point in points)
                {
                    calculatedCenter.X += point.X;
                    calculatedCenter.Y += point.Y;
                }
                calculatedCenter.X /= points.Count;
                calculatedCenter.Y /= points.Count;

                offset.X = calculatedCenter.X - center.X;
                offset.Y = calculatedCenter.Y - center.Y;

                Console.WriteLine("Input of point list must have exactly 4 points to calculate Theta");
                return offset;
            }

            // 1. Calculate difference of Theta
            // 1.1 Find location of each point in Quadrant
            //     Result of sort: 1st point -> 1st Quadrant ... 4nd point -> 4nd Quadrant
            points = points.OrderBy(p => p.Y).ThenBy(p => p.X).ToList();
            points.Reverse(2, 2);

            // 1.2 Do math to calculate difference of Theta
            int dX = points[0].X - points[1].X;
            int dY = points[0].Y - points[1].Y;
            double Theta = Math.Atan(1.0 * dY / dX) * 180.0 / Math.PI;

            offset.Theta = Theta;

            // 2. Calculate center of all points
            foreach (CPoint point in points)
            {
                calculatedCenter.X += point.X;
                calculatedCenter.Y += point.Y;
            }
            calculatedCenter.X /= points.Count;
            calculatedCenter.Y /= points.Count;

            offset.X = calculatedCenter.X - center.X;
            offset.Y = calculatedCenter.Y - center.Y;

            return offset;
        }

        public static List<CPoint> GetCPoints(this List<Tuple<CPoint, double>> tuples)
        {
            List<CPoint> CPoints = new List<CPoint>();

            tuples.ForEach(tuple => CPoints.Add(tuple.Item1));

            return CPoints;
        }

        public static List<CPoint> GetCPoints(this List<Tuple<Rect, double>> tuples)
        {
            List<CPoint> CPoints = new List<CPoint>();

            tuples.ForEach(tuple =>
            {
                Point centerPointOfRect = new Point();
                centerPointOfRect += (tuple.Item1.TopLeft + tuple.Item1.BottomRight) * 0.5;

                CPoints.Add(new CPoint(centerPointOfRect.X, centerPointOfRect.Y));
            });

            return CPoints;
        }

        public static List<Point> PointList(this IEnumerable<Point2f> points2f)
        {
            List<Point> result = new List<Point> { };

            foreach (Point2f point2f in points2f)
            {
                result.Add(new Point(point2f.X, point2f.Y));
            }

            return result;
        }
    }
}
