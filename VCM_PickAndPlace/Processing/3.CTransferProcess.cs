using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Processing;
using TopMotion;
using VCM_PickAndPlace.Define;

namespace VCM_PickAndPlace.Processing
{
    public class CTransferProcess : ProcessingBase
    {
        #region Constructor(s)
        public CTransferProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }
        #endregion

        #region Properties
        #region Motion(s)
        public MotionFas16000 X1Axis
        {
            get { return (MotionFas16000)CDef.AllAxis.X1Axis; }
        }

        public MotionPlusE XXAxis
        {
            get { return (MotionPlusE)CDef.AllAxis.XXAxis; }
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

        #region Positions
        
        public double X1Axis_TransferInspect_Position
        {
            get
            {
                double position = 0;
                double offset = 0;

                if (Flags.Inspect.InspectColumn == -1) return X1Axis.Status.ActualPosition;

                position = CDef.TransferRecipe.X1Axis_Inspect_Position;
                offset = Flags.Inspect.InspectColumn * CDef.CommonRecipe.UnloadingTray_X_Pitch;

                return position - offset;
            }
        }

        public double X1Axis_Pick_Position
        {
            get
            {
                double position = 0;
                double offset = 0;

                double visionOffsetX = Datas.Data_LoadingInspect_Result[0].DetectedOffset.X;

                offset = CDef.CurrentLoadingTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;
                position = CDef.TransferRecipe.X1Axis_Pick_Position;

                if (CDef.CurrentLoadingTray != CDef.LoadingTrays[0])
                {
                    position = CDef.TransferRecipe.X1Axis_Pick_Position + CDef.CommonRecipe.LoadingTray_Offset_X;
                }

                return (position - offset + visionOffsetX);
            }
        }

        public double XXAxis_Pick_Position
        {
            get
            {
                double[] visionOffsetX = new double[] { Datas.Data_LoadingInspect_Result[0].DetectedOffset.X,
                                                        Datas.Data_LoadingInspect_Result[1].DetectedOffset.X};
                double position = 0;

                position = CDef.TransferRecipe.XXAxis_Pick_Position;

                return position - visionOffsetX[0] + visionOffsetX[1];
            }
        }

        public double X1Axis_Place_Position
        {
            get
            {
                double visionOffsetX = Datas.Data_UnloadingInspect_Result[0].DetectedOffset.X
                                      - Datas.Data_UnderInspect_Result[0].DetectedOffset.X;

                double position = 0;
                double offset = 0;

                offset = CDef.CurrentUnloadingTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;
                position = CDef.TransferRecipe.X1Axis_Place_Position;

                if (CDef.CurrentUnloadingTray != CDef.UnloadingTrays[0])
                {
                    position += CDef.CommonRecipe.UnloadingTray_Offset_X;
                }

                return (position - offset + visionOffsetX);
            }
        }

        public double XXAxis_Place_Position
        {
            get
            {
                double[] visionOffsetX = new double[] { Datas.Data_UnloadingInspect_Result[0].DetectedOffset.X - Datas.Data_UnderInspect_Result[0].DetectedOffset.X
                                                      , Datas.Data_UnloadingInspect_Result[1].DetectedOffset.X - Datas.Data_UnderInspect_Result[1].DetectedOffset.X };

                double position = 0;

                position = CDef.TransferRecipe.XXAxis_Place_Position;

                return position - visionOffsetX[0] + visionOffsetX[1];
            }
        }
        #endregion

        #region Flag(s)
        public bool Flag_Transfer_LoadVision_Avoid_Position
        {
            get
            {
                return X1Axis.Status.ActualPosition >= CDef.TransferRecipe.X1Axis_LoadVS_Avoid_Position - 0.01;
            }
        }

        public bool Flag_Transfer_UnloadVision_Avoid_Position
        {
            get
            {
                return X1Axis.Status.ActualPosition <= CDef.TransferRecipe.X1Axis_UnloadVS_Avoid_Position + 0.01;
            }
        }

        public bool Flag_Transfer_Pick_Position
        {
            get
            {
                return Flag_X1Axis_Pick_Position && Flag_XXAxis_Pick_Position;
            }
        }

        private bool Flag_X1Axis_Pick_Position
        {
            get
            {
                return X1Axis.IsOnPosition(X1Axis_Pick_Position);
            }
        }

        private bool Flag_XXAxis_Pick_Position
        {
            get
            {
                return XXAxis.IsOnPosition(XXAxis_Pick_Position);
            }
        }

        public bool Flag_Transfer_UnderVision1_Position
        {
            get
            {
                return X1Axis.IsOnPosition(CDef.TransferRecipe.X1Axis_UnderVS1_Position);
            }
        }

        public bool Flag_Transfer_UnderVision2_Position
        {
            get
            {
                return Flag_X1Axis_UnderVision2_Position && Flag_XXAxis_UnderVision_Position;
            }
        }

        private bool Flag_X1Axis_UnderVision2_Position
        {
            get
            {
                return X1Axis.IsOnPosition(CDef.TransferRecipe.X1Axis_UnderVS2_Position);
            }
        }

        public bool Flag_Transfer_Place_Position
        {
            get
            {
                return Flag_X1Axis_Place_Position && Flag_XXAxis_Place_Position;
            }
        }

        public bool Flag_Transfer_Inspect_Position
        {
            get
            {
                return X1Axis_Inspect_PositionMoveDone() & XXAxis_Inspect_PositionMoveDone();
            }
        }

        private bool Flag_X1Axis_Place_Position
        {
            get
            {
                return X1Axis.IsOnPosition(X1Axis_Place_Position);
            }
        }

        private bool Flag_XXAxis_Place_Position
        {
            get
            {
                return XXAxis.IsOnPosition(XXAxis_Place_Position);
            }
        }

        private bool Flag_XXAxis_UnderVision_Position
        {
            get
            {
                return XXAxis.IsOnPosition(CDef.TransferRecipe.XXAxis_UnderVision_Position);
            }
        }

        public bool Flag_Transfer_Pick_Done { get; set; }
        public bool Flag_Transfer_Place_Done { get; set; }
        #endregion

        #region Data(s)
        #endregion

        #region Overrider(s)
        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;
            switch ((ETransferHomeStep)Step.HomeStep)
            {
                case ETransferHomeStep.Start:
                    Log.Debug("Home Start");
                    Step.HomeStep++;
                    break;
                case ETransferHomeStep.AllZAxis_HomeWait:
                    if (HomeStatus.IsAllZAxisHomeDone())
                    {
                        Step.HomeStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETransferHomeStep.XAxis_HomeSearch:
                    X1Axis.HomeSearch();
                    XXAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case ETransferHomeStep.XAxis_HomeSearchWait:
                    if (X1Axis.Status.IsHomeDone && XXAxis.Status.IsHomeDone)
                    {
                        Log.Debug($"{X1Axis} and {XXAxis} origin done");
                        X1Axis.ClearPosition();
                        XXAxis.ClearPosition();
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
                            CDef.RootProcess.SetWarning($"{X1Axis} and {XXAxis} home search timeout");
                        }
                    }
                    break;
                case ETransferHomeStep.End:
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
            switch ((ETransferToRunStep)Step.ToRunStep)
            {
                case ETransferToRunStep.Start:
                    Log.Debug("To Run Start");
                    Step.ToRunStep++;
                    break;
                case ETransferToRunStep.AllZAxis_ReadyPosition_Wait:
                    if (Flags.AllZAxis_Ready)
                    {
                        Step.ToRunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETransferToRunStep.XAxis_WorkPosition_Move:
                    X1Axis_Vision_Avoid_Position_Move(EVisionArea.LOAD);
                    XXAxis_PickPlace_PositionMove(EPickPlace.PICK);
                    Step.ToRunStep++;
                    break;
                case ETransferToRunStep.XAxis_WorkPosition_MoveWait:
                    if (X1Axis_Vision_Avoid_Position_MoveDone(EVisionArea.LOAD) && Flag_XXAxis_Pick_Position)
                    {
                        Log.Debug($"{X1Axis} move Load Vision Avoid Position Done");
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
                            if (X1Axis_Vision_Avoid_Position_MoveDone(EVisionArea.LOAD) == false)
                            {
                                CDef.RootProcess.SetWarning($"{X1Axis} move Load Vision Avoid Position timeout");
                            }
                            if (Flag_XXAxis_Pick_Position == false)
                            {
                                CDef.RootProcess.SetWarning($"{XXAxis} move Load Vision Avoid Position timeout");
                            }
                        }
                    }
                    break;
                case ETransferToRunStep.End:
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
                    break;
#if USPCUTTING
                case ERunMode.Manual_Press:
                    Running_Press();
                    break;
#endif
                case ERunMode.Manual_LoadVision:
                    Running_UpperVision(EVisionArea.LOAD);
                    break;
                case ERunMode.Manual_Pick:
                    Running_PickPlace(EPickPlace.PICK);
                    break;
                case ERunMode.Manual_UnderVision:
                    Running_UnderVision();
                    break;
                case ERunMode.Manual_UnloadVision:
                    Running_UpperVision(EVisionArea.UNLOAD);
                    break;
                case ERunMode.Manual_Place:
                    Running_PickPlace(EPickPlace.PLACE);
                    break;
                case ERunMode.Manual_Inspect:
                    Running_Inspect();
                    break;
                case ERunMode.Manual_TrayChange:
                    Sleep(20);
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
            switch ((ETransferPressStep)Step.RunStep)
            {
                case ETransferPressStep.Start:
                    Log.Debug("Press Start");
                    Step.RunStep++;
                    break;
                case ETransferPressStep.End:
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
                    Sleep(10);
                    break;
            }
        }
#endif

        private void Running_UpperVision(EVisionArea visionZone)
        {
            if (CDef.CurrentUnloadingTray == null)
            {
                this.RunMode = ERunMode.Manual_Inspect;
                return;
            }

            if (CDef.RootProcess.RunMode == ERunMode.AutoRun && visionZone == EVisionArea.LOAD && CDef.GlobalRecipe.SkipLoadVision)
            {
                PTimer.TactTimeCounter[0] = PTimer.Now;
                this.RunMode = ERunMode.Manual_Pick;
                return;
            }

            if (CDef.RootProcess.RunMode == ERunMode.AutoRun && visionZone == EVisionArea.UNLOAD && CDef.GlobalRecipe.SkipUnloadVision)
            {
                this.RunMode = ERunMode.Manual_Place;
                return;
            }

            switch ((ETransferLoadVisionStep)Step.RunStep)
            {
                case ETransferLoadVisionStep.Start:
                    Log.Debug($"{visionZone} Vision Start");
                    PTimer.TactTimeCounter[0] = PTimer.Now;
                    Step.RunStep++;
                    break;
                case ETransferLoadVisionStep.LoadVision_DoneCheck:
                    bool flag_VisionInspectDone = visionZone == EVisionArea.LOAD ? Flags.LoadInspect_Done : Flags.UnloadInspect_Done;
                    if (flag_VisionInspectDone == false)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)ETransferLoadVisionStep.LoadVision_DoneWait;
                    }
                    break;
                case ETransferLoadVisionStep.HeadZAxis_ReadyPosition_Wait:
                    if (Flags.AllHead_ZReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETransferLoadVisionStep.XAxis_VisionAvoid_PositionCheck:
                    bool flag_VisionAvoidPosition = visionZone == EVisionArea.LOAD
                                                    ? Flag_Transfer_LoadVision_Avoid_Position
                                                    : Flag_Transfer_UnloadVision_Avoid_Position;
                    if (flag_VisionAvoidPosition)
                    {
                        Step.RunStep = (int)ETransferLoadVisionStep.LoadVision_DoneWait;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETransferLoadVisionStep.XAxis_VisionAvoid_PositionMove:
                    X1Axis_Vision_Avoid_Position_Move(visionZone);
                    Step.RunStep++;
                    break;
                case ETransferLoadVisionStep.XAxis_VisionAvoid_PositionMoveWait:
                    bool flag_VisionAvoidPosition_1 = visionZone == EVisionArea.LOAD
                                                    ? Flag_Transfer_LoadVision_Avoid_Position
                                                    : Flag_Transfer_UnloadVision_Avoid_Position;
                    if (flag_VisionAvoidPosition_1)
                    {
                        Log.Debug($"{X1Axis} move {visionZone} Vision Avoid Position Done");
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
                            CDef.RootProcess.SetWarning($"{X1Axis} move {visionZone} Vision Avoid Position timeout");
                        }
                    }
                    break;
                case ETransferLoadVisionStep.LoadVision_DoneWait:
                    bool flag_VisionInspectDone_ = visionZone == EVisionArea.LOAD ? Flags.LoadInspect_Done : Flags.UnloadInspect_Done;
                    if (flag_VisionInspectDone_)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        // Timeout will be throw in UpperVision process
                        Sleep(10);
                    }
                    break;
                case ETransferLoadVisionStep.End:
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
            switch ((ETransferPickStep)Step.RunStep)
            {
                case ETransferPickStep.Start:
                    Log.Debug($"{mode} Start");
                    if (mode == EPickPlace.PICK)
                    {
                        PTimer.TactTimeCounter[1] = PTimer.Now;
                    }
                    else
                    {
                        PTimer.TactTimeCounter[2] = PTimer.Now;
                    }
                    Step.RunStep++;
                    break;
                case ETransferPickStep.PickPlace_DoneCheck:
                    bool flag_PickPlace_DoneCheck = mode == EPickPlace.PICK ? HeadWorkStatus.PickDone : HeadWorkStatus.PlaceDone;

                    if (flag_PickPlace_DoneCheck)
                    {
                        Sleep(20);
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETransferPickStep.HeadZAxis_ReadyPosition_Wait:
                    if (Flags.AllHead_ZReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETransferPickStep.CheckFlag_Vision_AvoidRequest:
                    bool flag_Vision_AvoidRequest = mode == EPickPlace.PICK ? Flags.LoadVision_AvoidRequest : Flags.UnloadVision_AvoidRequest;

                    if (flag_Vision_AvoidRequest)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)ETransferPickStep.XAxisXXAxis_Pick_PositionMove;
                    }
                    break;
                case ETransferPickStep.XAxis_AvoidPosition_Check:
                    bool flag_Vision_AvoidPosition = mode == EPickPlace.PICK
                                                    ? Flag_Transfer_LoadVision_Avoid_Position
                                                    : Flag_Transfer_UnloadVision_Avoid_Position;

                    if (flag_Vision_AvoidPosition == false)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        // Check if UpperVision still need transfer to avoid
                        Step.RunStep = (int)ETransferPickStep.CheckFlag_Vision_AvoidRequest;
                    }
                    break;
                case ETransferPickStep.XAxis_AvoidPosition_Move:
                    if (mode == EPickPlace.PICK)
                    {
                        X1Axis_Vision_Avoid_Position_Move(EVisionArea.LOAD);
                    }
                    else
                    {
                        X1Axis_Vision_Avoid_Position_Move(EVisionArea.UNLOAD);
                    }
                    Step.RunStep++;
                    break;
                case ETransferPickStep.XAxis_AvoidPosition_Wait:
                    bool avoidMoveDone = mode == EPickPlace.PICK
                                        ? X1Axis_Vision_Avoid_Position_MoveDone(EVisionArea.LOAD)
                                        : X1Axis_Vision_Avoid_Position_MoveDone(EVisionArea.UNLOAD);

                    if (avoidMoveDone)
                    {
                        Log.Debug($"{X1Axis} move Load/Unload Vision Avoid Position");
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
                            CDef.RootProcess.SetWarning($"{X1Axis} and move Vision avoid Position timeout");
                        }
                    }
                    break;
                case ETransferPickStep.Vision_DoneWait:
                    bool visionDone = mode == EPickPlace.PICK
                                    ? Flags.LoadInspect_Done
                                    : Flags.UnloadInspect_Done;

                    if (visionDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        // Timeout will be throw in UpperVision process
                        Sleep(10);
                    }
                    break;
                case ETransferPickStep.XAxisXXAxis_Pick_PositionMove:
                    X1Axis_PickPlace_PositionMove(mode);
                    XXAxis_PickPlace_PositionMove(mode);
                    Step.RunStep++;
                    break;
                case ETransferPickStep.XAxisXXAxis_Pick_PositionMoveWait:
                    bool flag_PickPlace_Position_1 = mode == EPickPlace.PICK ? Flag_Transfer_Pick_Position : Flag_Transfer_Place_Position;

                    if (flag_PickPlace_Position_1)
                    {
                        // TODO: PASSING Transfer Ready Flag (to HeadProcess)
                        Log.Debug($"{X1Axis} and {XXAxis} move {mode} Position done");
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
                            CDef.RootProcess.SetWarning($"{X1Axis} and {XXAxis} move {mode} Position timeout");
                        }
                    }
                    break;
                case ETransferPickStep.PickPlace_DoneWait:
                    bool pickPlaceDone = mode == EPickPlace.PICK ? HeadWorkStatus.PickDone : HeadWorkStatus.PlaceDone;

                    if (pickPlaceDone == false)
                    {
                        Sleep(10);
                        break;
                    }

                    if (mode == EPickPlace.PICK)
                    {
                        Flag_Transfer_Pick_Done = true;
                    }
                    else
                    {
                        Flag_Transfer_Place_Done = true;
                    }
                    Step.RunStep++;
                    break;
                case ETransferPickStep.XXAxis_UnderVision_PreMove:
                    if (CDef.RootProcess.RunMode != ERunMode.AutoRun)
                    {
                        Step.RunStep++;
                        break;
                    }

                    if (mode == EPickPlace.PICK)
                    {
                        XXAxis_UnderVision_PositionMove();
                    }
                    else
                    {
                        XXAxis_PickPlace_PositionMove(EPickPlace.PICK);
                    }
                    Step.RunStep++;
                    break;
                case ETransferPickStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        if (mode == EPickPlace.PICK)
                        {
                            this.RunMode = ERunMode.Manual_UnderVision;
                        }
                        else
                        {
                            this.RunMode = ERunMode.Manual_LoadVision;
                        }
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Pick || CDef.RootProcess.RunMode == ERunMode.Manual_Place)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Running_UnderVision()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
               CDef.GlobalRecipe.SkipUnderVision)
            {
                PTimer.TactTimeCounter[3] = PTimer.Now;
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

            switch ((ETransferUnderVisionStep)Step.RunStep)
            {
                case ETransferUnderVisionStep.Start:
                    Log.Debug("Under Vision Start");
                    PTimer.TactTimeCounter[3] = PTimer.Now;
                    Step.RunStep++;
                    break;
                case ETransferUnderVisionStep.HeadZAxis_ReadyPosition_Wait:
                    if (Flags.AllHead_ZReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETransferUnderVisionStep.CheckFlag_UnloadVision_AvoidRequest:
                    if (Flags.UnloadVision_AvoidRequest)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)ETransferUnderVisionStep.XAxisXXAxis_UnderVision2_PositionMove;
                    }
                    break;
                case ETransferUnderVisionStep.XAxis_AvoidPosition_Check:
                    if (Flag_Transfer_UnloadVision_Avoid_Position == false)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)ETransferUnderVisionStep.Vision_DoneWait;
                    }
                    break;
                case ETransferUnderVisionStep.XAxis_AvoidPosition_Move:
                    X1Axis_Vision_Avoid_Position_Move(EVisionArea.UNLOAD);
                    Step.RunStep++;
                    break;
                case ETransferUnderVisionStep.XAxis_AvoidPosition_Wait:
                    if (X1Axis_Vision_Avoid_Position_MoveDone(EVisionArea.UNLOAD))
                    {
                        Log.Debug($"{X1Axis} move Unload Vision Avoid Position done ");
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
                            CDef.RootProcess.SetWarning($"{X1Axis} and move unload vision avoid Position timeout");
                        }
                    }
                    break;
                case ETransferUnderVisionStep.Vision_DoneWait:
                    if (Flags.UnloadInspect_Done)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETransferUnderVisionStep.XAxisXXAxis_UnderVision2_PositionMove:
                    XXAxis_UnderVision_PositionMove();
                    X1Axis_UnderVision2_PositionMove();
                    Step.RunStep++;
                    break;
                case ETransferUnderVisionStep.XAxisXXAxis_UnderVision2_PositionMoveWait:
                    if (Flag_Transfer_UnderVision2_Position)
                    {
                        Log.Debug($"{X1Axis} and {XXAxis} move Under Vision 2 Position done");
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
                            if (Flag_XXAxis_UnderVision_Position == false)
                            {
                                CDef.RootProcess.SetWarning($"{XXAxis} move Under Vision Position timeout");
                            }
                            if (Flag_X1Axis_Place_Position == false)
                            {
                                CDef.RootProcess.SetWarning($"{X1Axis} move Under Vision 2 Position timeout");
                            }
                        }
                    }
                    break;
                case ETransferUnderVisionStep.UnderVision2_InspectDoneWait:
                    if (Flags.UnderVision_Inspect_Head2_Done)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETransferUnderVisionStep.XAxisXXAxis_UnderVision1_PositionMove:
                    X1Axis_UnderVision1_PositionMove();
                    Step.RunStep++;
                    break;
                case ETransferUnderVisionStep.XAxisXXAxis_UnderVision1_PositionMoveWait:
                    if (Flag_Transfer_UnderVision1_Position)
                    {
                        Log.Debug($"{X1Axis} move Under Vision 1 Position done");
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
                            CDef.RootProcess.SetWarning($"{X1Axis} move Under Vision 1 Position timeout");
                        }
                    }
                    break;
                case ETransferUnderVisionStep.UnderVision1_InspectDoneWait:
                    if (Flags.UnderVision_Inspect_Done)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETransferUnderVisionStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        Datas.WorkData.TaktTime.UnderVisionProcess = 1.0 * (PTimer.Now - CDef.RootProcess.TransferProcess.PTimer.TactTimeCounter[3]) / 1000;
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
                    }
                    break;
                default:
                    break;
            }
        }

        private void Running_Inspect()
        {
            switch ((ETransferInspectStep)Step.RunStep)
            {
                case ETransferInspectStep.Start:
                    if (CDef.GlobalRecipe.SkipBallInspect == false)
                    {
                        Log.Debug($"Inspect start");
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)ETransferInspectStep.End;
                    }
                    break;
                case ETransferInspectStep.HeadZ_ReadyWait:
                    if (Flags.AllHead_ZReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Inspect waiting Z ready timeout");
                        }
                    }
                    break;
                case ETransferInspectStep.Transfer_InspectPosition_Move:
                    if (Flags.Inspect.Transfer_InspectPosition)
                    {
                        Sleep(50);
                    }
                    else
                    {
                        Transfer_Inspect_PositionMove();
                        Step.RunStep++;
                    }
                    break;
                case ETransferInspectStep.Transfer_InspectPosition_MoveWait:
                    if (Flag_Transfer_Inspect_Position)
                    {
                        Log.Debug($"Transfer ({X1Axis}, {XXAxis}) move Inspect Position done");
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
                            if (X1Axis_Inspect_PositionMoveDone() == false)
                            {
                                CDef.RootProcess.SetWarning($"{X1Axis} move Inspect Position TimeOut");
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"{XXAxis} move Inspect Position TimeOut");
                            }
                        }
                    }
                    break;
                case ETransferInspectStep.CheckIfInspectAllDone:
                    if (Flags.Inspect.IsAllCellInspectDone == false
                        /*&& CDef.RootProcess.RunMode == ERunMode.AutoRun*/)
                    {
                        Step.RunStep = (int)ETransferInspectStep.HeadZ_ReadyWait;
                    }
                    else
                    {
                        Step.RunStep = (int)ETransferInspectStep.End;
                    }
                    break;
                case ETransferInspectStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_TrayChange;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Inspect)
                    {
                        Log.Debug($"Transfer inspect done");
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Moving Function(s)
        public void X1Axis_Vision_Avoid_Position_Move(EVisionArea visionZone)
        {
            if (visionZone == EVisionArea.LOAD)
            {
                X1Axis.MoveAbs(CDef.TransferRecipe.X1Axis_LoadVS_Avoid_Position);
            }
            else
            {
                X1Axis.MoveAbs(CDef.TransferRecipe.X1Axis_UnloadVS_Avoid_Position);
            }
        }

        public bool X1Axis_Vision_Avoid_Position_MoveDone(EVisionArea visionZone)
        {
            if (visionZone == EVisionArea.LOAD)
            {
                return X1Axis.IsOnPosition(CDef.TransferRecipe.X1Axis_LoadVS_Avoid_Position);
            }
            else
            {
                return X1Axis.IsOnPosition(CDef.TransferRecipe.X1Axis_UnloadVS_Avoid_Position);
            }
        }

        public void X1Axis_PickPlace_PositionMove(EPickPlace mode)
        {
            if (mode == EPickPlace.PICK)
            {
                X1Axis.MoveAbs(X1Axis_Pick_Position);
            }
            else
            {
                X1Axis.MoveAbs(X1Axis_Place_Position);
            }
        }

        public void XXAxis_PickPlace_PositionMove(EPickPlace mode)
        {
            if (mode == EPickPlace.PICK)
            {
                XXAxis.MoveAbs(XXAxis_Pick_Position);
            }
            else
            {
                XXAxis.MoveAbs(XXAxis_Place_Position);
            }
        }

        public void X1Axis_UnderVision1_PositionMove()
        {
            X1Axis.MoveAbs(CDef.TransferRecipe.X1Axis_UnderVS1_Position);
        }

        public void X1Axis_UnderVision2_PositionMove()
        {
            X1Axis.MoveAbs(CDef.TransferRecipe.X1Axis_UnderVS2_Position);
        }

        public void XXAxis_UnderVision_PositionMove()
        {
            XXAxis.MoveAbs(CDef.TransferRecipe.XXAxis_UnderVision_Position);
        }

        public void Transfer_Inspect_PositionMove()
        {
            X1Axis.MoveAbs(X1Axis_TransferInspect_Position);
            XXAxis.MoveAbs(CDef.TransferRecipe.XXAxis_Inspect_Position);
        }

        public bool X1Axis_Inspect_PositionMoveDone()
        {
            return X1Axis.IsOnPosition(X1Axis_TransferInspect_Position);
        }

        public bool XXAxis_Inspect_PositionMoveDone()
        {
            return XXAxis.IsOnPosition(CDef.TransferRecipe.XXAxis_Inspect_Position);
        }
        #endregion
    }
}
