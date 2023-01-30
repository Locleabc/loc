using TopVision.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TopVision.Helpers;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision Parameter for <see cref="FourLinesDetection"/> process
    /// </summary>
    public class FourLinesDetectionParameter : VisionParameterBase
    {
        private double _CoutourArea;

        public double CoutourArea
        {
            get { return _CoutourArea; }
            set
            {
                if (_CoutourArea == value) return;

                _CoutourArea = value;
                OnPropertyChanged();
            }

        }
    }

    /// <summary>
    /// Vision Result for <see cref="FourLinesDetection"/> process
    /// </summary>
    public class FourLinesDetectionResult : VisionResultBase
    {
        private List<Tuple<CPoint, double>> _DetectedPoints = new List<Tuple<CPoint, double>>();
        public List<Tuple<CPoint, double>> DetectedPoints
        {
            get { return _DetectedPoints; }
            set
            {
                _DetectedPoints = value;
            }
        }

        public List<Point[]> DetectedContours { get; set; } = new List<Point[]>();
        public List<Tuple<RotatedRect, double>> DetectedRects { get; set; } = new List<Tuple<RotatedRect, double>>();

        public List<Line2D> DetectedLines { get; set; } = new List<Line2D>();

        public override string ToString()
        {
            return $"[{Judge}] {DetectedOffset}, Cost: {Cost:0.###}ms";
        }
    }

    [DisplayName("Four Lines Detection")]
    public class FourLinesDetection : VisionProcessBase
    {
        private FourLinesDetectionParameter ThisParameter
        {
            get { return (FourLinesDetectionParameter)Parameter; }
        }

        private FourLinesDetectionResult ThisResult
        {
            get { return (FourLinesDetectionResult)Result; }
        }

        public FourLinesDetection()
            : this(new FourLinesDetectionParameter())
        {
        }

        public FourLinesDetection(FourLinesDetectionParameter parameter)
        {
            Parameter = parameter;
            Result = new FourLinesDetectionResult();
        }

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new FourLinesDetectionResult();

            if (ThisParameter.ROIs.Count != 4)
            {
                ThisResult.Judge = EVisionJudge.NG;
                Log.Error("Exactly 4 ROI must to be set!");
                return EVisionRtnCode.FAIL;
            }

            // Find all matching contour
            foreach (CRectangle ROI in ThisParameter.ROIs)
            {
                using (Mat imgROI = PreProcessedMat.SubMat(ROI.OCvSRect))
                {
                    Point[][] contours = new Point[][] { };
                    HierarchyIndex[] tmpHierachyIndex = new HierarchyIndex[] { };
                    Cv2.FindContours(imgROI, out contours, out tmpHierachyIndex, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);

                    if (contours.Count() <= 0) continue /* Next ROI */;

                    int foundIndex = 0;
                    double minDiff = double.MaxValue;
                    double detectedArea = 0;

                    for (int i = 0; i < contours.Count(); i++)
                    {
                        double area = Cv2.ContourArea(contours[i]);
                        if (area == 0) continue /* Next contour */;

                        if (Math.Abs(ThisParameter.CoutourArea - area) < minDiff)
                        {
                            minDiff = Math.Abs(ThisParameter.CoutourArea - area);
                            foundIndex = i;
                            detectedArea = area;
                        }
                    }

                    if ((ThisParameter.CoutourArea - minDiff) / ThisParameter.CoutourArea < ThisParameter.Threshold) continue /* Next ROI */;

                    Moments Moment = Cv2.Moments(contours[foundIndex]);

                    for (int i = 0; i < contours[foundIndex].Count(); i++)
                    {
                        contours[foundIndex][i].X += ROI.X;
                        contours[foundIndex][i].Y += ROI.Y;
                    }

                    ThisResult.DetectedContours.Add(contours[foundIndex]);
                    ThisResult.DetectedRects.Add(new Tuple<RotatedRect, double>(Cv2.MinAreaRect(contours[foundIndex]), detectedArea));
                    ThisResult.DetectedLines.Add(Cv2.FitLine(contours[foundIndex], DistanceTypes.L2, 0, 0.01, 0.01));
                }
            }

            if (ThisResult.DetectedContours.Count != ThisParameter.ROIs.Count ||
                ThisResult.DetectedContours.Count == 0)
            {
                ThisResult.Judge = EVisionJudge.NG;
                return EVisionRtnCode.OK;
            }

            ThisResult.DetectedOffset = new XYTOffset();
            foreach (Tuple<RotatedRect, double> rect in ThisResult.DetectedRects)
            {
                ThisResult.DetectedOffset.X += rect.Item1.Center.X;
                ThisResult.DetectedOffset.Y += rect.Item1.Center.Y;
            }
            ThisResult.DetectedOffset.X /= ThisResult.DetectedRects.Count;
            ThisResult.DetectedOffset.Y /= ThisResult.DetectedRects.Count;

            ThisResult.DetectedOffset.X -= InputMat.Width / 2;
            ThisResult.DetectedOffset.Y -= InputMat.Height / 2;

            ThisResult.DetectedOffset.Theta = (ThisResult.DetectedRects[0].Item1.Angle + ThisResult.DetectedRects[2].Item1.Angle) / 2;

            return EVisionRtnCode.OK;
        }

        internal override void GenerateOutputMat_DetectedMask()
        {
            foreach (Tuple<RotatedRect, double> tuple in ThisResult.DetectedRects)
            {
                Point[] points = new Point[] { };
                Cv2.DrawContours(
                    OutputMat,
                    new List<Point[]> { Cv2.BoxPoints(tuple.Item1).PointList().ToArray() },
                    0,
                    Colors.Box,
                    thickness: Thinknesses.DetectedRegion
                );

                Cv2.DrawMarker(OutputMat,
                    new Point(
                        (int)tuple.Item1.Center.X, (int)tuple.Item1.Center.Y),
                    Colors.Point, MarkerTypes.Cross, 20, 10);

                Cv2.PutText(
                    OutputMat,
                    $"{tuple.Item2}",
                    new Point((int)tuple.Item1.Center.X, (int)tuple.Item1.Center.Y + 60),
                    HersheyFonts.HersheySimplex,
                    FontSizes.MarkingText,
                    Colors.Text,
                    thickness: Thinknesses.MarkingText
                );
            }

            if (Result.Judge == EVisionJudge.OK)
            {
                Cv2.DrawMarker(OutputMat,
                    new Point(
                        ThisResult.DetectedOffset.X + OutputMat.Width / 2,
                        ThisResult.DetectedOffset.Y + OutputMat.Height / 2),
                    Colors.Point, MarkerTypes.Cross, 20, 10);
            }
        }
    }
}
