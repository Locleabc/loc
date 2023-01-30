using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;
using TopCom.Processing;
using TopMotion;
using TopUI.Models;
using VCM_FullAssy.Define;

namespace VCM_FullAssy.Processing
{
    public class CTrayProcess : ProcessingBase
    {
        #region EventHandler
        public EventHandler InWorkingChanged;
        #endregion

        #region Properties
        private bool _InWorking;

        public bool InWorking
        {
            get { return _InWorking; }
            set
            {
                if (_InWorking == value) return;

                _InWorking = value;
                InWorkingChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
            }
        }


        public string FriendlyName { get; set; }

        public MotionPlusE YAxis
        {
            get
            {
                return (MotionPlusE)(ProcessName == "LTray" ? CDef.AllAxis.Y1Axis : CDef.AllAxis.Y2Axis);
            }
        }

        // Working with front tray first, then rear tray
        public ITrayModel FrontLoadTray
        {
            get
            {
                return ProcessName == "LTray" ? CTray.LoadTray1 : CTray.LoadTray3;
            }
        }
        public ITrayModel RearLoadTray
        {
            get
            {
                return ProcessName == "LTray" ? CTray.LoadTray2 : CTray.LoadTray4;
            }
        }

        public ITrayModel CurrentLoadTray
        {
            get
            {
                if (FrontLoadTray != null && FrontLoadTray.WorkIndexInRage)
                {
                    return FrontLoadTray;
                }
                else
                {
                    return RearLoadTray != null && RearLoadTray.WorkIndexInRage ? RearLoadTray : null;
                }
            }
        }

        public ITrayModel FrontUnloadTray
        {
            get
            {
                return ProcessName == "LTray" ? CTray.UnloadTray1 : CTray.UnloadTray3;
            }
        }
        public ITrayModel RearUnloadTray
        {
            get
            {
                return ProcessName == "LTray" ? CTray.UnloadTray2 : CTray.UnloadTray4;
            }
        }

        public ITrayModel CurrentUnloadTray
        {
            get
            {
                if (FrontUnloadTray != null && FrontUnloadTray.WorkIndexInRage)
                {
                    return FrontUnloadTray;
                }
                else if (RearUnloadTray != null && RearUnloadTray.WorkIndexInRage)
                {
                    return RearUnloadTray;
                }
                else
                {
                    return null;
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

        public bool Flag_Tray_LoadVisionPosition_MoveDone { get; set; }

        public bool Flag_Tray_PickPosition_MoveDone { get; set; }

        public bool Flag_Tray_UnloadVisionPosition_MoveDone { get; set; }

        public bool Flag_Tray_PlacePosition_MoveDone { get; set; }
        #endregion

        public CTrayProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }

        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch ((TrayHomeStep)Step.HomeStep)
            {
                case TrayHomeStep.HomeStart:
                    Log.Debug("Home start");
                    Step.HomeStep++;
                    break;
                case TrayHomeStep.ZAxis_HomeWait:
                    if (HomeStatus.ZDone == true)
                    {
                        Step.HomeStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayHomeStep.YAxis_HomeMove:
                    YAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case TrayHomeStep.YAxis_HomeWait:
                    if (YAxis.Status.IsHomeDone == true)
                    {
                        Log.Debug($"{YAxis} origin done");
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
                            Log.Warn($"{YAxis} home search timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayHomeStep.HomeEnd:
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

            switch ((TrayToRunStep)Step.ToRunStep)
            {
                case TrayToRunStep.ToRunStart:
                    Log.Debug("ToRun Start");
                    Flag_Tray_LoadVisionPosition_MoveDone = false;
                    Flag_Tray_PickPosition_MoveDone = false;
                    Flag_Tray_PlacePosition_MoveDone = false;
                    Flag_Tray_UnloadVisionPosition_MoveDone = false;
                    Step.ToRunStep++;
                    break;
                case TrayToRunStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.ToRunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayToRunStep.TrayStatus_Check:
                    if (CurrentLoadTray == null || CurrentUnloadTray == null)
                    {
                        this.RunMode = ERunMode.Manual_TrayChange;
                        Step.ToRunStep = (int)TrayToRunStep.ToRunEnd;
                    }
                    else
                    {
                        Step.ToRunStep++;
                    }
                    break;
                case TrayToRunStep.YAxis_LoadVisionMove:
                    YAxis_LoadVisionMove();
                    Step.ToRunStep++;
                    break;
                case TrayToRunStep.YAxis_LoadVisionWait:
                    if (YAxis_LoadVisionMove_Done())
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
                            Log.Warn($"{YAxis} load vision moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayToRunStep.ToRunDecision:
                    if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                    {
                        if (InWorking)
                        {
                            this.RunMode = ERunMode.Manual_TrayChange;
                        }
                        else
                        {
                            this.RunMode = ERunMode.Stop;
                        }
                    }
                    else
                    {
                        this.RunMode = CDef.RootProcess.RunMode;
                    }
                    Step.ToRunStep++;
                    break;
                case TrayToRunStep.ToRunEnd:
                    // Total, Pick, Place takt time reset
                    PTimer.TactTimeCounter = new System.Collections.ObjectModel.ObservableCollection<int> { 0, 0, 0 };

                    Log.Debug("ToRun End");
                    Step.ToRunStep++;
                    ProcessingStatus = EProcessingStatus.ToRunDone;
                    break;
                default:
                    break;
            }
            return nRtn;
        }

        public override PRtnCode ProcessRun()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            if (InWorking == false && RunMode != ERunMode.Manual_TrayChange)
            {
                Sleep(100);
                return nRtn;
            }

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
        private void Running_LoadVision()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                CDef.GlobalRecipe.SkipLoadVision)
            {
                this.RunMode = ERunMode.Manual_Pick;
                return;
            }

            switch ((TrayLoadVisionStep)Step.RunStep)
            {
                case TrayLoadVisionStep.LoadVisionStart:
                    Log.Debug("Load vision start");
                    Step.RunStep++;
                    break;
                case TrayLoadVisionStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayLoadVisionStep.Wait_TrayChangeOrPickPlace:
                    if (CurrentLoadTray == null || CurrentUnloadTray == null)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case TrayLoadVisionStep.YAxis_LoadVisionMove:
                    YAxis_LoadVisionMove();
                    Step.RunStep++;
                    break;
                case TrayLoadVisionStep.YAxis_LoadVisionWait:
                    if (YAxis_LoadVisionMove_Done())
                    {
                        Log.Debug($"{YAxis} move Load Vision Position Done");
                        Flag_Tray_LoadVisionPosition_MoveDone = true;
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
                            Log.Warn($"{YAxis} load vision moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayLoadVisionStep.LoadVision_ProcessWait:
                    if (Flags.Head_LoadVision_ProcessDone)
                    {
                        Flag_Tray_LoadVisionPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayLoadVisionStep.LoadVisionEnd:
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
            switch ((TrayPickStep)Step.RunStep)
            {
                case TrayPickStep.TrayPickStart:
                    Log.Debug("Pick Position start");
                    Step.RunStep++;
                    break;
                case TrayPickStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayPickStep.YAxis_PickMove:
                    YAxis_PickMove();
                    Step.RunStep++;
                    break;
                case TrayPickStep.YAxis_PickMoveWait:
                    if (YAxis_PickMove_Done())
                    {
                        Log.Debug($"{YAxis} move Pick_Position Done");
                        Flag_Tray_PickPosition_MoveDone = true;
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
                            Log.Warn($"{YAxis} pick position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayPickStep.Pick_ProcessWait:
                    if (Flags.Head_PickDone)
                    {
                        Flag_Tray_PickPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayPickStep.TrayPickEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnloadVision;
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
            switch ((TrayUnderVisionStep)Step.RunStep)
            {
                case TrayUnderVisionStep.UnderVisionDecision:
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
                this.RunMode = ERunMode.Manual_Place;
                return;
            }

            switch ((TrayUnloadVisionStep)Step.RunStep)
            {
                case TrayUnloadVisionStep.UnloadVisionStart:
                    Log.Debug("Unload vision start");
                    Step.RunStep++;
                    break;
                case TrayUnloadVisionStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayUnloadVisionStep.YAxis_UnloadVisionMove:
                    YAxis_UnloadVisionMove();
                    Step.RunStep++;
                    break;
                case TrayUnloadVisionStep.YAxis_UnloadVisionWait:
                    if (YAxis_UnloadVisionMove_Done())
                    {
                        Log.Debug($"{YAxis} move UnLoad Vision Position Done");
                        Flag_Tray_UnloadVisionPosition_MoveDone = true;
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
                            Log.Warn($"{YAxis} unload vision moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayUnloadVisionStep.UnloadVision_ProcessWait:
                    if (Flags.Head_UnloadVision_ProcessDone)
                    {
                        Flag_Tray_UnloadVisionPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayUnloadVisionStep.UnloadVisionEnd:
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
            switch ((TrayPlaceStep)Step.RunStep)
            {
                case TrayPlaceStep.PlaceStart:
                    Log.Debug("Place start");
                    Step.RunStep++;
                    break;
                case TrayPlaceStep.ZAxis_ReadyWait:
                    if (Flags.ZAxisReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayPlaceStep.YAxis_PlaceMove:
                    YAxis_PlaceMove();
                    Step.RunStep++;
                    break;
                case TrayPlaceStep.YAxis_PlaceMoveWait:
                    if (YAxis_PlaceMove_Done())
                    {
                        Log.Debug($"{YAxis} move Place_Position Done");
                        Flag_Tray_PlacePosition_MoveDone = true;
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
                            Log.Warn($"{YAxis} place position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayPlaceStep.Place_ProcessWait:
                    /* Make sure TransferProcess clear Transfer_PlacePosition_MoveDone flag first */
                    if (Flags.Head_PlaceDone && Flags.Transfer_PlacePosition_MoveDone == false)
                    {
                        Flag_Tray_PlacePosition_MoveDone = false;

                        // TODO: move this to ProcessRun function
                        CDef.RootProcess.CurrentLoadTray.WorkStartIndex++;
                        CDef.RootProcess.CurrentUnloadTray.WorkStartIndex++;

                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case TrayPlaceStep.PlaceEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        if (CurrentLoadTray == null || CurrentUnloadTray == null)
                        {
                            InWorking = false;
                            this.RunMode = ERunMode.Manual_TrayChange;
                        }
                        else
                        {
                            this.RunMode = ERunMode.Manual_LoadVision;
                        }
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Place)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
            }
        }

        private void Running_TrayChange()
        {
            switch ((TrayTrayChangeStep)Step.RunStep)
            {
                case TrayTrayChangeStep.TrayChangeStart:
                    Log.Debug("Tray change start");
                    Step.RunStep++;
                    break;
                case TrayTrayChangeStep.YAxis_TrayChangeMove:
                    YAxis_TrayChangeMove();
                    Step.RunStep++;
                    break;
                case TrayTrayChangeStep.YAxis_TrayChangeMoveWait:
                    if (YAxis_TrayChangeMove_Done())
                    {
                        Log.Debug($"{YAxis} move TrayChange_Position Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime <= CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn($"{YAxis} tray change position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayTrayChangeStep.CheckBothTrayStatus:
                    if (ProcessName == "LTray" &&
                            (CDef.RootProcess.RightTrayProcess.CurrentLoadTray == null || CDef.RootProcess.RightTrayProcess.CurrentUnloadTray == null))
                    {
                        CDef.MessageViewModel.Show($"Out of Material\nChange Both Tray Please!");

                        CDef.RootProcess.RightTrayProcess.FrontLoadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.RightTrayProcess.RearLoadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.RightTrayProcess.FrontUnloadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.RightTrayProcess.RearUnloadTray.ResetCommand.Execute(null);

                        CDef.RootProcess.LeftTrayProcess.FrontLoadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.LeftTrayProcess.RearLoadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.LeftTrayProcess.FrontUnloadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.LeftTrayProcess.RearUnloadTray.ResetCommand.Execute(null);

                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    else if (ProcessName == "RTray" &&
                        (CDef.RootProcess.LeftTrayProcess.CurrentLoadTray == null || CDef.RootProcess.LeftTrayProcess.CurrentUnloadTray == null))
                    {
                        CDef.MessageViewModel.Show($"Out of Material\nChange Both Tray Please!");

                        CDef.RootProcess.RightTrayProcess.FrontLoadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.RightTrayProcess.RearLoadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.RightTrayProcess.FrontUnloadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.RightTrayProcess.RearUnloadTray.ResetCommand.Execute(null);

                        CDef.RootProcess.LeftTrayProcess.FrontLoadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.LeftTrayProcess.RearLoadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.LeftTrayProcess.FrontUnloadTray.ResetCommand.Execute(null);
                        CDef.RootProcess.LeftTrayProcess.RearUnloadTray.ResetCommand.Execute(null);

                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case TrayTrayChangeStep.TrayChangeMessage_Display:
                    CDef.MessageViewModel.ShowDialog($"Out of Material\nChange {FriendlyName} Please!");

                    Step.RunStep++;
                    break;
                case TrayTrayChangeStep.Tray_Clear:
                    FrontLoadTray.ResetCommand.Execute(null);
                    RearLoadTray.ResetCommand.Execute(null);
                    FrontUnloadTray.ResetCommand.Execute(null);
                    RearUnloadTray.ResetCommand.Execute(null);

                    Step.RunStep++;
                    break;
                case TrayTrayChangeStep.YAxis_WorkPositionMove:
                    YAxis_LoadVisionMove();
                    Step.RunStep++;
                    break;
                case TrayTrayChangeStep.YAxis_WorkPositionMoveWait:
                    if (YAxis_LoadVisionMove_Done())
                    {
                        Log.Debug($"{YAxis} move Work Position Done");
                        Flag_Tray_LoadVisionPosition_MoveDone = true;
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
                            Log.Warn($"{YAxis} Work position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayTrayChangeStep.TrayChangeEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_LoadVision;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Moving Functions
        // YAxis Load Vision Position
        public double YAxis_LoadVisionPosition
        {
            get
            {
                double position = 0;
                //offset_Y = offset_Y of "Load Tray pitch"
                double offsetY = CDef.RootProcess.CurrentLoadTray.WorkStartIndex_Y * CDef.CommonRecipe.LoadingTray_Y_Pitch;

                if (CDef.RootProcess.LeftTrayProcess.InWorking)
                {
                    if (CurrentLoadTray == FrontLoadTray)
                    {
                        position = CDef.LeftTrayRecipe.FrontLoadTray_FirstPosition - offsetY;
                    }
                    else
                    {
                        position = CDef.LeftTrayRecipe.RearLoadTray_FirstPosition - offsetY;
                    }
                }
                else
                {
                    if (CurrentLoadTray == FrontLoadTray)
                    {
                        position = CDef.RightTrayRecipe.FrontLoadTray_FirstPosition - offsetY;
                    }
                    else
                    {
                        position = CDef.RightTrayRecipe.RearLoadTray_FirstPosition - offsetY;
                    }
                }
                return position;
            }
        }
        private void YAxis_LoadVisionMove()
        {

            YAxis.MoveAbs(YAxis_LoadVisionPosition);
        }
        private bool YAxis_LoadVisionMove_Done()
        {
            SimSleep(300);
            return YAxis.IsOnPosition(YAxis_LoadVisionPosition);
        }

        // YAxis Pick Position
        public double YAxis_Pick_Position
        {
            get
            {
                double position = 0;
                //offset_Y = offset_Y of "Load Tray pitch" 
                double offsetY = CDef.RootProcess.CurrentLoadTray.WorkStartIndex_Y * CDef.CommonRecipe.LoadingTray_Y_Pitch;
                double offsetVisionY = 0;

                if (CDef.RootProcess.LeftTrayProcess.InWorking)
                {
                    if (CurrentLoadTray == FrontLoadTray)
                    {
                        position = CDef.LeftTrayRecipe.FrontLoadTray_FirstPosition + CDef.CommonRecipe.VisionToPicker_YOffset - offsetY + offsetVisionY;
                    }
                    else
                    {
                        position = CDef.LeftTrayRecipe.RearLoadTray_FirstPosition + CDef.CommonRecipe.VisionToPicker_YOffset - offsetY + offsetVisionY;
                    }
                }
                else
                {
                    if (CurrentLoadTray == FrontLoadTray)
                    {
                        position = CDef.RightTrayRecipe.FrontLoadTray_FirstPosition + CDef.CommonRecipe.VisionToPicker_YOffset - offsetY + offsetVisionY;
                    }
                    else
                    {
                        position = CDef.RightTrayRecipe.RearLoadTray_FirstPosition + CDef.CommonRecipe.VisionToPicker_YOffset - offsetY + offsetVisionY;
                    }
                }
                return position;
            }
        }
        private void YAxis_PickMove()
        {
            YAxis.MoveAbs(YAxis_Pick_Position);
        }
        private bool YAxis_PickMove_Done()
        {
            SimSleep(300);
            return YAxis.IsOnPosition(YAxis_Pick_Position);
        }

        // YAxis Place Position
        public double YAxis_Place_Position
        {
            get
            {
                double position = 0;
                //offset_Y = offset_Y "Unload Tray pitch"
                double offsetY = CDef.RootProcess.CurrentLoadTray.WorkStartIndex_Y * CDef.CommonRecipe.UnloadingTray_Y_Pitch;
                double offsetVisionY = 0;
                if (CDef.RootProcess.LeftTrayProcess.InWorking)
                {
                    if (CurrentUnloadTray == FrontUnloadTray)
                    {
                        position = CDef.LeftTrayRecipe.FrontUnloadTray_FirstPosition + CDef.CommonRecipe.VisionToPicker_YOffset - offsetY + offsetVisionY;
                    }
                    else
                    {
                        position = CDef.LeftTrayRecipe.RearUnloadTray_FirstPosition + CDef.CommonRecipe.VisionToPicker_YOffset - offsetY + offsetVisionY;
                    }
                }
                else
                {
                    if (CurrentUnloadTray == FrontUnloadTray)
                    {
                        position = CDef.RightTrayRecipe.FrontUnloadTray_FirstPosition + CDef.CommonRecipe.VisionToPicker_YOffset - offsetY + offsetVisionY;
                    }
                    else
                    {
                        position = CDef.RightTrayRecipe.RearUnloadTray_FirstPosition + CDef.CommonRecipe.VisionToPicker_YOffset - offsetY + offsetVisionY;
                    }
                }
                return position;
            }
        }
        private void YAxis_PlaceMove()
        {
            YAxis.MoveAbs(YAxis_Place_Position);
        }
        private bool YAxis_PlaceMove_Done()
        {
            SimSleep(300);
            return YAxis.IsOnPosition(YAxis_Place_Position);
        }

        // YAxis Unload Vision Position
        public double YAxis_UnloadVisionPosition
        {
            get
            {
                double position = 0;
                //offset_Y = offset_Y "Unload Tray pitch" 
                double offsetY = CDef.RootProcess.CurrentLoadTray.WorkStartIndex_Y * CDef.CommonRecipe.UnloadingTray_Y_Pitch;
                if (CDef.RootProcess.LeftTrayProcess.InWorking)
                {
                    if (CurrentUnloadTray == FrontUnloadTray)
                    {
                        position = CDef.LeftTrayRecipe.FrontUnloadTray_FirstPosition - offsetY;
                    }
                    else
                    {
                        position = CDef.LeftTrayRecipe.RearUnloadTray_FirstPosition - offsetY;
                    }
                }
                else
                {
                    if (CurrentUnloadTray == FrontUnloadTray)
                    {
                        position = CDef.RightTrayRecipe.FrontUnloadTray_FirstPosition - offsetY;
                    }
                    else
                    {
                        position = CDef.RightTrayRecipe.RearUnloadTray_FirstPosition - offsetY;
                    }
                }
                return position;
            }
        }
        private void YAxis_UnloadVisionMove()
        {
            YAxis.MoveAbs(YAxis_UnloadVisionPosition);
        }
        private bool YAxis_UnloadVisionMove_Done()
        {
            SimSleep(300);
            return YAxis.IsOnPosition(YAxis_UnloadVisionPosition);
        }

        //YAxis Tray change Position
        private double YAxis_TrayChange_Position
        {
            get
            {
                return CDef.RootProcess.LeftTrayProcess.InWorking ? CDef.LeftTrayRecipe.TrayChange_Position : CDef.RightTrayRecipe.TrayChange_Position;
            }
        }
        public void YAxis_TrayChangeMove()
        {
            YAxis.MoveAbs(YAxis_TrayChange_Position);
        }
        private bool YAxis_TrayChangeMove_Done()
        {
            SimSleep(300);
            return YAxis.IsOnPosition(YAxis_TrayChange_Position);
        }
        #endregion
    }
}
