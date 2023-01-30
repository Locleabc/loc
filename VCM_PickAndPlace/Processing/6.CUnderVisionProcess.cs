using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Processing;
using TopVision;
using TopVision.Models;
using VCM_PickAndPlace.Define;

namespace VCM_PickAndPlace.Processing
{
    public class CUnderVisionProcess : ProcessingBase
    {
        #region Constructor(s)
        public CUnderVisionProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }
        #endregion

        #region Properties
        #region Motion(s)
        #endregion

        private ERunMode _RunMode;

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

        private EHeads CurrentHead = EHeads.Head2;

        public bool Input_Vacuum
        {
            get
            {
                if (CurrentHead == EHeads.Head1)
                {
                    return CDef.IO.Input.Head1_VAC;
                }
                else
                {
                    return CDef.IO.Input.Head2_VAC;
                }
            }
        }
        #endregion

        #region Flag(s)
        public bool Flag_UnderVision_Inspect_Head1_Done { get; set; }
        public bool Flag_UnderVision_Inspect_Head2_Done { get; set; }

        #endregion

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
            PRtnCode nRtn = PRtnCode.RtnOk;
            switch ((EUnderVisionToRunStep)Step.ToRunStep)
            {
                case EUnderVisionToRunStep.Start:
                    Step.ToRunStep++;
                    break;
                case EUnderVisionToRunStep.End:
                    ProcessingStatus = EProcessingStatus.ToRunDone;
                    this.RunMode = CDef.RootProcess.RunMode;
                    Step.ToRunStep++;
                    break;
                default:
                    break;
            }
            return nRtn;
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
                    // Clear UnderVision Inspect Done after Start
                    Flags.UnderVision_Inspect_Done = false;
                    RunMode = ERunMode.Manual_UnderVision;
                    break;
#if USPCUTTING
                case ERunMode.Manual_Press:
                    Sleep(20);
                    break;
#endif
                case ERunMode.Manual_LoadVision:
                    Sleep(20);
                    break;
                case ERunMode.Manual_Pick:
                    Sleep(20);
                    break;
                case ERunMode.Manual_UnderVision:
                    Running_UnderVision();
                    break;
                case ERunMode.Manual_UnloadVision:
                    Sleep(20);
                    break;
                case ERunMode.Manual_Place:
                    Sleep(20);
                    break;
                case ERunMode.Manual_TrayChange:
                    Sleep(20);
                    break;
                default:
                    break;
            }

            return nRtnCode;
        }
        #endregion

        #region Run Function(s)
#if USPCUTTING
        private void Running_Press()
        {
            switch ((EUnderVisionPressStep)Step.RunStep)
            {
                case EUnderVisionPressStep.Start:
                    Log.Debug("Press Start");
                    Step.RunStep++;
                    break;
                case EUnderVisionPressStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_LoadVision;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Press)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }
#endif

        private void Running_UnderVision()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                CDef.GlobalRecipe.SkipUnderVision)
            {
                this.RunMode = ERunMode.Stop;
                Data_UnderVisionInspect_Result = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
                return;
            }

            switch ((EUnderVisionUnderVisionStep)Step.RunStep)
            {
                case EUnderVisionUnderVisionStep.Start:
                    //Log.Debug("Under Vision Start");
                    // Clear UnderVision_Inspect_Done in manual mode
                    if (CDef.RootProcess.RunMode == ERunMode.Manual_UnderVision)
                    {
                        Flags.UnderVision_Inspect_Done = false;
                    }
                    // Under vision inspect Head 2 first then inspect Head 1
                    CurrentHead = EHeads.Head2;
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.UnderVision_ProcessDoneCheck:
                    if (Flags.UnderVision_Inspect_Done)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EUnderVisionUnderVisionStep.Head_UnderVision_PositionWait:
                    if (Flags.Head_UnderVision_Position)
                    {
                        Log.Debug("Head_UnderVision_Position ready");
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EUnderVisionUnderVisionStep.Transfer_UnderVision_PositionWait:
                    bool transferReady = CurrentHead == EHeads.Head1 ? Flags.Transfer_UnderVision_Head1_Position :
                                                                       Flags.Transfer_UnderVision_Head2_Position;
                    if (transferReady)
                    {
                        Log.Debug($"Transfer_UnderVision_{CurrentHead}_Position ready");
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EUnderVisionUnderVisionStep.UnderVision_Start:
                    Log.Debug($"UnderVision {CurrentHead} Start");
                    RetryCount = 0;
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.GrabImage:
                    Log.Debug($"UnderVision Grab {CurrentHead}, Retry={RetryCount}");
                    Grab();
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.GrabImageResult:
                    if (CDef.BotCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"UnderVision Grab {CurrentHead} Timeout, Retry={RetryCount}");
                        }
                        break;
                    }

                    if (ImageGrabResult() == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        CDef.RootProcess.SetWarning($"UnderVision Image Grab {CurrentHead} failed, Retry={RetryCount}");
                    }
                    break;
                case EUnderVisionUnderVisionStep.VisionInspect:
                    Log.Debug($"UnderVision Inspect {CurrentHead} , Retry={RetryCount}");
                    CDef.MainViewModel.VisionAutoVM.UnderVisionProcess.TargetID = (int)CurrentHead + 1;
                    Inspect();
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.VisionInspectWait:
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
                            CDef.RootProcess.SetWarning($"UnderVision Inspect {CurrentHead} timeout, Retry={RetryCount}");
                        }
                    }
                    break;
                case EUnderVisionUnderVisionStep.UnderVision_EndCheck:
                    FirstThetaOffset = 0;
                    // Skip double under vision check if Theta offset is 0
                    if (CDef.GlobalRecipe.DoubleUnderVisionCheck
                        && (VisionResult().DetectedOffset.Theta != 0 || VisionResult().Judge == EVisionJudge.NG))
                    {
                        FirstThetaOffset = VisionResult().DetectedOffset.Theta;
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EUnderVisionUnderVisionStep.VisionInspectResultApply;
                    }
                    break;
                case EUnderVisionUnderVisionStep.VisionOffset_Move:
                    if (CurrentHead == EHeads.Head1)
                    {
                        CDef.RootProcess.Head1Process.TAxis.MoveAbs(CDef.HeadRecipe.T1Axis_UnderVS_Position -
                                                                    VisionResult().DetectedOffset.Theta * 10.0 / 360.0);
                    }
                    else
                    {
                        CDef.RootProcess.Head2Process.TAxis.MoveAbs(CDef.HeadRecipe.T2Axis_UnderVS_Position -
                                                                    VisionResult().DetectedOffset.Theta * 10.0 / 360.0);
                    }
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.VisionOffset_MoveDone:
                    bool isThetaOnPosition = false;
                    if (CurrentHead == EHeads.Head1)
                    {
                        isThetaOnPosition = CDef.RootProcess.Head1Process.TAxis.IsOnPosition(CDef.HeadRecipe.T1Axis_UnderVS_Position -
                                                                         VisionResult().DetectedOffset.Theta * 10.0 / 360.0);
                    }
                    else
                    {
                        isThetaOnPosition = CDef.RootProcess.Head2Process.TAxis.IsOnPosition(CDef.HeadRecipe.T2Axis_UnderVS_Position -
                                                                         VisionResult().DetectedOffset.Theta * 10.0 / 360.0);
                    }

                    string tAxis = CurrentHead == EHeads.Head1 ? CDef.RootProcess.Head1Process.TAxis.ToString() : CDef.RootProcess.Head2Process.TAxis.ToString();
                    if (isThetaOnPosition)
                    {
                        Log.Debug($"{tAxis} move align+offset position done, Retry={RetryCount}");
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
                            CDef.RootProcess.SetWarning($"{tAxis} move align+offset position timeout, Retry={RetryCount}");
                        }
                    }
                    break;
                case EUnderVisionUnderVisionStep.VisionInspect_Retry:
                    RetryCount++;
                    if (RetryCount >= 2)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EUnderVisionUnderVisionStep.GrabImage;
                    }
                    break;
                case EUnderVisionUnderVisionStep.VisionInspectResultApply:
                    if (VisionResult().Judge == EVisionJudge.NG)
                    {
                        Datas.WorkData.CountData.UnderVisionNG[(int)CurrentHead]++;

                        // Only throw warning in AUTO MODE
                        if (CDef.RootProcess.RunMode == ERunMode.AutoRun && Input_Vacuum)
                        {
                            CDef.RootProcess.SetWarning($"UnderVision Inspection {CurrentHead} NG\nRemove both head product and try again.");
                        }
                    }
                    Data_UnderVisionInspect_Result[(int)CurrentHead] = VisionResult();
                    Data_UnderVisionInspect_Result[(int)CurrentHead].DetectedOffset.Theta += FirstThetaOffset;
                    Datas.WorkData.TaktTime.BotVision[(int)CurrentHead].ProcessTime = VisionResult().Cost / 1000;
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.StatusUpdate:
                    if (CurrentHead == EHeads.Head1)
                    {
                        Flag_UnderVision_Inspect_Head1_Done = true;
                    }
                    else
                    {
                        Flag_UnderVision_Inspect_Head2_Done = true;
                    }
                    Step.RunStep++;
                    break;
                case EUnderVisionUnderVisionStep.Check_CurrentHead:
                    if (CurrentHead == EHeads.Head2)
                    {
                        CurrentHead = EHeads.Head1;
                        Step.RunStep = (int)EUnderVisionUnderVisionStep.Transfer_UnderVision_PositionWait;
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
                default:
                    break;
            }
        }
        #endregion

        #region Moving Function(s)
        public void Grab()
        {
            CDef.LightController.SetLightLevel(2, CDef.UnderVisionRecipe.UnderVision_LightLevel);
            CDef.IO.Output.LightUnder = true;
            CDef.MainViewModel.VisionAutoVM.GrabCommand_Under.Execute(EVisionArea.UNDER);
        }

        public void Inspect()
        {
            CDef.MainViewModel.VisionAutoVM.InspectCommand.Execute(EVisionArea.UNDER);
            CDef.IO.Output.LightUnder = false;
        }
        #endregion

        #region Privates
        private IVisionResult VisionResult()
        {
            return CDef.MainViewModel.VisionAutoVM.UnderVisionProcess.Result;
        }

        private EGrabRtnCode ImageGrabResult()
        {
            return CDef.BotCamera.GrabResult.RtnCode;
        }

        private bool IsVisionInspectDone()
        {
            return CDef.MainViewModel.VisionAutoVM.UnderVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
        }

        private int RetryCount = 0;

        private double FirstThetaOffset = 0;
        #endregion
    }
}