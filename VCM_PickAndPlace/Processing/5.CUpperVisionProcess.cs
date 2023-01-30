using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Processing;
using TopMotion;
using TopUI.Models;
using TopVision;
using TopVision.Models;
using VCM_PickAndPlace.Define;

namespace VCM_PickAndPlace.Processing
{
    public class CUpperVisionProcess : ProcessingBase
    {
        #region Constructor(s)
        public CUpperVisionProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }
        #endregion

        #region Properties
        #region Motion(s)
        public MotionFas16000 X2Axis
        {
            get { return (MotionFas16000)CDef.AllAxis.X2Axis; }
        }

        public MotionPlusE Z3Axis
        {
            get { return (MotionPlusE)CDef.AllAxis.Z3Axis; }
        }
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
        #endregion

        #region Flag(s)
        public bool Flag_LoadInspect_Done
        {
            get
            {
                if (CDef.GlobalRecipe.SkipLoadVision) return true;

                bool result = true;

                ITrayModel tmpTray = CDef.CurrentLoadingTray;

                try
                {
                    if (tmpTray != null)
                    {
                        int startCell = tmpTray.WorkStartIndex - tmpTray.FirstIndex;
                        startCell /= tmpTray.HeadCount;
                        startCell *= tmpTray.HeadCount;
                        startCell += tmpTray.FirstIndex;

                        for (int i = 0; i < tmpTray.HeadCount; i++)
                        {
                            result &= tmpTray.GetCellStatus(startCell + i) > TopUI.Define.ECellStatus.Processing;
                        }
                    }
                    else
                    {
                        // If current tray is null, that mean all cell work completed => vision done
                        result = true;
                    }
                }
                catch
                {
                    result = true;
                }

                return result;
            }
        }

        public bool Flag_UnloadInspect_Done
        {
            get
            {
                if (CDef.GlobalRecipe.SkipUnloadVision) return true;

                bool result = true;

                ITrayModel tmpTray = CDef.CurrentUnloadingTray;

                try
                {
                    if (tmpTray != null)
                    {
                        int startCell = tmpTray.WorkStartIndex - tmpTray.FirstIndex;
                        startCell /= tmpTray.HeadCount;
                        startCell *= tmpTray.HeadCount;
                        startCell += tmpTray.FirstIndex;

                        for (int i = 0; i < tmpTray.HeadCount; i++)
                        {
                            result &= tmpTray.GetCellStatus(startCell + i) > TopUI.Define.ECellStatus.Processing;
                        }
                    }
                    else
                    {
                        // If current tray is null, that mean all cell work completed => vision done
                        result = true;
                    }
                }
                catch
                {
                    result = true;
                }

                return result;
            }
        }

        public bool Flag_LoadVision_AvoidRequest { get; private set; }

        public bool Flag_UnloadVision_AvoidRequest { get; private set; }
        #endregion

        #region Data(s)
        public List<IVisionResult> Data_LoadingInspect_Result
        {
            get; set;
        } = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };

        public List<IVisionResult> Data_UnloadingInspect_Result
        {
            get; set;
        } = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
        #endregion

        #region Overrider(s)
        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;
            switch ((EUpperVisionHomeStep)Step.HomeStep)
            {
                case EUpperVisionHomeStep.Start:
                    Log.Debug("Home Start");
                    Step.HomeStep++;
                    break;
                case EUpperVisionHomeStep.ZAxis_HomeSearch:
                    Z3Axis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case EUpperVisionHomeStep.ZAxis_HomeSearchWait:
                    if (Z3Axis.Status.IsHomeDone)
                    {
                        Log.Debug($"{Z3Axis} origin done");
                        HomeStatus.Z3Done = true;
                        Z3Axis.ClearPosition();
                        Step.HomeStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisHomeSearch_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{Z3Axis} home search timeout");
                        }
                    }

                    break;
                case EUpperVisionHomeStep.XAxis_HomeSearch:
                    X2Axis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case EUpperVisionHomeStep.XAxis_HomeSearchWait:
                    if (X2Axis.Status.IsHomeDone)
                    {
                        Log.Debug($"{X2Axis} origin done");
                        X2Axis.ClearPosition();
                        Step.HomeStep++;
                    }
                    else
                    {
                        {
                            if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisHomeSearch_TimeOut * 1000)
                            {
                                Sleep(10);
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"{Z3Axis} home search timeout");
                            }
                        }
                    }
                    break;
                case EUpperVisionHomeStep.End:
                    Log.Debug("Home done");
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
            switch ((EUpperVisionToRunStep)Step.ToRunStep)
            {
                case EUpperVisionToRunStep.Start:
                    Log.Debug("To Run Start");

                    Flag_LoadVision_AvoidRequest = false;
                    Flag_UnloadVision_AvoidRequest = false;

                    Step.ToRunStep++;
                    break;
                case EUpperVisionToRunStep.ZAxis_ReadyPosition_Move:
                    ZAxis_ReadyPosition_Move();
                    Step.ToRunStep++;
                    break;
                case EUpperVisionToRunStep.ZAxis_ReadyPosition_MoveWait:
                    if (ZAxis_ReadyPosition_MoveDone())
                    {
                        Log.Debug($"{Z3Axis} move Ready Position Done");
                        Step.ToRunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{Z3Axis} move Ready Position Timeout");
                        }
                    }
                    break;
                case EUpperVisionToRunStep.XAxis_WorkPosition_Move:
                    XAxis_Vision1_Position_Move(EVisionArea.LOAD);
                    Step.ToRunStep++;
                    break;
                case EUpperVisionToRunStep.XAxis_WorkPosition_MoveWait:
                    if (XAxis_Vision1_Position_MoveDone(EVisionArea.LOAD))
                    {
                        Log.Debug($"{X2Axis} move Load Vision Position 1  Done");
                        Step.ToRunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{X2Axis} move Load Vision Position 1 Timeout");
                        }
                    }
                    break;
                case EUpperVisionToRunStep.End:
                    Log.Debug("To Run End");
                    ProcessingStatus = EProcessingStatus.ToRunDone;
                    this.RunMode = CDef.RootProcess.RunMode;
                    Step.ToRunStep++;
                    break;
                default:
                    Sleep(20);
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
                    if (CDef.CurrentLoadingTray != null)
                    {
                        if (CDef.IO.HeadStatus.HeadOccupied)
                        {
                            if (CDef.CurrentUnloadingTray.GetCellStatus(CDef.CurrentUnloadingTray.WorkStartIndex) == TopUI.Define.ECellStatus.NGVision ||
                            CDef.CurrentUnloadingTray.GetCellStatus(CDef.CurrentUnloadingTray.WorkStartIndex + 1) == TopUI.Define.ECellStatus.NGVision)
                            {
                                CDef.CurrentUnloadingTray.SetSetOfCell(TopUI.Define.ECellStatus.Processing, CDef.CurrentUnloadingTray.WorkStartIndex);
                            }
                            RunMode = ERunMode.Manual_UnloadVision;
                        }
                        else
                        {
                            RunMode = ERunMode.Manual_LoadVision;
                        }
                    }
                    else if (CDef.CurrentUnloadingTray != null)
                    {
                        if (CDef.CurrentUnloadingTray.GetCellStatus(CDef.CurrentUnloadingTray.WorkStartIndex) == TopUI.Define.ECellStatus.NGVision ||
                            CDef.CurrentUnloadingTray.GetCellStatus(CDef.CurrentUnloadingTray.WorkStartIndex + 1) == TopUI.Define.ECellStatus.NGVision)
                        {
                            CDef.CurrentUnloadingTray.SetSetOfCell( TopUI.Define.ECellStatus.Processing, CDef.CurrentUnloadingTray.WorkStartIndex);
                        }
                        RunMode = ERunMode.Manual_UnloadVision;
                    }
                    else
                    {
                        RunMode = ERunMode.Manual_Inspect;
                    }
                    break;
#if USPCUTTING
                case ERunMode.Manual_Press:
                    this.RunMode = ERunMode.Stop;
                    break;
#endif
                case ERunMode.Manual_LoadVision:
                    CurrentVisionZone = EVisionArea.LOAD;
                    Running_Vision();
                    break;
                case ERunMode.Manual_Pick:
                    this.RunMode = ERunMode.Stop;
                    break;
                case ERunMode.Manual_UnderVision:
                    this.RunMode = ERunMode.Stop;
                    break;
                case ERunMode.Manual_UnloadVision:
                    CurrentVisionZone = EVisionArea.UNLOAD;
                    Running_Vision();
                    break;
                case ERunMode.Manual_Place:
                    this.RunMode = ERunMode.Stop;
                    break;
                case ERunMode.Manual_Inspect:
                    Running_Inspect();
                    break;
                case ERunMode.Manual_TrayChange:
                    this.RunMode = ERunMode.Stop;
                    break;
                default:
                    Sleep(20);
                    break;
            }

            return nRtnCode;
        }
        #endregion

        private EHeads CurrentHead = EHeads.Head1;
        private EVisionArea CurrentVisionZone;
        private ITrayModel CurrentTray
        {
            get
            {
                if (CurrentVisionZone == EVisionArea.LOAD) return CDef.CurrentLoadingTray;
                else return CDef.CurrentUnloadingTray;
            }
        }

        #region Run Function(s)
        private void Running_Vision()
        {
            if (CDef.CurrentUnloadingTray == null)
            {
                this.RunMode = ERunMode.Manual_Inspect;
                return;
            }

            if (CDef.RootProcess.RunMode == ERunMode.AutoRun && CurrentVisionZone == EVisionArea.LOAD && CDef.GlobalRecipe.SkipLoadVision)
            {
                this.RunMode = ERunMode.Manual_UnloadVision;
                Data_LoadingInspect_Result = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
                return;
            }

            if (CDef.RootProcess.RunMode == ERunMode.AutoRun && CurrentVisionZone == EVisionArea.UNLOAD && CDef.GlobalRecipe.SkipUnloadVision)
            {
                this.RunMode = ERunMode.Manual_LoadVision;
                Data_UnloadingInspect_Result = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
                return;
            }

            switch ((EUpperVisionLoadVisionStep)Step.RunStep)
            {
                case EUpperVisionLoadVisionStep.Start:
                    Log.Debug($"{CurrentVisionZone} Vision Start");
                    CurrentHead = EHeads.Head1;
                    Step.RunStep++;
                    break;
                case EUpperVisionLoadVisionStep.LoadVision_DoneCheck:
                    bool flag_ThisZoneInspectDone = CurrentVisionZone == EVisionArea.LOAD ? Flag_LoadInspect_Done : Flag_UnloadInspect_Done;
                    bool flag_OtherZoneInspectDone = CurrentVisionZone == EVisionArea.LOAD ? Flag_UnloadInspect_Done : Flag_LoadInspect_Done;

                    if (flag_ThisZoneInspectDone)
                    {
                        // Skip this Upper Vision step if process done already.
                        if (CDef.RootProcess.RunMode != ERunMode.AutoRun)
                        {
                            Step.RunStep = (int)EUpperVisionLoadVisionStep.End;
                            break;
                        }

                        if (flag_OtherZoneInspectDone)
                        {
                            // Waitting for pick/place
                            Sleep(10);
                        }
                        else
                        {
                            Step.RunStep = (int)EUpperVisionLoadVisionStep.End;
                        }
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EUpperVisionLoadVisionStep.Set_Flag_Vision_AvoidRequest:
                    if (CurrentVisionZone == EVisionArea.LOAD)
                    {
                        Flag_LoadVision_AvoidRequest = true;
                    }
                    else
                    {
                        Flag_UnloadVision_AvoidRequest = true;
                    }
                    Step.RunStep++;
                    break;
                case EUpperVisionLoadVisionStep.XAxis_Head1_LoadVision_PositionMove:
                    XAxis_Vision1_Position_Move(CurrentVisionZone);
                    ZAxis_VisionPosition_Move(CurrentVisionZone);
                    Step.RunStep++;
                    break;
                case EUpperVisionLoadVisionStep.XAxis_Head1_LoadVision_PositionMoveWait:
                    if (XAxis_Vision1_Position_MoveDone(CurrentVisionZone))
                    {
                        Log.Debug($"{X2Axis} move {CurrentVisionZone} Vision Position 1  Done");
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
                            CDef.RootProcess.SetWarning($"{X2Axis} move {CurrentVisionZone} Vision Position 1 Timeout");
                        }
                    }
                    break;
                case EUpperVisionLoadVisionStep.Transfer_VisionAvoid_PositionWait:
                    bool transferVisionAvoidPositionFlag = CurrentVisionZone == EVisionArea.LOAD
                                                ? Flags.Transfer_LoadVision_Avoid_Position
                                                : Flags.Transfer_UnloadVision_Avoid_Position;
                    if (transferVisionAvoidPositionFlag)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.PickerVisionAvoid_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Transfer moving {CurrentVisionZone} vision avoid timeout");
                        }
                    }
                    break;
                case EUpperVisionLoadVisionStep.Tray_Work_PositionWait:
                    bool trayWorkPositionFlag = CurrentVisionZone == EVisionArea.LOAD
                                                ? Flags.TrayLoad_WorkPosition
                                                : Flags.TrayUnload_WorkPosition;
                    if (trayWorkPositionFlag)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EUpperVisionLoadVisionStep.ZAxis_LoadVision_Position:
                    ZAxis_VisionPosition_Move(CurrentVisionZone);
                    Step.RunStep++;
                    break;
                case EUpperVisionLoadVisionStep.ZAxis_LoadVision_PositionWait:
                    if (ZAxis_VisionPosition_MoveDone(CurrentVisionZone))
                    {
                        Log.Debug($"{Z3Axis} move {CurrentVisionZone} Vision Position Done");
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
                            CDef.RootProcess.SetWarning($"{Z3Axis} move {CurrentVisionZone} Vision Position Timeout");
                        }
                    }
                    break;
                case EUpperVisionLoadVisionStep.GrabImage:
                    Log.Debug($"{CurrentVisionZone} Vision Grab {CurrentHead} start");
                    Grab(CurrentVisionZone);
                    Step.RunStep++;
                    break;
                case EUpperVisionLoadVisionStep.GrabImageResult:
                    if (CDef.TopCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{CurrentVisionZone} Vision Grab {CurrentHead} Timeout");
                        }
                        break;
                    }

                    if (ImageGrabResult($"{CurrentVisionZone}{(int)CurrentHead + 1}") == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        CDef.RootProcess.SetWarning($"{CurrentVisionZone} Vision Grab {CurrentHead} done but failed");
                    }
                    break;
                case EUpperVisionLoadVisionStep.VisionInspect:
                    Log.Debug($"Inspect {CurrentVisionZone} Vision {CurrentHead} Start");
                    if (CurrentVisionZone == EVisionArea.LOAD)
                    {
                        CDef.MainViewModel.VisionAutoVM.LoadVisionProcess.TargetID = CDef.CurrentLoadingTray.WorkStartIndex + 1 - (int)CurrentHead;
                    }
                    else
                    {
                        CDef.MainViewModel.VisionAutoVM.UnloadVisionProcess.TargetID = CDef.CurrentUnloadingTray.WorkStartIndex + 1 - (int)CurrentHead;
                    }
                    AlignRun(CurrentVisionZone);
                    Step.RunStep++;
                    break;
                case EUpperVisionLoadVisionStep.VisionInspectWait:
                    if (CurrentVisionZone == EVisionArea.LOAD && IsVisionInspectDone("Loading") || CurrentVisionZone == EVisionArea.UNLOAD && IsVisionInspectDone("Unloading"))
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
                            CDef.RootProcess.SetWarning($"Inspect {CurrentVisionZone} Vision {CurrentHead} timeout");
                        }
                    }
                    break;
                case EUpperVisionLoadVisionStep.VisionInspectResultApply:
                    IVisionResult visionResult = VisionResult(CurrentVisionZone);
                    Log.Debug($"Inspect {CurrentVisionZone} Vision {CurrentHead} DONE");

                    if (CurrentHead == EHeads.Head2)
                    {
                        if (CurrentVisionZone == EVisionArea.LOAD)
                        {
                            Flag_LoadVision_AvoidRequest = false;
                        }
                        else
                        {
                            Flag_UnloadVision_AvoidRequest = false;
                        }
                    }

                    if (CurrentVisionZone == EVisionArea.LOAD)
                    {
                        Data_LoadingInspect_Result[(int)CurrentHead] = visionResult;
                        Datas.WorkData.TaktTime.LoadVision[(int)CurrentHead].ProcessTime = visionResult.Cost / 1000;

                        if (visionResult.Judge == EVisionJudge.OK)
                        {
                            CurrentTray.SetSingleCell(TopUI.Define.ECellStatus.OKVision, CurrentTray.WorkStartIndex + 1 - (int)CurrentHead);
                        }
                        else
                        {
                            CurrentTray.SetSingleCell(TopUI.Define.ECellStatus.NGVision, CurrentTray.WorkStartIndex + 1 - (int)CurrentHead);
                            Datas.WorkData.CountData.LoadVisionNG[(int)CurrentHead]++;
                        }
                    }
                    else
                    {
                        Data_UnloadingInspect_Result[(int)CurrentHead] = visionResult;
                        Datas.WorkData.TaktTime.UnloadVision[(int)CurrentHead].ProcessTime = visionResult.Cost / 1000;

                        if (visionResult.Judge == EVisionJudge.OK)
                        {
                            CurrentTray.SetSingleCell(TopUI.Define.ECellStatus.OKVision, CurrentTray.WorkStartIndex + 1 - (int)CurrentHead);
                        }
                        else
                        {
                            CurrentTray.SetSingleCell(TopUI.Define.ECellStatus.NGVision, CurrentTray.WorkStartIndex + 1 - (int)CurrentHead);
                            Datas.WorkData.CountData.UnloadVisionNG[(int)CurrentHead]++;

                            CDef.RootProcess.SetWarning($"{CurrentVisionZone} Vision Inspect NG \"{visionResult.NGMessage}\"\nCheck sample and start again.");
                        }
                    }
                    Step.RunStep++;
                    break;
                case EUpperVisionLoadVisionStep.Check_CurrentHead:
                    if (CurrentHead == EHeads.Head1)
                    {
                        CurrentHead = EHeads.Head2;
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EUpperVisionLoadVisionStep.StatusUpdate;
                    }
                    break;
                case EUpperVisionLoadVisionStep.XAxis_Head2_LoadVision_PositionMove:
                    XAxis_Vision2_Position_Move(CurrentVisionZone);
                    Step.RunStep++;
                    break;
                case EUpperVisionLoadVisionStep.XAxis_Head2_LoadVision_PositionMoveWait:
                    if (XAxis_Vision2_Position_MoveDone(CurrentVisionZone))
                    {
                        Log.Debug($"{X2Axis} move {CurrentVisionZone} Vision Position 2  Done");
                        Step.RunStep = (int)EUpperVisionLoadVisionStep.GrabImage;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{X2Axis} move {CurrentVisionZone} Vision Position 2 Timeout");
                        }
                    }
                    break;
                case EUpperVisionLoadVisionStep.StatusUpdate:
                    Step.RunStep++;
                    break;
                case EUpperVisionLoadVisionStep.End:
                    Log.Debug($"Inspect {CurrentVisionZone} END");
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        if (CurrentVisionZone == EVisionArea.LOAD)
                        {
                            this.RunMode = ERunMode.Manual_UnloadVision;
                        }
                        else
                        {
                            this.RunMode = ERunMode.Manual_LoadVision;
                        }
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_LoadVision ||
                             CDef.RootProcess.RunMode == ERunMode.Manual_UnloadVision)
                    {
                        this.RunMode = ERunMode.Stop;
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Running_Inspect()
        {
            switch ((EUpperVisionInspectStep)Step.RunStep)
            {
                case EUpperVisionInspectStep.Start:
                    if (CDef.GlobalRecipe.SkipBallInspect == false)
                    {
                        CDef.LightController.SetLightLevel(3, CDef.UpperVisionRecipe.Inspect_LightLevel);
                        CDef.IO.Output.LightInspect = true;

                        Log.Debug("Inspect start");
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EUpperVisionInspectStep.End;
                    }
                    break;
                case EUpperVisionInspectStep.Vision_InspectPosion_Move:
                    ZAxis_InspectPosition_Move();
                    X2Axis_InspectPosition_Move();
                    Step.RunStep++;
                    break;
                case EUpperVisionInspectStep.Vision_InspectPosion_MoveWait:
                    if (ZAxis_InspectPosition_MoveDone() && X2Axis_InspectPosition_MoveDone())
                    {
                        Log.Debug($"{Z3Axis} move Inspect Position Done");
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
                            CDef.RootProcess.SetWarning($"{Z3Axis} move Inspect Position Timeout");
                        }
                    }
                    break;
                case EUpperVisionInspectStep.TrayHeadTransfer_InspectPosition_MoveWait:
                    if (Flags.Inspect.Tray_InspectPosition && Flags.Inspect.Transfer_InspectPosition)
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
                            if (Flags.Inspect.Tray_InspectPosition == false)
                            {
                                CDef.RootProcess.SetWarning($"Tray inspect position moving timeout");
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"Transfer inspect position moving timeout");
                            }
                        }
                    }
                    break;
                case EUpperVisionInspectStep.GrabImage:
                    Log.Debug($"Grabbing inspect vision");
                    InspectGrab();
                    Step.RunStep++;
                    break;
                case EUpperVisionInspectStep.GrabImageResult:
                    if (CDef.TopCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Inspect Grab Timeout");
                        }
                        break;
                    }

                    if (ImageGrabResult($"Inspect") == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        CDef.RootProcess.SetWarning($"Inspect Image Grab failed");
                    }
                    break;
                case EUpperVisionInspectStep.Inspect:
                    Log.Debug($"Inspect Run");
                    CDef.MainViewModel.VisionAutoVM.InspectVisionProcess.TargetID = Flags.Inspect.InspectIndex % (CDef.UnloadingTrays[0].ColumnCount * CDef.UnloadingTrays[0].RowCount + 1);
                    InspectRun();
                    Step.RunStep++;
                    break;
                case EUpperVisionInspectStep.Inspect_Wait:
                    if (IsVisionInspectDone("Inspect"))
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
                            CDef.RootProcess.SetWarning($"Inspect Run timeout");
                        }
                    }
                    break;
                case EUpperVisionInspectStep.Inspect_ResultUpdate:
                    TopUI.Define.ECellStatus inspectResult = CDef.MainViewModel.VisionAutoVM.InspectVisionProcess.Result.Judge == EVisionJudge.OK ?
                                                             TopUI.Define.ECellStatus.InspOK : TopUI.Define.ECellStatus.InspNG;

                    if (Flags.Inspect.InspectIndex <= CDef.UnloadingTrays[0].ColumnCount * CDef.UnloadingTrays[0].RowCount)
                    {
                        CDef.UnloadingTrays[0].SetSingleCell(inspectResult, Flags.Inspect.InspectIndex);
                    }
                    else
                    {
                        CDef.UnloadingTrays[1].SetSingleCell(inspectResult, Flags.Inspect.InspectIndex - CDef.UnloadingTrays[0].ColumnCount * CDef.UnloadingTrays[0].RowCount);
                    }
                    Step.RunStep++;
                    break;
                case EUpperVisionInspectStep.CheckIfInspectAllDone:
                    if (Flags.Inspect.IsAllCellInspectDone == false
                        /*&& CDef.RootProcess.RunMode == ERunMode.AutoRun*/)
                    {
                        Step.RunStep = (int)EUpperVisionInspectStep.Vision_InspectPosion_Move;
                    }
                    else
                    {
                        Log.Debug($"Upper inspect done");
                        Step.RunStep = (int)EUpperVisionInspectStep.End;
                    }
                    break;
                case EUpperVisionInspectStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_TrayChange;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Inspect)
                    {
                        Log.Debug($"UpperVision inspect done");
                        this.RunMode = ERunMode.Stop;
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Moving Function(s)
        public void ZAxis_ReadyPosition_Move()
        {
            Z3Axis.MoveAbs(CDef.UpperVisionRecipe.Z3Axis_Ready_Position);
        }

        public bool ZAxis_ReadyPosition_MoveDone()
        {
            return Z3Axis.IsOnPosition(CDef.UpperVisionRecipe.Z3Axis_Ready_Position);
        }

        public void ZAxis_VisionPosition_Move(EVisionArea visionZone)
        {
            if (visionZone == EVisionArea.LOAD)
            {
                Z3Axis.MoveAbs(CDef.UpperVisionRecipe.Z3Axis_LoadVision_Position);
            }
            else
            {
                Z3Axis.MoveAbs(CDef.UpperVisionRecipe.Z3Axis_UnLoadVision_Position);
            }
        }

        public bool ZAxis_VisionPosition_MoveDone(EVisionArea visionZone)
        {
            if (visionZone == EVisionArea.LOAD)
            {
                return Z3Axis.IsOnPosition(CDef.UpperVisionRecipe.Z3Axis_LoadVision_Position);
            }
            else
            {
                return Z3Axis.IsOnPosition(CDef.UpperVisionRecipe.Z3Axis_UnLoadVision_Position);
            }
        }

        public void ZAxis_InspectPosition_Move()
        {
            Z3Axis.MoveAbs(CDef.UpperVisionRecipe.Z3Axis_InspectVision_Position);
        }

        public bool ZAxis_InspectPosition_MoveDone()
        {
            return Z3Axis.IsOnPosition(CDef.UpperVisionRecipe.Z3Axis_InspectVision_Position);
        }

        public void X2Axis_InspectPosition_Move()
        {
            double offsetX = Flags.Inspect.InspectColumn * CDef.CommonRecipe.UnloadingTray_X_Pitch;
            X2Axis.MoveAbs(CDef.UpperVisionRecipe.X2Axis_UnLoadVision_Position - offsetX);
        }

        public bool X2Axis_InspectPosition_MoveDone()
        {
            double offsetX = Flags.Inspect.InspectColumn * CDef.CommonRecipe.UnloadingTray_X_Pitch;
            return X2Axis.IsOnPosition(CDef.UpperVisionRecipe.X2Axis_UnLoadVision_Position - offsetX);
        }

        public double X2Axis_LoadVision_Position_Head1
        {
            get
            {
                if (CDef.CurrentLoadingTray == null) return X2Axis.Status.ActualPosition;

                double offsetX = CDef.CurrentLoadingTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;
                offsetX += CDef.CurrentLoadingTray.ColumnCount / CDef.CurrentLoadingTray.HeadCount * CDef.CommonRecipe.LoadingTray_X_Pitch;
                if (CDef.CurrentLoadingTray == CDef.LoadingTrays[0])
                {
                    return CDef.UpperVisionRecipe.X2Axis_LoadVision_Position - offsetX;
                }
                else
                {
                    return CDef.UpperVisionRecipe.X2Axis_LoadVision_Position + CDef.CommonRecipe.LoadingTray_Offset_X - offsetX;
                }
            }
        }

        public double X2Axis_UnloadVision_Position_Head1
        {
            get
            {
                if (CDef.CurrentUnloadingTray == null) return X2Axis.Status.ActualPosition;

                double offsetX = CDef.CurrentUnloadingTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;
                offsetX += CDef.CurrentUnloadingTray.ColumnCount / CDef.CurrentUnloadingTray.HeadCount * CDef.CommonRecipe.UnloadingTray_X_Pitch;
                if (CDef.CurrentUnloadingTray == CDef.UnloadingTrays[0])
                {
                    return CDef.UpperVisionRecipe.X2Axis_UnLoadVision_Position - offsetX;
                }
                else
                {
                    return CDef.UpperVisionRecipe.X2Axis_UnLoadVision_Position + CDef.CommonRecipe.UnloadingTray_Offset_X - offsetX;
                }
            }
        }

        public void XAxis_Vision1_Position_Move(EVisionArea visionZone)
        {
            if (visionZone == EVisionArea.LOAD)
            {
                X2Axis.MoveAbs(X2Axis_LoadVision_Position_Head1);
            }
            else
            {
                X2Axis.MoveAbs(X2Axis_UnloadVision_Position_Head1);
            }
        }


        public bool XAxis_Vision1_Position_MoveDone(EVisionArea visionZone)
        {
            if (visionZone == EVisionArea.LOAD)
            {
                return X2Axis.IsOnPosition(X2Axis_LoadVision_Position_Head1);
            }
            else
            {
                return X2Axis.IsOnPosition(X2Axis_UnloadVision_Position_Head1);
            }
        }

        public double X2Axis_LoadVision_Position_Head2
        {
            get
            {
                if (CDef.CurrentLoadingTray == null) return X2Axis.Status.ActualPosition;

                double offsetX = CDef.CurrentLoadingTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;
                if (CDef.CurrentLoadingTray == CDef.LoadingTrays[0])
                {
                    return CDef.UpperVisionRecipe.X2Axis_LoadVision_Position - offsetX;
                }
                else
                {
                    return CDef.UpperVisionRecipe.X2Axis_LoadVision_Position + CDef.CommonRecipe.LoadingTray_Offset_X - offsetX;
                }
            }
        }

        public double X2Axis_UnloadVision_Position_Head2
        {
            get
            {
                if (CDef.CurrentUnloadingTray == null) return X2Axis.Status.ActualPosition;

                double offsetX = CDef.CurrentUnloadingTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;
                if (CDef.CurrentUnloadingTray == CDef.UnloadingTrays[0])
                {
                    return CDef.UpperVisionRecipe.X2Axis_UnLoadVision_Position - offsetX;
                }
                else
                {
                    return CDef.UpperVisionRecipe.X2Axis_UnLoadVision_Position + CDef.CommonRecipe.UnloadingTray_Offset_X - offsetX;
                }
            }
        }

        public void XAxis_Vision2_Position_Move(EVisionArea visionZone)
        {
            if (visionZone == EVisionArea.LOAD)
            {
                X2Axis.MoveAbs(X2Axis_LoadVision_Position_Head2);
            }
            else
            {
                X2Axis.MoveAbs(X2Axis_UnloadVision_Position_Head2);
            }
        }

        public bool XAxis_Vision2_Position_MoveDone(EVisionArea visionZone)
        {
            if (visionZone == EVisionArea.LOAD)
            {
                return X2Axis.IsOnPosition(X2Axis_LoadVision_Position_Head2);
            }
            else
            {
                return X2Axis.IsOnPosition(X2Axis_UnloadVision_Position_Head2);
            }
        }

        private void Grab(EVisionArea area)
        {
            CDef.LightController.SetLightLevel(1, area == EVisionArea.LOAD ? CDef.UpperVisionRecipe.LoadVision_LightLevel : CDef.UpperVisionRecipe.UnloadVision_LightLevel);
            CDef.IO.Output.LightUpper = true;
            if (area == EVisionArea.LOAD)
            {
                CDef.MainViewModel.VisionAutoVM.GrabCommand_Load.Execute(area);
            }
            else
            {
                CDef.MainViewModel.VisionAutoVM.GrabCommand_Unload.Execute(EVisionArea.UNLOAD);
            }
        }

        private void AlignRun(EVisionArea area)
        {
            CDef.MainViewModel.VisionAutoVM.InspectCommand.Execute(area);
            CDef.IO.Output.LightUpper = false;
        }

        private void InspectGrab()
        {
            CDef.MainViewModel.VisionAutoVM.GrabCommand_Unload.Execute(EVisionArea.INSPECT);
        }

        private void InspectRun()
        {
            CDef.MainViewModel.VisionAutoVM.InspectCommand.Execute(EVisionArea.INSPECT);
        }
        #endregion

        #region Privates
        private IVisionResult VisionResult(EVisionArea area)
        {
            if (area == EVisionArea.LOAD)
            {
                return CDef.MainViewModel.VisionAutoVM.LoadVisionProcess.Result;
            }
            else
            {
                return CDef.MainViewModel.VisionAutoVM.UnloadVisionProcess.Result;
            }
        }

        private EGrabRtnCode ImageGrabResult(string Mode)
        {
            return CDef.TopCamera.GrabResult.RtnCode;
        }

        private bool IsVisionInspectDone(string Mode = null)
        {
            if (Mode.Contains("Loading"))
            {
                return CDef.MainViewModel.VisionAutoVM.LoadVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
            }
            else if ((Mode.Contains("Unloading")))
            {
                return CDef.MainViewModel.VisionAutoVM.UnloadVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
            }
            else
            {
                return CDef.MainViewModel.VisionAutoVM.InspectVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
            }
        }
        #endregion
    }
}