using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Processing;
using TopMotion;
using VCM_PickAndPlace.Define;
using VCM_PickAndPlace.Processing;

namespace VCM_PickAndPlace.Processing
{
    public class CHeadProcess : ProcessingBase
    {
        #region Constructor(s)
        public CHeadProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs, EHeads head)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
            Head = head;
        }
        #endregion

        #region Properties
        #region Motion(s)
        public MotionPlusE YYAxis
        {
            get
            {
                if (Head == EHeads.Head1)
                {
                    return (MotionPlusE)CDef.AllAxis.YY1Axis;
                }
                else
                {
                    return (MotionPlusE)CDef.AllAxis.YY2Axis;
                }
            }
        }

        public MotionPlusE ZAxis
        {
            get
            {
                if (Head == EHeads.Head1)
                {
                    return (MotionPlusE)CDef.AllAxis.Z1Axis;
                }
                else
                {
                    return (MotionPlusE)CDef.AllAxis.Z2Axis;
                }
            }
        }

        public MotionFas16000 ZZAxis
        {
            get
            {
                if (Head == EHeads.Head1)
                {
                    return (MotionFas16000)CDef.AllAxis.ZZ1Enc;
                }
                else
                {
                    return (MotionFas16000)CDef.AllAxis.ZZ2Enc;
                }
            }
        }

        public MotionPlusE TAxis
        {
            get
            {
                if (Head == EHeads.Head1)
                {
                    return (MotionPlusE)CDef.AllAxis.T1Axis;
                }
                else
                {
                    return (MotionPlusE)CDef.AllAxis.T2Axis;
                }
            }
        }
        #endregion

        public EHeads Head { get; private set; }

        public bool HeadPlace_TouchDetected { get; private set; }

        public bool Input_Vacuum
        {
            get
            {
                if (Head == EHeads.Head1)
                {
                    return CDef.IO.Input.Head1_VAC;
                }
                else
                {
                    return CDef.IO.Input.Head2_VAC;
                }
            }
        }

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
        public bool Flag_ZAxisReady
        {
            get
            {
                if (Head == EHeads.Head1)
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z1Axis_Ready_Position);
                }
                else
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z2Axis_Ready_Position);
                }
            }
        }

        public bool Flag_UnderVision_Position
        {
            get
            {
                if (Head == EHeads.Head1)
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z1Axis_UnderVS_Position) &
                           YYAxis.IsOnPosition(CDef.HeadRecipe.YY1Axis_UnderVS_Position) &
                           TAxis.IsOnPosition(CDef.HeadRecipe.T1Axis_UnderVS_Position);
                }
                else
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z2Axis_UnderVS_Position) &
                           YYAxis.IsOnPosition(CDef.HeadRecipe.YY2Axis_UnderVS_Position) &
                           TAxis.IsOnPosition(CDef.HeadRecipe.T2Axis_UnderVS_Position);
                }
            }
        }
        #endregion

        #region Data(s)
        #endregion

        #region Overrider(s)
        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;
            switch ((EHeadHomeStep)Step.HomeStep)
            {
                case EHeadHomeStep.Start:
                    Log.Debug("Home Start");
                    Step.HomeStep++;
                    break;
                case EHeadHomeStep.ZAxis_HomeSearch:
                    ZAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case EHeadHomeStep.ZAxis_HomeSearchWait:
                    if (ZAxis.Status.IsHomeDone)
                    {
                        Log.Debug($"{ZAxis} origin done");
                        if (Head == EHeads.Head1)
                        {
                            HomeStatus.Z1Done = true;
                        }
                        else
                        {
                            HomeStatus.Z2Done = true;
                        }
                        ZAxis.ClearPosition();
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
                            CDef.RootProcess.SetWarning($"{ZAxis} home search timeout");
                        }
                    }
                    break;
                case EHeadHomeStep.YYAxisTAxis_HomeSearch:
                    YYAxis.HomeSearch();
                    TAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case EHeadHomeStep.YYAxisTAxis_HomeSearchWait:
                    if (YYAxis.Status.IsHomeDone && TAxis.Status.IsHomeDone)
                    {
                        Log.Debug($"{YYAxis} && {TAxis} origin done");
                        YYAxis.ClearPosition();
                        TAxis.ClearPosition();
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
                            CDef.RootProcess.SetWarning($"{ZAxis} && {TAxis} home search timeout");
                        }
                    }
                    break;
                case EHeadHomeStep.End:
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
            switch ((EHeadToRunStep)Step.ToRunStep)
            {
                case EHeadToRunStep.Start:
                    Log.Debug("To Run Start");
                    Step.ToRunStep++;
                    break;
                case EHeadToRunStep.ZAxis_ReadyPosition_Move:
                    ZAxis_ReadyPosition_Move();
                    Step.ToRunStep++;
                    break;
                case EHeadToRunStep.ZAxis_ReadyPosition_MoveWait:
                    if (Flag_ZAxisReady)
                    {
                        Log.Debug($"{ZAxis} move Ready Position Done");
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
                            CDef.RootProcess.SetWarning($"{ZAxis} move Ready Position timeout");
                        }
                    }
                    break;
                case EHeadToRunStep.AllZAxis_ReadyWait:
                    if (Flags.AllZAxis_Ready)
                    {
                        Step.ToRunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EHeadToRunStep.YAxisTAxis_ReadyPosition_Move:
                    YAxis_PickPlace_Position_Move(EPickPlace.PICK);
                    TAxis_PickPlace_Position_Move(EPickPlace.PICK);
                    Step.ToRunStep++;
                    break;
                case EHeadToRunStep.YAxisTAxis_ReadyPosition_MoveWait:
                    if (YAxis_PickPlace_Position_MoveDone(EPickPlace.PICK) & TAxis_PickPlace_Position_MoveDone(EPickPlace.PICK))
                    {
                        Log.Debug($"{YYAxis} and {TAxis} move {EPickPlace.PICK} Position done");
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
                            CDef.RootProcess.SetWarning($"{YYAxis} and {TAxis} move {EPickPlace.PICK} Position timeout");
                        }
                    }
                    break;
                case EHeadToRunStep.End:
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
#if USPCUTTING
                    RunMode = ERunMode.Manual_Press;
#else
                    if (CDef.CurrentLoadingTray != null)
                    {
                        if (CDef.IO.HeadStatus.HeadOccupied)
                        {
                            RunMode = ERunMode.Manual_UnderVision;
                        }
                        else
                        {
                            RunMode = ERunMode.Manual_LoadVision;
                        }
                    }
                    else if (CDef.CurrentUnloadingTray != null)
                    {
                        RunMode = ERunMode.Manual_UnloadVision;
                    }
                    else
                    {
                        RunMode = ERunMode.Manual_Inspect;
                    }
#endif
                    break;
#if USPCUTTING
                case ERunMode.Manual_Press:
                    Running_Press();
                    break;
#endif
                case ERunMode.Manual_LoadVision:
                    Running_Vision(EVisionArea.LOAD);
                    break;
                case ERunMode.Manual_Pick:
                    Running_PickPlace(EPickPlace.PICK);
                    break;
                case ERunMode.Manual_UnderVision:
                    Running_UnderVision();
                    break;
                case ERunMode.Manual_UnloadVision:
                    Running_Vision(EVisionArea.UNLOAD);
                    break;
                case ERunMode.Manual_Place:
                    Running_PickPlace(EPickPlace.PLACE);
                    break;
                case ERunMode.Manual_Inspect:
                    Running_Inspect();
                    break;
                case ERunMode.Manual_TrayChange:
                    Running_TrayChange();
                    break;
                default:
                    Sleep(20);
                    break;
            }

            return nRtnCode;
        }

        #endregion

        #region Run Function(s)
#if USPCUTTING
        private void Running_Press()
        {
            switch ((EHeadPressStep)Step.RunStep)
            {
                case EHeadPressStep.Start:
                    Log.Debug("Press Start");
                    Step.RunStep++;
                    break;
                case EHeadPressStep.ZAxis_ReadyPosition_Check:
                    if (Flag_ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EHeadPressStep.ZAxis_ReadyPosition_Move:
                    ZAxis_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case EHeadPressStep.ZAxis_ReadyPosition_MoveWait:
                    if (Flag_ZAxisReady)
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
                            Log.Warn($"{ZAxis} move Ready Position timeout");
                        }
                    }
                    break;
                case EHeadPressStep.End:
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

        private void Running_Vision(EVisionArea visionZone)
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun && visionZone == EVisionArea.LOAD && CDef.GlobalRecipe.SkipLoadVision)
            {
                this.RunMode = ERunMode.Manual_Pick;
                return;
            }

            if (CDef.RootProcess.RunMode == ERunMode.AutoRun && visionZone == EVisionArea.UNLOAD && CDef.GlobalRecipe.SkipUnloadVision)
            {
                this.RunMode = ERunMode.Manual_Place;
                return;
            }

            switch ((EHeadLoadVisionStep)Step.RunStep)
            {
                case EHeadLoadVisionStep.Start:
                    Log.Debug($"{visionZone} Vision Start");
                    Step.RunStep++;
                    break;
                case EHeadLoadVisionStep.LoadVision_DoneCheck:
                    bool flag_InspectDone = visionZone == EVisionArea.LOAD ? Flags.LoadInspect_Done : Flags.UnloadInspect_Done;

                    if (flag_InspectDone)
                    {
                        Step.RunStep = (int)EHeadLoadVisionStep.LoadVision_DoneWait;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EHeadLoadVisionStep.ZAxis_ReadyPosition_Check:
                    if (Flag_ZAxisReady)
                    {
                        Step.RunStep = (int)EHeadLoadVisionStep.LoadVision_DoneWait;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EHeadLoadVisionStep.ZAxis_ReadyPosition_Move:
                    ZAxis_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case EHeadLoadVisionStep.ZAxis_ReadyPosition_MoveWait:
                    if (Flag_ZAxisReady)
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
                            CDef.RootProcess.SetWarning($"{ZAxis} move Ready Position timeout");
                        }
                    }
                    break;
                case EHeadLoadVisionStep.LoadVision_DoneWait:
                    bool flag_InspectDone_1 = visionZone == EVisionArea.LOAD ? Flags.LoadInspect_Done : Flags.UnloadInspect_Done;

                    if (flag_InspectDone_1)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EHeadLoadVisionStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        if (visionZone == EVisionArea.LOAD)
                        {
                            this.RunMode = ERunMode.Manual_Pick;
                        }
                        else
                        {
                            this.RunMode = ERunMode.Manual_Place;
                        }
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_LoadVision)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Running_PickPlace(EPickPlace mode)
        {
            switch ((EHeadPickStep)Step.RunStep)
            {
                case EHeadPickStep.Start:
                    Log.Debug($"{mode} Start");
                    Step.RunStep++;
                    break;
                case EHeadPickStep.ZAxis_ReadyPosition_Move:
                    ZAxis_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case EHeadPickStep.ZAxis_ReadyPosition_MoveWait:
                    if (Flag_ZAxisReady)
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
                            CDef.RootProcess.SetWarning($"{ZAxis} move Ready Position timeout");
                        }
                    }
                    break;
                case EHeadPickStep.PickPlace_DoneCheck:
                    bool thisZonePickPlaceDone = mode == EPickPlace.PICK
                                          ? HeadWorkStatus.PickDone
                                          : HeadWorkStatus.PlaceDone;

                    if (thisZonePickPlaceDone)
                    {
                        Step.RunStep = (int)EHeadPickStep.End;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EHeadPickStep.YAxisTAxis_Pick_PositionMove:
                    YAxis_PickPlace_Position_Move(mode);
                    TAxis_PickPlace_Position_Move(mode);
                    Step.RunStep++;
                    break;
                case EHeadPickStep.YAxisTAxis_Pick_PositionMoveWait:
                    if (YAxis_PickPlace_Position_MoveDone(mode) & TAxis_PickPlace_Position_MoveDone(mode))
                    {
                        Log.Debug($"{YYAxis} and {TAxis} move {mode} Position done");
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
                            CDef.RootProcess.SetWarning($"{YYAxis} and {TAxis} move {mode} Position timeout");
                        }
                    }
                    break;
                case EHeadPickStep.Transfer_Pick_PositionWait:
                    bool flag_Transfer_PickPlace_Position = mode == EPickPlace.PICK
                                                            ? Flags.Transfer_Pick_Position
                                                            : Flags.Transfer_Place_Position;
                    if (flag_Transfer_PickPlace_Position)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EHeadPickStep.Tray_Work_PositionMoveWait:
                    bool flag_Tray_PickPlace_Position = mode == EPickPlace.PICK
                                                        ? Flags.TrayLoad_WorkPosition
                                                        : Flags.TrayUnload_WorkPosition;

                    if (flag_Tray_PickPlace_Position)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EHeadPickStep.ZAxis_PickPosition1_Move:
                    // Clear ZZ Encode value, before checking place touch
                    if (CDef.GlobalRecipe.UsePlaceTouchCheck)
                    {
                        ZZAxis.ClearPosition();
                        HeadPlace_TouchDetected = false;
                    }

                    ZAxis_PickPlace_Position1_Move(mode);
                    Step.RunStep++;
                    break;
                case EHeadPickStep.ZAxis_PickPosition1_MoveWait:
                    if (ZAxis_PickPlace_Position1_MoveDone(mode))
                    {
                        Log.Debug($"{ZAxis} move {mode} Position 1 done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (CDef.GlobalRecipe.UsePlaceTouchCheck && mode == EPickPlace.PLACE)
                        {
                            // ZZ Encoder position is minus value
                            if (Math.Abs(ZZAxis.Status.ActualPosition) >= CDef.GlobalRecipe.PlaceTouchCheckLimit)
                            {
                                if (ZAxis.Status.ActualPosition + CDef.GlobalRecipe.PlaceTouchAllowGap < CDef.HeadRecipe.Z2Axis_Place_Position)
                                {
                                    ZAxis.EMGStop();
                                    HeadPlace_TouchDetected = true;
                                    Log.Error($"Touching Detected (Depth = {ZZAxis.Status.ActualPosition}mm) while {ZAxis} moving {mode} Position 2;" +
                                        $"{ZAxis}: {ZAxis.Status.ActualPosition}mm");
                                    Step.RunStep = (int)EHeadPickStep.Head_VAC_On;
                                }
                            }
                        }

                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{ZAxis} move {mode} Position 1 timeout");
                        }
                    }
                    break;
                case EHeadPickStep.ZAxis_PickPosition2_Move:
                    ZAxis_PickPlace_Position2_Move(mode);
                    if (mode == EPickPlace.PLACE)
                    {
                        YYAxis.MoveInc(CDef.HeadRecipe.YYAxis_Place_OverrideDistance);
                    }
                    Step.RunStep++;
                    break;
                case EHeadPickStep.ZAxis_PickPosition2_MoveWait:
                    if (ZAxis_PickPlace_Position2_MoveDone(mode))
                    {
                        Log.Debug($"{ZAxis} move {mode} Position 2 done");
                        Datas.PlaceResult[(int)Head] = true;
                        Step.RunStep++;
                    }
                    else
                    {
                        if (CDef.GlobalRecipe.UsePlaceTouchCheck && mode == EPickPlace.PLACE)
                        {
                            // ZZ Encoder position is minus value
                            if (Math.Abs(ZZAxis.Status.ActualPosition) >= CDef.GlobalRecipe.PlaceTouchCheckLimit)
                            {
                                if (ZAxis.Status.ActualPosition + CDef.GlobalRecipe.PlaceTouchAllowGap < CDef.HeadRecipe.Z2Axis_Place_Position)
                                {
                                    ZAxis.EMGStop();
                                    HeadPlace_TouchDetected = true;
                                    Log.Error($"Touching Detected (Depth = {ZZAxis.Status.ActualPosition}mm) while {ZAxis} moving {mode} Position 2;" +
                                        $"{ZAxis}: {ZAxis.Status.ActualPosition}mm");
                                    Datas.PlaceResult[(int)Head] = false;
                                    Step.RunStep++;
                                }
                            }
                        }

                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{ZAxis} move {mode} Position 2 timeout");
                        }
                    }
                    break;
                case EHeadPickStep.Head_VAC_On:
                    if (mode == EPickPlace.PICK)
                    {
                        VacuumOn();
                    }
                    else
                    {
                        PurgeOn();
                    }
                    Step.RunStep++;
                    break;
                case EHeadPickStep.Head_VAC_Delay:
                    double delayTime = mode == EPickPlace.PICK
                                        ? CDef.CommonRecipe.VAC_Delay
                                        : CDef.CommonRecipe.Purge_Delay;

                    // TODO: Consider to use this option
                    //if (mode == EPickPlace.PICK)
                    //{
                    //    // If VACUUM check OK => Jump to next step instead of waiting whole delay time
                    //    if (Input_Vacuum)
                    //    {
                    //        Log.Debug($"{Head} vacuum/purge delay for {PTimer.StepLeadTime / 1000}s");
                    //        Step.RunStep++;
                    //        break;
                    //    }
                    //}

                    if (PTimer.StepLeadTime < delayTime * 1000)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        Log.Debug($"Head vacuum/purge delay for {PTimer.StepLeadTime / 1000}s");
                        Step.RunStep++;
                    }
                    break;
                case EHeadPickStep.ZAxis_UpPosition_Move:
                    if (mode == EPickPlace.PLACE)
                    {
                        PurgeOff();
                    }

                    ZAxis_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case EHeadPickStep.ZAxis_UpPosition_MoveWait:
                    if (Flag_ZAxisReady)
                    {
                        Log.Debug($"{ZAxis} move Ready Position done");
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
                            CDef.RootProcess.SetWarning($"{ZAxis} move Ready Position timeout");
                        }
                    }
                    break;
                case EHeadPickStep.StatusUpdate:
                    if (mode == EPickPlace.PICK)
                    {
                        if (Input_Vacuum || CDef.GlobalRecipe.UseVacuumCheck == false)
                        {
                            CDef.CurrentLoadingTray.SetSingleCell(TopUI.Define.ECellStatus.OK, CDef.CurrentLoadingTray.WorkStartIndex + (1 - (int)Head));
                            Log.Info($"{Head} PICK index #{CDef.CurrentLoadingTray.WorkStartIndex + (1 - (int)Head)} DONE!");
                            Datas.PickResult[(int)Head] = true;
                        }
                        else
                        {
                            Datas.WorkData.CountData.PickNG++;
                            CDef.CurrentLoadingTray.SetSingleCell(TopUI.Define.ECellStatus.NGPickOrPlace, CDef.CurrentLoadingTray.WorkStartIndex + (1 - (int)Head));
                            Log.Info($"{Head} PICK index #{CDef.CurrentLoadingTray.WorkStartIndex + (1 - (int)Head)} Fail!");
                            Datas.PickResult[(int)Head] = false;
                        }

                        Datas.WorkData.TaktTime.Pick = 1.0 * (PTimer.Now - CDef.RootProcess.TransferProcess.PTimer.TactTimeCounter[1]) / 1000;
                        Flags.UnderVision_Inspect_Done = false;
                    }
                    else
                    {
                        // If under vision NG, or HeadPlace_TouchDetected => Place fail
                        if (Datas.Data_UnderInspect_Result[(int)Head].Judge != TopVision.EVisionJudge.OK ||
                            HeadPlace_TouchDetected == true)
                        {
                            CDef.CurrentUnloadingTray.SetSingleCell(TopUI.Define.ECellStatus.NGPickOrPlace, CDef.CurrentUnloadingTray.WorkStartIndex + (1 - (int)Head));
                            Datas.WorkData.CountData.PlaceNG++;
                            Log.Debug($"{Head} PLACE index #{CDef.CurrentUnloadingTray.WorkStartIndex + (1 - (int)Head)} FAIL!");
                        }
                        else
                        {
                            CDef.CurrentUnloadingTray.SetSingleCell(TopUI.Define.ECellStatus.OK, CDef.CurrentUnloadingTray.WorkStartIndex + (1 - (int)Head));
                            Datas.WorkData.CountData.OK += 1;
                            Log.Debug($"{Head} PLACE index #{CDef.CurrentUnloadingTray.WorkStartIndex + (1 - (int)Head)} DONE!");
                        }
                        Datas.WorkData.TaktTime.Place = 1.0 * (PTimer.Now - CDef.RootProcess.TransferProcess.PTimer.TactTimeCounter[2]) / 1000;
                        Datas.WorkData.TaktTime.Total = 1.0 * (PTimer.Now - CDef.RootProcess.TransferProcess.PTimer.TactTimeCounter[0]) / 1000;
#if USPCUTTING
                        Datas.WorkData.TaktTime.Total = 1.0 * (PTimer.Now - CDef.RootProcess.TransferProcess.PTimer.TactTimeCounter[0]) / 1000
                                                        + Datas.WorkData.TaktTime.Press / 30.0;
#endif
                        Datas.WorkData.CountData.Total += 1;

                        CDef.MES.SavePD((int)Head);
                        CDef.MES.SaveTT((int)Head);
                    }
                    Step.RunStep++;
                    break;
                case EHeadPickStep.Wait_TrayAndTransfer_ReachToEnd:
                    if (Head == EHeads.Head1)
                    {
                        Step.RunStep++;
                        break;
                    }

                    if (mode == EPickPlace.PICK)
                    {
                        if (Flags.Transfer_PickDone && Flags.TrayLoad_PickDone)
                        {
                            Flags.Transfer_PickDone = false;
                            Flags.TrayLoad_PickDone = false;
                            Step.RunStep++;
                        }
                        else
                        {
                            Sleep(10);
                        }
                    }
                    else
                    {
                        if (Flags.Transfer_PlaceDone && Flags.TrayUnload_PlaceDone)
                        {
                            Flags.Transfer_PlaceDone = false;
                            Flags.TrayUnload_PlaceDone = false;
                            Step.RunStep++;
                        }
                        else
                        {
                            Sleep(10);
                        }
                    }
                    break;
                case EHeadPickStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        if (mode == EPickPlace.PICK)
                        {
                            if (Head == EHeads.Head2 && CDef.CurrentLoadingTray != null)
                            {
                                CDef.CurrentLoadingTray.WorkStartIndex += 2;
                            }
                            this.RunMode = ERunMode.Manual_UnderVision;
                        }
                        else
                        {
                            if (Head == EHeads.Head2 && CDef.CurrentUnloadingTray != null)
                            {
                                CDef.CurrentUnloadingTray.WorkStartIndex += 2;
                            }
                            if (CDef.CurrentUnloadingTray != null)
                            {
                                this.RunMode = ERunMode.Manual_LoadVision;
                            }
                            else
                            {
                                this.RunMode = ERunMode.Manual_Inspect;
                            }
                        }
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Pick || CDef.RootProcess.RunMode == ERunMode.Manual_Place)
                    {
                        Sleep(500);
                        this.RunMode = ERunMode.Stop;
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    }
                    break;
                default:
                    Sleep(20);
                    break;
            }
        }

        private void Running_UnderVision()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
               CDef.GlobalRecipe.SkipUnderVision)
            {
                if (Flags.UnloadInspect_Done)
                {
                    this.RunMode = ERunMode.Manual_Place;
                }
                else
                {
                    this.RunMode = ERunMode.Manual_UnloadVision;
                }
                return;
            }

            switch ((EHeadUnderVisionStep)Step.RunStep)
            {
                case EHeadUnderVisionStep.Start:
                    Log.Debug("Under Vision Start");
                    Step.RunStep++;
                    break;
                case EHeadUnderVisionStep.ZAxis_ReadyPosition_Check:
                    if (Flag_ZAxisReady)
                    {
                        Step.RunStep = (int)EHeadUnderVisionStep.YTAxis_UnderVision_Move;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EHeadUnderVisionStep.ZAxis_ReadyPosition_Move:
                    ZAxis_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case EHeadUnderVisionStep.ZAxis_ReadyPosition_MoveWait:
                    if (Flag_ZAxisReady)
                    {
                        Log.Debug($"{ZAxis} move Ready Position done");
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
                            CDef.RootProcess.SetWarning($"{ZAxis} move Ready Position timeout");
                        }
                    }
                    break;
                case EHeadUnderVisionStep.YTAxis_UnderVision_Move:
                    YAxis_UnderVisionPosition_Move();
                    TAxis_UnderVisionPosition_Move();
                    Step.RunStep++;
                    break;
                case EHeadUnderVisionStep.YTAxis_UnderVision_MoveWait:
                    if (YAxis_UnderVisionPosition_MoveDone() & TAxis_UnderVisionPosition_MoveDone())
                    {
                        Log.Debug($"{YYAxis} and {TAxis} move Under Vision Position done");
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
                            CDef.RootProcess.SetWarning($"{YYAxis} and {TAxis} move Under Vision Position timeout");
                        }
                    }
                    break;
                case EHeadUnderVisionStep.Transfer_UnderVision2_PositionMoveWait:
                    if (Flags.Transfer_UnderVision_Head2_Position)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EHeadUnderVisionStep.ZAxis_UnderVision_Move:
                    ZAxis_UnderVisionPosition_Move();
                    Step.RunStep++;
                    break;
                case EHeadUnderVisionStep.ZAxis_UnderVision_MoveWait:
                    if (ZAxis_UnderVisionPosition_MoveDone())
                    {
                        Log.Debug($"{ZAxis} move Under Vision Position done");
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
                            CDef.RootProcess.SetWarning($"{ZAxis} move Under Vision Position timeout");
                        }
                    }
                    break;
                case EHeadUnderVisionStep.UnderVision1_InspectDoneWait:
                    if (Flags.UnderVision_Inspect_Done)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EHeadUnderVisionStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        if (Flags.UnloadInspect_Done)
                        {
                            this.RunMode = ERunMode.Manual_Place;
                        }
                        else
                        {
                            this.RunMode = ERunMode.Manual_UnloadVision;
                        }
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UnderVision)
                    {
                        this.RunMode = ERunMode.Stop;
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Running_TrayChange()
        {

        }

        private void Running_Inspect()
        {
            switch ((EHeadInspectStep)Step.RunStep)
            {
                case EHeadInspectStep.Start:
                    if (CDef.GlobalRecipe.SkipBallInspect == false)
                    {
                        Log.Debug($"{Head} inspect start.");
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EHeadInspectStep.End;
                    }
                    break;
                case EHeadInspectStep.ZAxis_ReadyPosition_Move:
                    ZAxis_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case EHeadInspectStep.ZAxis_ReadyPosition_MoveWait:
                    if (Flag_ZAxisReady)
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
                            CDef.RootProcess.SetWarning($"{ZAxis} move Ready Position timeout");
                        }
                    }
                    break;
                case EHeadInspectStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_TrayChange;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Inspect)
                    {
                        Log.Debug($"Head inspect done");
                        this.RunMode = ERunMode.Stop;
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
            if (Head == EHeads.Head1)
            {
                ZAxis.MoveAbs(CDef.HeadRecipe.Z1Axis_Ready_Position);
            }
            else
            {
                ZAxis.MoveAbs(CDef.HeadRecipe.Z2Axis_Ready_Position);
            }
        }

        public void ZAxis_PickPlace_Position1_Move(EPickPlace mode)
        {
            if (Head == EHeads.Head1)
            {
                if (mode == EPickPlace.PICK)
                {
                    ZAxis.MoveAbs(CDef.HeadRecipe.Z1Axis_Pick_Position - Math.Abs(CDef.HeadRecipe.ZAxis_PickPlace_OverrideHeight));
                }
                else
                {
                    ZAxis.MoveAbs(CDef.HeadRecipe.Z1Axis_Place_Position - Math.Abs(CDef.HeadRecipe.ZAxis_PickPlace_OverrideHeight));
                }
            }
            else
            {
                if (mode == EPickPlace.PICK)
                {
                    ZAxis.MoveAbs(CDef.HeadRecipe.Z2Axis_Pick_Position - Math.Abs(CDef.HeadRecipe.ZAxis_PickPlace_OverrideHeight));
                }
                else
                {
                    ZAxis.MoveAbs(CDef.HeadRecipe.Z2Axis_Place_Position - Math.Abs(CDef.HeadRecipe.ZAxis_PickPlace_OverrideHeight));
                }
            }
        }

        public bool ZAxis_PickPlace_Position1_MoveDone(EPickPlace mode)
        {
            if (Head == EHeads.Head1)
            {
                if (mode == EPickPlace.PICK)
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z1Axis_Pick_Position - Math.Abs(CDef.HeadRecipe.ZAxis_PickPlace_OverrideHeight));
                }
                else
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z1Axis_Place_Position - Math.Abs(CDef.HeadRecipe.ZAxis_PickPlace_OverrideHeight));
                }
            }
            else
            {
                if (mode == EPickPlace.PICK)
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z2Axis_Pick_Position - Math.Abs(CDef.HeadRecipe.ZAxis_PickPlace_OverrideHeight));
                }
                else
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z2Axis_Place_Position - Math.Abs(CDef.HeadRecipe.ZAxis_PickPlace_OverrideHeight));
                }
            }
        }

        public void ZAxis_PickPlace_Position2_Move(EPickPlace mode)
        {
            if (Head == EHeads.Head1)
            {
                if (mode == EPickPlace.PICK)
                {
                    ZAxis.MoveAbs(CDef.HeadRecipe.Z1Axis_Pick_Position, ZAxis.Speed * CDef.HeadRecipe.ZAxis_PickPlace_OverrideSpeed / 100);
                }
                else
                {
                    ZAxis.MoveAbs(CDef.HeadRecipe.Z1Axis_Place_Position, ZAxis.Speed * CDef.HeadRecipe.ZAxis_PickPlace_OverrideSpeed / 100);
                }
            }
            else
            {
                if (mode == EPickPlace.PICK)
                {
                    ZAxis.MoveAbs(CDef.HeadRecipe.Z2Axis_Pick_Position, ZAxis.Speed * CDef.HeadRecipe.ZAxis_PickPlace_OverrideSpeed / 100);
                }
                else
                {
                    ZAxis.MoveAbs(CDef.HeadRecipe.Z2Axis_Place_Position, ZAxis.Speed * CDef.HeadRecipe.ZAxis_PickPlace_OverrideSpeed / 100);
                }
            }
        }

        public bool ZAxis_PickPlace_Position2_MoveDone(EPickPlace mode)
        {
            if (Head == EHeads.Head1)
            {
                if (mode == EPickPlace.PICK)
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z1Axis_Pick_Position);
                }
                else
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z1Axis_Place_Position);
                }
            }
            else
            {
                if (mode == EPickPlace.PICK)
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z2Axis_Pick_Position);
                }
                else
                {
                    return ZAxis.IsOnPosition(CDef.HeadRecipe.Z2Axis_Place_Position);
                }
            }
        }

        public void ZAxis_UnderVisionPosition_Move()
        {
            if (Head == EHeads.Head1)
            {
                ZAxis.MoveAbs(CDef.HeadRecipe.Z1Axis_UnderVS_Position);
            }
            else
            {
                ZAxis.MoveAbs(CDef.HeadRecipe.Z2Axis_UnderVS_Position);
            }
        }
        public bool ZAxis_UnderVisionPosition_MoveDone()
        {
            if (Head == EHeads.Head1)
            {
                return ZAxis.IsOnPosition(CDef.HeadRecipe.Z1Axis_UnderVS_Position);
            }
            else
            {
                return ZAxis.IsOnPosition(CDef.HeadRecipe.Z2Axis_UnderVS_Position);
            }
        }

        public void YAxis_PickPlace_Position_Move(EPickPlace mode)
        {
            if (Head == EHeads.Head1)
            {
                if (mode == EPickPlace.PICK)
                {
                    YYAxis.MoveAbs(CDef.HeadRecipe.YY1Axis_Pick_Position - Datas.Data_LoadingInspect_Result[0].DetectedOffset.Y);
                }
                else
                {
                    double underVisionOffset = - Datas.Data_UnderInspect_Result[0].DetectedOffset.Y;
                    double unloadVisionOffset = - Datas.Data_UnloadingInspect_Result[0].DetectedOffset.Y;
                    double _YYAxisPlacePosition = CDef.HeadRecipe.YY1Axis_Place_Position + underVisionOffset + unloadVisionOffset;
                    
                    YYAxis.MoveAbs(_YYAxisPlacePosition);

                    Log.Info($"[{YYAxis.AxisName} Place Position]: {_YYAxisPlacePosition:0.000} mm " +
                        $"[Under Vison Offset]: {underVisionOffset:0.000} mm " +
                        $"[Unload Vison Offset]: {unloadVisionOffset:0.000} mm");
                }
            }
            else
            {
                if (mode == EPickPlace.PICK)
                {
                    YYAxis.MoveAbs(CDef.HeadRecipe.YY2Axis_Pick_Position - Datas.Data_LoadingInspect_Result[1].DetectedOffset.Y);
                }
                else
                {
                    double underVisionOffset = -Datas.Data_UnloadingInspect_Result[1].DetectedOffset.Y;
                    double unloadVisionOffset = -Datas.Data_UnderInspect_Result[1].DetectedOffset.Y;
                    double _YYAxisPlacePosition = CDef.HeadRecipe.YY2Axis_Place_Position + underVisionOffset + unloadVisionOffset;

                    YYAxis.MoveAbs(_YYAxisPlacePosition);

                    Log.Info($"[{YYAxis.AxisName} Place Position]: {_YYAxisPlacePosition:0.000} mm " +
                        $"[Under Vison Offset]: {underVisionOffset:0.000} mm " +
                        $"[Unload Vison Offset]: {unloadVisionOffset:0.000} mm");
                }
            }
        }
        public bool YAxis_PickPlace_Position_MoveDone(EPickPlace mode)
        {
            if (Head == EHeads.Head1)
            {
                if (mode == EPickPlace.PICK)
                {
                    return YYAxis.IsOnPosition(CDef.HeadRecipe.YY1Axis_Pick_Position - Datas.Data_LoadingInspect_Result[0].DetectedOffset.Y);
                }
                else
                {
                    double underVisionOffset = -Datas.Data_UnderInspect_Result[0].DetectedOffset.Y;
                    double unloadVisionOffset = -Datas.Data_UnloadingInspect_Result[0].DetectedOffset.Y;
                    double _YYAxisPlacePosition = CDef.HeadRecipe.YY1Axis_Place_Position + underVisionOffset + unloadVisionOffset;

                    return YYAxis.IsOnPosition(_YYAxisPlacePosition);
                }
            }
            else
            {
                if (mode == EPickPlace.PICK)
                {
                    return YYAxis.IsOnPosition(CDef.HeadRecipe.YY2Axis_Pick_Position - Datas.Data_LoadingInspect_Result[1].DetectedOffset.Y);
                }
                else
                {
                    double underVisionOffset = -Datas.Data_UnloadingInspect_Result[1].DetectedOffset.Y;
                    double unloadVisionOffset = -Datas.Data_UnderInspect_Result[1].DetectedOffset.Y;
                    double _YYAxisPlacePosition = CDef.HeadRecipe.YY2Axis_Place_Position + underVisionOffset + unloadVisionOffset;

                    return YYAxis.IsOnPosition(_YYAxisPlacePosition);
                }
            }
        }

        public void YAxis_UnderVisionPosition_Move()
        {
            if (Head == EHeads.Head1)
            {
                YYAxis.MoveAbs(CDef.HeadRecipe.YY1Axis_UnderVS_Position);
            }
            else
            {
                YYAxis.MoveAbs(CDef.HeadRecipe.YY2Axis_UnderVS_Position);
            }
        }
        public bool YAxis_UnderVisionPosition_MoveDone()
        {
            if (Head == EHeads.Head1)
            {
                return YYAxis.IsOnPosition(CDef.HeadRecipe.YY1Axis_UnderVS_Position);
            }
            else
            {
                return YYAxis.IsOnPosition(CDef.HeadRecipe.YY2Axis_UnderVS_Position);
            }
        }

        public void TAxis_PickPlace_Position_Move(EPickPlace mode)
        {
            if (Head == EHeads.Head1)
            {
                if (mode == EPickPlace.PICK)
                {
                    Log.Debug($"Theta 1 pick vision offset = {- Datas.Data_LoadingInspect_Result[0].DetectedOffset.Theta * 10.0 / 360.0}");
                    TAxis.MoveAbs(CDef.HeadRecipe.T1Axis_Pick_Position - Datas.Data_LoadingInspect_Result[0].DetectedOffset.Theta * 10.0 / 360.0);
                }
                else
                {
                    double underVisionOffset = -Datas.Data_UnderInspect_Result[0].DetectedOffset.Theta * 10.0 / 360.0;
                    double unloadVisionOffset = -Datas.Data_UnloadingInspect_Result[0].DetectedOffset.Theta * 10.0 / 360.0;
                    double _TAxisPlacePosition = CDef.HeadRecipe.T1Axis_Place_Position + unloadVisionOffset + underVisionOffset;
                    
                    TAxis.MoveAbs(_TAxisPlacePosition);

                    Log.Info($"[{TAxis.AxisName} Place Position]: {_TAxisPlacePosition:0.000} mm " +
                        $"[Under Vison Offset]: {underVisionOffset:0.000} mm " +
                        $"[Unload Vison Offset]: {unloadVisionOffset:0.000} mm");
                }
            }
            else
            {
                if (mode == EPickPlace.PICK)
                {
                    Log.Debug($"Theta 2 pick vision offset = {- Datas.Data_LoadingInspect_Result[1].DetectedOffset.Theta * 10.0 / 360.0}");
                    TAxis.MoveAbs(CDef.HeadRecipe.T2Axis_Pick_Position - Datas.Data_LoadingInspect_Result[1].DetectedOffset.Theta * 10.0 / 360.0);
                }
                else
                {
                    double underVisionOffset = -Datas.Data_UnderInspect_Result[1].DetectedOffset.Theta * 10.0 / 360.0;
                    double unloadVisionOffset = -Datas.Data_UnloadingInspect_Result[1].DetectedOffset.Theta * 10.0 / 360.0;
                    double _TAxisPlacePosition = CDef.HeadRecipe.T2Axis_Place_Position + unloadVisionOffset + underVisionOffset;

                    TAxis.MoveAbs(_TAxisPlacePosition);

                    Log.Info($"[{TAxis.AxisName} Place Position]: {_TAxisPlacePosition:0.000} mm " +
                        $"[Under Vison Offset]: {underVisionOffset:0.000} mm " +
                        $"[Unload Vison Offset]: {unloadVisionOffset:0.000} mm");
                }
            }
        }
        public bool TAxis_PickPlace_Position_MoveDone(EPickPlace mode)
        {
            if (Head == EHeads.Head1)
            {
                if (mode == EPickPlace.PICK)
                {
                    return TAxis.IsOnPosition(CDef.HeadRecipe.T1Axis_Pick_Position - Datas.Data_LoadingInspect_Result[0].DetectedOffset.Theta * 10.0 / 360.0);
                }
                else
                {
                    double underVisionOffset = -Datas.Data_UnderInspect_Result[0].DetectedOffset.Theta * 10.0 / 360.0;
                    double unloadVisionOffset = -Datas.Data_UnloadingInspect_Result[0].DetectedOffset.Theta * 10.0 / 360.0;
                    double _TAxisPlacePosition = CDef.HeadRecipe.T1Axis_Place_Position + unloadVisionOffset + underVisionOffset;

                    return TAxis.IsOnPosition(_TAxisPlacePosition);
                }
            }
            else
            {
                if (mode == EPickPlace.PICK)
                {
                    return TAxis.IsOnPosition(CDef.HeadRecipe.T2Axis_Pick_Position - Datas.Data_LoadingInspect_Result[1].DetectedOffset.Theta * 10.0 / 360.0);
                }
                else
                {
                    double underVisionOffset = -Datas.Data_UnderInspect_Result[1].DetectedOffset.Theta * 10.0 / 360.0;
                    double unloadVisionOffset = -Datas.Data_UnloadingInspect_Result[1].DetectedOffset.Theta * 10.0 / 360.0;
                    double _TAxisPlacePosition = CDef.HeadRecipe.T2Axis_Place_Position + unloadVisionOffset + underVisionOffset;

                    return TAxis.IsOnPosition(_TAxisPlacePosition);
                }
            }
        }

        public void TAxis_UnderVisionPosition_Move()
        {
            if (Head == EHeads.Head1)
            {
                TAxis.MoveAbs(CDef.HeadRecipe.T1Axis_UnderVS_Position);
            }
            else
            {
                TAxis.MoveAbs(CDef.HeadRecipe.T2Axis_UnderVS_Position);
            }
        }
        public bool TAxis_UnderVisionPosition_MoveDone()
        {
            if (Head == EHeads.Head1)
            {
                return TAxis.IsOnPosition(CDef.HeadRecipe.T1Axis_UnderVS_Position);
            }
            else
            {
                return TAxis.IsOnPosition(CDef.HeadRecipe.T2Axis_UnderVS_Position);
            }
        }

        public void VacuumOn()
        {
            if (Head == EHeads.Head1)
            {
                CDef.IO.Output.Head1VAC = true;
            }
            else
            {
                CDef.IO.Output.Head2VAC = true;
            }
        }
        public void VacuumOff()
        {
            if (Head == EHeads.Head1)
            {
                CDef.IO.Output.Head1VAC = false;
            }
            else
            {
                CDef.IO.Output.Head2VAC = false;
            }
        }

        public void PurgeOn()
        {
            VacuumOff();

            if (Head == EHeads.Head1)
            {
                CDef.IO.Output.Head1Purge = true;
            }
            else
            {
                CDef.IO.Output.Head2Purge = true;
            }
        }
        public void PurgeOff()
        {
            if (Head == EHeads.Head1)
            {
                CDef.IO.Output.Head1Purge = false;
            }
            else
            {
                CDef.IO.Output.Head2Purge = false;
            }
        }
        #endregion
    }
}
