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
    /// Vision Parameter for <see cref="MultiTemplateMatching"/> process
    /// </summary>
    public class MultiTemplateMatchingParameter : VisionParameterBase
    {
        #region Properties
        public string TemplateImagePath
        {
            get { return _TemplateImagePath; }
            set
            {
                if (_TemplateImagePath == value) return;
                _TemplateImagePath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Total of template to find
        /// </summary>
        public int TemplateCount
        {
            get { return _TemplateCount; }
            set
            {
                if (_TemplateCount == value) return;
                _TemplateCount = value;
                OnPropertyChanged();
            }
        }

        public int RefTemplateCount
        {
            get { return _RefTemplateCount; }
            set
            {
                if (_RefTemplateCount == value) return;
                _RefTemplateCount = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private string _TemplateImagePath;
        private int _TemplateCount = 4;
        private int _RefTemplateCount = 2;
        #endregion
    }

    /// <summary>
    /// Vision Result for <see cref="MultiTemplateMatching"/> process
    /// </summary>
    public class MultiTemplateMatchingResult : VisionResultBase
    {
        private List<Tuple<Rect, double>> detectedRects = new List<Tuple<Rect, double>>();
        public List<Tuple<Rect, double>> DetectedRects
        {
            get { return detectedRects; }
            set
            {
                detectedRects = value;
            }
        }

        public override string ToString()
        {
            return $"[{Judge}] {DetectedOffset}, Cost: {Cost:0.###}ms";
        }
    }

    [DisplayName("Multiple Template Matching")]
    public class MultiTemplateMatching : VisionProcessBase
    {
        private MultiTemplateMatchingParameter ThisParameter
        {
            get { return (MultiTemplateMatchingParameter)Parameter; }
        }

        private MultiTemplateMatchingResult ThisResult
        {
            get { return (MultiTemplateMatchingResult)Result; }
        }

        public MultiTemplateMatching()
            : this(new MultiTemplateMatchingParameter())
        {
        }

        public MultiTemplateMatching(MultiTemplateMatchingParameter parameter)
        {
            Parameter = parameter;
            Result = new MultiTemplateMatchingResult();
        }

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new MultiTemplateMatchingResult();

            foreach (CRectangle ROI in ThisParameter.ROIs)
            {
                using (Mat imgROI = PreProcessedMat.SubMat(ROI.OCvSRect))
                {
                    for (int patternNumber = 0; patternNumber < ThisParameter.RefTemplateCount; patternNumber++)
                    {
                        // 1. Getting Template Image
                        string patternFileName = ThisParameter.TemplateImagePath.Replace(".jpg", $"{patternNumber}.jpg");
                        if (File.Exists(patternFileName) == false)
                        {
                            //Log.Info($"Pattern file not exist {patternFileName}");
                            continue;
                        }
                        Mat imgTemplate = new Mat();

                        if (imgROI.Channels() == 1)
                        {
                            imgTemplate = new Mat(patternFileName, ImreadModes.Grayscale);
                        }
                        else if (imgROI.Channels() == 3)
                        {
                            imgTemplate = new Mat(patternFileName, ImreadModes.Color);
                        }
                        else
                        {
                            Log.Error("Input image must have 1 or 3 channel(s)");
                            ThisResult.Judge = EVisionJudge.NG;
                            return EVisionRtnCode.FAIL;
                        }

                        double minVal, maxVal;
                        double bestVal = 0;
                        Point minLoc, maxLoc;
                        Point bestLoc = new Point();

                        TemplateMatchModes matchModes = TemplateMatchModes.CCoeffNormed;

                        // 2. Template Matching Calculation
                        // 3. Getting min/max value/position
                        using (Mat TMResultImage = new Mat())
                        {
                            Cv2.MatchTemplate(imgROI, imgTemplate, TMResultImage, matchModes);
                            Cv2.MinMaxLoc(TMResultImage, out minVal, out maxVal, out minLoc, out maxLoc);
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

                        // 4. Getting best result
                        if (bestVal >= ThisParameter.Threshold)
                        {
                            ThisResult.DetectedRects.Add(new Tuple<Rect, double>(new Rect(
                                bestLoc.X + ROI.X,
                                bestLoc.Y + ROI.Y,
                                imgTemplate.Width,
                                imgTemplate.Height
                            ), bestVal));

                            imgTemplate.Dispose();
                            // Go to next ROI to find another template, if one template found on current ROI
                            break;
                        }

                        imgTemplate.Dispose();
                    }
                }
            }

            if (ThisResult.DetectedRects.Count == ThisParameter.TemplateCount)
            {
                ThisResult.Judge = EVisionJudge.OK;

                ThisResult.DetectedOffset = ThisResult.DetectedRects.GetCPoints().GetXYTOffset(new CPoint(InputMat.Width / 2, InputMat.Height / 2));
            }
            else
            {
                ThisResult.Judge = EVisionJudge.NG;
            }

            return EVisionRtnCode.OK;
        }

        internal override void GenerateOutputMat_DetectedMask()
        {
            foreach (Tuple<Rect, double> tuple in ThisResult.DetectedRects)
            {
                Cv2.Rectangle(OutputMat, tuple.Item1,
                    tuple.Item2 >= ThisParameter.Threshold ? new Scalar(0, 255, 0, 255) : new Scalar(0, 0, 255, 255)
                    , Thinknesses.DetectedRegion
                );

                Cv2.PutText(
                    OutputMat,
                    $"{tuple.Item2: 0.0000}",
                    new Point(tuple.Item1.TopLeft.X - 20, tuple.Item1.TopLeft.Y - 20),
                    HersheyFonts.HersheySimplex,
                    FontSizes.MarkingText,
                    tuple.Item2 >= ThisParameter.Threshold ? Colors.TextPositive : Colors.TextNegative,
                    thickness: Thinknesses.MarkingText
                );
            }

            if (Result.Judge == EVisionJudge.OK)
            {
                Cv2.DrawMarker(OutputMat,
                    new Point(
                        ThisResult.DetectedOffset.X + OutputMat.Width / 2,
                        ThisResult.DetectedOffset.Y + OutputMat.Height / 2),
                    Colors.Point, MarkerTypes.Cross, 20, Thinknesses.DetectedRegion);
            }
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
}
