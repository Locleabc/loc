using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Models;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision Parameter for <see cref="CircleDetection"/> process
    /// </summary>
    public class CircleDetectionParameter : VisionParameterBase
    {
        public int MinRadius
        {
            get { return _MinRadius; }
            set
            {
                _MinRadius = value;
                OnPropertyChanged();
            }
        }

        public int MaxRadius
        {
            get { return _MaxRadius; }
            set
            {
                _MaxRadius = value;
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

        public CircleDetectionParameter()
        {
        }

        #region Privates
        private int _MinRadius = 40;
        private int _MaxRadius = 80;

        private double _CannyVal = 200;
        private double _Accumulator = 10;
        #endregion
    }

    public class CircleDetectionResult : VisionResultBase
    {
        public List<CCircle> DetectedCircles { get; set; } = new List<CCircle>();
        public override string ToString()
        {
            return $"[{Judge}] Cost: {Cost:0.###}ms, {DetectedCircles.Count}EA circle detected";
        }
    }

    [DisplayName(" Circle Detection [Ball Inspection]")]
    public class CircleDetection : VisionProcessBase
    {
        #region Privates
        private CircleDetectionParameter ThisParameter
        {
            get { return (CircleDetectionParameter)Parameter; }
        }

        private CircleDetectionResult ThisResult
        {
            get { return (CircleDetectionResult)Result; }
        }
        #endregion

        #region Constructors
        public CircleDetection()
            : this(new CircleDetectionParameter())
        {
        }

        public CircleDetection(CircleDetectionParameter parameter)
        {
            Parameter = parameter;
            Result = new CircleDetectionResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new CircleDetectionResult();

            if (ThisParameter.ROIs.Count != 1)
            {
                ThisResult.Judge = EVisionJudge.NG;
                Log.Error("Exactly 1 ROI must to be set!");
                return EVisionRtnCode.FAIL;
            }

            using (Mat imgROI = PreProcessedMat.SubMat(ThisParameter.ROIs[0].OCvSRect))
            {
                //ROI Image 대신, Detect on Contour that contour area is [Radius - RadiusThreshol] ranger.
                Point[][] contours = new Point[][] { };
                HierarchyIndex[] tmpHierachyIndex = new HierarchyIndex[] { };

                Cv2.FindContours(imgROI,
                                out contours,
                                out tmpHierachyIndex,
                                RetrievalModes.CComp,
                                ContourApproximationModes.ApproxSimple);

                using (Mat ContourImg = new Mat(imgROI.Height, imgROI.Width, MatType.CV_8UC1, new Scalar(0)))
                {
                    for (int i = 0; i < contours.Count(); i++)
                    {
                        ////////////////// METHOD 1 START //////////////////////////
                        //RotatedRect BoundingRect = Cv2.MinAreaRect(contours[i]);
                        //double Ratio = (double)BoundingRect.Size.Width / (double)BoundingRect.Size.Height;
                        //if (BoundingRect.Size.Width < ThisParameter.Radius * 3
                        //    && BoundingRect.Size.Height > (ThisParameter.Radius - ThisParameter.RadiusThreshold)
                        //    && (Ratio > ThisParameter.Threshold && Ratio < (2 - ThisParameter.Threshold)))
                        //{
                        //    Cv2.DrawContours(ContourImg, contours, i, new Scalar(255, 255, 255), 3);
                        //}
                        ////////////////// METHOD 1 END //////////////////////////

                        ////////////////// METHOD 2 START ////////////////////////
                        double area = Cv2.ContourArea(contours[i]);
                        double arclength = Cv2.ArcLength(contours[i], true);
                        double circularity = 4 * Cv2.PI * area / (arclength * arclength); //https://en.wikipedia.org/wiki/Roundness
                        if (arclength > 2 * Cv2.PI * (ThisParameter.MinRadius)
                           && arclength < 2 * Cv2.PI * (ThisParameter.MaxRadius))
                        {
                            if (circularity > ThisParameter.Threshold)
                            {
                                Cv2.DrawContours(ContourImg, contours, i, new Scalar(255, 255, 255), 1);
                            }
                        }
                        ////////////////// METHOD 2 END ////////////////////////
                    }

                    //Cv2.ImWrite(@"D:\TOP\TOPVEQ\Images\0ContourImg.jpg", ContourImg);

                    CircleSegment[] Circles = Cv2.HoughCircles(ContourImg,
                                                               HoughModes.Gradient,
                                                               1,
                                                               ThisParameter.MinRadius * 2,
                                                               ThisParameter.CannyVal,
                                                               ThisParameter.Accumulator,
                                                               (int)(ThisParameter.MinRadius),
                                                               (int)(ThisParameter.MaxRadius));

                    // 2. Check if result is null (No circle found)
                    if (Circles.Count() == 0)
                    {
                        ThisResult.Judge = EVisionJudge.OK;
                        return EVisionRtnCode.OK;
                    }

                    // 4. Apply result (Circle)
                    for (int i = 0; i < Circles.Count(); i++)
                    {
                        ThisResult.DetectedCircles.Add(new CCircle
                        {
                            Center = new CPoint2f
                            {
                                X = Circles[i].Center.X + ThisParameter.ROIs[0].X,
                                Y = Circles[i].Center.Y + ThisParameter.ROIs[0].Y
                            },
                            Radius = Circles[i].Radius
                        });
                    }
                }
            }

            ThisResult.Judge = EVisionJudge.NG;
            Log.Error("Circle[Ball] is detected !");
            return EVisionRtnCode.FAIL;
        }

        internal override void GenerateOutputMat_DetectedMask()
        {
            if (Result.Judge == EVisionJudge.NG)
            {
                foreach (CCircle circle in ThisResult.DetectedCircles)
                {
                    Cv2.Circle(OutputMat,
                           (int)circle.Center.X,
                           (int)circle.Center.Y,
                           (int)circle.Radius,
                           new Scalar(0, 255, 255, 255),
                           Thinknesses.DetectedRegion);

                    Cv2.PutText(
                        OutputMat,
                        $"{circle.Radius}",
                        new Point(circle.Center.X - 20, circle.Center.Y - 20),
                        HersheyFonts.HersheySimplex,
                        FontSizes.MarkingText,
                        new Scalar(255, 255, 0, 255),
                        thickness: Thinknesses.MarkingText
                    );
                }
            }
        }
    }
}
