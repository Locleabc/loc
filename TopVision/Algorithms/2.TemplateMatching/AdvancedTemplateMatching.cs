using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Helpers;
using TopVision.Models;

namespace TopVision.Algorithms
{
    public class AdvancedTemplateMatchingParameter : VisionParameterBase
    {
        #region Properties
        public RotatedRect TeachingResultRect
        {
            get { return _TeachingResultRect; }
            set
            {
                if (_TeachingResultRect == value) return;

                _TeachingResultRect = value;
                OnPropertyChanged();
            }
        }

        public List<Point[]> TeachingContours
        {
            get { return _TeachingContours; }
            set
            {
                if (_TeachingContours == value) return;

                _TeachingContours = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// ROI location based contour rectangle
        /// </summary>
        public List<Rect> TeachingContoursRect
        {
            get { return _TeachingContoursRect; }
            set
            {
                if (_TeachingContoursRect == value) return;

                _TeachingContoursRect = value;
                OnPropertyChanged();
            }
        }

        public List<string> TeachingContoursMatEncodeString
        {
            get { return _TeachingContoursMatEncodeString; }
            set
            {
                if (_TeachingContoursMatEncodeString == value) return;

                _TeachingContoursMatEncodeString = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private RotatedRect _TeachingResultRect = new RotatedRect();

        private List<Point[]> _TeachingContours = new List<Point[]>();
        private List<Rect> _TeachingContoursRect = new List<Rect>();
        private List<string> _TeachingContoursMatEncodeString = new List<string>();
        #endregion
    }

    public class AdvancedTemplateMatchingResult : VisionResultBase
    {
        public RotatedRect DetectedRect = new RotatedRect();

        public List<Rect> DetectedContourRect = new List<Rect>();

        public List<string> DetectedContourId = new List<string>();

        public List<Point[]> DetectedContours { get; set; } = new List<Point[]>();

        public override string ToString()
        {
            return $"[{Judge}] {DetectedOffset}, Cost: {Cost:0.###}ms";
        }
    }

    [DisplayName("Advanced Template Matching")]
    public class AdvancedTemplateMatching : VisionProcessBase
    {
        #region Privates 
        private AdvancedTemplateMatchingParameter ThisParameter
        {
            get { return (AdvancedTemplateMatchingParameter)Parameter; }
        }

        private AdvancedTemplateMatchingResult ThisResult
        {
            get { return (AdvancedTemplateMatchingResult)Result; }
        }
        #endregion

        public AdvancedTemplateMatching()
            : this(new AdvancedTemplateMatchingParameter())
        {
        }

        public AdvancedTemplateMatching(AdvancedTemplateMatchingParameter parameter)
        {
            Parameter = parameter;
            Result = new AdvancedTemplateMatchingResult();
        }

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new AdvancedTemplateMatchingResult();

            if (ThisParameter.ROIs.Count != 1)
            {
                ThisResult.Judge = EVisionJudge.NG;
                Log.Error("Exactly 1 ROI must to be set!");
                return EVisionRtnCode.FAIL;
            }

            using (Mat imgROI = PreProcessedMat.SubMat(ThisParameter.ROIs[0].OCvSRect))
            {
                Point[][] contours = new Point[][] { };
                HierarchyIndex[] tmpHierachyIndex = new HierarchyIndex[] { };
                Cv2.FindContours(imgROI, out contours, out tmpHierachyIndex, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

                for (int i = 0; i < contours.Length; i++)
                {
                    ThisResult.DetectedContours.Add(contours[i]);
                }

                if (ThisResult.DetectedContours.Count() <= 0)
                {
                    ThisResult.Judge = EVisionJudge.NG;
                    Log.Debug("No feature detected!");
                    return EVisionRtnCode.FAIL;
                }

                // Flatten DetectedContours into 1D array
                Point[] contour = ThisResult.DetectedContours.SelectMany(item => item).Distinct().ToArray();

                if (isTeachingMode)
                {
                    ThisParameter.TeachingContours = new List<Point[]>();

                    for (int i = 0; i < ThisResult.DetectedContours.Count / 2; i++)
                    {
                        ThisParameter.TeachingContours.Add(ThisResult.DetectedContours[i * 2]);
                        ThisParameter.TeachingContours[i] = ThisParameter.TeachingContours[i].Concat(ThisResult.DetectedContours[i * 2 + 1]).ToArray();
                        //ThisParameter.TeachingContours[i] = (Point[])ThisParameter.TeachingContours[i].ToArray().Concat(ThisParameter.TeachingContours[i].ToArray());
                    }

                    ThisParameter.TeachingContoursRect = new List<Rect>();
                    ThisParameter.TeachingContoursMatEncodeString = new List<string>();
                    Mat contourMat = new Mat();

                    for (int i = 0; i < ThisParameter.TeachingContours.Count; i++)
                    {
                        ThisParameter.TeachingContoursRect.Add(Cv2.BoundingRect(ThisParameter.TeachingContours[i]));
                        List<Point[]> points = new List<Point[]>();
                        
                        contourMat = new Mat(ThisParameter.TeachingContoursRect[i].Height + ThisParameter.TeachingContoursRect[i].Y,
                                             ThisParameter.TeachingContoursRect[i].Width + ThisParameter.TeachingContoursRect[i].X,
                                             MatType.CV_8UC1);

                        Cv2.DrawContours(contourMat, new List<Point[]> { ThisParameter.TeachingContours[i] }, -1, 255, -1);
                        contourMat = contourMat.SubMat(ThisParameter.TeachingContoursRect[i].Y, contourMat.Height,
                                                       ThisParameter.TeachingContoursRect[i].X, contourMat.Width);

                        Cv2.ImWrite($"img_contour_org_{i}.jpg", contourMat);

                        ThisParameter.TeachingContoursMatEncodeString.Add(contourMat.ToBase64String());
                    }
                }
                else
                {
                    #region Prevent parameter error
                    if (ThisParameter.TeachingContours.Count <= 0)
                    {
                        ThisResult.Judge = EVisionJudge.NG;
                        Log.Error("Teaching master sample first");
                        return EVisionRtnCode.FAIL;
                    }

                    if (ThisParameter.TeachingContours.Count != ThisParameter.TeachingContoursRect.Count)
                    {
                        ThisParameter.TeachingContoursRect = new List<Rect>();
                        for (int i = 0; i < ThisParameter.TeachingContours.Count; i++)
                        {
                            ThisParameter.TeachingContoursRect.Add(Cv2.BoundingRect(ThisParameter.TeachingContours[i]));
                        }
                    }
                    #endregion

                    Mat teachingContour = new Mat();
                    Mat roi = new Mat();
                    var TMResultImage = new Mat();
                    double minval = 0, maxval = 0;

                    for (int i = 0; i < ThisParameter.TeachingContours.Count; i++)
                    {
                        teachingContour = ConvertHelpers.FromBase64String(ThisParameter.TeachingContoursMatEncodeString[i]);

                        roi = imgROI.SubMat(new Rect(ThisParameter.TeachingContoursRect[i].X - 50,
                                                     ThisParameter.TeachingContoursRect[i].Y - 50,
                                                     ThisParameter.TeachingContoursRect[i].Width + 100,
                                                     ThisParameter.TeachingContoursRect[i].Height + 100));

                        Cv2.ImWrite($"img_contour_{i}.jpg", teachingContour);
                        Cv2.ImWrite($"img_roi_{i}.jpg", roi);

                        Cv2.MatchTemplate(roi, teachingContour, TMResultImage, TemplateMatchModes.CCoeffNormed);

                        Cv2.ImWrite($"img_match_{i}.jpg", TMResultImage);

                        Point minloc, maxloc;
                        Cv2.MinMaxLoc(TMResultImage, out minval, out maxval, out minloc, out maxloc);

                        //System.Diagnostics.Debug.WriteLine($"MaxVal = {maxval}");
                        if (maxval > ThisParameter.Threshold)
                        {
                            Cv2.Rectangle(PreProcessedMat,
                                          new Rect(maxloc.X + ThisParameter.ROIs[0].X,
                                                   maxloc.Y + ThisParameter.ROIs[0].Y,
                                                   teachingContour.Width,
                                                   teachingContour.Height),
                                          new Scalar(0, 255, 0, 255),
                                          10);

                            ThisResult.DetectedContourRect.Add(new Rect(maxloc.X + ThisParameter.ROIs[0].X + ThisParameter.TeachingContoursRect[i].X - 50,
                                                                        maxloc.Y + ThisParameter.ROIs[0].Y + ThisParameter.TeachingContoursRect[i].Y - 50,
                                                                        ThisParameter.TeachingContoursRect[i].Width,
                                                                        ThisParameter.TeachingContoursRect[i].Height));
                            ThisResult.DetectedContourId.Add(i.ToString());

                            System.Diagnostics.Debug.WriteLine($"[img_{i}] dX = {maxloc.X - 50}, dY = {maxloc.Y - 50}");
                        }
                    }
                }




                foreach (var cont in ThisResult.DetectedContours)
                {
                    for (int i = 0; i < cont.Count(); i++)
                    {
                        cont[i].X += ThisParameter.ROIs[0].X;
                        cont[i].Y += ThisParameter.ROIs[0].Y;
                    }
                }

                ThisResult.DetectedRect = Cv2.MinAreaRect(contour);
                ThisResult.DetectedRect.Center.X += ThisParameter.ROIs[0].X;
                ThisResult.DetectedRect.Center.Y += ThisParameter.ROIs[0].Y;

                ThisResult.DetectedOffset = new XYTOffset
                {
                    X = ThisResult.DetectedRect.Center.X - (PreProcessedMat.Width / 2),
                    Y = ThisResult.DetectedRect.Center.Y - (PreProcessedMat.Height / 2),
                    Theta = ThisResult.DetectedRect.Angle % 90 < 45 ? ThisResult.DetectedRect.Angle % 90 : (ThisResult.DetectedRect.Angle % 90) - 90
                };

                int boxArea = (int)(ThisResult.DetectedRect.Size.Width * ThisResult.DetectedRect.Size.Height);

                if (isTeachingMode)
                {
                    ThisParameter.TeachingResultRect = ThisResult.DetectedRect;
                    ThisResult.Judge = EVisionJudge.OK;
                }
                else
                {
                    
                }
            }

            return EVisionRtnCode.OK;
        }

        internal override void GenerateOutputMat_DetectedMask()
        {
            // Draw all contours
            foreach (Point[] contour in ThisResult.DetectedContours)
            {
                Cv2.DrawContours(OutputMat, new List<Point[]> { contour }, -1, Colors.TextPositive, thickness: Thinknesses.Contour);
            }

            // Draw center mark
            if (Result.Judge == EVisionJudge.OK)
            {
                Cv2.DrawMarker(OutputMat,
                    new Point(ThisResult.DetectedOffset.X + OutputMat.Width / 2,
                              ThisResult.DetectedOffset.Y + OutputMat.Height / 2),
                    Colors.Point,
                    MarkerTypes.Cross,
                    30,
                    Thinknesses.CenterMark);
            }

            for (int i = 0; i < ThisResult.DetectedContourRect.Count; i++)
            {
                Cv2.Rectangle(OutputMat, ThisResult.DetectedContourRect[i], new Scalar(255, 0, 0, 255), thickness: Thinknesses.DetectedRegion);

                Cv2.PutText(
                    OutputMat,
                    $"C#{ThisResult.DetectedContourId[i]}",
                    new Point(ThisResult.DetectedContourRect[i].X, ThisResult.DetectedContourRect[i].Y),
                    HersheyFonts.HersheySimplex,
                    FontSizes.MarkingText,
                    Colors.TextPositive,
                    thickness: Thinknesses.MarkingText
                );
            }

            // Draw detected rotated rectangle
            //Cv2.Polylines(OutputMat, new List<Point[]> { Cv2.BoxPoints(ThisResult.DetectedRect).ToPointArray() }, true, Colors.Box, 5);
        }
    }
}
