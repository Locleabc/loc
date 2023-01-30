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
using PLV_BracketAssemble.Processing;
using TopCom.Models;
using System.Web.UI.WebControls;
using TopVision.Models;
using PLV_BracketAssemble.MVVM.ViewModels;
using System.Threading;

namespace PLV_BracketAssemble.Processing
{
    public class CHeadProcess : ProcessingBase
    {
        #region Constructor(s)
        public CHeadProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }
        #endregion

        #region Motion(s)
        public MotionPlusR YAxis
        {
            get
            {
                return (MotionPlusR)CDef.AllAxis.YAxis;
            }
        }

        public MotionPlusR XXAxis
        {
            get
            {
                return (MotionPlusR)CDef.AllAxis.XXAxis;
            }
        }

        public CPickerCylinder PickerCylinder1 { get; } = new CPickerCylinder()
        {
            MoveBackwardHandler = () => { CDef.IO.Output.Picker1_Up = true; CDef.IO.Output.Picker1_Down = false; },
            ConfirmBackwardHandler = () => { return CDef.IO.Input.Picker1_UpDetect == true; },

            MoveForwardHandler = () => { CDef.IO.Output.Picker1_Up = false; CDef.IO.Output.Picker1_Down = true; },
            ConfirmForwardHandler = () => { return CDef.IO.Input.Picker1_DownDetect == true; },

            VacuumOnHandler = () => { CDef.IO.Output.Picker1_Purge = false; CDef.IO.Output.Picker1_Vac = true; },
            VacuumOffHandler = () => { CDef.IO.Output.Picker1_Vac = false;},
            PurgeOnHandler = () => { CDef.IO.Output.Picker1_Purge = true; CDef.IO.Output.Picker1_Vac = false; },
            PurgeOffHandler = () => { CDef.IO.Output.Picker1_Purge = false;},
            ConfirmVacuumHandler = () => { return CDef.IO.Input.Picker1_VacDetect == true; },
        };

        public CPickerCylinder PickerCylinder2 { get; } = new CPickerCylinder()
        {
            MoveBackwardHandler = () => { CDef.IO.Output.Picker2_Up = true; CDef.IO.Output.Picker2_Down = false; },
            ConfirmBackwardHandler = () => { return CDef.IO.Input.Picker2_UpDetect == true; },

            MoveForwardHandler = () => { CDef.IO.Output.Picker2_Up = false; CDef.IO.Output.Picker2_Down = true; },
            ConfirmForwardHandler = () => { return CDef.IO.Input.Picker2_DownDetect == true; },

            VacuumOnHandler = () => { CDef.IO.Output.Picker2_Purge = false; CDef.IO.Output.Picker2_Vac = true; },
            VacuumOffHandler = () => { CDef.IO.Output.Picker2_Vac = false;},
            PurgeOnHandler = () => { CDef.IO.Output.Picker2_Purge = true; CDef.IO.Output.Picker2_Vac = false; },
            PurgeOffHandler = () => { CDef.IO.Output.Picker2_Purge = false;},
            ConfirmVacuumHandler = () => { return CDef.IO.Input.Picker2_VacDetect == true; },
        };

        public CCylinder Clamp { get; } = new CCylinder()
        {
            // TODO: Fill clamp define
            MoveBackwardHandler = () => { Thread.Sleep(200); },
            ConfirmBackwardHandler = () => { return true; },

            MoveForwardHandler = () => { Thread.Sleep(200); },
            ConfirmForwardHandler = () => { return true; },
        };
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

        public TrayViewModel InputTray
        {
            get
            {
                return CDef.MainViewModel.MainContentVM.AutoVM.InputTrayVM;
            }
        }

        public TrayViewModel OutputTray
        {
            get
            {
                return CDef.MainViewModel.MainContentVM.AutoVM.OutputTrayVM;
            }
        }

        #region Flags
        public bool Flag_HeadHomeDone { get; set; }
        public bool Flag_Request_UnderVision_Start { get; set; }

        /// <summary>
        /// This flag will be set by HeadProcess in Running_Pick (base on Input IO)
        /// </summary>
        public bool Flag_Picker1_PickDone { get; private set; }
        /// <summary>
        /// This flag will be set by HeadProcess in Running_Pick (base on Input IO)
        /// </summary>
        public bool Flag_Picker2_PickDone { get; private set; }

        /// <summary>
        /// This flag will be set by HeadProcess in Running_Place (base on Input IO)
        /// </summary>
        public bool Flag_Picker1_PlaceDone { get; private set; }
        /// <summary>
        /// This flag will be set by HeadProcess in Running_Place (base on Input IO)
        /// </summary>
        public bool Flag_Picker2_PlaceDone { get; private set; }
        #endregion

        #region Overrider(s)
        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtnCode = PRtnCode.RtnOk;

            switch ((EHeadHomeStep)Step.HomeStep)
            {
                case EHeadHomeStep.Start:
                    Log.Debug("Home start");
                    Step.HomeStep++;
                    break;
                case EHeadHomeStep.AllPickerCylinderUp:
                    PickerCylinder1.MoveBackward();
                    PickerCylinder2.MoveBackward();
                    Step.HomeStep++;
                    break;
                case EHeadHomeStep.ClampCylinder_Backward:
                    //Clamp Cylinder Backward
                    Step.HomeStep++;
                    break;
                case EHeadHomeStep.AllPickerCylinderUp_Wait:
                    if (PickerCylinder1.IsBackward && PickerCylinder2.IsBackward)
                    {
                        Step.HomeStep++;
                        break;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < 10000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Picker Up timeout");
                        }
                    }
                    break;
                case EHeadHomeStep.ClampCylinder_Backward_Wait:
                    //ClampCylinder_Backward_Wait
                    Step.HomeStep++;
                    break;
                case EHeadHomeStep.Head_CM_ExistCheck:
                    if (CDef.IO.Input.Picker1_VacDetect || CDef.IO.Input.Picker2_VacDetect)
                    {
                        CDef.RootProcess.SetWarning("Remove CM on Picker");
                    }
                    else
                    {
                        Step.HomeStep++;
                    }
                    break;
                case EHeadHomeStep.Head_HomeDone_Set:
                    if (CDef.IO.Output.Picker1_Vac)
                    {
                        CDef.IO.Output.Picker1_Vac = false;
                    }

                    if (CDef.IO.Output.Picker2_Vac)
                    {
                        CDef.IO.Output.Picker2_Vac = false;
                    }

                    Flag_HeadHomeDone = true;
                    Step.HomeStep++;
                    break;
                case EHeadHomeStep.Y_XXAxisHomeSearch:
                    YAxis.HomeSearch();
                    XXAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case EHeadHomeStep.Y_XXAxisHomeSearch_Wait:
                    if (YAxis.Status.IsHomeDone && XXAxis.Status.IsHomeDone)
                    {
                        Log.Debug($"{XXAxis} && {YAxis} origin done");
                        XXAxis.ClearPosition();
                        YAxis.ClearPosition();
                        Step.HomeStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < 30000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            if (!YAxis.Status.IsHomeDone)
                            {
                                CDef.RootProcess.SetWarning($"{YAxis} home search timeout");
                            }

                            if (!XXAxis.Status.IsHomeDone)
                            {
                                CDef.RootProcess.SetWarning($"{XXAxis} home search timeout");
                            }
                        }
                    }
                    break;
                case EHeadHomeStep.End:
                    Log.Debug("Head home done");
                    ProcessingStatus = EProcessingStatus.OriginDone;
                    Step.HomeStep++;
                    break;
                default:
                    Sleep(20);
                    break;
            }
            return nRtnCode;
        }

        public override PRtnCode ProcessToRun()
        {
            PRtnCode nRtnCode = PRtnCode.RtnOk;

            switch ((EHeadToRunStep)(Step.ToRunStep))
            {
                case EHeadToRunStep.Start:
                    Log.Debug("ToRun Start");
                    Step.ToRunStep++;
                    break;
                case EHeadToRunStep.CheckIf_CMExistOnPreAlignUnit:
                    if (CDef.IO.Input.PreAlgin1_CMDetect | CDef.IO.Input.PreAlgin2_CMDetect)
                    {
                        CDef.RootProcess.SetWarning($"CM detected on PreAlign, Please remove");
                    }
                    else
                    {
                        Step.ToRunStep++;
                    }
                    break;
                case EHeadToRunStep.AllPickerCylinderUp:
                    PickerCylinder1.MoveBackward();
                    PickerCylinder2.MoveBackward();
                    Step.ToRunStep++;
                    break;
                case EHeadToRunStep.ClampCylinder_Backward:
                    Clamp.MoveBackward();
                    Step.ToRunStep++;
                    break;
                case EHeadToRunStep.AllPickerCylinderUp_Wait:
                    if (PickerCylinder1.IsBackward && PickerCylinder2.IsBackward)
                    {
                        Step.ToRunStep++;
                        break;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < 10000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Picker Up timeout");
                        }
                    }
                    break;
                case EHeadToRunStep.ClampCylinder_Backward_Wait:
                    if (Clamp.IsBackward)
                    {
                        Step.ToRunStep++;
                        break;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < 10000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Clamp backward timeout");
                        }
                    }
                    break;
                case EHeadToRunStep.Set_PickerPickPlaceDone_Status:
                    // Set this before running Pick, so TrayProcess can get this information correctly.
                    if (PickerCylinder1.IsVacuumOn)
                    {
                        Flag_Picker1_PickDone = true;
                    }
                    else
                    {
                        if (CDef.IO.Output.Picker1_Vac && CDef.GlobalRecipe.UseVacuumCheck)
                        {
                            CDef.IO.Output.Picker1_Vac = false;
                        }
                        Flag_Picker1_PickDone = false;
                    }

                    if (PickerCylinder2.IsVacuumOn)
                    {
                        Flag_Picker2_PickDone = true;
                    }
                    else
                    {
                        if (CDef.IO.Output.Picker2_Vac && CDef.GlobalRecipe.UseVacuumCheck)
                        {
                            CDef.IO.Output.Picker2_Vac = false;
                        }

                        Flag_Picker2_PickDone = false;
                    }

                    Flag_Picker1_PlaceDone = false;
                    Flag_Picker2_PlaceDone = false;

                    Step.ToRunStep++;
                    break;
                case EHeadToRunStep.End:
                    Log.Debug("ToRun End");
                    ProcessingStatus = EProcessingStatus.ToRunDone;
                    this.RunMode = CDef.RootProcess.RunMode;
                    Step.ToRunStep++;
                    break;
                default:
                    Sleep(20);
                    break;
            }

            return nRtnCode;
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
                    Running_Auto();
                    break;
                case ERunMode.Manual_Change:
                    Running_Change();
                    break;
                case ERunMode.Manual_Pick:
                    Running_Pick();
                    break;
                case ERunMode.Manual_PreAlign:
                    Running_PreAlign();
                    break;
                case ERunMode.Manual_UnderVision:
                    Running_UnderVision();
                    break;
                case ERunMode.Manual_Place:
                    Running_Place();
                    break;
                default:
                    Sleep(20);
                    break;
            }

            return nRtnCode;
        }
        #endregion

        #region Run Functions
        private void Running_Auto()
        {
            switch ((EHeadAutoStep)(Step.RunStep))
            {
                case EHeadAutoStep.Start:
                    Log.Debug("AutoRun Start");
                    Step.RunStep++;
                    break;
                case EHeadAutoStep.CheckIf_HeadIsReadyPosition:
                    if (CDef.IO.Input.HeadReadyPositionDetect == false)
                    {
                        CDef.RootProcess.SetWarning($"Machine is not in CHANGE position");
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EHeadAutoStep.CheckIf_CMExistOnPicker:
                    if (Flags.Picker1_Pick_Done & Flags.Picker2_Pick_Done & OutputTray.WorkIndex > 0)
                    {
                        Log.Debug("CM Detected on both picker. Moving to PreAlign...");
                        RunMode = ERunMode.Manual_PreAlign;
                        
                        PTimer.TactTimeCounter[0] = PTimer.Now;
                    }
                    else if ((Flags.Picker1_Pick_Done || Flags.Picker2_Pick_Done) && InputTray.WorkIndex > 0 && OutputTray.CountReadySlot > 1)
                    {
                        RunMode = ERunMode.Manual_Pick;
                    }
                    else if ((Flags.Picker1_Pick_Done || Flags.Picker2_Pick_Done) && OutputTray.WorkIndex > 0)
                    {
                        RunMode = ERunMode.Manual_PreAlign;

                        PTimer.TactTimeCounter[0] = PTimer.Now;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EHeadAutoStep.Tray_CheckStatus:
                    if (InputTray.WorkIndex < 0 && OutputTray.WorkIndex < 0)
                    {
                        Sleep(20);
                    }
                    else if (InputTray.WorkIndex < 0)
                    {
                        Sleep(20);
                    }
                    else if (OutputTray.WorkIndex < 0)
                    {
                        Sleep(20);
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EHeadAutoStep.End:
                    RunMode = ERunMode.Manual_Pick;
                    break;
                default:
                    Sleep(20);
                    break;
            }
        }

        private void Running_Change()
        {
            switch ((EHeadChangeStep)Step.RunStep)
            {
                case EHeadChangeStep.Start:
                    Log.Debug("Change start");
                    Step.RunStep++;
                    break;
                case EHeadChangeStep.Head_Move_ReadyPosition:
                    Head_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case EHeadChangeStep.Head_Move_ReadyPosition_Wait:
                    if (IsHeadReadyPosition)
                    {
                        Step.RunStep++;
                        break;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Head move to CHANGE position timeout");
                        }
                    }
                    break;
                case EHeadChangeStep.End:
                    Log.Debug("Change End");
                    // Change only call as ManualRun (Not AutoRun)
                    this.RunMode = ERunMode.Stop;
                    break;
                default:
                    Sleep(20);
                    break;
            }
        }

        public EPickers CurrentPicker { get; private set; }
        public CPickerCylinder CurrentPickerCylinder
        {
            get
            {
                if (CurrentPicker == EPickers.Picker1) return PickerCylinder1;
                else return PickerCylinder2;
            }
        }
        public bool CurrentPicker_PickDoneStatus
        {
            get
            {
                if (CurrentPicker == EPickers.Picker1) return Flag_Picker1_PickDone;
                else return Flag_Picker2_PickDone;
            }
            set
            {
                if (CurrentPicker == EPickers.Picker1) Flag_Picker1_PickDone = value;
                else Flag_Picker2_PickDone = value;
            }
        }

        private void Running_Pick()
        {
            switch ((EHeadPickStep)Step.RunStep)
            {
                case EHeadPickStep.Start:
                    Log.Debug("Pick Start");
                    Step.RunStep++;
                    break;
                case EHeadPickStep.Head_CM_ExistCheck:
                    if (PickerCylinder1.IsVacuumOn & PickerCylinder2.IsVacuumOn)
                    {
                        // This case should be never happend, of CM detected on both picker, the RunMode will be Pre-Align, not Picker
                        // Just trying to handle all cases
                        CDef.RootProcess.SetWarning($"CM detected on both picker");
                        break;
                    }
                    Step.RunStep++;
                    break;
                case EHeadPickStep.SetCurrentHead:
                    CurrentPicker = EPickers.Picker1;
                    Step.RunStep++;
                    break;
                case EHeadPickStep.Head_PreSet_PickDone_Status:
                    if (CurrentPicker_PickDoneStatus == false)
                    {
                        // There is no CM in the CurrentPicker -> Go pick
                        Step.RunStep++;
                    }
                    else
                    {
                        // There is CM in the CurrentPicker -> Update PickDone flag -> jump UpdateCurrentHead
                        Log.Debug($"CM on {CurrentPicker} already.");
                        Step.RunStep = (int)EHeadPickStep.UpdateCurrentHead;
                    }
                    break;
                case EHeadPickStep.Head_Move_PickPosition:
                    if (InputTray.WorkIndex < 0)
                    {
                        Sleep(20);
                        break;
                    }
                    Head_PickPosition_Move(CurrentPicker);

                    //Set Timer:
                    PTimer.TactTimeCounter[0] = PTimer.Now;
                    
                    Step.RunStep++;
                    break;
                case EHeadPickStep.Head_Move_PickPosition_Wait:
                    if (IsHeadPickPosition(CurrentPicker))
                    {
                        Step.RunStep++;
                        break;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Head move to ready position timeout");
                        }
                    }
                    break;
                case EHeadPickStep.Tray_PickPosition_Wait:
                    if (CDef.RootProcess.TrayProcess.IsTrayPickPosition(CurrentPicker))
                    {
                        Log.Debug($"Tray is in {CurrentPicker} pick position");
                        Step.RunStep++;
                        break;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Head move to ready position timeout");
                        }
                    }
                    break;
                case EHeadPickStep.Head_PickerCylinder_Down:
                    CurrentPickerCylinder.MoveForward();
                    Log.Debug($"Move {CurrentPicker} cylinder DOWN");
                    Step.RunStep++;
                    break;
                case EHeadPickStep.Head_PickerCylinder_Down_Wait:
                    if (CurrentPickerCylinder.IsForward)
                    {
                        Step.RunStep++;
                        break;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < 9999)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{CurrentPicker} cylinder move DOWN timeout");
                        }
                    }
                    break;
                case EHeadPickStep.Head_PickerCylinder_VacOn:
                    CurrentPickerCylinder.VacuumOn();
                    Log.Debug($"Turn {CurrentPicker} vacuum ON");
                    Step.RunStep++;
                    break;
                case EHeadPickStep.Head_PickerCylinder_VacOn_Delay:
                    // Vacuum delay
                    if (PTimer.StepLeadTime < CDef.CommonRecipe.VAC_Delay * 1000)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EHeadPickStep.Head_PickerCylinder_Up:
                    CurrentPickerCylinder.MoveBackward();
                    Log.Debug($"Move {CurrentPicker} cylinder UP");
                    Step.RunStep++;
                    break;
                case EHeadPickStep.Head_PickerCylinder_Up_Wait:
                    if (CurrentPickerCylinder.IsBackward)
                    {
                        if (CDef.GlobalRecipe.UseVacuumCheck)
                        {
                            if (CurrentPickerCylinder.IsVacuumOn == false)
                            {
                                InputTray.SetSingleCell(TopCom.ECellSimpleStatus.Fail, InputTray.WorkIndex);
                                CDef.RootProcess.SetWarning($"{CurrentPicker} pick failed");
                                break;
                            }
                        }
                        Step.RunStep++;
                        break;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < 9999)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{CurrentPicker} cylinder move UP timeout");
                        }
                    }
                    break;
                case EHeadPickStep.Head_PostSet_PickDone_Status:
                    CurrentPicker_PickDoneStatus = true;
                    
                    //Set Status Cell 
                    InputTray.SetSingleCell(TopCom.ECellSimpleStatus.Pass, InputTray.WorkIndex);
                    
                    Log.Debug($"{CurrentPicker} pick done");
                    Step.RunStep++;
                    break;
                case EHeadPickStep.UpdateCurrentHead:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        if (InputTray.WorkIndex < 0)
                        {
                            Flag_Picker1_PlaceDone = false;
                            Step.RunStep = (int)EHeadPickStep.End;
                            break;
                        }
                        // Update CurrentPicker to next picker -> Repeate the sequence
                        CurrentPicker = EPickers.Picker2;
                        Step.RunStep = (int)EHeadPickStep.Head_PreSet_PickDone_Status;
                    }
                    else
                    {
                        Flag_Picker1_PlaceDone = false;
                        Flag_Picker2_PlaceDone = false;

                        Log.Debug("Both picker pick done");
                        Step.RunStep = (int)EHeadPickStep.End;
                    }
                    break;
                case EHeadPickStep.End:
                    Log.Debug("Head Pick End");
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_PreAlign;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Pick)
                    {
                        this.RunMode = ERunMode.Stop;
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Running_PreAlign()
        {
            switch ((EHeadPreAlignStep)Step.RunStep)
            {
                case EHeadPreAlignStep.Start:
                    Log.Debug("Head Pre Align Start");
                    Step.RunStep++;
                    break;
                case EHeadPreAlignStep.If_UsePreAlgin:
                    if (CDef.GlobalRecipe.SkipPreAlign && CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnderVision;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case EHeadPreAlignStep.Head_Move_PreAlginPosition:
                    XXAxis.MoveAbs(CDef.HeadRecipe.XXAxis_PreAlign_Position);
                    YAxis.MoveAbs(CDef.HeadRecipe.YAxis_PreAlign_Position);
                    Log.Debug("Head move Pre Align Position");
                    Step.RunStep++;
                    break;
                case EHeadPreAlignStep.Head_Move_PreAlginPosition_Wait:
                    if (Is_Head_PreAlign_Position)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            if (!Is_XXAxis_PreAlign_Position)
                            {
                                CDef.RootProcess.SetWarning($"XX Axis move Pre Align Position timeout");
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"Y Axis move Pre Align Position timeout");
                            }

                        }
                    }
                    break;
                case EHeadPreAlignStep.ClampCylinder_Backward_First:
                    Clamp.MoveBackward();
                    Log.Debug("Clamp move Backward first");

                    Step.RunStep++;
                    break;
                case EHeadPreAlignStep.ClampCylinder_Backward_Wait_First:
                    if (Clamp.IsBackward)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Clamp move Backward first timeout");
                        }
                    }
                    break;
                case EHeadPreAlignStep.Head_PickerCylinder_Down:
                    if (Flags.Picker1_Pick_Done)
                    {
                        PickerCylinder1.MoveForward();
                        Log.Debug("Picker Cylinder 1 move Forward");
                    }

                    if (Flags.Picker2_Pick_Done)
                    {
                        PickerCylinder2.MoveForward();
                        Log.Debug("Picker Cylinder 2 move Forward");
                    }

                    Step.RunStep++;
                    break;
                case EHeadPreAlignStep.Head_PickerCylinder_Down_Wait:
                    if ((PickerCylinder1.IsForward || !Flags.Picker1_Pick_Done) && (PickerCylinder2.IsForward || !Flags.Picker2_Pick_Done))
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            if (PickerCylinder1.IsForward)
                            {
                                CDef.RootProcess.SetWarning($"Picker Cylinder 1 move Forward timeout");
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"Picker Cylinder 2 move Forward timeout");
                            }

                        }
                    }
                    break;
                case EHeadPreAlignStep.ClampCylinder_Forward:
                    Clamp.MoveForward();
                    Log.Debug("Clamp move Forward");

                    Step.RunStep++;
                    break;
                case EHeadPreAlignStep.ClampCylinder_Forward_Wait:
                    if (Clamp.IsForward)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Clamp move Forward timeout");
                        }
                    }
                    break;
                case EHeadPreAlignStep.ClampCylinder_Backward:
                    Clamp.MoveBackward();
                    Log.Debug("Clamp move Backward");

                    Step.RunStep++;
                    break;
                case EHeadPreAlignStep.ClampCylinder_Backward_Wait:
                    if (Clamp.IsBackward)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Clamp move Backward timeout");
                        }
                    }
                    break;
                case EHeadPreAlignStep.Head_PickerCylinder_Up:
                    PickerCylinder1.MoveBackward();
                    Log.Debug("Picker Cylinder 1 move Backward");

                    PickerCylinder2.MoveBackward();
                    Log.Debug("Picker Cylinder 2 move Backward");

                    Step.RunStep++;
                    break;
                case EHeadPreAlignStep.Head_PickerCylinder_Up_Wait:
                    if (PickerCylinder1.IsBackward && PickerCylinder2.IsBackward)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.AxisMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            if (PickerCylinder1.IsBackward)
                            {
                                CDef.RootProcess.SetWarning($"Picker Cylinder 1 move Backward timeout");
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"Picker Cylinder 2 move Backward timeout");
                            }

                        }
                    }
                    break;
                case EHeadPreAlignStep.End:
                    Log.Debug("Head Pre Align End");
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnderVision;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_PreAlign)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
            }
        }

        private void Running_UnderVision()
        {
            switch ((EHeadUnderVisionStep)Step.RunStep)
            {
                case EHeadUnderVisionStep.Start:
                    Log.Debug("Head Under Vision Start");
                    Step.RunStep++;
                    break;
                case EHeadUnderVisionStep.If_UseVision:
                    if (CDef.GlobalRecipe.SkipUnderVision && CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Place;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case EHeadUnderVisionStep.UnderVisionStart_Flag_Set:
                    Flag_Request_UnderVision_Start = true;
                    Step.RunStep++;
                    break;
                case EHeadUnderVisionStep.Head_Move_UnderVisionPosition1:
                    if (Flag_Picker1_PickDone)
                    {
                        Head_UnderVisionPosition_Move(EPickers.Picker1);
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EHeadUnderVisionStep.Head_Move_UnderVisionPosition2;
                    }
                    break;
                case EHeadUnderVisionStep.Head_Move_UnderVisionPosition1_Wait:
                    if (Is_Picker1_UnderVision_Position)
                    {
                        Log.Debug("Head move Under Vision position #01 done");
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
                            if (!Is_XXAxis_Picker1_UnderVision_Position)
                            {
                                CDef.RootProcess.SetWarning($"XX Axis move to Under Vision position #01 timeout");
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"Y Axis move to Under Vision position #01 timeout");
                            }
                        }
                    }
                    break;
                case EHeadUnderVisionStep.UnderVision_Head1_InspectDone_Wait:
                    if (Flags.UnderVision_Inspect_Picker1_Done)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Wait Under Vision Inspect #01 timeout");
                        }
                    }
                    break;
                case EHeadUnderVisionStep.Head_Move_UnderVisionPosition2:
                    if (Flags.Picker2_Pick_Done)
                    {
                        Head_UnderVisionPosition_Move(EPickers.Picker2);
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EHeadUnderVisionStep.UnderVisionStart_Flag_Clear;
                    }
                    break;
                case EHeadUnderVisionStep.Head_Move_UnderVisionPosition2_Wait:
                    if (Is_Picker2_UnderVision_Position)
                    {
                        Log.Debug("Head move Under Vision position #02 done");
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
                            if (!Is_XXAxis_Picker2_UnderVision_Position)
                            {
                                CDef.RootProcess.SetWarning($"XX Axis move to Under Vision position #02 timeout");
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"Y Axis move to Under Vision position #02 timeout");
                            }
                        }
                    }
                    break;
                case EHeadUnderVisionStep.UnderVision_Head2_InspectDone_Wait:
                    if (Flags.UnderVision_Inspect_Picker2_Done)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"Wait Under Vision Inspect #02 timeout");
                        }
                    }
                    break;
                case EHeadUnderVisionStep.UnderVisionStart_Flag_Clear:
                    Flags.UnderVision_Inspect_Done = false;
                    Step.RunStep++;
                    break;
                case EHeadUnderVisionStep.End:
                    Log.Debug("Head Under Vision End");
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Place;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UnderVision)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
            }
        }

        private void Running_Place()
        {
            switch ((EHeadPlaceStep)Step.RunStep)
            {
                case EHeadPlaceStep.Start:
                    Log.Debug("Head Place Start");
                    Step.RunStep++;
                    break;
                case EHeadPlaceStep.Head_CM_ExistCheck:
                    Step.RunStep++;
                    break;
                case EHeadPlaceStep.Head_PickerCylinder_PlaceDone_Clear:
                    Flag_Picker1_PlaceDone = false;
                    Flag_Picker2_PlaceDone = false;
                    Step.RunStep++;
                    break;
                case EHeadPlaceStep.SetCurrentHead:
                    CurrentPicker = EPickers.Picker1;

                    if (Flags.Picker1_Pick_Done)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EHeadPlaceStep.UpdateCurrentHead;
                    }
                    break;
                case EHeadPlaceStep.Head_Move_PlacePosition:
                    if (OutputTray.WorkIndex < 0)
                    {
                        Sleep(20);
                        break;
                    }

                    Head_PlacePosition_Move(CurrentPicker);
                    Step.RunStep++;
                    break;
                case EHeadPlaceStep.Head_Move_PlacePosition_Wait:
                    if (IsHeadPlaceVisionPosition(CurrentPicker))
                    {
                        Log.Debug($"{CurrentPicker} move Place Position donee");
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
                            CDef.RootProcess.SetWarning($"{CurrentPicker} move Place position  timeout");
                        }
                    }
                    break;
                case EHeadPlaceStep.Tray_PlacePosition_Wait:
                    if (CDef.RootProcess.TrayProcess.IsTrayPlacePosition(CurrentPicker))
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
                            CDef.RootProcess.SetWarning($"Wait Tray move Place position timeout");
                        }
                    }
                    break;
                case EHeadPlaceStep.Head_PickerCylinder_Down:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        PickerCylinder1.MoveForward();
                    }
                    else
                    {
                        PickerCylinder2.MoveForward();
                    }
                    Log.Debug($"{CurrentPicker} Cylinder move DOWN");
                    Step.RunStep++;
                    break;
                case EHeadPlaceStep.Head_PickerCylinder_Down_Wait:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        if (PickerCylinder1.IsForward)
                        {
                            Step.RunStep++;
                        }
                        else
                        {
                            if (PTimer.StepLeadTime < 9999)
                            {
                                Sleep(10);
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"{CurrentPicker} Cylinder move DOWN timeout");
                            }
                        }
                    }
                    else
                    {
                        if (PickerCylinder2.IsForward)
                        {
                            Step.RunStep++;
                        }
                        else
                        {
                            if (PTimer.StepLeadTime < 9999)
                            {
                                Sleep(10);
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"{CurrentPicker} Cylinder move DOWN timeout");
                            }
                        }
                    }
                    break;
                case EHeadPlaceStep.Head_PickerCylinder_PurgeOn:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        PickerCylinder1.PurgeOn();
                    }
                    else
                    {
                        PickerCylinder2.PurgeOn();
                    }
                    Log.Debug($"{CurrentPicker} Cylinder Purge on");
                    Step.RunStep++;
                    break;
                case EHeadPlaceStep.Head_PickerCylinder_PurgeOn_Delay:
                    if (PTimer.StepLeadTime < CDef.CommonRecipe.Purge_Delay * 1000)
                    {
                        break;
                    }
                    else
                    {
                        if (CurrentPicker == EPickers.Picker1)
                        {
                            PickerCylinder1.PurgeOff();
                        }
                        else
                        {
                            PickerCylinder2.PurgeOff();
                        }
                        Step.RunStep++;
                    }
                    break;
                case EHeadPlaceStep.Head_PickerCylinder_Up:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        PickerCylinder1.MoveBackward();
                    }
                    else
                    {
                        PickerCylinder2.MoveBackward();
                    }
                    Log.Debug($"{CurrentPicker} Cylinder move UP");
                    Step.RunStep++;
                    break;
                case EHeadPlaceStep.Head_PickerCylinder_Up_Wait:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        if (PickerCylinder1.IsBackward)
                        {
                            Step.RunStep++;
                        }
                        else
                        {
                            if (PTimer.StepLeadTime < 9999)
                            {
                                Sleep(10);
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"{CurrentPicker} Cylinder move UP timeout");
                            }
                        }
                    }
                    else
                    {
                        if (PickerCylinder2.IsBackward)
                        {
                            Step.RunStep++;
                        }
                        else
                        {
                            if (PTimer.StepLeadTime < 9999)
                            {
                                Sleep(10);
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"{CurrentPicker} Cylinder move UP timeout");
                            }
                        }
                    }
                    break;
                case EHeadPlaceStep.Head_PickerCylinder_PlaceDone_Set:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        Flag_Picker1_PlaceDone = true;
                        Flag_Picker1_PickDone = false;
                        Log.Debug("Picker 1 Place done");
                    }
                    else
                    {
                        Flag_Picker2_PlaceDone = true;
                        Flag_Picker2_PickDone = false;
                        Log.Debug("Picker 2 Place done");
                    }

                    //Update Count Data, Set Status Cell
                    if (Datas.Data_UnderInspect_Result[(int)CurrentPicker].Judge == TopVision.EVisionJudge.OK || CDef.GlobalRecipe.SkipUnderVision)
                    {
                        Datas.WorkData.CountData.OK += 1;
                        OutputTray.SetSingleCell(TopCom.ECellSimpleStatus.Pass, OutputTray.WorkIndex);
                    }
                    else
                    {
                        Datas.WorkData.CountData.VisionNG += 1;
                        OutputTray.SetSingleCell(TopCom.ECellSimpleStatus.Fail, OutputTray.WorkIndex);
                    }

                    Step.RunStep++;
                    break;
                case EHeadPlaceStep.UpdateCurrentHead:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        CurrentPicker = EPickers.Picker2;

                        if (Flag_Picker2_PickDone)
                        {
                            Step.RunStep = (int)EHeadPlaceStep.Head_Move_PlacePosition;
                        }
                        else
                        {
                            Step.RunStep++;
                        }
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EHeadPlaceStep.UpdateWorkData:
                    if (Flag_Picker1_PlaceDone && Flag_Picker2_PlaceDone)
                    {
                        Datas.WorkData.TaktTime.CycleCurrent = 1.0 * (PTimer.Now - PTimer.TactTimeCounter[0]) / 2000;

                        Datas.WorkData.TaktTime.Total += Datas.WorkData.TaktTime.CycleCurrent * 2;
                    }
                    else
                    {
                        Datas.WorkData.TaktTime.CycleCurrent = 1.0 * (PTimer.Now - PTimer.TactTimeCounter[0]) / 1000;

                        Datas.WorkData.TaktTime.Total += Datas.WorkData.TaktTime.CycleCurrent;
                    }

                    if (Datas.WorkData.TaktTime.Maximum < Datas.WorkData.TaktTime.CycleCurrent)
                    {
                        Datas.WorkData.TaktTime.Maximum = Datas.WorkData.TaktTime.CycleCurrent;
                    }

                    Step.RunStep++;
                    break;
                case EHeadPlaceStep.End:
                    Log.Debug("Head Place End");
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Pick;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Place)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
            }
        }
        #endregion

        #region IsPosition
        public bool Is_Head_PreAlign_Position
        {
            get
            {
                return Is_XXAxis_PreAlign_Position && Is_YAxis_PreAlign_Position;
            }
        }
        public bool Is_XXAxis_PreAlign_Position
        {
            get
            {
                return XXAxis.IsOnPosition(CDef.HeadRecipe.XXAxis_PreAlign_Position);
            }
        }
        public bool Is_YAxis_PreAlign_Position
        {
            get
            {
                return YAxis.IsOnPosition(CDef.HeadRecipe.YAxis_PreAlign_Position);
            }
        }

        public bool Is_Picker1_UnderVision_Position
        {
            get
            {
                return Is_XXAxis_Picker1_UnderVision_Position && Is_YAxis_Picker1_UnderVision_Position;
            }
        }
        public bool Is_XXAxis_Picker1_UnderVision_Position
        {
            get
            {
                return XXAxis.IsOnPosition(CDef.HeadRecipe.XXAxis_Picker1_UnderVision_Position);
            }
        }
        public bool Is_YAxis_Picker1_UnderVision_Position
        {
            get
            {
                return YAxis.IsOnPosition(CDef.HeadRecipe.YAxis_Picker1_UnderVision_Position);
            }
        }

        public bool Is_Picker2_UnderVision_Position
        {
            get
            {
                return Is_XXAxis_Picker2_UnderVision_Position && Is_YAxis_Picker2_UnderVision_Position;
            }
        }
        public bool Is_XXAxis_Picker2_UnderVision_Position
        {
            get
            {
                return XXAxis.IsOnPosition(CDef.HeadRecipe.XXAxis_Picker2_UnderVision_Position);
            }
        }
        public bool Is_YAxis_Picker2_UnderVision_Position
        {
            get
            {
                return YAxis.IsOnPosition(CDef.HeadRecipe.YAxis_Picker2_UnderVision_Position);
            }
        }
        #endregion

        #region Moving Functions
        private void Head_ReadyPosition_Move()
        {
            YAxis.MoveAbs(CDef.HeadRecipe.YAxis_Change_Position);
        }

        public bool IsHeadReadyPosition
        {
            get { return CDef.IO.Input.HeadReadyPositionDetect; }
        }

        private void Head_PickPosition_Move(EPickers picker)
        {
            double position, offset;

            XXAxis.MoveAbs(CDef.HeadRecipe.XXAxis_Pick_First_Position);

            if (picker == EPickers.Picker1)
            {
                position = CDef.HeadRecipe.YAxis_Picker1_Pick_First_Position;

                offset = (InputTray.WorkIndex_Y - 1) * CDef.CommonRecipe.InputTray_Y_Pitch;

                position -= offset;

                YAxis.MoveAbs(position);
            }
            else
            {
                position = CDef.HeadRecipe.YAxis_Picker2_Pick_First_Position;

                offset = (InputTray.WorkIndex_Y - 1) * CDef.CommonRecipe.InputTray_Y_Pitch;

                position -= offset;

                YAxis.MoveAbs(position);
            }
        }

        private void Head_UnderVisionPosition_Move(EPickers picker)
        {
            if (picker == EPickers.Picker1)
            {
                YAxis.MoveAbs(CDef.HeadRecipe.YAxis_Picker1_UnderVision_Position);
                XXAxis.MoveAbs(CDef.HeadRecipe.XXAxis_Picker1_UnderVision_Position);
            }
            else
            {
                YAxis.MoveAbs(CDef.HeadRecipe.YAxis_Picker2_UnderVision_Position);
                XXAxis.MoveAbs(CDef.HeadRecipe.XXAxis_Picker2_UnderVision_Position);
            }
            Log.Debug($"Head move Under Vision Position {(int)picker + 1}");
        }

        private void Head_PlacePosition_Move(EPickers picker)
        {
            double positionXX, positionY, offset, underVisionOffsetXX = 0, underVisionOffsetY =0;

            positionXX = CDef.HeadRecipe.XXAxis_Place_First_Position;

            if (picker == EPickers.Picker1)
            {
                underVisionOffsetXX = Datas.Data_UnderInspect_Result[0].DetectedOffset.X;
                underVisionOffsetY = Datas.Data_UnderInspect_Result[0].DetectedOffset.Y;

                positionY = CDef.HeadRecipe.YAxis_Picker1_Place_First_Position;
                offset = (OutputTray.WorkIndex_Y - 1) * CDef.CommonRecipe.OutputTray_Y_Pitch;

                positionXX += underVisionOffsetXX;
                
                positionY -= offset;
                positionY += underVisionOffsetY;
            }
            else
            {
                underVisionOffsetXX = Datas.Data_UnderInspect_Result[1].DetectedOffset.X;
                underVisionOffsetY = Datas.Data_UnderInspect_Result[1].DetectedOffset.Y;

                positionY = CDef.HeadRecipe.YAxis_Picker2_Place_First_Position;
                offset = (OutputTray.WorkIndex_Y - 1) * CDef.CommonRecipe.OutputTray_Y_Pitch;

                positionXX += underVisionOffsetXX;
                
                positionY -= offset;
                positionY += underVisionOffsetY;
            }

            XXAxis.MoveAbs(positionXX);
            YAxis.MoveAbs(positionY);
            Log.Debug($"{CurrentPicker} move Place Position #{OutputTray.WorkIndex}");
        }

        // TODO: Picker1? Picker2?
        public bool IsHeadPickPosition(EPickers picker)
        {
            double position, offset;

            if (picker == EPickers.Picker1)
            {
                position = CDef.HeadRecipe.YAxis_Picker1_Pick_First_Position;

                offset = (InputTray.WorkIndex_Y - 1) * CDef.CommonRecipe.InputTray_Y_Pitch;

                position -= offset;

                return YAxis.IsOnPosition(position)
                     & XXAxis.IsOnPosition(CDef.HeadRecipe.XXAxis_Pick_First_Position);
            }
            else
            {
                position = CDef.HeadRecipe.YAxis_Picker2_Pick_First_Position;

                offset = (InputTray.WorkIndex_Y - 1) * CDef.CommonRecipe.InputTray_Y_Pitch;

                position -= offset;

                return YAxis.IsOnPosition(position)
                     & XXAxis.IsOnPosition(CDef.HeadRecipe.XXAxis_Pick_First_Position);
            }
        }

        private bool IsHeadPlaceVisionPosition(EPickers picker)
        {
            double positionXX, positionY, offset, underVisionOffsetXX, underVisionOffsetY;
            positionXX = CDef.HeadRecipe.XXAxis_Place_First_Position;

            if (picker == EPickers.Picker1)
            {
                underVisionOffsetXX = Datas.Data_UnderInspect_Result[0].DetectedOffset.X;
                underVisionOffsetY = Datas.Data_UnderInspect_Result[0].DetectedOffset.Y;

                positionY = CDef.HeadRecipe.YAxis_Picker1_Place_First_Position;

                offset = (OutputTray.WorkIndex_Y - 1) * CDef.CommonRecipe.OutputTray_Y_Pitch;

                positionXX += underVisionOffsetXX;
                positionY -= offset;
                positionY += underVisionOffsetY;

            }
            else
            {
                underVisionOffsetXX = Datas.Data_UnderInspect_Result[1].DetectedOffset.X;
                underVisionOffsetY = Datas.Data_UnderInspect_Result[1].DetectedOffset.Y;

                positionY = CDef.HeadRecipe.YAxis_Picker2_Place_First_Position;

                offset = (OutputTray.WorkIndex_Y - 1) * CDef.CommonRecipe.OutputTray_Y_Pitch;

                positionXX += underVisionOffsetXX;
                positionY -= offset;
                positionY += underVisionOffsetY;

                
            }
            return YAxis.IsOnPosition(positionY) &&
                       XXAxis.IsOnPosition(positionXX);
        }
        #endregion

        #region Privates
        private ERunMode _RunMode;
        #endregion
    }
}
