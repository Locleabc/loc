using TopVision.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using TopCom.Command;
using TopVision.Helpers;
using System.Windows.Ink;
using TopVision.Views;
using TopVision.ViewModels;
using System.Collections.ObjectModel;
using System.Web.UI;
using TopCom;

namespace TopVision.Algorithms
{
    public interface IMaskingParameter
    {
        string MaskingMatEncodeString { get; set; }
        string MaskingStrokesEncodeString { get; set; }
    }

    /// <summary>
    /// Vision Parameter for <see cref="SingleTemplateMatching"/>
    /// </summary>
    public class SingleTemplateMatchingParameter : VisionParameterBase, IMaskingParameter
    {
        #region Properties
        public string TemplateImageEncodeString { get; set; }

        public CRectangle TemplateTechingRectangle
        {
            get { return _TemplateTechingRectangle; }
            set
            {
                if (_TemplateTechingRectangle == value) return;

                _TemplateTechingRectangle = value;
                OnPropertyChanged();
            }
        }

        public int Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale == value) return;

                if (value <= 0)
                {
                    _Scale = 1;
                }
                else
                {
                    _Scale = value;
                }

                OnPropertyChanged();
            }
        }

        public bool UseMasking
        {
            get { return _UseMasking; }
            set
            {
                _UseMasking = value;
                OnPropertyChanged();
            }
        }

        public string MaskingMatEncodeString { get; set; }

        public string MaskingStrokesEncodeString
        {
            get { return _MaskingStrokesEncodeString; }
            set
            {
                _MaskingStrokesEncodeString = value;
            }
        }
        #endregion

        #region Commands
        [JsonIgnore]
        public CommonDelegate SetTemplateImageHandler { get; set; }
        [JsonIgnore]
        public CommonDelegate DisplayTemplateImageHandler { get; set; }

        [JsonIgnore]
        public RelayCommand SetTemplateImageCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (SetTemplateImageHandler != null)
                    {
                        SetTemplateImageHandler.Invoke();
                    }
                });
            }
        }

        [JsonIgnore]
        public RelayCommand DisplayTemplateImageCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (DisplayTemplateImageHandler != null)
                    {
                        DisplayTemplateImageHandler.Invoke();
                    }
                });
            }
        }

        [JsonIgnore]
        public RelayCommand CreateMaskingCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    using (Mat templateMat = ConvertHelpers.FromBase64String(TemplateImageEncodeString))
                    {
                        if (templateMat == null) return;

                        TemplateMaskingWindowView maskingView = new TemplateMaskingWindowView();
                        var maskingViewModel = maskingView.DataContext as TemplateMaskingWindowViewModel;

                        maskingViewModel.TemplateMaskingVM.TemplateMat = templateMat;
                        maskingViewModel.TemplateMaskingVM.Strokes = ConvertHelpers.Base64StringToStrokeCollection(MaskingStrokesEncodeString);

                        maskingView.Closing += (s, e) => {
                            if (maskingViewModel.TemplateMaskingVM.MaskMat == null) return;

                            MaskingMatEncodeString = ConvertHelpers.ToBase64String(maskingViewModel.TemplateMaskingVM.MaskMat);

                            MaskingStrokesEncodeString = maskingViewModel.TemplateMaskingVM.Strokes.ToBase64String();
                        };

                        maskingView.ShowDialog();
                    }
                });
            }
        }
        #endregion

        #region Privates
        private int _Scale = 1;
        private string _MaskingStrokesEncodeString;
        private bool _UseMasking = false;
        private CRectangle _TemplateTechingRectangle = new CRectangle(0, 0, 200, 200);
        #endregion
    }

    /// <summary>
    /// Vision Result for <see cref="SingleTemplateMatching"/> process
    /// </summary>
    public class SingleTemplateMatchingResult : VisionResultBase
    {
        private Rect detectedRect = new Rect();
        public Rect DetectedRect
        {
            get { return detectedRect; }
            set
            {
                detectedRect = value;
            }
        }

        private Point detectedCenterPoint = new Point(0, 0);
        public Point DetectedCenterPoint
        {
            get { return detectedCenterPoint; }
            set
            {
                detectedCenterPoint = value;
            }
        }
        internal int ROIIndexContainPattern { get; set; }

        public override string ToString()
        {
            return $"[{Judge}] Cost: {Cost}ms, Score: {Score:0.###}\nOffset: {DetectedOffset}";
        }
    }

    public class SingleTemplateMatching : VisionProcessBase
    {
        #region Privates 
        private SingleTemplateMatchingParameter ThisParameter
        {
            get { return (SingleTemplateMatchingParameter)Parameter; }
        }

        private SingleTemplateMatchingResult ThisResult
        {
            get { return (SingleTemplateMatchingResult)Result; }
        }
        #endregion

        #region Constructors
        public SingleTemplateMatching()
            : this(new SingleTemplateMatchingParameter())
        {
        }

        public SingleTemplateMatching(SingleTemplateMatchingParameter parameter)
        {
            Parameter = parameter;
            Result = new SingleTemplateMatchingResult();

            ThisParameter.SetTemplateImageHandler += SetTemplateImageExecuter;
            ThisParameter.DisplayTemplateImageHandler += DisplayTemplateImageExecuter;
        }

        void SetTemplateImageExecuter()
        {
            if (ThisParameter.ROIs.Count <= 0)
            {
                System.Windows.MessageBox.Show("Set ROI first");
                return;
            }

            if (ThisParameter.IsUseInputImageAsInput)
            {
                if (InputMat.IsNullOrEmpty())
                {
                    System.Windows.MessageBox.Show("Input image is null or empty");
                    return;
                }

                try
                {
                    ThisParameter.TemplateImageEncodeString = InputMat.SubMat(ThisParameter.TemplateTechingRectangle.OCvSRect).ToBase64String();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Set template image failed!\n{ex.Message}");
                }
            }
            else
            {
                if (PreProcessedMat.IsNullOrEmpty())
                {
                    System.Windows.MessageBox.Show("Pre-processed is null or empty");
                    return;
                }
                
                try
                {
                    ThisParameter.TemplateImageEncodeString = PreProcessedMat.SubMat(ThisParameter.TemplateTechingRectangle.OCvSRect).ToBase64String();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Set template image failed!\n{ex.Message}");
                }
            }
        }

        void DisplayTemplateImageExecuter()
        {
            using (Mat imgTemplate = ConvertHelpers.FromBase64String(ThisParameter.TemplateImageEncodeString))
            {
                if (imgTemplate == null)
                {
                    System.Windows.MessageBox.Show($"Template image is null. Set template image first");
                }

                try
                {
                    using (Window window = new Window("Template image", imgTemplate, flags: WindowFlags.KeepRatio))
                    {
                        int width;
                        int height;

                        if (((double)imgTemplate.Width / (double)imgTemplate.Height) > 600.0 / 400.0)
                        {
                            width = 600;
                            height = (int)((double)imgTemplate.Height * width / (double)imgTemplate.Width);
                        }
                        else
                        {
                            height = 400;
                            width = (int)((double)imgTemplate.Width * height / (double)imgTemplate.Height);
                        }

                        window.Resize(width, height);
                        //window.ShowImage(imgTemplate);
                        Cv2.WaitKey(10000);
                    }
                }
                catch { }
            }
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            // Execute SetTemplate in teaching mode
            if (isTeachingMode)
            {
                SetTemplateImageExecuter();
            }

            Result = new SingleTemplateMatchingResult();

            if (ThisParameter.ROIs.Count != 1)
            {
                ThisResult.Judge = EVisionJudge.NG;
                Log.Error("Exactly 1 ROI must to be set!");
                return EVisionRtnCode.FAIL;
            }

            // Load template image frome encode string
            Mat imgTemplate = new Mat();
            Mat imgMasking = new Mat();
            try
            {
                imgTemplate = ConvertHelpers.FromBase64String(ThisParameter.TemplateImageEncodeString);
                imgMasking = ConvertHelpers.FromBase64String(ThisParameter.MaskingMatEncodeString);
            }
            catch (Exception ex)
            {
                Log.Error($"Cannot convert template image. {ex.Message}");
                ThisResult.Judge = EVisionJudge.NG;
                return EVisionRtnCode.FAIL;
            }

            if (imgTemplate == null)
            {
                Log.Error($"Template image is not valid, \"Set template Image\" and try again.");
                ThisResult.Judge = EVisionJudge.NG;
                return EVisionRtnCode.FAIL;
            }

            if (ThisParameter.UseMasking)
            {
                if (imgMasking == null)
                {
                    Log.Error($"Masking is not valid, \"Set Masking\" and try again.");
                ThisResult.Judge = EVisionJudge.NG;
                    return EVisionRtnCode.FAIL;
                }
                if (imgMasking.Size() != imgTemplate.Size())
                {
                    Log.Error($"Masking is not valid, \"Set Masking\" and try again.");
                    ThisResult.NGMessage = "Masking size not match";
                    ThisResult.Judge = EVisionJudge.NG;
                    return EVisionRtnCode.FAIL;
                }
            }

            //Resize ROI & PreProcessImage by Scale
            CRectangle ROI = ThisParameter.ROIs[0];
            using (Mat PreProcessedMatReSize = new Mat())
            {
                if (ThisParameter.IsUseInputImageAsInput)
                {
                    Cv2.Resize(InputMat, PreProcessedMatReSize, new OpenCvSharp.Size(PreProcessedMat.Width / ThisParameter.Scale, PreProcessedMat.Height / ThisParameter.Scale));
                }
                else
                {
                    Cv2.Resize(PreProcessedMat, PreProcessedMatReSize, new OpenCvSharp.Size(PreProcessedMat.Width / ThisParameter.Scale, PreProcessedMat.Height / ThisParameter.Scale));
                }
                ROI = ROI.Resize(ThisParameter.Scale);

                using (Mat imgROI = PreProcessedMatReSize.SubMat(ROI.OCvSRect))
                {
                    // Resize Template Img
                    Cv2.Resize(imgTemplate, imgTemplate, new OpenCvSharp.Size(imgTemplate.Width / ThisParameter.Scale, imgTemplate.Height / ThisParameter.Scale));
                    if (ThisParameter.UseMasking)
                    {
                        Cv2.Resize(imgMasking, imgMasking, new OpenCvSharp.Size(imgMasking.Width / ThisParameter.Scale, imgMasking.Height / ThisParameter.Scale));
                    }
                    else
                    {
                        imgMasking = null;
                    }

                    // Global Min/Max location
                    double minVal, maxVal;
                    double bestVal = 0;
                    Point minLoc, maxLoc;
                    Point bestLoc = new Point();

                    TemplateMatchModes matchModes = TemplateMatchModes.CCoeffNormed;

                    if (imgROI.Size().Height < imgTemplate.Size().Height ||
                        imgROI.Size().Width < imgTemplate.Size().Width)
                    {
                        ThisResult.Judge = EVisionJudge.NG;
                        Log.Error("ROI/Template size error");
                        ThisResult.NGMessage = "ROI/Template size error";
                        return EVisionRtnCode.OK;
                    }

                    using (Mat TMResultImage = new Mat())
                    {
                        if (imgMasking != null)
                        {
                            Cv2.MatchTemplate(imgROI, imgTemplate, TMResultImage, matchModes, imgMasking);
                        }
                        else
                        {
                            Cv2.MatchTemplate(imgROI, imgTemplate, TMResultImage, matchModes);
                        }
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

                    // Getting best result
                    if (bestVal >= ThisParameter.Threshold)
                    {
                        ThisResult.DetectedCenterPoint = new Point(bestLoc.X + ROI.X + imgTemplate.Width / 2, bestLoc.Y + ROI.Y + imgTemplate.Height / 2);

                        int cX = ThisResult.DetectedCenterPoint.X * ThisParameter.Scale - PreProcessedMat.Width / 2;
                        int cY = ThisResult.DetectedCenterPoint.Y * ThisParameter.Scale - PreProcessedMat.Height / 2;

                        ThisResult.DetectedOffset = new XYTOffset { X = cX, Y = cY };

                        ThisResult.DetectedRect = new Rect(
                            (bestLoc.X + ROI.X) * ThisParameter.Scale,
                            (bestLoc.Y + ROI.Y) * ThisParameter.Scale,
                            imgTemplate.Width * ThisParameter.Scale,
                            imgTemplate.Height * ThisParameter.Scale
                        );

                        ThisResult.Judge = EVisionJudge.OK;
                    }
                    else
                    {
                        ThisResult.Judge = EVisionJudge.NG;
                    }

                    ThisResult.Score = bestVal;
                }
            }

            imgTemplate.Dispose();

            return EVisionRtnCode.OK;
        }

        internal override void GenerateOutputMat_DetectedMask()
        {
            if (Result.Judge == EVisionJudge.OK)
            {
                //Cv2.Rectangle(OutputMat, ThisResult.DetectedRect, new Scalar(0, 255, 0, 255), 10);
                Helpers.DrawHelpers.RotationRect(OutputMat, new Point2f(ThisResult.DetectedCenterPoint.X * ThisParameter.Scale, ThisResult.DetectedCenterPoint.Y * ThisParameter.Scale), ThisResult.DetectedRect, ThisResult.DetectedOffset.Theta);
            }
        }
    }
}
