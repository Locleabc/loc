using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Define;
using TopVision.Models;

namespace TopVision.Algorithms
{
    public enum ERotatePatternDetectMethod
    {
        TemplateMatching,
        CircleDetection,
    }

    /// <summary>
    /// Vision Parameter for <see cref="DirectDetection"/> process
    /// </summary>
    public class DirectDetectionParameter : VisionParameterBase
    {
        #region Properties
        /// <summary>
        /// Minimal phase diff (degree), this value must be divisible by 360 (degree)
        /// </summary>
        public double MinimalPhaseDiff
        {
            get { return _MinimalPhaseDiff; }
            set
            {
                if (_MinimalPhaseDiff == value) return;
                _MinimalPhaseDiff = value;
                OnPropertyChanged();
                OnPropertyChanged("NumberOfCase");
            }
        }

        public int NumberOfCase { get { return (int)(360.0 / MinimalPhaseDiff); } }

        /// <summary>
        /// Start at user input single ROI (which Direction is 0 degree)<br/>
        /// Another ROIs will be generated in this setting direction (center image rotate)<br/>
        /// If pattern found in #n ROI, the DetectedOffset.Theta would be -(n * MinimalPhaseDiff)
        /// </summary>
        public ERotateDirect RotateDirection
        {
            get { return _RotateDirection; }
            set
            {
                if (_RotateDirection == value) return;
                _RotateDirection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Default as image center
        /// </summary>
        public CPoint RotatePoint
        {
            get { return _RotatePoint; }
            set
            {
                if (_RotatePoint == value) return;
                _RotatePoint = value;
                OnPropertyChanged();
            }
        }

        public ERotatePatternDetectMethod RotatePatternDetectMethod
        {
            get { return _RotatePatternDetectMethod; }
            set
            {
                if (_RotatePatternDetectMethod == value) return;
                _RotatePatternDetectMethod = value;
                OnPropertyChanged();
            }
        }

        public string TemplateImageFolder
        {
            get { return _TemplateImageFolder; }
            set
            {
                if (_TemplateImageFolder == value) return;
                _TemplateImageFolder = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _MinimalPhaseDiff;
        private ERotateDirect _RotateDirection;
        private CPoint _RotatePoint = new CPoint(Default.ImageWidth / 2, Default.ImageHeight / 2);

        private ERotatePatternDetectMethod _RotatePatternDetectMethod;
        private string _TemplateImageFolder;
        #endregion
    }

    /// <summary>
    /// Vision Result for <see cref="DirectDetection"/> process
    /// </summary>
    public class DirectDetectionResult : VisionResultBase
    {
        public Rect DetectedRect { get; set; }

        public override string ToString()
        {
            return $"[{Judge}], Angle: {DetectedOffset.Theta}, Cost: {Cost}ms, Score: {Score:0.###}";
        }
    }

    [DisplayName("Direct Detection")]
    public class DirectDetection : VisionProcessBase
    {
        #region Privates
        private DirectDetectionParameter ThisParameter
        {
            get { return (DirectDetectionParameter)Parameter; }
        }

        private DirectDetectionResult ThisResult
        {
            get { return (DirectDetectionResult)Result; }
        }
        #endregion

        #region Constructors
        public DirectDetection()
            : this(new DirectDetectionParameter())
        {
        }

        public DirectDetection(DirectDetectionParameter parameter)
        {
            Parameter = parameter;
            Result = new DirectDetectionResult();
        }
        #endregion
        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new DirectDetectionResult();

            EVisionRtnCode rtnCode = EVisionRtnCode.OK;

            if (ThisParameter.ROIs.Count <= 0)
            {
                Log.Error("ROIs need to be set");
                return EVisionRtnCode.FAIL;
            }

            // Keep first ROIs only, another ROIs will be auto generate
            ThisParameter.ROIs = new System.Collections.ObjectModel.ObservableCollection<CRectangle> { ThisParameter.ROIs[0] };

            // Loop start with 1, the #0 ROI is user setting ROI
            for (int i = 1; i < ThisParameter.NumberOfCase; i++)
            {
                Point newPoint = RotatePoint(new Point(ThisParameter.ROIs[0].X + ThisParameter.ROIs[0].Width / 2,
                                                        ThisParameter.ROIs[0].Y + ThisParameter.ROIs[0].Height / 2),
                                             ThisParameter.RotatePoint.OCvSPoint,
                                             ThisParameter.MinimalPhaseDiff * i * (ThisParameter.RotateDirection == ERotateDirect.CW ? 1 : -1));

                newPoint.X -= ThisParameter.ROIs[0].Width / 2;
                newPoint.Y -= ThisParameter.ROIs[0].Height / 2;

                CRectangle newROI = new CRectangle(newPoint, ThisParameter.ROIs[0].OCvSRect.Size);
                ThisParameter.ROIs.Add(newROI);
            }

            SingleTemplateMatching singleTemplateMatching = new SingleTemplateMatching(
                new SingleTemplateMatchingParameter
                {
                    ROIs = ThisParameter.ROIs,
                    //TemplateImageFolder = ThisParameter.TemplateImageFolder,
                    Threshold = ThisParameter.Threshold,
                }
            );
            singleTemplateMatching.PixelSize = this.PixelSize;
            singleTemplateMatching.IsPreProcess = true;

            singleTemplateMatching.InputMat = this.PreProcessedMat;
            singleTemplateMatching.PreProcessedMat = this.PreProcessedMat;
            singleTemplateMatching.Run();

            ThisResult.Judge = singleTemplateMatching.Result.Judge;
            ThisResult.Score = singleTemplateMatching.Result.Score;
            if (ThisResult.Judge == EVisionJudge.OK)
            {
                ThisResult.DetectedOffset.Theta = (singleTemplateMatching.Result as SingleTemplateMatchingResult).ROIIndexContainPattern * ThisParameter.MinimalPhaseDiff * -1;
                ThisResult.DetectedRect = (singleTemplateMatching.Result as SingleTemplateMatchingResult).DetectedRect;
            }

            return rtnCode;
        }

        internal override void GenerateOutputMat_DetectedMask()
        {
            if (Result.Judge == EVisionJudge.OK)
            {
                Cv2.Rectangle(OutputMat, ThisResult.DetectedRect, new Scalar(0, 255, 255, 255), Thinknesses.DetectedRegion);

                Cv2.PutText(
                    OutputMat,
                    $"{ThisResult.Score: 0.0000}",
                    new Point(ThisResult.DetectedRect.TopLeft.X - 20, ThisResult.DetectedRect.TopLeft.Y - 20),
                    HersheyFonts.HersheySimplex,
                    FontSizes.MarkingText,
                    new Scalar(255, 255, 0, 255),
                    thickness: Thinknesses.MarkingText
                );
            }
        }

        private Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X = (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y = (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
}
