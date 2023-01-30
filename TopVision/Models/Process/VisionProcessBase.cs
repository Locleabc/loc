using log4net;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TopCom;
using TopCom.Define;
using TopVision.Algorithms;
using TopVision.Helpers;

namespace TopVision.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VisionProcessBase : PropertyChangedNotifier, IVisionProcess
    {
        #region Properties
        public int TargetID
        {
            get { return _TargetID; }
            set
            {
                if (_TargetID == value) return;

                _TargetID = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _Desciption; }
            set
            {
                if (_Desciption == value) return;

                _Desciption = value;
                OnPropertyChanged();
            }
        }

        public bool IsMainProcess
        {
            get
            {
                return IsPreProcess == false & IsSiblingProcess == false;
            }
        }

        public bool IsPreProcess
        {
            get { return _IsPreProcess; }
            set
            {
                _IsPreProcess = value;
                OnPropertyChanged();
            }
        }

        public bool IsSiblingProcess
        {
            get { return _IsSiblingProcess; }
            set
            {
                _IsSiblingProcess = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Process's Display Name
        /// </summary>
        public string DisplayName
        {
            get
            {
                DisplayNameAttribute[] DisplayNameAttributes = (DisplayNameAttribute[])Attribute.GetCustomAttributes(this.GetType(), typeof(DisplayNameAttribute));
                if (DisplayNameAttributes.Length == 0)
                {
                    return this.GetType().Name;
                }
                else
                {
                    return DisplayNameAttributes[0].DisplayName;
                }
            }
        }

        [JsonProperty]
        public string ClassFullName
        {
            get { return $"{this.GetType().FullName}, {this.GetType().Assembly.GetName().Name}"; }
        }

        public Mat InputMat
        {
            get { return _InputMat; }
            set
            {
                _InputMat = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty]
        public IVisionParameter Parameter
        {
            get { return _Parameter; }
            set
            {
                _Parameter = value;
                OnPropertyChanged();
            }
        }

        public double PixelSize
        {
            get { return _PixelSize; }
            set
            {
                _PixelSize = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty]
        public ObservableCollection<IVisionProcess> PreProcessors { get; set; } = new ObservableCollection<IVisionProcess>();

        [JsonProperty]
        public ObservableCollection<IVisionProcess> SiblingProcessors { get; set; } = new ObservableCollection<IVisionProcess>();

        public Mat PreProcessedMat { get; internal set; } = new Mat();

        public Mat OutputMat
        {
            get { return _OutputMat; }
            protected set
            {
                if (_OutputMat != value)
                {
                    _OutputMat = value;
                }
            }
        }
        public IVisionResult Result { get; internal set; }

        public EVisionProcessStatus Status { get; protected set; } = EVisionProcessStatus.IDLE;

        public ILog Log { get; protected set; }
        #endregion

        #region Constructor
        public VisionProcessBase()
        {
            Log = LogManager.GetLogger("VISION");
            LogFactory.Configure();

            Result = new VisionResultBase();
            Parameter = new VisionParameterBase();
        }
        #endregion

        #region Member Functions
        public virtual EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            return EVisionRtnCode.OK;
        }

        internal virtual void GenerateOutputMat_ROI()
        {
            double fixtureAlignX = 0;
            double fixtureAlignY = 0;
            if (Parameter.UseFixtureAlign)
            {
                fixtureAlignX = - Parameter.TeachingFixtureOffset.FromMillimeterToPixel(PixelSize / 1000).X + Parameter.RuntimeFixtureOffset.FromMillimeterToPixel(PixelSize / 1000).X;
                fixtureAlignY = - Parameter.TeachingFixtureOffset.FromMillimeterToPixel(PixelSize / 1000).Y + Parameter.RuntimeFixtureOffset.FromMillimeterToPixel(PixelSize / 1000).Y;
            }

            for (int i = 0; i < Parameter.ROIs.Count; i++)
            {
                Cv2.Rectangle(OutputMat
                              , new Rect
                              {
                                  Width = Parameter.ROIs[i].OCvSRect.Width,
                                  Height = Parameter.ROIs[i].OCvSRect.Height,
                                  X = (int)(Parameter.ROIs[i].OCvSRect.X + fixtureAlignX),
                                  Y = (int)(Parameter.ROIs[i].OCvSRect.Y + fixtureAlignY)
                              }
                              , new Scalar(255, 0, 0, 255), thickness: 10);
                Cv2.PutText(
                    OutputMat,
                    $"{i}",
                    new Point(Parameter.ROIs[i].OCvSRect.TopLeft.X + fixtureAlignX, Parameter.ROIs[i].OCvSRect.TopLeft.Y + fixtureAlignY),
                    HersheyFonts.HersheySimplex,
                    FontSizes.MarkingText,
                    Colors.ROI,
                    thickness: Thinknesses.MarkingText
                );
            }
        }

        internal virtual void GenerateOutputMat_ResultString()
        {
            string[] lines = Result.ToString().Replace("\r", "").Split('\n');
            int row = 1;
            foreach (string line in lines)
            {
                Cv2.PutText(
                    OutputMat,
                    line,
                    new Point(100, 150 * (row++)),
                    HersheyFonts.HersheySimplex,
                    FontSizes.ResulText,
                    Result.Judge == EVisionJudge.OK ? Colors.TextPositive : Colors.TextNegative,
                    thickness: Thinknesses.ResulText
                );
            }

            if (Result.Judge == EVisionJudge.NG)
            {
                if (string.IsNullOrEmpty(Result.NGMessage)) return;

                int baseline;

                var size = Cv2.GetTextSize(
                    Result.NGMessage,
                    HersheyFonts.HersheySimplex,
                    FontSizes.ResulText + 2,
                    Thinknesses.ResulText + 2, out baseline);

                Cv2.PutText(
                    OutputMat,
                    Result.NGMessage,
                    new Point((OutputMat.Width - size.Width) / 2, (OutputMat.Height + size.Height) / 2),
                    HersheyFonts.HersheySimplex,
                    FontSizes.ResulText + 2,
                    Result.Judge == EVisionJudge.OK ? Colors.TextPositive : Colors.TextNegative,
                    thickness: Thinknesses.ResulText + 2
                );
            }
        }

        internal virtual void GenerateOutputMat_DetectedMask()
        {
            
        }

        // TODO: Considering change the modify accessor to internal or private
        public EVisionRtnCode GenerateOutputMat()
        {
            OutputMat = new Mat(PreProcessedMat.Size(), MatType.CV_8UC4);

            if (Parameter.OutputMatOption.ShowROI)
            {
                try
                {
                    GenerateOutputMat_ROI();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

            if (Parameter.OutputMatOption.ShowDetectedMask)
            {
                try
                {
                    GenerateOutputMat_DetectedMask();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

            if (Parameter.OutputMatOption.ShowResultString)
            {
                try
                {
                    GenerateOutputMat_ResultString();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

            return EVisionRtnCode.OK;
        }
        
        private EVisionRtnCode CheckRunable()
        {
            if (InputMat.IsNullOrEmpty())
            {
                Log.Error("Input image is null or empty");
                return EVisionRtnCode.FAIL;
            }
            
            if (InputMat.Dims < 2)
            {
                Log.Error("Input image size must bigger than 2*2");
                return EVisionRtnCode.FAIL;
            }

            return EVisionRtnCode.OK;
        }

        public EVisionRtnCode Teaching()
        {
            return Run(isTeachingMode: true);
        }

        Stopwatch watch = new Stopwatch();
        public enum VisionProcessStep
        {
            Start,
            CheckRunable,
            ExecutePreProcessors,
            CheckInputImageOption,
            WaitPreProcessExecuteDone,
            CallDIPExecution,
            CheckPreProcessExecuteStatus,
            ExecuteSiblingProcessors,
            WaitDIPExecution,
            WaitSiblingExecution,

            MainProcessThetaAdjust,

            ProcessDone,

            GenerateOutpuImage,
            MergeOutputImage,

            End,
        }

        public EVisionRtnCode Run(bool isTeachingMode = false)
        {
            int step = 0;

            StartOver:
            switch ((VisionProcessStep)step)
            {
                case VisionProcessStep.Start:
                    watch.Restart();
                    Status = EVisionProcessStatus.IN_PROCESSING;

                    //Result.DetectedOffset = new XYTOffset();

                    if (IsPreProcess | IsSiblingProcess)
                    {
                        // TODO: JUMPO TO SOMEWHERE
                        step = (int)VisionProcessStep.CallDIPExecution;
                    }
                    else
                    {
                        DIPRunDone = false;
                        step++;
                    }
                    goto StartOver;
                case VisionProcessStep.CheckRunable:
                    if (CheckRunable() != EVisionRtnCode.OK)
                    {
                        // Input image wrong format? Data broken? => Should end function, do nothing else.
                        Status = EVisionProcessStatus.PROCESS_DONE;

                        watch.Stop();
                        Result.Cost = watch.ElapsedMilliseconds;
                        ((VisionResultBase)Result).Judge = EVisionJudge.NG;

                        Log.Error($"{DisplayName} Done; result: {Result.ToString().Replace('\n', ' ')}");

                        break;
                    }

                    // Resize ROI
                    int minROISize = 10;
                    foreach (CRectangle ROI in Parameter.ROIs)
                    {
                        if (ROI.X < 0) ROI.X = 0;
                        if (ROI.X > InputMat.Width - 1 - minROISize) ROI.X = InputMat.Width - 1 - minROISize;
                        if (ROI.Y < 0) ROI.Y = 0;
                        if (ROI.Y > InputMat.Height - 1 - minROISize) ROI.Y = InputMat.Height - 1 - minROISize;
                        if (ROI.Width < minROISize) ROI.Width = minROISize;
                        if (ROI.Height < minROISize) ROI.Height = minROISize;

                        if (ROI.Y + ROI.Height > InputMat.Height - 1)
                        {
                            ROI.Height = InputMat.Height - 1 - ROI.Y;
                        }
                        if (ROI.X + ROI.Width > InputMat.Width - 1)
                        {
                            ROI.Width = InputMat.Width - 1 - ROI.X;
                        }
                    }
                    step++;
                    goto StartOver;
                case VisionProcessStep.ExecutePreProcessors:
                    ExcutePreProcessors();
                    step++;
                    goto StartOver;
                case VisionProcessStep.CheckInputImageOption:
                    if (Parameter.IsUseInputImageAsInput == false)
                    {
                        // Use Pre-Processed Image as Input
                        step = (int)VisionProcessStep.WaitPreProcessExecuteDone;
                    }
                    else
                    {
                        // Use Input image as input, no need to wait Pre-Processors execution done
                        step = (int)VisionProcessStep.CallDIPExecution;
                    }
                    goto StartOver;
                case VisionProcessStep.WaitPreProcessExecuteDone:
                    if (PreProcessExecuteTask.ExecuteDone())
                    {
                        Log.Debug($"Preprocessing DONE, cost: {watch.ElapsedMilliseconds}ms");
                        step++;
                    }
                    else
                    {
                        // TODO: Handle timeout exception
                        Thread.Sleep(5);
                    }
                    goto StartOver;
                case VisionProcessStep.CallDIPExecution:
                    OutputMat = new Mat();

                    if (IsPreProcess)
                    {
                        DIPFunction(isTeachingMode);
                        break;
                    }
                    else if (IsSiblingProcess)
                    {
                        try
                        {
                            DIPFunction(isTeachingMode);
                        }
                        catch (Exception ex)
                        {
                            ((VisionResultBase)Result).Judge = EVisionJudge.NG;
                            Result.NGMessage = "Sibling Process Failed";
                            Log.Error($"SiblingProcessException: [{ex.Message}] in [{DisplayName}]");
                        }
                        step = (int)VisionProcessStep.ProcessDone;
                        goto StartOver;
                    }
                    else
                    {
                        DIPFunctionTask = Task.Factory.StartNew(() =>
                        {
                            DIPFunction(isTeachingMode);                      // PreProcessMat -> OutputMat
                        });
                        step++;
                        goto StartOver;
                    }
                case VisionProcessStep.CheckPreProcessExecuteStatus:
                    if (PreProcessExecuteTask.ExecuteDone())
                    {
                        if (Parameter.IsUseInputImageAsInput)
                        {
                            Log.Debug($"Preprocessing DONE, cost: {watch.ElapsedMilliseconds}ms");
                        }
                        step++;
                    }
                    else
                    {
                        // TODO: Handle timeout exception
                        Thread.Sleep(5);
                        goto StartOver;
                    }
                    goto StartOver;
                case VisionProcessStep.ExecuteSiblingProcessors:
                    ExcuteSiblingProcessors(isTeachingMode);
                    step++;
                    goto StartOver;
                case VisionProcessStep.WaitDIPExecution:
                    if (DIPFunctionTask.ExecuteDone())
                    {
                        if (DIPFunctionTask.Status == TaskStatus.Faulted)
                        {
                            if (DIPFunctionTask.Exception.InnerExceptions.Count > 0)
                            {
                                Result.NGMessage = "InnerExceptions";
                                Log.Error($"DIPException: [{DIPFunctionTask.Exception.InnerExceptions[0]}] in [{DisplayName}]");
                            }

                            ((VisionResultBase)Result).Judge = EVisionJudge.NG;
                            step = (int)VisionProcessStep.ProcessDone;
                            goto StartOver;
                        }

                        // TODO: Handle if TaskStatus is Faulted
                        DIPRunDone = true;
                        Result.DetectedOffset.Theta *= Parameter.ThetaAdjust;

                        step++;
                        goto StartOver;
                    }
                    else
                    {
                        // TODO: Handle timeout exception
                        Thread.Sleep(5);
                        goto StartOver;
                    }
                case VisionProcessStep.WaitSiblingExecution:
                    if (SiblingProcessorExecuteTasks.ExecuteDone())
                    {
                        Log.Debug($"Sibling Processors execute DONE.");
                        step++;
                    }
                    else
                    {
                        // TODO: Handle timeout exception
                        // TODO: Display not done process index?
                        Thread.Sleep(5);
                        goto StartOver;
                    }
                    goto StartOver;
                case VisionProcessStep.MainProcessThetaAdjust:
                    foreach (IVisionProcess siblingProcess in SiblingProcessors)
                    {
                        Result.DetectedOffset.Theta += siblingProcess.Result.DetectedOffset.Theta * siblingProcess.Parameter.ThetaAdjust;
                        if (siblingProcess.Result.Judge == EVisionJudge.NG)
                        {
                            ((VisionResultBase)Result).Judge = EVisionJudge.NG;
                        }
                    }
                    step++;
                    goto StartOver;
                case VisionProcessStep.ProcessDone:
                    // TODO: Change pixel to mm
                    Result.DetectedOffset = Result.DetectedOffset.FromPixelToMillimeter(PixelSize / 1000);

                    if (Result.Judge == EVisionJudge.NG)
                    {
                        Result.DetectedOffset = new XYTOffset()
                        {
                            X = 0,
                            Y = 0,
                            Theta = 0
                        };
                    }
                    if (Parameter.UseOffsetLimit)
                    {
                        if (Math.Abs(Result.DetectedOffset.X) >= Parameter.OffsetLimit.X ||
                            Math.Abs(Result.DetectedOffset.Y) >= Parameter.OffsetLimit.Y ||
                            Math.Abs(Result.DetectedOffset.Theta) >= Parameter.OffsetLimit.Theta)
                        {
                            Result.NGMessage = "Over limit offset";
                            Log.Error($"Detected offset limit over:" +
                                $" [X] {Result.DetectedOffset.X:0.###}mm/{Parameter.OffsetLimit.X:0.###}mm" +
                                $" [Y] {Result.DetectedOffset.Y:0.###}mm/{Parameter.OffsetLimit.Y:0.###}" +
                                $" [T] {Result.DetectedOffset.Theta:0.###}°/{Parameter.OffsetLimit.Theta:0.###}°");

                            Result.DetectedOffset = new XYTOffset()
                            {
                                X = 0,
                                Y = 0,
                                Theta = 0
                            };

                            ((VisionResultBase)Result).Judge = EVisionJudge.NG;
                        }
                    }

                    if (isTeachingMode)
                    {
                        Parameter.TeachingFixtureOffset = Result.DetectedOffset;
                    }

                    Status = EVisionProcessStatus.PROCESS_DONE;

                    watch.Stop();
                    Result.Cost = watch.ElapsedMilliseconds;

                    if (Result.Judge == EVisionJudge.OK)
                    {
                        Log.Info($"{DisplayName} Done; result: {Result.ToString().Replace('\n', ' ')}");
                    }
                    else
                    {
                        Log.Error($"{DisplayName} Done; result: {Result.ToString().Replace('\n', ' ')}");
                    }

                    if (IsMainProcess)
                    {
                        step++;
                    }
                    else
                    {
                        step = (int)VisionProcessStep.End;
                    }
                    goto StartOver;
                case VisionProcessStep.GenerateOutpuImage:
                    if (!IsMainProcess)
                    {
                        goto case VisionProcessStep.End;
                    }

                    ExcuteSiblingProcessorsGenerateOutputMat();
                    Task taskGenerateOutput = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            GenerateOutputMat();
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Output Mat Generating Bug Detected: {ex.Message}");
                        }
                    });

                    Task.WaitAll(SiblingProcessorGenerateOutputMatTasks.ToArray());
                    taskGenerateOutput.Wait();
                    step++;
                    goto StartOver;
                case VisionProcessStep.MergeOutputImage:
                    MergeSiblingProcessorsOutputMat();
                    OnOutputMatChanged(EventArgs.Empty);
                    step = (int)VisionProcessStep.End;
                    goto StartOver;
                case VisionProcessStep.End:
                    if (IsMainProcess)
                    {
                        GC.Collect();
                    }
                    step++;
                    break;
                default:
                    break;
            }

            return EVisionRtnCode.OK;
        }
        #endregion

        #region Private Functions
        private EVisionRtnCode PreProcessorsRealExecution()
        {
            if (InputMat.IsNullOrEmpty())
            {
                Log.Warn("InputMat is null / empty or disposed");
                return EVisionRtnCode.FAIL;
            }

            PreProcessedMat = InputMat.Clone();

            for (int i = 0; i < PreProcessors.Count; i++)
            {
                if (IsMainProcess)
                {
                    PreProcessors[i].Parameter.ROIs = new ObservableCollection<CRectangle>();
                    if (Parameter.IsUseInputImageAsInput == false)
                    {
                        foreach (CRectangle roi in Parameter.ROIs)
                        {
                            PreProcessors[i].Parameter.ROIs.Add(
                                new CRectangle
                                {
                                    Height = roi.Height,
                                    Width = roi.Width,
                                    X = roi.X,
                                    Y = roi.Y,
                                });
                        }
                    }
                    foreach (IVisionProcess sibling in SiblingProcessors)
                    {
                        foreach (CRectangle roi in sibling.Parameter.ROIs)
                        {
                            PreProcessors[i].Parameter.ROIs.Add(
                                new CRectangle
                                {
                                    Height = roi.Height,
                                    Width = roi.Width,
                                    X = roi.X,
                                    Y = roi.Y,
                                });
                        }
                    }
                }
                PreProcessors[i].IsPreProcess = true;
                PreProcessors[i].InputMat = PreProcessedMat;

                PreProcessors[i].Run();
                
                PreProcessedMat = PreProcessors[i].OutputMat.Clone();

                PreProcessors[i].InputMat.Dispose();
                PreProcessors[i].OutputMat.Dispose();
                PreProcessors[i].PreProcessedMat.Dispose();
            }

            return EVisionRtnCode.OK;
        }

        public EVisionRtnCode ExcutePreProcessors(bool isManual = false)
        {
            if (isManual)
            {
                watch.Restart();

                EVisionRtnCode rtnCode = PreProcessorsRealExecution();
                Log.Debug($"Preprocessing DONE, cost: {watch.ElapsedMilliseconds}ms");

                return rtnCode;
            }
            else
            {
                PreProcessExecuteTask = Task<EVisionRtnCode>.Factory.StartNew(() => {
                    EVisionRtnCode rtnCode = PreProcessorsRealExecution();

                    return rtnCode;
                });

                // This means nothing
                return EVisionRtnCode.OK;
            }
        }

        private EVisionRtnCode ExcuteSiblingProcessors(bool isTeachingMode = false)
        {
            Log.Debug($"Sibling processors execute START");
            SiblingProcessorExecuteTasks = new List<Task>();
            for (int i = 0; i < SiblingProcessors.Count; i++)
            {
                object arg = i;
                var task = new TaskFactory().StartNew((index) =>
                {
                    if (SiblingProcessors[(int)index].Parameter.IsUseInputImageAsInput)
                    {
                        ((VisionProcessBase)SiblingProcessors[(int)index]).PreProcessedMat = InputMat.Clone();
                    }
                    else
                    {
                        ((VisionProcessBase)SiblingProcessors[(int)index]).PreProcessedMat = PreProcessedMat.Clone();
                    }
                    if (SiblingProcessors[(int)index].Parameter.UseFixtureAlign)
                    {
                        while (DIPRunDone == false) Thread.Sleep(10);

                        SiblingProcessors[(int)index].Parameter.RuntimeFixtureOffset = this.Result.DetectedOffset.FromPixelToMillimeter(PixelSize / 1000);
                        if (isTeachingMode)
                        {
                            SiblingProcessors[(int)index].Parameter.TeachingFixtureOffset = this.Result.DetectedOffset.FromPixelToMillimeter(PixelSize / 1000);
                        }
                    }
                    else
                    {
                        SiblingProcessors[(int)index].Parameter.RuntimeFixtureOffset = new XYTOffset();
                        SiblingProcessors[(int)index].Parameter.TeachingFixtureOffset = new XYTOffset();
                    }
                    ((VisionProcessBase)SiblingProcessors[(int)index]).InputMat = ((VisionProcessBase)SiblingProcessors[(int)index]).PreProcessedMat;
                    SiblingProcessors[(int)index].PixelSize = PixelSize;
                    SiblingProcessors[(int)index].IsSiblingProcess = true;
                    SiblingProcessors[(int)index].Run(isTeachingMode);
                }, arg);
                SiblingProcessorExecuteTasks.Add(task);
            }

            return EVisionRtnCode.OK;
        }

        private EVisionRtnCode ExcuteSiblingProcessorsGenerateOutputMat()
        {
            if (IsPreProcess | IsSiblingProcess) return EVisionRtnCode.OK;

            SiblingProcessorGenerateOutputMatTasks = new List<Task>();
            for (int i = 0; i < SiblingProcessors.Count; i++)
            {
                object arg = i;
                var task = new TaskFactory().StartNew((index) =>
                {
                    SiblingProcessors[(int)index].GenerateOutputMat();
                }, arg);
                SiblingProcessorGenerateOutputMatTasks.Add(task);
            }

            return EVisionRtnCode.OK;
        }

        private EVisionRtnCode MergeSiblingProcessorsOutputMat()
        {
            if (IsPreProcess | IsSiblingProcess) return EVisionRtnCode.OK;

            for (int i = 0; i < SiblingProcessors.Count; i++)
            {
                if (SiblingProcessors[i].OutputMat.Channels() != 4)
                {
                    Cv2.CvtColor(SiblingProcessors[i].OutputMat, SiblingProcessors[i].OutputMat, ColorConversionCodes.GRAY2BGRA);
                }
                OutputMat += SiblingProcessors[i].OutputMat;
            }

            Cv2.Resize(OutputMat, OutputMat, new Size(OutputMat.Width / 5, OutputMat.Height / 5));

            using (Mat FourChannelInputMat = new Mat())
            {
                // Convert InputMat to 4 channel image (same as OutputMat)
                Cv2.CvtColor(InputMat, FourChannelInputMat, ColorConversionCodes.GRAY2BGRA);

                Cv2.Resize(FourChannelInputMat, FourChannelInputMat, new Size(FourChannelInputMat.Width / 5, FourChannelInputMat.Height / 5));

                // Set FourChannelInputMat 0 with A channel of Output mask as Mask
                // Which mean, whereever Current OutputMat has pixel value bigger than 0
                // the InputMat Corresponding pixel will be set to 0 (replace that pixel by OutputMat's pixel)
                FourChannelInputMat.SetTo(new Scalar(0, 0, 0, 0), Cv2.Split(OutputMat)[3]);

                OutputMat += FourChannelInputMat;
            }

            return EVisionRtnCode.OK;
        }
        #endregion

        #region Privates
        private int _TargetID;
        private string _Desciption;
        private bool _IsPreProcess = false;
        private bool _IsSiblingProcess = false;
        private Mat _InputMat;
        private IVisionParameter _Parameter = new VisionParameterBase();
        private Mat _OutputMat = new Mat();

        private bool DIPRunDone = false;

        private double _PixelSize = 1;

        private Task DIPFunctionTask;
        private Task<EVisionRtnCode> PreProcessExecuteTask;
        private List<Task> SiblingProcessorExecuteTasks;
        private List<Task> SiblingProcessorGenerateOutputMatTasks;
        #endregion

        #region Events
        public event EventHandler OutputMatGenerated;

        protected virtual void OnOutputMatChanged(EventArgs e)
        {
            //if OutputMatChanged is not null then call delegate
            OutputMatGenerated?.Invoke(this, e);
        }
        #endregion
    }
}
