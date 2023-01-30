using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Helpers;
using TopVision.Models;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision Parameter for <see cref="BoundaryLineAngleDetection"/> process
    /// </summary>
    public class BoundaryLineAngleDetectionParameter : VisionParameterBase
    {
        public EDetectionDirection DetectionDirection
        {
            get { return _DetectionDirection; }
            set
            {
                if (_DetectionDirection == value) return;
                _DetectionDirection = value;
                OnPropertyChanged();
            }
        }

        #region Properies

        public int ReferenceAngle
        {
            get { return _ReferenceAngle; }
            set
            {
                if (_ReferenceAngle == value) return;

                _ReferenceAngle = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private int _ReferenceAngle = 0;
        private EDetectionDirection _DetectionDirection = EDetectionDirection.Bottom2Top;
        #endregion
    }

    /// <summary>
    /// Vision Result for <see cref="BoundaryLineAngleDetection"/> process
    /// </summary>
    public class BoundaryLineAngleDetectionResult : VisionResultBase
    {
        #region Properties
        public List<Point> DetectedPoints { get; set; }
        public Line2D DetectedLine { get; set; }
        public Point[] DetectedPointofLine { get; set; } 
        #endregion

        public BoundaryLineAngleDetectionResult()
        {
            DetectedPoints = new List<Point>();
            DetectedLine = new Line2D(0, 0, 0, 0);
            DetectedPointofLine = new Point[2];
        }
    }

    [DisplayName("Boundary Line Angle Detection Detection")]
    public class BoundaryLineAngleDetection : VisionProcessBase
    {
        #region Privates
        private BoundaryLineAngleDetectionParameter ThisParameter
        {
            get { return (BoundaryLineAngleDetectionParameter)Parameter; }
        }

        private BoundaryLineAngleDetectionResult ThisResult
        {
            get { return (BoundaryLineAngleDetectionResult)Result; }
        }
        #endregion

        #region Constructors
        public BoundaryLineAngleDetection()
            : this(new BoundaryLineAngleDetectionParameter())
        {
        }

        public BoundaryLineAngleDetection(BoundaryLineAngleDetectionParameter parameter)
        {
            Parameter = parameter;
            Result = new BoundaryLineAngleDetectionResult();
        }
        #endregion

        private CRectangle fixtureROI = new CRectangle();

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new BoundaryLineAngleDetectionResult();

            EVisionRtnCode rtnCode = EVisionRtnCode.OK;

            if (ThisParameter.ROIs.Count != 1)
            {
                Log.Error("Exactly 1 ROI need to be set");
                return EVisionRtnCode.FAIL;
            }

            fixtureROI = new CRectangle()
            {
                X = (int)(ThisParameter.ROIs[0].X - ThisParameter.TeachingFixtureOffset.FromMillimeterToPixel(PixelSize / 1000).X + ThisParameter.RuntimeFixtureOffset.FromMillimeterToPixel(PixelSize / 1000).X),
                Y = (int)(ThisParameter.ROIs[0].Y - ThisParameter.TeachingFixtureOffset.FromMillimeterToPixel(PixelSize / 1000).Y + ThisParameter.RuntimeFixtureOffset.FromMillimeterToPixel(PixelSize / 1000).Y),
                Width = ThisParameter.ROIs[0].Width,
                Height = ThisParameter.ROIs[0].Height,
            };

            using (Mat imgROI = PreProcessedMat.SubMat(fixtureROI.OCvSRect))
            {
                ///////////////////////////////////METHOD: BASE DIRECTION EDGE////////////
                int[] colDetected = new int[imgROI.Width];
                int[] rowDetected = new int[imgROI.Height];

                switch (ThisParameter.DetectionDirection)
                {
                    case EDetectionDirection.Left2Right:
                        {
                            for (int i = 1; i < imgROI.Width; i++)
                            {
                                for (int j = 0; j < imgROI.Height; j++)
                                {
                                    if (rowDetected[j] == 1) continue;
                                    if (imgROI.Get<byte>(j, i) == 255)
                                    {
                                        ThisResult.DetectedPoints.Add(new OpenCvSharp.Point(i, j)); // METHOD 2
                                        rowDetected[j] = 1;
                                    }
                                }
                            }
                        }
                        break;
                    case EDetectionDirection.Right2Left:
                        {
                            for (int i = imgROI.Width - 1; i > 0 ; i--)
                            {
                                for (int j = 0; j < imgROI.Height; j++)
                                {
                                    if (rowDetected[j] == 1) continue;
                                    if (imgROI.Get<byte>(j, i) == 255)
                                    {
                                        ThisResult.DetectedPoints.Add(new OpenCvSharp.Point(i, j)); // METHOD 2
                                        rowDetected[j] = 1;
                                    }
                                }
                            }
                        }
                        break;
                    case EDetectionDirection.Top2Bottom:
                        {
                            for (int i = 1; i < imgROI.Height; i++)
                            {
                                for (int j = 0; j < imgROI.Width; j++)
                                {
                                    if (colDetected[j] == 1) continue;
                                    if (imgROI.Get<byte>(i, j) == 255)
                                    {
                                        ThisResult.DetectedPoints.Add(new OpenCvSharp.Point(j, i)); // METHOD 2
                                        colDetected[j] = 1;
                                    }
                                }
                            }
                        }
                        break;
                    case EDetectionDirection.Bottom2Top:
                        {
                            for (int i = imgROI.Height - 1; i > 0; i--)
                            {
                                for (int j = 0; j < imgROI.Width; j++)
                                {
                                    if (colDetected[j] == 1) continue;
                                    if (imgROI.Get<byte>(i, j) == 255)
                                    {
                                        ThisResult.DetectedPoints.Add(new OpenCvSharp.Point(j, i)); // METHOD 2
                                        colDetected[j] = 1;
                                    }
                                }
                            }
                        }
                        break;
                }

                Log.Debug($"Total {ThisResult.DetectedPoints.Count} point detected");
                if (ThisResult.DetectedPoints.Count < 2)
                {
                    ThisResult.Judge = EVisionJudge.NG;
                    return EVisionRtnCode.FAIL;
                }

                if (ThisParameter.DetectionDirection == EDetectionDirection.Bottom2Top || ThisParameter.DetectionDirection == EDetectionDirection.Top2Bottom)
                {
                    double HeightArg = 0.0;
                    for (int i = 0; i < ThisResult.DetectedPoints.Count; i++)
                    {
                        HeightArg += ThisResult.DetectedPoints[i].Y;
                        if (i == ThisResult.DetectedPoints.Count - 1)
                        {
                            HeightArg = HeightArg / ThisResult.DetectedPoints.Count;
                        }
                    }
                    ThisResult.DetectedPoints.Sort((P1, P2) => Math.Abs(P1.Y - HeightArg).CompareTo(Math.Abs(P2.Y - HeightArg)));
                }
                else
                {
                    double WidthAgr = 0.0;
                    for (int i = 0; i < ThisResult.DetectedPoints.Count; i++)
                    {
                        WidthAgr += ThisResult.DetectedPoints[i].X;
                        if (i == ThisResult.DetectedPoints.Count - 1)
                        {
                            WidthAgr = WidthAgr / ThisResult.DetectedPoints.Count;
                        }
                    }
                    ThisResult.DetectedPoints.Sort((P1, P2) => Math.Abs(P1.X - WidthAgr).CompareTo(Math.Abs(P2.X - WidthAgr)));
                }

                int StartPoint = ThisResult.DetectedPoints.Count - (int)(ThisResult.DetectedPoints.Count * (1- ThisParameter.Threshold));
                int EndPoint = ThisResult.DetectedPoints.Count - 1;
                for (int index = EndPoint; index >= StartPoint; index--)
                {
                    ThisResult.DetectedPoints.RemoveAt(index);
                }

#if SIMULATION
                Mat FinalPointsImg = new Mat(imgROI.Rows, imgROI.Cols, MatType.CV_8UC1, Scalar.Black);
                for (int i = 0; i < ThisResult.DetectedPoints.Count; i++)
                {
                    FinalPointsImg.Set<byte>(ThisResult.DetectedPoints[i].Y, ThisResult.DetectedPoints[i].X, 255);
                }
                Cv2.ImWrite(@"D:\TOP\TOPVEQ\Images\2DetectFinalImg.jpg", FinalPointsImg);
                Cv2.ImWrite(@"D:\TOP\TOPVEQ\Images\0ROI.jpg", imgROI);
#endif

                if (ThisResult.DetectedPoints.Count < 2)
                {
                    Log.Error("Not enough point detected (require atleast 2 points)");
                    ThisResult.Judge = EVisionJudge.NG;
                    return EVisionRtnCode.FAIL;
                }

                // Fitline
                ThisResult.DetectedLine = Cv2.FitLine(ThisResult.DetectedPoints, DistanceTypes.L2, 0, 0.01, 0.01);
                ThisResult.DetectedOffset.Theta = -ThisResult.DetectedLine.GetVectorAngle();

                if (ThisResult.DetectedOffset.Theta >= 0)
                {
                    ThisResult.DetectedOffset.Theta -= ThisParameter.ReferenceAngle;
                }
                else
                {
                    ThisResult.DetectedOffset.Theta += ThisParameter.ReferenceAngle;
                }

                ThisResult.Judge = EVisionJudge.OK;
            }

            return rtnCode;
        }

        internal override void GenerateOutputMat_DetectedMask()
        {
            if (Result.Judge == EVisionJudge.OK)
            {
                if (ThisParameter.DetectionDirection == EDetectionDirection.Bottom2Top || ThisParameter.DetectionDirection == EDetectionDirection.Top2Bottom)
                {
                    var leftP = Math.Round((-ThisResult.DetectedLine.X1 * ThisResult.DetectedLine.Vy / ThisResult.DetectedLine.Vx) + ThisResult.DetectedLine.Y1);
                    var rightP = Math.Round(((fixtureROI.Width - ThisResult.DetectedLine.X1) * ThisResult.DetectedLine.Vy / ThisResult.DetectedLine.Vx) + ThisResult.DetectedLine.Y1);
                    ThisResult.DetectedPointofLine[0] = new OpenCvSharp.Point(fixtureROI.Width - 1, rightP) + new Point(fixtureROI.X, fixtureROI.Y); // Right Point.
                    ThisResult.DetectedPointofLine[1] = new OpenCvSharp.Point(0, leftP) + new Point(fixtureROI.X, fixtureROI.Y); // Left Point.
                }
                else if (ThisParameter.DetectionDirection == EDetectionDirection.Left2Right || ThisParameter.DetectionDirection == EDetectionDirection.Right2Left)
                {
                    var upP = Math.Round((-ThisResult.DetectedLine.Y1 * ThisResult.DetectedLine.Vx / ThisResult.DetectedLine.Vy) + ThisResult.DetectedLine.X1);
                    var downP = Math.Round(((fixtureROI.Height - ThisResult.DetectedLine.Y1) * ThisResult.DetectedLine.Vx / ThisResult.DetectedLine.Vy) + ThisResult.DetectedLine.X1);
                    ThisResult.DetectedPointofLine[0] = new OpenCvSharp.Point(downP, fixtureROI.Height - 1) + new Point(fixtureROI.X, fixtureROI.Y); // Down Point.
                    ThisResult.DetectedPointofLine[1] = new OpenCvSharp.Point(upP, 0) + new Point(fixtureROI.X, fixtureROI.Y); // Up Point.
                }

                Cv2.Line(OutputMat, ThisResult.DetectedPointofLine[0], ThisResult.DetectedPointofLine[1], Colors.Point, 5);
            }
        }
    }
}
