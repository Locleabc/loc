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
using PLV_BracketAssemble.MVVM.ViewModels;

namespace PLV_BracketAssemble.Processing
{
    public class CTrayProcess : ProcessingBase
    {
        #region Constructor(s)
        public CTrayProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }
        #endregion

        #region Motion(s)
        public MotionPlusR XAxis
        {
            get
            {
                return (MotionPlusR)CDef.AllAxis.XAxis;
            }
        }
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

        #region Overrider(s)
        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtnCode = PRtnCode.RtnOk;

            switch ((ETrayHomeStep)Step.HomeStep)
            {
                case ETrayHomeStep.Start:
                    Log.Debug("Tray Home Start");
                    Step.HomeStep++;
                    break;
                case ETrayHomeStep.Head_HomeDone_Wait:
                    if (!Flags.HeadHomeDone)
                    {
                        break;
                    }
                    Step.HomeStep++;
                    break;
                case ETrayHomeStep.XAxis_HomeSearch:
                    XAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case ETrayHomeStep.XAxis_HomeSearch_Wait:
                    if (XAxis.Status.IsHomeDone)
                    {
                        Log.Debug($"{XAxis} origin done");
                        XAxis.ClearPosition();
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
                            CDef.RootProcess.SetWarning($"{XAxis} home search timeout");
                        }
                    }
                    break;
                case ETrayHomeStep.End:
                    Log.Debug("Tray Home Done");
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
            switch ((ETrayAutoStep)Step.RunStep)
            {
                case ETrayAutoStep.Start:
                    Log.Debug("AutoRun Start");
                    Step.RunStep++;
                    break;
                case ETrayAutoStep.CheckIf_TrayIsReadyPosition:
                    if (CDef.IO.Input.TrayReadyPositionDetect == false)
                    {
                        CDef.RootProcess.SetWarning($"Machine is not in CHANGE position");
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayAutoStep.CheckIf_TrayExist:
                    if (CDef.IO.Input.InputTrayDetect == false || CDef.IO.Input.OutputTrayDetect == false)
                    {
                        CDef.RootProcess.SetWarning($"Tray is not detected");
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayAutoStep.CheckIf_CMExistOnBothPicker:
                    if (Flags.Picker1_Pick_Done && Flags.Picker2_Pick_Done && OutputTray.WorkIndex > 0)
                    {
                        Log.Debug("CM Detected on both picker. Placing...");
                        RunMode = ERunMode.Manual_Place;
                    }
                    else if ((Flags.Picker1_Pick_Done || Flags.Picker2_Pick_Done) && InputTray.WorkIndex > 0 && OutputTray.CountReadySlot > 1)
                    {
                        RunMode = ERunMode.Manual_Pick;
                    }
                    else if ((Flags.Picker1_Pick_Done || Flags.Picker2_Pick_Done) && OutputTray.WorkIndex > 0)
                    {
                        RunMode = ERunMode.Manual_Place;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayAutoStep.Tray_CheckStatus:
                    if (InputTray.WorkIndex < 0 && OutputTray.WorkIndex < 0)
                    {
                        CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                    }
                    else if (InputTray.WorkIndex < 0)
                    {
                        CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                    }
                    else if (OutputTray.WorkIndex < 0)
                    {
                        CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayAutoStep.End:
                    RunMode = ERunMode.Manual_Pick;
                    break;
                default:
                    Sleep(20);
                    break;
            }
        }

        private void Running_Change()
        {
            switch ((ETrayChangeStep)Step.RunStep)
            {
                case ETrayChangeStep.Start:
                    Log.Debug("Change Start");
                    Step.RunStep++;
                    break;
                case ETrayChangeStep.Tray_Move_ReadyPosition:
                    Tray_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case ETrayChangeStep.Tray_Move_ReadyPosition_Wait:
                    if (IsTrayReadyPosition)
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
                            CDef.RootProcess.SetWarning($"Tray move to CHANGE position timeout");
                        }
                    }
                    break;
                case ETrayChangeStep.HeadReadyPosition_Wait:
                    if (CDef.RootProcess.HeadProcess.IsHeadReadyPosition)
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
                            CDef.RootProcess.SetWarning($"Waiting Head CHANGE position timeout");
                        }
                    }
                    break;
                case ETrayChangeStep.End:
                    // Change only call as ManualRun (Not AutoRun)
                    if (InputTray.WorkIndex < 0 && OutputTray.WorkIndex < 0)
                    {
                        CDef.MessageViewModel.Show("Please change both tray");
                    }
                    else if (InputTray.WorkIndex < 0)
                    {
                        CDef.MessageViewModel.Show("Please change input tray");
                    }
                    else if (OutputTray.WorkIndex < 0)
                    {
                        CDef.MessageViewModel.Show("Please change output tray");
                    }

                    Log.Debug("Change End");
                    this.RunMode = ERunMode.Stop;
                    CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    break;
                default:
                    Sleep(20);
                    break;
            }
        }

        public EPickers CurrentPicker { get; private set; }

        private void Running_Pick()
        {
            switch ((ETrayPickStep)Step.RunStep)
            {
                case ETrayPickStep.Start:
                    Log.Debug("Tray Pick Start");
                    Step.RunStep++;
                    break;
                case ETrayPickStep.PreSet_CurrentPicker:
                    CurrentPicker = EPickers.Picker1;
                    Step.RunStep++;
                    break;
                case ETrayPickStep.CheckIf_Picker_PickDone:
                    bool isPreHeadPickDone =
                        CurrentPicker == EPickers.Picker1 ? Flags.Picker1_Pick_Done :
                                                            Flags.Picker2_Pick_Done;

                    if (isPreHeadPickDone)
                    {
                        Step.RunStep = (int)ETrayPickStep.PostSet_CurrentPicker;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayPickStep.Tray_CheckStatus:
                    if (InputTray.WorkIndex < 0 && OutputTray.WorkIndex < 0)
                    {
                        CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                        break;
                    }
                    else if (InputTray.WorkIndex < 0)
                    {
                        if (CurrentPicker != EPickers.Picker2)
                        {
                            CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                        }
                        else
                        {
                            RunMode = ERunMode.Manual_PreAlign;
                        }
                        break;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayPickStep.Tray_Move_PickPosition:
                    Tray_PickPosition_Move(CurrentPicker);
                    Step.RunStep++;
                    break;
                case ETrayPickStep.Tray_Move_PickPosition_Wait:
                    if (IsTrayPickPosition(CurrentPicker))
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
                            CDef.RootProcess.SetWarning($"Tray move to pick position timeout");
                        }
                    }
                    break;
                case ETrayPickStep.Head_PickerCylinder_PickDone_Wait:
                    bool isPostHeadPickDone =
                        CurrentPicker == EPickers.Picker1 ? Flags.Picker1_Pick_Done :
                                                            Flags.Picker2_Pick_Done;

                    if (isPostHeadPickDone)
                    {
                        Log.Debug($"{CurrentPicker} pick done");
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
                            CDef.RootProcess.SetWarning($"Wait {CurrentPicker} pick timeout");
                        }
                    }
                    break;
                case ETrayPickStep.PostSet_CurrentPicker:
                    if (CurrentPicker == EPickers.Picker1)
                    {
                        // Update CurrentPicker to next picker -> Repeate the sequence
                        CurrentPicker = EPickers.Picker2;
                        Step.RunStep = (int)ETrayPickStep.CheckIf_Picker_PickDone;
                    }
                    else
                    {
                        Log.Debug("Both picker pick done");
                        Step.RunStep = (int)ETrayPickStep.End;
                    }
                    break;
                case ETrayPickStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Place;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Running_PreAlign()
        {
            switch ((ETrayPreAlignStep)Step.RunStep)
            {
                case ETrayPreAlignStep.Start:
                    Step.RunStep++;
                    break;
                case ETrayPreAlignStep.End:
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
                default:
                    Sleep(20);
                    break;
            }
        }

        private void Running_UnderVision()
        {
            switch ((ETrayUnderVisionStep)Step.RunStep)
            {
                case ETrayUnderVisionStep.Start:
                    Step.RunStep++;
                    break;
                case ETrayUnderVisionStep.End:
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
                default:
                    Sleep(20);
                    break;
            }
        }

        private void Running_Place()
        {
            switch ((ETrayPlaceStep)Step.RunStep)
            {
                case ETrayPlaceStep.Start:
                    Log.Debug("Tray Place Start");
                    Step.RunStep++;
                    break;
                case ETrayPlaceStep.Tray_Move_PlacePosition1:
                    if (InputTray.WorkIndex < 0 && OutputTray.WorkIndex < 0)
                    {
                        CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                        break;
                    }
                    else if (OutputTray.WorkIndex < 0)
                    {
                        CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                        break;
                    }

                    if (Flags.Picker1_Pick_Done)
                    {
                        Tray_PlacePosition_Move(EPickers.Picker1);
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)ETrayPlaceStep.Tray_Move_PlacePosition2;
                    }

                    break;
                case ETrayPlaceStep.Tray_Move_PlacePosition1_Wait:
                    if (IsTrayPlacePosition(EPickers.Picker1))
                    {
                        Log.Debug($"Tray move Picker 1 Place Position #{OutputTray.WorkIndex} done ");
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
                            CDef.RootProcess.SetWarning($"Tray move to place position timeout");
                        }
                    }
                    break;
                case ETrayPlaceStep.Head_PickerCylinder1_PlaceDone_Wait:
                    if (Flags.Picker1_Place_Done)
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
                            CDef.RootProcess.SetWarning($"Wait Picker #01 place timeout");
                        }
                    }
                    break;
                case ETrayPlaceStep.Tray_Move_PlacePosition2:
                    if (InputTray.WorkIndex < 0 && OutputTray.WorkIndex < 0)
                    {
                        CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                        break;
                    }
                    else if (OutputTray.WorkIndex < 0)
                    {
                        CDef.RootProcess.RunMode = ERunMode.Manual_Change;
                        break;
                    }

                    if (Flags.Picker2_Pick_Done)
                    {
                        Tray_PlacePosition_Move(EPickers.Picker2);
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)ETrayPlaceStep.End;
                    }

                    break;
                case ETrayPlaceStep.Tray_Move_PlacePosition2_Wait:
                    if (IsTrayPlacePosition(EPickers.Picker2))
                    {
                        Log.Debug($"Tray move Picker 2 Place Position #{OutputTray.WorkIndex} done ");
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
                            CDef.RootProcess.SetWarning($"Tray move to place position timeout");
                        }
                    }
                    break;
                case ETrayPlaceStep.Head_PickerCylinder2_PlaceDone_Wait:
                    if (Flags.Picker2_Place_Done)
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
                            CDef.RootProcess.SetWarning($"Wait Picker #02 place timeout");
                        }
                    }
                    break;
                case ETrayPlaceStep.End:
                    Log.Debug("Tray Place End");
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

        #region Moving Functions
        private void Tray_ReadyPosition_Move()
        {
            XAxis.MoveAbs(CDef.TrayRecipe.XAxis_Change_Position);
        }

        private void Tray_PickPosition_Move(EPickers picker)
        {
            double position, offset;
            
            if (picker == EPickers.Picker1)
            {
                position = CDef.TrayRecipe.XAxis_Tray_Picker1_Pick_First_Position;

                offset = (InputTray.WorkIndex_X - 1) * CDef.CommonRecipe.InputTray_X_Pitch;
                
                position -= offset;

                XAxis.MoveAbs(position);
            }
            else
            {
                position = CDef.TrayRecipe.XAxis_Tray_Picker2_Pick_First_Position;

                offset = (InputTray.WorkIndex_X - 1) * CDef.CommonRecipe.InputTray_X_Pitch;

                position -= offset;

                XAxis.MoveAbs(position);
            }
            Log.Debug($"Tray move {picker} Pick Position #{InputTray.WorkIndex}");
        }

        private void Tray_PlacePosition_Move(EPickers picker)
        {
            double position, offset;

            if (picker == EPickers.Picker1)
            {
                position = CDef.TrayRecipe.XAxis_Tray_Picker1_Place_First_Position;

                offset = (OutputTray.WorkIndex_X - 1) * CDef.CommonRecipe.OutputTray_X_Pitch;
                
                position -= offset;

                XAxis.MoveAbs(position);
            }
            else
            {
                position = CDef.TrayRecipe.XAxis_Tray_Picker2_Place_First_Position;

                offset = (OutputTray.WorkIndex_X - 1) * CDef.CommonRecipe.OutputTray_X_Pitch;

                position -= offset;

                XAxis.MoveAbs(position);
            }
            Log.Debug($"Tray move {picker} Place Position #{OutputTray.WorkIndex}");
        }

        public bool IsTrayPickPosition(EPickers picker)
        {
            double position, offset;

            if (picker == EPickers.Picker1)
            {
                position = CDef.TrayRecipe.XAxis_Tray_Picker1_Pick_First_Position;

                offset = (InputTray.WorkIndex_X - 1) * CDef.CommonRecipe.InputTray_X_Pitch;

                position -= offset;

                return XAxis.IsOnPosition(position);
            }
            else
            {
                position = CDef.TrayRecipe.XAxis_Tray_Picker2_Pick_First_Position;

                offset = (InputTray.WorkIndex_X - 1) * CDef.CommonRecipe.InputTray_X_Pitch;

                position -= offset;

                return XAxis.IsOnPosition(position);
            }
        }

        public bool IsTrayPlacePosition(EPickers picker)
        {
            double position, offset;

            if (picker == EPickers.Picker1)
            {
                position = CDef.TrayRecipe.XAxis_Tray_Picker1_Place_First_Position;

                offset = (OutputTray.WorkIndex_X - 1) * CDef.CommonRecipe.OutputTray_X_Pitch;

                position -= offset;

                return XAxis.IsOnPosition(position);
            }
            else
            {
                position = CDef.TrayRecipe.XAxis_Tray_Picker2_Place_First_Position;

                offset = (OutputTray.WorkIndex_X - 1) * CDef.CommonRecipe.OutputTray_X_Pitch;

                position -= offset;

                return XAxis.IsOnPosition(position);
            }
        }

        private bool IsTrayReadyPosition
        {
            get
            {
                return XAxis.IsOnPosition(CDef.TrayRecipe.XAxis_Change_Position);
            }
        }

        #endregion

        #region Privates
        private ERunMode _RunMode;
        #endregion
    }
}