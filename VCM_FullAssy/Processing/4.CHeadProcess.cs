using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Processing;
using TopMotion;
using TopVision;
using VCM_FullAssy.Define;

namespace VCM_FullAssy.Processing
{
    public enum HeadHomeStep
    {
        HomeStart,
        ZAxis_HomeMove,
        ZAxis_HomeWait,
        TAxis_HomeMove,
        TAxis_HomeWait,
        HomeEnd,
    }

    public enum HeadToRunStep
    {
        ToRunStart,
        ZAxis_ReadyMove,
        ZAxis_ReadyWait,
        ToRunDecision,
        ToRunEnd,
    }

    public class CHeadProcess : ProcessingBase
    {
        #region Properties
        public MotionPlusE ZAxis
        {
            get { return (MotionPlusE)CDef.AllAxis.ZAxis; }
        }

        public MotionPlusE TAxis
        {
            get { return (MotionPlusE)CDef.AllAxis.TAxis; }
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

        #region Flags
        public bool Flag_ZAxisReady
        {
            get
            {
                return ZAxis.Status.ActualPosition <= CDef.HeadRecipe.ZAxisReady_Position + 0.01;
            }
        }

        public bool Flag_PickDone
        {
            get
            {
                return CDef.RootProcess.CurrentLoadTray == null
                    ? false
                    : CDef.RootProcess.CurrentLoadTray.GetCellStatus(CDef.RootProcess.CurrentLoadTray.WorkStartIndex) >= TopUI.Define.ECellStatus.NGPickOrPlace;
            }
        }

        public bool Flag_PlaceDone
        {
            get
            {
                return CDef.RootProcess.CurrentUnloadTray == null
                    ? false
                    : CDef.RootProcess.CurrentUnloadTray == null || CDef.RootProcess.CurrentUnloadTray.GetCellStatus(CDef.RootProcess.CurrentUnloadTray.WorkStartIndex) >= TopUI.Define.ECellStatus.NGPickOrPlace;
            }
        }

        public bool Flag_Head_LoadVisionPosition_MoveDone { get; set; }
        public bool Flag_Head_UnderVisionPosition_MoveDone { get; set; }
        public bool Flag_Head_UnloadVisionPosition_MoveDone { get; set; }
        #endregion

        public CHeadProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }

        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch ((HeadHomeStep)Step.HomeStep)
            {
                case HeadHomeStep.HomeStart:
                    Log.Debug("Head start origin");
                    Step.HomeStep++;
                    break;
                case HeadHomeStep.ZAxis_HomeMove:
                    ZAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case HeadHomeStep.ZAxis_HomeWait:
                    if (ZAxis.Status.IsHomeDone == true)
                    {
                        Log.Debug("Z Axis origin done");
                        HomeStatus.ZDone = true;
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
                            Log.Warn("Z Axis home search timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadHomeStep.TAxis_HomeMove:
                    TAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case HeadHomeStep.TAxis_HomeWait:
                    if (TAxis.Status.IsHomeDone == true)
                    {
                        Log.Debug("T Axis origin done");
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
                            Log.Warn("T Axis home search timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadHomeStep.HomeEnd:
                    Log.Debug("Head home done");
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

            switch ((HeadToRunStep)Step.ToRunStep)
            {
                case HeadToRunStep.ToRunStart:
                    Log.Debug("Head ToRun Start");
                    Flag_Head_LoadVisionPosition_MoveDone = false;
                    Flag_Head_UnderVisionPosition_MoveDone = false;
                    Flag_Head_UnloadVisionPosition_MoveDone = false;
                    Step.ToRunStep++;
                    break;
                case HeadToRunStep.ZAxis_ReadyMove:
                    Head_ZAxis_ReadyMove();
                    Step.ToRunStep++;
                    break;
                case HeadToRunStep.ZAxis_ReadyWait:
                    if (Head_ZAxis_ReadyMove_Done())
                    {
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
                            Log.Warn("Head Z Axis ready position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadToRunStep.ToRunDecision:
                    RunMode = CDef.RootProcess.RunMode;
                    Step.ToRunStep++;
                    break;
                case HeadToRunStep.ToRunEnd:
                    // Total, Pick, Place takt time reset
                    PTimer.TactTimeCounter = new System.Collections.ObjectModel.ObservableCollection<int> { 0, 0, 0 };

                    Log.Debug("Head ToRun End");
                    ProcessingStatus = EProcessingStatus.ToRunDone;
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
            PRtnCode nRtn = PRtnCode.RtnOk;
            switch (RunMode)
            {
                case ERunMode.Stop:
                    Sleep(20);
                    break;
                case ERunMode.AutoRun:
                    Running_LoadVision();
                    break;
                case ERunMode.Manual_LoadVision:
                    Running_LoadVision();
                    break;
                case ERunMode.Manual_Pick:
                    Running_Pick();
                    break;
                case ERunMode.Manual_UnderVision:
                    Running_UnderVision();
                    break;
                case ERunMode.Manual_UnloadVision:
                    Running_UnloadVision();
                    break;
                case ERunMode.Manual_Place:
                    Running_Place();
                    break;
                case ERunMode.Manual_TrayChange:
                    Running_TrayChange();
                    break;
                default:
                    break;
            }
            return nRtn;
        }

        #region Run Functions
        public enum HeadLoadVisionStep
        {
            LoadVisionStart,
            Tray_LoadVisionWait,
            Transfer_LoadVisionWait,
            ZAxis_LoadVisionDown_Move,
            ZAxis_LoadVisionDown_MoveWait,
            UpperVision_InspectDone,
            ZAxis_Ready_Move,
            ZAxis_Ready_MoveWait,
            LoadVisionEnd,
        }

        public void Running_LoadVision()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                CDef.GlobalRecipe.SkipLoadVision)
            {
                this.RunMode = ERunMode.Manual_Pick;
                return;
            }

            switch ((HeadLoadVisionStep)Step.RunStep)
            {
                case HeadLoadVisionStep.LoadVisionStart:
                    Log.Debug("Head Load Vision Start");
                    Step.RunStep++;
                    break;
                case HeadLoadVisionStep.Tray_LoadVisionWait:
                    if (Flags.Tray_LoadVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadLoadVisionStep.Transfer_LoadVisionWait:
                    if (Flags.Transfer_LoadVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadLoadVisionStep.ZAxis_LoadVisionDown_Move:
                    ZAxis_LoadVisionDown_Move();
                    Step.RunStep++;
                    break;
                case HeadLoadVisionStep.ZAxis_LoadVisionDown_MoveWait:
                    if (ZAxis_LoadVisionDownMove_Done())
                    {
                        Log.Debug("ZAxis move Load Vision Position Done");
                        Flag_Head_LoadVisionPosition_MoveDone = true;
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
                            Log.Warn($"{ZAxis} LoadVision down position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadLoadVisionStep.UpperVision_InspectDone:
                    if (Flags.Upper_LoadVision_InspectDone)
                    {
                        Flag_Head_LoadVisionPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadLoadVisionStep.ZAxis_Ready_Move:
                    Head_ZAxis_ReadyMove();
                    Step.RunStep++;
                    break;
                case HeadLoadVisionStep.ZAxis_Ready_MoveWait:
                    if (Head_ZAxis_ReadyMove_Done())
                    {
                        Log.Debug("ZAxis move Ready Position Done");
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
                            Log.Warn($"{ZAxis} ready position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadLoadVisionStep.LoadVisionEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Pick;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_LoadVision)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public enum HeadPickStep
        {
            PickStart,
            Tray_PickPosition_MoveWait,
            Transfer_PickPosition_MoveWait,
            ZAxis_PickDown1_Move,
            ZAxis_PickDown1_MoveWait,
            ZAxis_PickDown2_Move,
            ZAxis_PickDown2_MoveWait,
            Head_VacuumOn,
            Head_VacuumDelay,
            ZAxis_Ready_Move,
            ZAxis_Ready_MoveWait,
            Pick_ResultUpdate,
            PickEnd,
        }
        public void Running_Pick()
        {
            switch ((HeadPickStep)Step.RunStep)
            {
                case HeadPickStep.PickStart:
                    Log.Debug("Head Pick Start");
                    Step.RunStep++;
                    break;
                case HeadPickStep.Tray_PickPosition_MoveWait:
                    if (Flags.Tray_PickPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadPickStep.Transfer_PickPosition_MoveWait:
                    if (Flags.Transfer_PickPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadPickStep.ZAxis_PickDown1_Move:
                    ZAxis_PickDown1_Move();
                    Step.RunStep++;
                    break;
                case HeadPickStep.ZAxis_PickDown1_MoveWait:
                    if (ZAxis_PickDown1_Move_Done())
                    {
                        Log.Debug("ZAxis move Pick_Down #1 position done");
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
                            Log.Warn($"{ZAxis} pick down 1_position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadPickStep.ZAxis_PickDown2_Move:
                    ZAxis_PickDown2_Move();
                    Step.RunStep++;
                    break;
                case HeadPickStep.ZAxis_PickDown2_MoveWait:
                    if (ZAxis_PickDown2_Move_Done())
                    {
                        Log.Debug("ZAxis move Pick_Down #2 position done");
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
                            Log.Warn($"{ZAxis} pick down 2_position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadPickStep.Head_VacuumOn:
                    Head_VacuumOn();
                    Step.RunStep++;
                    break;
                case HeadPickStep.Head_VacuumDelay:
                    if (PTimer.StepLeadTime < CDef.CommonRecipe.Vacuum_Delay * 1000)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        Log.Debug($"Head vacuum Delay for {PTimer.StepLeadTime / 1000}s");
                        Step.RunStep++;
                    }
                    break;
                case HeadPickStep.ZAxis_Ready_Move:
                    Head_ZAxis_ReadyMove();
                    Step.RunStep++;
                    break;
                case HeadPickStep.ZAxis_Ready_MoveWait:
                    if (Head_ZAxis_ReadyMove_Done())
                    {
                        Log.Debug($"{ZAxis} move ready position done");
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
                            Log.Warn($"{ZAxis} ready position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadPickStep.Pick_ResultUpdate:
                    Log.Debug("Pick Done");
                    CDef.RootProcess.CurrentLoadTray.SetSetOfCell(TopUI.Define.ECellStatus.OK, CDef.RootProcess.CurrentLoadTray.WorkStartIndex);

                    Datas.WorkData.TaktTime.Pick = 1.0 * (PTimer.Now - CDef.RootProcess.TransferProcess.PTimer.TactTimeCounter[0]) / 1000;

                    Step.RunStep++;
                    break;
                case HeadPickStep.PickEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnderVision;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Pick)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public enum HeadUnderVisionStep
        {
            UnderVisionStart,
            Transfer_UnderVisionPosition_MoveWait,
            ZAxis_UnderVisionDown_Move,
            ZAxis_UnderVisionDown_MoveWait,
            UnderVision_InspectRequest,
            UnderVision_InspectWait,
            ZAxis_Ready_Move,
            ZAxis_Ready_MoveWait,
            UnderVisionEnd,
        }
        public void Running_UnderVision()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                CDef.GlobalRecipe.SkipUnderVision)
            {
                this.RunMode = ERunMode.Manual_UnloadVision;
                return;
            }

            switch ((HeadUnderVisionStep)Step.RunStep)
            {
                case HeadUnderVisionStep.UnderVisionStart:
                    Log.Debug("Head UnderVision Start");
                    Step.RunStep++;
                    break;
                case HeadUnderVisionStep.Transfer_UnderVisionPosition_MoveWait:
                    if (Flags.Transfer_UnderVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadUnderVisionStep.ZAxis_UnderVisionDown_Move:
                    ZAxis_UnderVision_Down_Move();
                    Step.RunStep++;
                    break;
                case HeadUnderVisionStep.ZAxis_UnderVisionDown_MoveWait:
                    if (ZAxis_UnderVisionDown_Move_Done())
                    {
                        Log.Debug($"{ZAxis} move Under Vision position done");
                        Flag_Head_UnderVisionPosition_MoveDone = true;
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
                            Log.Warn($"{ZAxis} UnderVision down position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadUnderVisionStep.UnderVision_InspectRequest:
                    Flags.UnderVision_InspectWork_Request = true;
                    Step.RunStep++;
                    break;
                case HeadUnderVisionStep.UnderVision_InspectWait:
                    if (Flags.UnderVision_InspectDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadUnderVisionStep.ZAxis_Ready_Move:
                    Head_ZAxis_ReadyMove();
                    Step.RunStep++;
                    break;
                case HeadUnderVisionStep.ZAxis_Ready_MoveWait:
                    if (Head_ZAxis_ReadyMove_Done())
                    {
                        Log.Debug("ZAxis move ready position done");
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
                            Log.Warn($"{ZAxis} ready position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadUnderVisionStep.UnderVisionEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnloadVision;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UnderVision)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public enum HeadUnloadVisionStep
        {
            UnloadVisionStart,
            Tray_UnloadVisionPosition_MoveWait,
            Transfer_UnloadVisionPosition_MoveWait,
            ZAxis_UnloadVisionDown_Move,
            ZAxis_UnloadVisionDown_MoveWait,
            UpperVision_InspectWait,
            ZAxis_Ready_Move,
            ZAxis_Ready_MoveWait,
            UnloadVisionEnd,
        }
        public void Running_UnloadVision()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                CDef.GlobalRecipe.SkipUnloadVision)
            {
                this.RunMode = ERunMode.Manual_Place;
                return;
            }

            switch ((HeadUnloadVisionStep)Step.RunStep)
            {
                case HeadUnloadVisionStep.UnloadVisionStart:
                    Log.Debug("Head Unload Vision Start");
                    Step.RunStep++;
                    break;
                case HeadUnloadVisionStep.Tray_UnloadVisionPosition_MoveWait:
                    if (Flags.Tray_UnloadVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadUnloadVisionStep.Transfer_UnloadVisionPosition_MoveWait:
                    if (Flags.Transfer_UnloadVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadUnloadVisionStep.ZAxis_UnloadVisionDown_Move:
                    ZAxis_UnloadVision_Down_Move();
                    Step.RunStep++;
                    break;
                case HeadUnloadVisionStep.ZAxis_UnloadVisionDown_MoveWait:
                    if (ZAxis_UnloadVisionDown_Move_Done())
                    {
                        Log.Debug("ZAxis move Unload Vision position done");
                        Flag_Head_UnloadVisionPosition_MoveDone = true;
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
                            Log.Warn($"{ZAxis} UnloadVision down position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadUnloadVisionStep.UpperVision_InspectWait:
                    if (Flags.Upper_UnloadVision_InspectDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadUnloadVisionStep.ZAxis_Ready_Move:
                    Head_ZAxis_ReadyMove();
                    Step.RunStep++;
                    break;
                case HeadUnloadVisionStep.ZAxis_Ready_MoveWait:
                    if (Head_ZAxis_ReadyMove_Done())
                    {
                        Log.Debug("ZAxis move ready position done");
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
                            Log.Warn($"{ZAxis} ready position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadUnloadVisionStep.UnloadVisionEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Place;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UnloadVision)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public enum HeadPlaceStep
        {
            PlaceStart,
            Tray_PlaceWait,
            Transfer_PlaceWait,
            ZAxis_PlaceDown1_Move,
            ZAxis_PlaceDown1_MoveWait,
            ZAxis_PlaceDown2_Move,
            ZAxis_PlaceDown2_MoveWait,
            Head_PurgeOn,
            Head_PurgeDelay,
            ZAxis_Ready_Move,
            ZAxis_Ready_MoveWait,
            WorkDataUpdate,
            PlaceEnd,
        }
        public void Running_Place()
        {
            switch ((HeadPlaceStep)Step.RunStep)
            {
                case HeadPlaceStep.PlaceStart:
                    Log.Debug("Head Place Start");
                    Step.RunStep++;
                    break;
                case HeadPlaceStep.Tray_PlaceWait:
                    if (Flags.Tray_PlacePosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadPlaceStep.Transfer_PlaceWait:
                    if (Flags.Transfer_PlacePosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case HeadPlaceStep.ZAxis_PlaceDown1_Move:
                    ZAxis_PlaceDown1_Move();
                    Step.RunStep++;
                    break;
                case HeadPlaceStep.ZAxis_PlaceDown1_MoveWait:
                    if (ZAxis_PlaceDown1_Move_Done())
                    {
                        Log.Debug("ZAxis move Place_Down #1 position done");
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
                            Log.Warn($"{ZAxis} place #1 position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadPlaceStep.ZAxis_PlaceDown2_Move:
                    ZAxis_PlaceDown2_Move();
                    Step.RunStep++;
                    break;
                case HeadPlaceStep.ZAxis_PlaceDown2_MoveWait:
                    if (ZAxis_PlaceDown2_Move_Done())
                    {
                        Log.Debug("ZAxis move Place_Down #2 position done");
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
                            Log.Warn($"{ZAxis} place #2 position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadPlaceStep.Head_PurgeOn:
                    Head_PurgeOn();
                    Step.RunStep++;
                    break;
                case HeadPlaceStep.Head_PurgeDelay:
                    if (PTimer.StepLeadTime < CDef.CommonRecipe.Purge_Delay * 1000)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        Log.Debug($"Head purge delay for {PTimer.StepLeadTime / 1000}s");
                        Step.RunStep++;
                    }
                    break;
                case HeadPlaceStep.ZAxis_Ready_Move:
                    Head_ZAxis_ReadyMove();
                    Step.RunStep++;
                    break;
                case HeadPlaceStep.ZAxis_Ready_MoveWait:
                    if (Head_ZAxis_ReadyMove_Done())
                    {
                        Log.Debug("Head ZAxis move ready position done");
                        Log.Debug("Place done");
                        Log.Info($"Pick index #{CDef.RootProcess.CurrentLoadTray.WorkStartIndex} &" +
                                 $"Place index #{CDef.RootProcess.CurrentUnloadTray.WorkStartIndex} DONE!");
                        CDef.RootProcess.CurrentUnloadTray.SetSetOfCell(TopUI.Define.ECellStatus.OK, CDef.RootProcess.CurrentUnloadTray.WorkStartIndex);
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
                            Log.Warn($"{ZAxis} ready position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case HeadPlaceStep.WorkDataUpdate:
                    Datas.WorkData.CountData.Total += 1;
                    Datas.WorkData.CountData.OK += (uint)(Datas.Data_UnloadingInspect_Result.Judge == EVisionJudge.OK ? 1 : 0);
                    Datas.WorkData.CountData.LoadVisionNG += (uint)(Datas.Data_LoadingInspect_Result.Judge == EVisionJudge.NG ? 1 : 0);
                    Datas.WorkData.CountData.UnderVisionNG += (uint)(Datas.Data_UnderInspect_Result.Judge == EVisionJudge.NG ? 1 : 0);
                    Datas.WorkData.CountData.UnloadVisionNG += (uint)(Datas.Data_UnloadingInspect_Result.Judge == EVisionJudge.NG ? 1 : 0);

                    Datas.WorkData.TaktTime.SingleCycle = 1.0 * (PTimer.Now - CDef.RootProcess.TransferProcess.PTimer.TactTimeCounter[0]) / 1000;
                    //Tak Time Total 30EA
                    double _total = 0;
                    Datas.WorkData.TaktTime.Last30EATaktTime.AddTT(Datas.WorkData.TaktTime.SingleCycle, ref _total);
                    Datas.WorkData.TaktTime.Total = _total;

                    Datas.WorkData.TaktTime.Place = 1.0 * (PTimer.Now - CDef.RootProcess.TransferProcess.PTimer.TactTimeCounter[2]) / 1000;
                    Datas.WorkData.TaktTime.LoadVision.ProcessTime = Datas.Data_LoadingInspect_Result.Cost / 1000;
                    Datas.WorkData.TaktTime.BotVision.ProcessTime = Datas.Data_UnderInspect_Result.Cost / 1000;
                    Datas.WorkData.TaktTime.UnloadVision.ProcessTime = Datas.Data_UnloadingInspect_Result.Cost / 1000;
                    Step.RunStep++;
                    break;
                case HeadPlaceStep.PlaceEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_LoadVision;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Place)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public void Running_TrayChange()
        {

        }
        #endregion
        #region Moving functions
        #region READY
        private void Head_ZAxis_ReadyMove()
        {
            ZAxis.MoveAbs(CDef.HeadRecipe.ZAxisReady_Position);
        }

        private bool Head_ZAxis_ReadyMove_Done()
        {
            return ZAxis.IsOnPosition(CDef.HeadRecipe.ZAxisReady_Position);
        }
        #endregion

        public void ZAxis_LoadVisionDown_Move()
        {
            ZAxis.MoveAbs(CDef.HeadRecipe.ZAxisLoadVision_Position);
        }

        public bool ZAxis_LoadVisionDownMove_Done()
        {
            SimSleep(300);
            return ZAxis.IsOnPosition(CDef.HeadRecipe.ZAxisLoadVision_Position);
        }

        public void ZAxis_PickDown1_Move()
        {
            ZAxis.MoveAbs(CDef.HeadRecipe.ZAxisPick_Position - 2);
        }

        public bool ZAxis_PickDown1_Move_Done()
        {
            SimSleep(300);
            return ZAxis.IsOnPosition(CDef.HeadRecipe.ZAxisPick_Position - 2);
        }

        public void ZAxis_PickDown2_Move()
        {
            ZAxis.MoveAbs(CDef.HeadRecipe.ZAxisPick_Position, CDef.HeadRecipe.ZAxisPick_Speed);
        }

        public bool ZAxis_PickDown2_Move_Done()
        {
            SimSleep(300);
            return ZAxis.IsOnPosition(CDef.HeadRecipe.ZAxisPick_Position);
        }

        public void ZAxis_UnderVision_Down_Move()
        {
            ZAxis.MoveAbs(CDef.HeadRecipe.ZAxisUnderVision_Position);
        }

        public bool ZAxis_UnderVisionDown_Move_Done()
        {
            SimSleep(300);
            return ZAxis.IsOnPosition(CDef.HeadRecipe.ZAxisUnderVision_Position);
        }

        public void ZAxis_UnloadVision_Down_Move()
        {
            ZAxis.MoveAbs(CDef.HeadRecipe.ZAxisUnloadVision_Position);
        }

        public bool ZAxis_UnloadVisionDown_Move_Done()
        {
            SimSleep(300);
            return ZAxis.IsOnPosition(CDef.HeadRecipe.ZAxisUnloadVision_Position);
        }

        public void ZAxis_PlaceDown1_Move()
        {
            ZAxis.MoveAbs(CDef.HeadRecipe.ZAxisPlace_Position - 2);
        }

        public bool ZAxis_PlaceDown1_Move_Done()
        {
            SimSleep(300);
            return ZAxis.IsOnPosition(CDef.HeadRecipe.ZAxisPlace_Position - 2);
        }

        public void ZAxis_PlaceDown2_Move()
        {
            ZAxis.MoveAbs(CDef.HeadRecipe.ZAxisPlace_Position, CDef.HeadRecipe.ZAxisPlace_Speed);
        }

        public bool ZAxis_PlaceDown2_Move_Done()
        {
            SimSleep(300);
            return ZAxis.IsOnPosition(CDef.HeadRecipe.ZAxisPlace_Position);
        }
        #endregion

        public void Head_VacuumOn()
        {

        }

        public void Head_PurgeOn()
        {

        }
    }
}
