using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Processing;
using TopMotion;
using VCM_FullAssy.Define;

namespace VCM_FullAssy.Processing
{
    public enum TransferHomeStep
    {
        HomeStart,
        ZAxis_HomeWait,
        XAxis_HomeMove,
        XAxis_HomeWait,
        HomeEnd,
    }

    public class CTransferProcess : ProcessingBase
    {
        #region Properties
        public MotionPlusE XAxis
        {
            get { return (MotionPlusE)CDef.AllAxis.XAxis; }
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

        public bool Flag_Transfer_LoadVisionPosition_MoveDone { get; set; }

        public bool Flag_Transfer_PickPosition_MoveDone { get; set; }

        public bool Flag_Transfer_UnderVisionPosition_MoveDone { get; set; }

        public bool Flag_Transfer_UnloadVisionPosition_MoveDone { get; set; }

        public bool Flag_Transfer_PlacePosition_MoveDone { get; set; }
        #endregion

        public CTransferProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }

        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch ((TransferHomeStep)Step.HomeStep)
            {
                case TransferHomeStep.HomeStart:
                    Log.Debug("Transfer home start");
                    Step.HomeStep++;
                    break;
                case TransferHomeStep.ZAxis_HomeWait:
                    if (HomeStatus.ZDone == true)
                    {
                        Step.HomeStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferHomeStep.XAxis_HomeMove:
                    XAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case TransferHomeStep.XAxis_HomeWait:
                    if (XAxis.Status.IsHomeDone == true)
                    {
                        Log.Debug("X Axis origin done");
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
                            Log.Warn("X Axis home search timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TransferHomeStep.HomeEnd:
                    Log.Debug("Transfer home done");
                    ProcessingStatus = EProcessingStatus.OriginDone;
                    Step.HomeStep++;
                    break;
                default:
                    Sleep(20);
                    break;
            }

            return nRtn;
        }

        public enum TransferToRunStep
        {
            ToRunStart,
            ZAxis_ReadyWait,
            ToRunDecision,
            ToRunEnd,
        }

        public override PRtnCode ProcessToRun()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch ((TransferToRunStep)Step.ToRunStep)
            {
                case TransferToRunStep.ToRunStart:
                    Log.Debug("ToRun Start");
                    Flag_Transfer_LoadVisionPosition_MoveDone = false;
                    Flag_Transfer_PickPosition_MoveDone = false;
                    Flag_Transfer_PlacePosition_MoveDone = false;
                    Flag_Transfer_UnderVisionPosition_MoveDone = false;
                    Flag_Transfer_UnloadVisionPosition_MoveDone = false;
                    Step.ToRunStep++;
                    break;
                case TransferToRunStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.ToRunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferToRunStep.ToRunDecision:
                    this.RunMode = CDef.RootProcess.RunMode;
                    Step.ToRunStep++;
                    break;
                case TransferToRunStep.ToRunEnd:
                    // Total, Pick, Place takt time reset
                    PTimer.TactTimeCounter = new System.Collections.ObjectModel.ObservableCollection<int> { 0, 0, 0 };

                    Log.Debug("ToRun End");
                    ProcessingStatus = EProcessingStatus.ToRunDone;
                    Step.ToRunStep++;
                    break;
                default:
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
                    Sleep(20);
                    break;
            }

            return nRtn;
        }

        public enum TransferLoadVisionStep
        {
            LoadVisionStart,
            ZAxis_ReadyWait,
            Wait_TrayChangeOrPickPlace,
            LoadVision_PositionMove,
            LoadVision_PositionMove_Wait,
            LoadVision_ProcessWait,
            LoadVisionEnd,
        }

        public enum TransferPickStep
        {
            PickStart,
            ZAxis_ReadyWait,
            Pick_PositionMove,
            Pick_PositionMove_Wait,
            Pick_ProcessWait,
            PickEnd,
        }

        public enum TransferUnderVisionStep
        {
            UnderVisionStart,
            ZAxis_ReadyWait,
            UnderVision_PositionMove,
            UnderVision_PositionMove_Wait,
            UnderVision_ProcessWait,
            UnderVisionEnd,
        }

        public enum TransferUnloadVisionStep
        {
            UnloadVisionStart,
            ZAxis_ReadyWait,
            UnloadVision_PositionMove,
            UnloadVision_PositionMove_Wait,
            UnloadVision_ProcessWait,
            UnloadVisionEnd,
        }

        public enum TransferPlaceStep
        {
            PlaceStart,
            ZAxis_ReadyWait,
            Place_PositionMove,
            Place_PositionMove_Wait,
            Place_ProcessWait,
            PlaceEnd,
        }

        private void Running_LoadVision()
        {
            switch ((TransferLoadVisionStep)Step.RunStep)
            {
                case TransferLoadVisionStep.LoadVisionStart:
                    Log.Debug("Transfer Load Vision start");
                    PTimer.TactTimeCounter[0] = PTimer.Now;
                    Step.RunStep++;
                    break;
                case TransferLoadVisionStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferLoadVisionStep.Wait_TrayChangeOrPickPlace:
                    if (CDef.RootProcess.CurrentLoadTray == null || CDef.RootProcess.CurrentUnloadTray == null)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                            CDef.GlobalRecipe.SkipLoadVision)
                        {
                            PTimer.TactTimeCounter[0] = PTimer.Now;

                            this.RunMode = ERunMode.Manual_Pick;
                            return;
                        }
                        else
                        {
                            Step.RunStep++;
                        }
                    }
                    break;
                case TransferLoadVisionStep.LoadVision_PositionMove:
                    XAxis_LoadVisionPosition_Move();
                    Step.RunStep++;
                    break;
                case TransferLoadVisionStep.LoadVision_PositionMove_Wait:
                    if (XAxis_LoadVisionPosition_Move_Wait())
                    {
                        Log.Debug("XAxis move Load Vision Position Done");
                        Flag_Transfer_LoadVisionPosition_MoveDone = true;
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
                            Log.Warn($"{XAxis} LoadVision position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TransferLoadVisionStep.LoadVision_ProcessWait:
                    if (Flags.Head_LoadVision_ProcessDone)
                    {
                        Flag_Transfer_LoadVisionPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferLoadVisionStep.LoadVisionEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Pick;
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

        private void Running_Pick()
        {
            switch ((TransferPickStep)Step.RunStep)
            {
                case TransferPickStep.PickStart:
                    Log.Debug("Transfer Pick start");
                    Step.RunStep++;
                    break;
                case TransferPickStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferPickStep.Pick_PositionMove:
                    XAxis_PickPosition_Move();
                    Step.RunStep++;
                    break;
                case TransferPickStep.Pick_PositionMove_Wait:
                    if (XAxis_PickPosition_Move_Wait())
                    {
                        Log.Debug($"{XAxis} move Pick_Position Done");
                        Flag_Transfer_PickPosition_MoveDone = true;
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
                            Log.Warn($"{XAxis} Pick position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TransferPickStep.Pick_ProcessWait:
                    if (Flags.Head_PickDone)
                    {
                        Flag_Transfer_PickPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferPickStep.PickEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnderVision;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Pick)
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
                this.RunMode = ERunMode.Manual_UnloadVision;
                return;
            }

            switch ((TransferUnderVisionStep)Step.RunStep)
            {
                case TransferUnderVisionStep.UnderVisionStart:
                    Log.Debug("Transfer Under Vision start");
                    Step.RunStep++;
                    break;
                case TransferUnderVisionStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferUnderVisionStep.UnderVision_PositionMove:
                    XAxis_UnderVisionPosition_Move();
                    Step.RunStep++;
                    break;
                case TransferUnderVisionStep.UnderVision_PositionMove_Wait:
                    if (XAxis_UnderVisionPosition_Move_Wait())
                    {
                        Log.Debug($"{XAxis} move Under Vision Position Done");
                        Flags.UnderVision_InspectDone = false;
                        Flag_Transfer_UnderVisionPosition_MoveDone = true;
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
                            Log.Warn($"{XAxis} UnderVision position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TransferUnderVisionStep.UnderVision_ProcessWait:
                    if (Flags.UnderVision_InspectDone)
                    {
                        Flag_Transfer_UnderVisionPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferUnderVisionStep.UnderVisionEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnloadVision;
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

        private void Running_UnloadVision()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                CDef.GlobalRecipe.SkipUnloadVision)
            {
                PTimer.TactTimeCounter[2] = PTimer.Now;

                this.RunMode = ERunMode.Manual_Place;
                return;
            }

            switch ((TransferUnloadVisionStep)Step.RunStep)
            {
                case TransferUnloadVisionStep.UnloadVisionStart:
                    Log.Debug("Transfer Unload Vision start");
                    PTimer.TactTimeCounter[2] = PTimer.Now;
                    Step.RunStep++;
                    break;
                case TransferUnloadVisionStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferUnloadVisionStep.UnloadVision_PositionMove:
                    XAxis_UnloadVisionPosition_Move();
                    Step.RunStep++;
                    break;
                case TransferUnloadVisionStep.UnloadVision_PositionMove_Wait:
                    if (XAxis_UnloadVisionPosition_Move_Wait())
                    {
                        Log.Debug($"{XAxis} move Unload Vision Position Done");
                        Flag_Transfer_UnloadVisionPosition_MoveDone = true;
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
                            Log.Warn($"{XAxis} UnloadVision position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TransferUnloadVisionStep.UnloadVision_ProcessWait:
                    if (Flags.Head_UnloadVision_ProcessDone)
                    {
                        Flag_Transfer_UnloadVisionPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferUnloadVisionStep.UnloadVisionEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Place;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UnloadVision)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Running_Place()
        {
            switch ((TransferPlaceStep)Step.RunStep)
            {
                case TransferPlaceStep.PlaceStart:
                    Log.Debug("Transfer Place start");
                    Step.RunStep++;
                    break;
                case TransferPlaceStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferPlaceStep.Place_PositionMove:
                    XAxis_PlacePosition_Move();
                    Step.RunStep++;
                    break;
                case TransferPlaceStep.Place_PositionMove_Wait:
                    if (XAxis_PlacePosition_Move_Wait())
                    {
                        Log.Debug($"{XAxis} move Place Position Done");
                        Flag_Transfer_PlacePosition_MoveDone = true;
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
                            Log.Warn($"{XAxis} Place position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TransferPlaceStep.Place_ProcessWait:
                    if (Flags.Head_PlaceDone)
                    {
                        Flag_Transfer_PlacePosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TransferPlaceStep.PlaceEnd:
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

        #region Move Functions
        //XAxis Load Vision Position
        private double XAxis_LoadVisionPosition
        {
            get
            {
                double position = 0;
                // offsetX = offsetX of "Load Tray X pitch"
                double offsetX = CDef.RootProcess.CurrentLoadTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;
                if (CDef.RootProcess.LeftTrayProcess.InWorking)
                {
                    if (CDef.RootProcess.LeftTrayProcess.CurrentLoadTray == CDef.RootProcess.LeftTrayProcess.FrontLoadTray)
                    {
                        position = CDef.TransferRecipe.LeftFrontLoadTrayWork_FirstPosition - offsetX;
                    }
                    else
                    {
                        position = CDef.TransferRecipe.LeftRearLoadTrayWork_FirstPosition - offsetX;
                    }
                }
                else
                {
                    if (CDef.RootProcess.RightTrayProcess.CurrentLoadTray == CDef.RootProcess.RightTrayProcess.FrontLoadTray)
                    {
                        position = CDef.TransferRecipe.RightFrontLoadTrayWork_FirstPosition - offsetX;
                    }
                    else
                    {
                        position = CDef.TransferRecipe.RightRearLoadTrayWork_FirstPosition - offsetX;
                    }
                }
                return position;
            }
        }
        private void XAxis_LoadVisionPosition_Move()
        {
            XAxis.MoveAbs(XAxis_LoadVisionPosition);
        }
        private bool XAxis_LoadVisionPosition_Move_Wait()
        {
            SimSleep(200);
            return XAxis.IsOnPosition(XAxis_LoadVisionPosition);
        }

        //XAxis Pick Position
        private double XAxis_Pick_Position
        {
            get
            {
                double position;
                // offsetX = offsetX of "Load Tray X pitch"
                double offsetX = CDef.RootProcess.CurrentLoadTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;
                double offsetVisionX = 0;
                if (CDef.RootProcess.LeftTrayProcess.InWorking)
                {
                    if (CDef.RootProcess.LeftTrayProcess.CurrentLoadTray == CDef.RootProcess.LeftTrayProcess.FrontLoadTray)
                    {
                        position = CDef.TransferRecipe.LeftFrontLoadTrayWork_FirstPosition + CDef.CommonRecipe.VisionToPicker_XOffset - offsetX + offsetVisionX;
                    }
                    else
                    {
                        position = CDef.TransferRecipe.LeftRearLoadTrayWork_FirstPosition + CDef.CommonRecipe.VisionToPicker_XOffset - offsetX + offsetVisionX;
                    }
                }
                else
                {
                    if (CDef.RootProcess.RightTrayProcess.CurrentLoadTray == CDef.RootProcess.RightTrayProcess.FrontLoadTray)
                    {
                        position = CDef.TransferRecipe.RightFrontLoadTrayWork_FirstPosition + CDef.CommonRecipe.VisionToPicker_XOffset - offsetX + offsetVisionX;
                    }
                    else
                    {
                        position = CDef.TransferRecipe.RightRearLoadTrayWork_FirstPosition + CDef.CommonRecipe.VisionToPicker_XOffset - offsetX + offsetVisionX;
                    }
                }
                return position;
            }
        }
        private void XAxis_PickPosition_Move()
        {
            XAxis.MoveAbs(XAxis_Pick_Position);
        }
        private bool XAxis_PickPosition_Move_Wait()
        {
            SimSleep(200);
            return XAxis.IsOnPosition(XAxis_Pick_Position);
        }

        //XAxis Under Vision Position
        private bool XAxis_UnderVisionPosition_Move_Wait()
        {
            SimSleep(200);
            return XAxis.IsOnPosition(CDef.TransferRecipe.UnderVisionWork_Position);
        }
        private void XAxis_UnderVisionPosition_Move()
        {
            XAxis.MoveAbs(CDef.TransferRecipe.UnderVisionWork_Position);
        }

        //XAxis Unload Vision Position
        private double XAxis_UnloadVisionPosition
        {
            get
            {
                double position;
                // offsetX = offsetX of "Unload Tray X pitch"
                double offsetX = CDef.RootProcess.CurrentLoadTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;
                if (CDef.RootProcess.LeftTrayProcess.InWorking)
                {
                    if (CDef.RootProcess.LeftTrayProcess.CurrentUnloadTray == CDef.RootProcess.LeftTrayProcess.FrontUnloadTray)
                    {
                        position = CDef.TransferRecipe.LeftFrontUnloadTrayWork_FirstPosition - offsetX;
                    }
                    else
                    {
                        position = CDef.TransferRecipe.LeftRearUnloadTrayWork_FirstPosition - offsetX;
                    }
                }
                else
                {
                    if (CDef.RootProcess.RightTrayProcess.CurrentUnloadTray == CDef.RootProcess.RightTrayProcess.FrontUnloadTray)
                    {
                        position = CDef.TransferRecipe.RightFrontUnloadTrayWork_FirstPosition - offsetX;
                    }
                    else
                    {
                        position = CDef.TransferRecipe.RightRearUnloadTrayWork_FirstPosition - offsetX;
                    }
                }
                return position;
            }
        }
        private void XAxis_UnloadVisionPosition_Move()
        {
            XAxis.MoveAbs(XAxis_UnloadVisionPosition);
        }
        private bool XAxis_UnloadVisionPosition_Move_Wait()
        {
            SimSleep(200);
            return XAxis.IsOnPosition(XAxis_UnloadVisionPosition);
        }

        //XAxis Place Position
        private double XAxis_PlacePosition
        {
            get
            {
                double position;
                // offsetX = offsetX of "Unload Tray X pitch"
                double offsetX = CDef.RootProcess.CurrentLoadTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;
                double offsetVisionX = 0;
                if (CDef.RootProcess.LeftTrayProcess.InWorking)
                {
                    if (CDef.RootProcess.LeftTrayProcess.CurrentUnloadTray == CDef.RootProcess.LeftTrayProcess.FrontUnloadTray)
                    {
                        position = CDef.TransferRecipe.LeftFrontUnloadTrayWork_FirstPosition + CDef.CommonRecipe.VisionToPicker_XOffset - offsetX + offsetVisionX;
                    }
                    else
                    {
                        position = CDef.TransferRecipe.LeftRearUnloadTrayWork_FirstPosition + CDef.CommonRecipe.VisionToPicker_XOffset - offsetX + offsetVisionX;
                    }
                }
                else
                {
                    if (CDef.RootProcess.RightTrayProcess.CurrentUnloadTray == CDef.RootProcess.RightTrayProcess.FrontUnloadTray)
                    {
                        position = CDef.TransferRecipe.RightFrontUnloadTrayWork_FirstPosition + CDef.CommonRecipe.VisionToPicker_XOffset - offsetX + offsetVisionX;
                    }
                    else
                    {
                        position = CDef.TransferRecipe.RightRearUnloadTrayWork_FirstPosition + CDef.CommonRecipe.VisionToPicker_XOffset - offsetX + offsetVisionX;
                    }
                }
                return position;
            }
        }
        private void XAxis_PlacePosition_Move()
        {
            XAxis.MoveAbs(XAxis_PlacePosition);
        }
        private bool XAxis_PlacePosition_Move_Wait()
        {
            SimSleep(200);
            return XAxis.IsOnPosition(XAxis_PlacePosition);
        }
        #endregion

    }
}