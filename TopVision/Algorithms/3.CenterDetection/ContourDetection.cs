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
    /// Vision Parameter for <see cref="ContourDetection"/> process
    /// </summary>
    public class ContourDetectionParameter : VisionParameterBase, ICoutourDetection
    {
        #region Properties
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
        #endregion

        #region Privates
        private double _CoutourArea;
        #endregion
    }

    /// <summary>
    /// Vision Result for <see cref="ContourDetection"/> process
    /// </summary>
    public class ContourDetectionResult : VisionResultBase
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

        public override string ToString()
        {
            return $"[{Judge}] {DetectedOffset}, Cost: {Cost:0.###}ms";
        }
    }

    [DisplayName("Contour Detection")]
    public class ContourDetection : VisionProcessBase
    {
        private ContourDetectionParameter ThisParameter
        {
            get { return (ContourDetectionParameter)Parameter; }
        }

        private ContourDetectionResult ThisResult
        {
            get { return (ContourDetectionResult)Result; }
        }

        public ContourDetection()
            : this(new ContourDetectionParameter())
        {
        }

        public ContourDetection(ContourDetectionParameter parameter)
        {
            Parameter = parameter;
            Result = new ContourDetectionResult();
        }

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new ContourDetectionResult();

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
                    ThisResult.DetectedPoints.Add(new Tuple<CPoint, double>
                        (new CPoint((int)(Moment.M10 / Moment.M00) + ROI.X, (int)(Moment.M01 / Moment.M00) + ROI.Y), detectedArea)
                    );
                }
            }

            if (ThisResult.DetectedPoints.Count != ThisParameter.ROIs.Count)
            {
                ThisResult.Judge = EVisionJudge.NG;
                return EVisionRtnCode.OK;
            }

            ThisResult.DetectedOffset = ThisResult.DetectedPoints.GetCPoints().GetXYTOffset(new CPoint(InputMat.Width / 2, InputMat.Height / 2));

            return EVisionRtnCode.OK;
        }

        internal override void GenerateOutputMat_DetectedMask()
        {
            foreach (Tuple<CPoint, double> tuple in ThisResult.DetectedPoints)
            {
                Cv2.DrawMarker(OutputMat,
                               tuple.Item1.OCvSPoint,
                               Colors.Point,
                               MarkerTypes.Cross, 20,
                               Thinknesses.DetectedRegion);

                Cv2.PutText(
                    OutputMat,
                    $"{tuple.Item2}",
                    new Point(tuple.Item1.OCvSPoint.X, tuple.Item1.OCvSPoint.Y - 30),
                    HersheyFonts.HersheySimplex,
                    FontSizes.MarkingText,
                    Colors.Text,
                    thickness: Thinknesses.MarkingText
                );
            }

            foreach (Point[] contour in ThisResult.DetectedContours)
            {
                Cv2.DrawContours(OutputMat, new List<Point[]> { contour }, -1, Colors.Box, 10);
            }

            if (Result.Judge == EVisionJudge.OK)
            {
                Cv2.DrawMarker(OutputMat,
                    new Point(
                        ThisResult.DetectedOffset.X + OutputMat.Width / 2,
                        ThisResult.DetectedOffset.Y + OutputMat.Height / 2),
                    Colors.Point, MarkerTypes.Cross, 20,
                    Thinknesses.DetectedRegion);
            }
        }
    }
}
