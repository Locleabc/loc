using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TopCom.Processing;
using TopLang;
using TopMotion;
using TopUI.Controls;
using PLV_BracketAssemble.Define;
using TopVision.Models;
using TopVision;

namespace PLV_BracketAssemble.Processing
{
    public class CUnderVisionProcess : ProcessingBase
    {
        #region Constructor(s)
        public CUnderVisionProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }
        #endregion

        #region Flag(s)
        public bool Flag_UnderVision_Inspect_Picker1_Done { get; set; }
        public bool Flag_UnderVision_Inspect_Picker2_Done { get; set; }
        #endregion

        public ERunMode RunMode
        {
            get { return _RunMode; }
            set
            {
                _RunMode = value;
                Step.RunStep = 0;
                ModeDetail = value.ToString();
            }
        }

        private EPickers CurrentPicker = EPickers.Picker1;

        #region Data(s)
        public List<IVisionResult> Data_UnderVisionInspect_Result
        {
            get; set;
        } = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
        #endregion

        #region Overrider(s)
        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;
            switch ((EUnderVisionHomeStep)Step.HomeStep)
            {
                case EUnderVisionHomeStep.Start:
                    Step.HomeStep++;
                    break;
                case EUnderVisionHomeStep.End:
                    ProcessingStatus = EProcessingStatus.OriginDone;
                    Step.HomeStep++;
                    break;
                default:
                    Sleep(20);
                    break;
            }
            return nRtn;
        }

        public override PRtnCode ProcessToRun()
        {
            this.RunMode = CDef.RootProcess.RunMode;
            return base.ProcessToRun();
        }

        public override PRtnCode ProcessRun()
        {
            PRtnCode nRtnCode = PRtnCode.RtnOk;

            switch (RunMode)
            {
                case ERunMode.Stop:
                    Sleep(20);
                    break;
                case ERunMode.AutoRun:
                    RunMode = ERunMode.Manual_UnderVision;
                    break;
                case ERunMode.Manual_UnderVision:
                    Running_UnderVision();
                    break;
                case ERunMode.Manual_Change:
                case ERunMode.Manual_Pick:
                case ERunMode.Manual_PreAlign:
                case ERunMode.Manual_Place:
                default:
                    Sleep(20);
                    break;
            }

            return nRtnCode;
        }
        #endregion

        #region Run Functions
        private void Running_UnderVision()
        {
            switch ((EUnderVisionUnderVisionStep)Step.RunStep)
            {
                case EUnderVisionUnderVisionStep.Start:
                    Log.Debug("Under Vision Start");
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.If_UseVision:
                    if (CDef.GlobalRecipe.SkipUnderVision && CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Stop;
                        Data_UnderVisionInspect_Result = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
                        break;
                    }

                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.UnderVisionStart_Flag_Wait:
                    if (Flags.Request_UnderVision_Start)
                    {
                        Flags.Request_UnderVision_Start = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EUnderVisionUnderVisionStep.SetCurrentHead:
                    CurrentPicker = EPickers.Picker1;
                    if (Flags.Picker1_Pick_Done)
                    {
                        Log.Debug($"Under Vision {CurrentPicker} Start");
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EUnderVisionUnderVisionStep.UpdateCurrentHead;
                    }
                    break;
                case EUnderVisionUnderVisionStep.Head_Move_UnderVisionPosition_Wait:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        if (Flags.Picker1_UnderVisionPosition)
                        {
                            Step.RunStep++;
                        }
                        else
                        {
                            if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                            {
                                Sleep(10);
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"Wait Head move Under Vision #01 timeout");
                            }
                        }
                    }
                    else
                    {
                        if (Flags.Picker2_UnderVisionPosition)
                        {
                            Step.RunStep++;
                        }
                        else
                        {
                            if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                            {
                                Sleep(10);
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"Wait Head move Under Vision #02 timeout");
                            }
                        }
                    }
                    break;
                case EUnderVisionUnderVisionStep.UnderVision_ImageGrab:
                    Log.Debug($"UnderVision Grab {CurrentPicker}, Retry={RetryCount}");
                    Grab();
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.UnderVision_ImageGrab_Wait:
                    if (CDef.BotCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"UnderVision Grab {CurrentPicker} Timeout, Retry={RetryCount}");
                        }
                        break;
                    }

                    if (ImageGrabResult() == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        CDef.RootProcess.SetWarning($"UnderVision Image Grab {CurrentPicker} failed, Retry={RetryCount}");
                    }
                    break;
                case EUnderVisionUnderVisionStep.UnderVision_Inspect:
                    Log.Debug($"UnderVision Inspect {CurrentPicker}, Retry={RetryCount}");
                    CDef.MainViewModel.MainContentVM.VisionAutoVM.UnderVisionProcess.TargetID = (int)CurrentPicker + 1;
                    Inspect();
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.UnderVision_Inspect_Wait:
                    if (IsVisionInspectDone())
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"UnderVision Inspect Picker #0{CurrentPicker} timeout");
                        }
                    }
                    break;
                case EUnderVisionUnderVisionStep.UnderVision_RetryCheck:
                    if (VisionResult.Judge == EVisionJudge.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        RetryCount++;
                        if (RetryCount >= CDef.UnderVisionRecipe.RetryCount)
                        {
                            Step.RunStep++;
                        }
                        else
                        {
                            Step.RunStep = (int)EUnderVisionUnderVisionStep.UnderVision_ImageGrab;
                        }
                    }
                    break;
                case EUnderVisionUnderVisionStep.UnderVision_ResultUpdate:
                    Data_UnderVisionInspect_Result[(int)CurrentPicker] = VisionResult;

                    if (CurrentPicker == EPickers.Picker1)
                    {
                        Flag_UnderVision_Inspect_Picker1_Done = true;
                    }
                    else
                    {
                        Flag_UnderVision_Inspect_Picker2_Done = true;
                    }
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.UpdateCurrentHead:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        if (!Flags.Picker2_Pick_Done)
                        {
                            Step.RunStep = (int)EUnderVisionUnderVisionStep.End;
                            break;
                        }
                        CurrentPicker = EPickers.Picker2;
                        Log.Debug($"Under Vision Picker #02 Start");
                        
                        RetryCount = 0;

                        Step.RunStep = (int)EUnderVisionUnderVisionStep.Head_Move_UnderVisionPosition_Wait;
                    }
                    else
                    {
                        Step.RunStep = (int)EUnderVisionUnderVisionStep.End;
                    }
                    break;
                case EUnderVisionUnderVisionStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnderVision;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UnderVision)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
            }
        }
        #endregion

        #region Methods
        private IVisionResult VisionResult
        {
            get
            {
                return CDef.MainViewModel.MainContentVM.VisionAutoVM.UnderVisionProcess.Result;
            }
        }

        private bool IsVisionInspectDone()
        {
            return CDef.MainViewModel.MainContentVM.VisionAutoVM.UnderVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
        }

        private void Inspect()
        {
            CDef.MainViewModel.MainContentVM.VisionAutoVM.InspectCommand.Execute(null);
        }

        private EGrabRtnCode ImageGrabResult()
        {
            return CDef.BotCamera.GrabResult.RtnCode;
        }

        public void Grab()
        {
            CDef.MainViewModel.MainContentVM.VisionAutoVM.GrabCommand_Under.Execute(null);
        }
        #endregion

        #region Privates
        private ERunMode _RunMode;
        private int RetryCount = 0;
        #endregion
    }
}