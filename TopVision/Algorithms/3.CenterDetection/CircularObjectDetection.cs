using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Helpers;
using TopVision.Models;

namespace TopVision.Algorithms
{
    public class CircularObjectDetectionParameter : VisionParameterBase, ICircleDetection
    {
        #region Properties
        public double Radius
        {
            get { return _Radius; }
            set
            {
                _Radius = value;
                OnPropertyChanged();
            }
        }

        public int RadiusThreshold
        {
            get { return _RadiusThreshold; }
            set
            {
                _RadiusThreshold = value;
                OnPropertyChanged();
            }
        }

        public double CannyVal
        {
            get { return _CannyVal; }
            set
            {
                _CannyVal = value;
                OnPropertyChanged();
            }
        }

        public double Accumulator
        {
            get { return _Accumulator; }
            set
            {
                _Accumulator = value;
                OnPropertyChanged();
            }
        }

        #region Angle detection option
        public bool EnableAngleDetection
        {
            get { return _EnableAngleDetection; }
            set
            {
                _EnableAngleDetection = value;
                // if AngleDetection enable, disable MultipleCircleDetection
                if (value == true)
                {
                    EnableMultipleCircleDetection = false;
                }
                OnPropertyChanged();
            }
        }

        public CArcRing ArcRing
        {
            get { return _ArcRing; }
            set
            {
                _ArcRing = value;
                OnPropertyChanged();
            }
        }

        public string TeachMatEncodeString
        {
            get { return _TeachMatEncodeString; }
            set
            {
                _TeachMatEncodeString = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Multiple circle detection option
        public bool EnableMultipleCircleDetection
        {
            get { return _EnableMultipleCircleDetection; }
            set
            {
                _EnableMultipleCircleDetection = value;
                // if MultipleCircleDetection enable, disable AngleDetection
                if (value == true)
                {
                    EnableAngleDetection = false;
                }
                OnPropertyChanged();
            }
        }

        public int NumberOfCircleToDetect
        {
            get
            {
                if (EnableMultipleCircleDetection)
                {
                    return _NumberOfCircleToDetect;
                }
                else
                {
                    return 1;
                }
            }
            set
            {
                if (value < 1) value = 1;
                _NumberOfCircleToDetect = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        internal Mat GetMask(Size imageSize)
        {
            Mat mask = Mat.Ones(imageSize, MatType.CV_8UC1);
            Cv2.Circle(mask,
                       ArcRing.Center.OCvSPoint,
                       ArcRing.OuterRadius,
                       0,
                       thickness: -1);
            Cv2.Circle(mask,
                       ArcRing.Center.OCvSPoint,
                       ArcRing.InnerRadius,
                       255,
                       thickness: -1);

            return mask;
        }

        #region Privates
        private double _Radius = 500;
        private double _CannyVal = 100;
        private int _RadiusThreshold = 10;
        private double _Accumulator = 10;

        private bool _EnableAngleDetection = false;
        private CArcRing _ArcRing = new CArcRing();
        private string _TeachMatEncodeString;

        private bool _EnableMultipleCircleDetection;
        private int _NumberOfCircleToDetect = 1;
        #endregion
    }

    public class CircularObjectDetectionResult : VisionResultBase
    {
        public List<CCircle> DetectedCircles { get; set; } = new List<CCircle>() { new CCircle() };

        public double AngleMatchingScore { get; set; } = 0;

        public override string ToString()
        {
            return $"[{Judge}] Cost: {Cost:0.###}ms\n{DetectedOffset}";
        }
    }

    [DisplayName("Circular Object Detection")]
    public class CircularObjectDetection : VisionProcessBase
    {
        #region Properties
        private CircularObjectDetectionParameter ThisParameter
        {
            get { return (CircularObjectDetectionParameter)Parameter; }
        }

        private CircularObjectDetectionResult ThisResult
        {
            get { return (CircularObjectDetectionResult)Result; }
        }
        #endregion

        public CircularObjectDetection()
            : this(new CircularObjectDetectionParameter())
        {
        }

        public CircularObjectDetection(CircularObjectDetectionParameter parameter)
        {
            Parameter = parameter;
            Result = new CircularObjectDetectionResult();
        }

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new CircularObjectDetectionResult();

            // 0. RUN CONDITION CHECK
            if (ThisParameter.ROIs.Count != ThisParameter.NumberOfCircleToDetect)
            {
                ThisResult.Judge = EVisionJudge.NG;
                Log.Error($"Exactly {ThisParameter.NumberOfCircleToDetect} ROI(s) must to be set!");
                return EVisionRtnCode.FAIL;
            }

            /****** PHASE 1 - CENTER CIRCLE DETECTION #####  ******/
            // 1. CIRCLE DETECTION
            ThisResult.DetectedCircles = new List<CCircle>();
            for (int i = 0; i < ThisParameter.ROIs.Count; i++)
            {
                using (Mat imgROI = PreProcessedMat.SubMat(ThisParameter.ROIs[i].OCvSRect))
                {
                    // Run CircularObjects Transfrom
                    List<CircleSegment> roiCircles;
                    roiCircles = Cv2.HoughCircles(image: imgROI, method: HoughModes.Gradient,
                                                  dp: 1,
                                                  minDist: 1,
                                                  param1: ThisParameter.CannyVal,
                                                  param2: ThisParameter.Accumulator,
                                                  minRadius: (int)(ThisParameter.Radius - ThisParameter.RadiusThreshold),
                                                  maxRadius: (int)(ThisParameter.Radius + ThisParameter.RadiusThreshold)
                                                 ).ToList();

                    Log.Debug($"Total {roiCircles.Count()} Detected!");
                    // Check if result is null (No circle found)
                    if (roiCircles.Count() <= 0)
                    {
                        ThisResult.Judge = EVisionJudge.NG;
                        Log.Error($"No Circle found!");
                        return EVisionRtnCode.FAIL;
                    }

                    roiCircles = roiCircles.OrderBy(c => Math.Abs(c.Radius - ThisParameter.Radius)).ToList();
                    ThisResult.DetectedCircles.Add(new CCircle
                    {
                        Center = new CPoint2f
                        {
                            X = roiCircles[0].Center.X + ThisParameter.ROIs[i].X,
                            Y = roiCircles[0].Center.Y + ThisParameter.ROIs[i].Y
                        },
                        Radius = roiCircles[0].Radius
                    });
                }
            }

            CPoint detectedCirclesCrossCenter = new CPoint(0, 0);
            float averageRadius = 0;
            // 4. Apply result (Circle)
            for (int i = 0; i < ThisParameter.NumberOfCircleToDetect; i++)
            {
                detectedCirclesCrossCenter.X += (int)ThisResult.DetectedCircles[i].Center.X;
                detectedCirclesCrossCenter.Y += (int)ThisResult.DetectedCircles[i].Center.Y;

                averageRadius += ThisResult.DetectedCircles[i].Radius;
            }
            detectedCirclesCrossCenter.X /= ThisParameter.NumberOfCircleToDetect;
            detectedCirclesCrossCenter.Y /= ThisParameter.NumberOfCircleToDetect;
            averageRadius /= ThisParameter.NumberOfCircleToDetect;

            ThisResult.DetectedOffset = new XYTOffset
            {
                X = detectedCirclesCrossCenter.X - PreProcessedMat.Width / 2,
                Y = detectedCirclesCrossCenter.Y - PreProcessedMat.Height / 2
            };

            ThisParameter.ArcRing.Center = detectedCirclesCrossCenter;

            if (isTeachingMode)
            {
                ThisParameter.Radius = averageRadius;
            }

            // 0. Check run condition(s)
            if (ThisParameter.EnableAngleDetection == false && ThisParameter.EnableMultipleCircleDetection == false)
            {
                return EVisionRtnCode.OK;
            }

            if (ThisParameter.EnableMultipleCircleDetection)
            {
                /****** PHASE 2.1 - Multiple Circle Detection #####  ******/
                if (ThisParameter.NumberOfCircleToDetect != 4)
                {
                    // TODO: Handle different case
                    return EVisionRtnCode.OK;
                }

                // 1. Calculate difference of Theta
                // 1.1 Find location of each point in Quadrant
                //     Result of sort: 1st point -> 1st Quadrant ... 4nd point -> 4nd Quadrant
                ThisResult.DetectedCircles = ThisResult.DetectedCircles.OrderBy(p => p.Center.Y).ThenBy(p => p.Center.X).ToList();
                ThisResult.DetectedCircles.Reverse(2, 2);

                float dX_top = ThisResult.DetectedCircles[0].Center.X - ThisResult.DetectedCircles[1].Center.X;
                float dY_top = ThisResult.DetectedCircles[0].Center.Y - ThisResult.DetectedCircles[1].Center.Y;
                double Theta_top = Math.Atan(1.0 * dY_top / dX_top) * 180.0 / Math.PI;

                float dX_bot = ThisResult.DetectedCircles[0].Center.X - ThisResult.DetectedCircles[1].Center.X;
                float dY_bot = ThisResult.DetectedCircles[0].Center.Y - ThisResult.DetectedCircles[1].Center.Y;
                double Theta_bot = Math.Atan(1.0 * dY_bot / dX_bot) * 180.0 / Math.PI;

                ThisResult.DetectedOffset.Theta = (Theta_top + Theta_bot) / 2.0;
            }
            else if (ThisParameter.EnableAngleDetection)
            {
                /****** PHASE 2.2 - ROTATION DETECTION #####  ******/
                if (ThisParameter.ArcRing.IsValid == false)
                {
                    ThisResult.DetectedOffset.Theta = 0;
                    Log.Error("Setting Arc Ring not valid!");
                    return EVisionRtnCode.OK;
                }

                // 1. Extract Arc Ring
                using (Mat InputClone = (InputMat.Clone()).SetTo(0, ThisParameter.GetMask(InputMat.Size())))
                using (Mat sample = InputClone.GetRectSubPix(new Size(ThisParameter.ArcRing.OuterRadius * 2,
                                    ThisParameter.ArcRing.OuterRadius * 2),
                                    ThisParameter.ArcRing.Center.OCvSPoint))
                {
                    Mat master = ConvertHelpers.FromBase64String(ThisParameter.TeachMatEncodeString);

                    // First run or Parameter invalid
                    if (master == null || isTeachingMode == true || ThisParameter.ArcRing.OuterRadius * 2 != master.Width)
                    {
                        master = sample.Clone();
                        ThisParameter.TeachMatEncodeString = master.ToBase64String();
                    }

                    // 2. Warping Arc Image to Linear Image
                    Point2f center1 = new Point2f((float)(master.Width / 2.0), (float)(master.Height / 2.0));
                    Point2f center2 = new Point2f((float)(sample.Width / 2.0), (float)(sample.Height / 2.0));

                    float radius1 = (float)(master.Width / 2.0);
                    float radius2 = (float)(sample.Width / 2.0);

                    using (Mat unrolledMaster = new Mat())
                    using (Mat unrolledSample = new Mat())
                    {
                        Size unrolledSize = new Size(radius1, master.Width * 2);
                        Cv2.WarpPolar(master, unrolledMaster, unrolledSize, center1, radius1, InterpolationFlags.Linear, WarpPolarMode.Linear);
                        Cv2.WarpPolar(sample, unrolledSample, unrolledSize, center2, radius2, InterpolationFlags.Linear, WarpPolarMode.Linear);

                        master.Dispose();

                        using (Mat unrolledMasterCropped = unrolledMaster.SubMat(new Rect(ThisParameter.ArcRing.InnerRadius, 0, ThisParameter.ArcRing.OuterRadius - ThisParameter.ArcRing.InnerRadius, unrolledMaster.Height)))
                        using (Mat unrolledSampleCropped = unrolledSample.SubMat(new Rect(ThisParameter.ArcRing.InnerRadius, 0, ThisParameter.ArcRing.OuterRadius - ThisParameter.ArcRing.InnerRadius, unrolledSample.Height)))
                        using (Mat doubleMaster = new Mat())
                        {
                            Cv2.VConcat(unrolledMasterCropped, unrolledMasterCropped, doubleMaster);

                            double degreesPerPixel = 360.0 / unrolledSize.Height;

                            double minVal, maxVal;
                            double bestVal = 0;
                            Point minLoc, maxLoc;
                            Point bestLoc = new Point();

                            TemplateMatchModes matchModes = TemplateMatchModes.SqDiffNormed;

                            // 3. Template Matching
                            using (Mat matchingResult = new Mat())
                            {
                                Cv2.MatchTemplate(doubleMaster, unrolledSampleCropped, matchingResult, matchModes);
                                Cv2.MinMaxLoc(matchingResult, out minVal, out maxVal, out minLoc, out maxLoc);
                            }

                            switch (matchModes)
                            {
                                case TemplateMatchModes.SqDiff:
                                case TemplateMatchModes.SqDiffNormed:
                                    bestLoc = minLoc;
                                    bestVal = 1 - minVal;
                                    break;
                                case TemplateMatchModes.CCorr:
                                case TemplateMatchModes.CCorrNormed:
                                case TemplateMatchModes.CCoeff:
                                case TemplateMatchModes.CCoeffNormed:
                                    bestLoc = maxLoc;
                                    bestVal = maxVal;
                                    break;
                            }

                            ThisResult.AngleMatchingScore = bestVal;

                            if (bestVal < ThisParameter.Threshold)
                            {
                                ThisResult.Judge = EVisionJudge.NG;
                                ThisResult.NGMessage = "Angle failed!";
                                Log.Error($"Rotation angle detect fail! score {bestVal:0.000} < {ThisParameter.Threshold}");
                                return EVisionRtnCode.OK;
                            }

                            double angleRight = bestLoc.Y * degreesPerPixel;
                            double angleLeft = bestLoc.Y * degreesPerPixel - 360.0;

                            double angleResult = angleLeft;
                            if (Math.Abs(angleRight) < Math.Abs(angleLeft))
                            {
                                angleResult = angleRight;
                            }

                            ThisResult.DetectedOffset.Theta = angleResult;

                            ThisResult.Judge = EVisionJudge.OK;
                        }
                    }
                }
            }

            return EVisionRtnCode.OK;
        }

        internal override void GenerateOutputMat_DetectedMask()
        {
            if (ThisResult.Judge == EVisionJudge.OK)
            {
                Cv2.DrawMarker(OutputMat,
                               new Point(
                                   OutputMat.Width / 2 + (int)ThisResult.DetectedOffset.FromMillimeterToPixel(PixelSize / 1000).X,
                                   OutputMat.Height / 2 + (int)ThisResult.DetectedOffset.FromMillimeterToPixel(PixelSize / 1000).Y),
                               Colors.Point, MarkerTypes.Cross, 50, Thinknesses.DetectedRegion);

                foreach (CCircle circle in ThisResult.DetectedCircles)
                {
                    Cv2.Circle(OutputMat,
                       (int)circle.Center.X,
                       (int)circle.Center.Y,
                       (int)circle.Radius,
                       Colors.Box,
                       Thinknesses.DetectedRegion);

                    Cv2.DrawMarker(OutputMat,
                        new Point(
                            circle.Center.X,
                            circle.Center.Y),
                        Colors.Point, MarkerTypes.Cross, 30, Thinknesses.DetectedRegion);

                    Cv2.PutText(
                        OutputMat,
                        $"{circle.Radius:0.00}",
                        new Point((int)circle.Center.X, (int)circle.Center.Y - 20),
                        HersheyFonts.HersheySimplex,
                        FontSizes.MarkingText,
                        Colors.TextPositive,
                        thickness: Thinknesses.MarkingText
                    );
                }
            }

            if (ThisParameter.EnableAngleDetection)
            {
                if (Result.Judge == EVisionJudge.OK)
                {
                    CArcRing Arc = new CArcRing
                    {
                        Center = ThisParameter.ArcRing.Center,
                        StartAngle = ThisParameter.ArcRing.StartAngle - ThisResult.DetectedOffset.Theta,
                        EndAngle = ThisParameter.ArcRing.EndAngle - ThisResult.DetectedOffset.Theta,
                        InnerRadius = ThisParameter.ArcRing.InnerRadius,
                        OuterRadius = ThisParameter.ArcRing.OuterRadius,
                    };

                    OutputMat.Draw(Arc);

                    Cv2.PutText(OutputMat,
                                $"{(ThisResult.AngleMatchingScore * 100):0.00}%",
                                new Point((int)ThisParameter.ArcRing.Center.X, (int)ThisParameter.ArcRing.Center.Y + 80),
                                HersheyFonts.HersheySimplex,
                                FontSizes.MarkingText,
                                Colors.ROI,
                                thickness: Thinknesses.MarkingText
                    );
                }
            }
        }
    }
}
