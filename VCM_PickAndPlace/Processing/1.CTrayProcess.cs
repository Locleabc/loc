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
using VCM_PickAndPlace.Define;

namespace VCM_PickAndPlace.Processing
{
    public class CTrayProcess : ProcessingBase
    {
        #region Constructor(s)
        public CTrayProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs, ETrays tray)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
            Tray = tray;
        }
        #endregion

        #region Properties
        #region Motion(s)
        public MotionPlusE YAxis
        {
            get
            {
                if (Tray == ETrays.LoadTray)
                {
                    return (MotionPlusE)CDef.AllAxis.Y1Axis;
                }
                else
                {
                    return (MotionPlusE)CDef.AllAxis.Y2Axis;
                }
            }
        }
        #endregion

        #region Position(s)
        public double TrayWork_Position
        {
            get
            {
                double position = 0;
                double offset = 0;

                if (Tray == ETrays.LoadTray)
                {
                    if (CDef.CurrentLoadingTray == null)
                    {
                        return YAxis.Status.ActualPosition;
                    }

                    if (CDef.CurrentLoadingTray == CDef.LoadingTrays[0])
                    {
                        position = CDef.TrayRecipe.Y1Axis_Tray1_Work_Position;
                    }
                    else
                    {
                        position = CDef.TrayRecipe.Y1Axis_Tray2_Work_Position;
                    }

                    offset = CDef.CurrentLoadingTray.WorkStartIndex_Y * CDef.CommonRecipe.LoadingTray_Y_Pitch;
#if USPCUTTING
                    offset *= 2;
#endif
                }
                else
                {
                    if (CDef.CurrentUnloadingTray == null)
                    {
                        return YAxis.Status.ActualPosition;
                    }

                    if (CDef.CurrentUnloadingTray == CDef.UnloadingTrays[0])
                    {
                        position = CDef.TrayRecipe.Y2Axis_Tray1_Work_Position;
                    }
                    else
                    {
                        position = CDef.TrayRecipe.Y2Axis_Tray2_Work_Position;
                    }

                    offset = CDef.CurrentUnloadingTray.WorkStartIndex_Y * CDef.CommonRecipe.UnloadingTray_Y_Pitch;
                }

                return (position - offset);
            }
        }

        public double TrayInspect_Position
        {
            get
            {
                double position = 0;
                double offset = 0;
                double inspectRow = Flags.Inspect.InspectRow;
                double rowCount = CDef.UnloadingTrays[0].RowCount;

                if (Tray == ETrays.LoadTray) return YAxis.Status.ActualPosition;

                if (inspectRow == -1) return YAxis.Status.ActualPosition;

                if (inspectRow < rowCount)
                {
                    position = CDef.TrayRecipe.Y2Axis_Tray1_Work_Position;
                }
                else
                {
                    position = CDef.TrayRecipe.Y2Axis_Tray2_Work_Position;
                }

                offset = (inspectRow % rowCount) * CDef.CommonRecipe.LoadingTray_Y_Pitch;

                return position - offset;
            }
        }

        /// <summary>
        /// Range [-1] or [1 -> 10]
        /// <br/> [-1] all tray is pressed
        /// <br/>[1 -> 5] <see cref="CDef.LoadingTray1"/>
        /// <br/>[6 -> 10] <see cref="CDef.LoadingTray2"/>
        /// </summary>
        public int FirstValidPressRow
        {
            get
            {
                int row = -1;
#if USPCUTTING
                if (Tray == ETrays.UnloadTray) throw new Exception("Unload tray doesn't have press position");

                if (CDef.LoadingTray1.Cells.Count(cell => cell.CellInfo.CellStatus > TopUI.Define.ECellStatus.Processing) > 0)
                {
                    // If tray contain Vision done or Pick&Place done cell => then skip pressing process
                    return row;
                }

                bool pressRowDetected = false;

                if (pressRowDetected == false)
                {
                    for (int i = 1; i <= CDef.LoadingTray1.RowCount; i++)
                    {
                        int preparedCellCount = CDef.LoadingTray1.Cells.Count(cell =>
                                                (cell.CellInfo.CellIndex <= i * 6)
                                                && (cell.CellInfo.CellIndex > (i - 1) * 6)
                                                && cell.CellInfo.CellStatus >= TopUI.Define.ECellStatus.PrepareDone
                                                && cell.CellInfo.CellStatus != TopUI.Define.ECellStatus.Processing);

                        // Row number i is not be pressed yet.
                        if (preparedCellCount == 0)
                        {
                            pressRowDetected = true;
                            row = i;

                            break;
                        }
                    }
                }

                if (CDef.LoadingTray2.Cells.Count(cell => cell.CellInfo.CellStatus > TopUI.Define.ECellStatus.Processing) > 0)
                {
                    // If tray contain Vision done or Pick&Place done cell => then skip pressing process
                    return row;
                }

                if (pressRowDetected == false)
                {
                    for (int i = 1; i <= CDef.LoadingTray2.RowCount; i++)
                    {
                        int preparedCellCount = CDef.LoadingTray2.Cells.Count(cell =>
                                                (cell.CellInfo.CellIndex <= i * 6)
                                                && (cell.CellInfo.CellIndex > (i - 1) * 6)
                                                && cell.CellInfo.CellStatus >= TopUI.Define.ECellStatus.PrepareDone
                                                && cell.CellInfo.CellStatus != TopUI.Define.ECellStatus.Processing);

                        // Row number i is not be pressed yet.
                        if (preparedCellCount == 0)
                        {
                            pressRowDetected = true;
                            row = i + 5;

                            break;
                        }
                    }
                }
#endif
                return row;
            }
        }

#if USPCUTTING
        public double TrayPress_Position
        {
            get
            {
                double position = 0;
                double offset = 0;

                if (FirstValidPressRow == -1) return position;

                if (FirstValidPressRow > 5)
                {
                    position = CDef.TrayRecipe.Y1Axis_Tray2_Press_Position;
                }
                else if (FirstValidPressRow >= 1)
                {
                    position = CDef.TrayRecipe.Y1Axis_Tray1_Press_Position;
                }
                offset = (FirstValidPressRow - 1) % 5 * CDef.CommonRecipe.LoadingTray_Y_Pitch;
#if USPCUTTING
                offset *= 2;
#endif

                return position - offset;
            }
        }
#endif
        #endregion

        public ETrays Tray { get; private set; }

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

        public bool Head { get; private set; }

        /// <summary>
        /// If ManualTrayChange is set, in ToRun mode tray will not be move to ready position
        /// </summary>
        public bool ManualTrayChange { get; set; }
        #endregion

        #region Flag(s)
        public bool Flag_TrayChange_Position
        {
            get
            {
                if (Tray == ETrays.LoadTray)
                {
                    return YAxis.IsOnPosition(CDef.TrayRecipe.Y1Axis_Change_Position);
                }
                else
                {
                    return YAxis.IsOnPosition(CDef.TrayRecipe.Y2Axis_Change_Position);
                }
            }
        }

        public bool Flag_TrayWork_Position
        {
            get
            {
                if (Tray == ETrays.LoadTray && CDef.CurrentLoadingTray == null) return false;
                if (Tray == ETrays.UnloadTray && CDef.CurrentUnloadingTray == null) return false;

                return YAxis.IsOnPosition(TrayWork_Position);
            }
        }

        public bool Flag_Tray_InspectPosition
        {
            get
            {
                return YAxis.IsOnPosition(TrayInspect_Position);
            }
        }

#if USPCUTTING
        public bool Flag_TrayPress_Position
        {
            get
            {
                return YAxis.IsOnPosition(TrayPress_Position);
            }
        }
#endif

        public bool Flag_SingleRow_Pressed
        {
            get; set;
        }

        public bool Flag_Tray_PickPlace_Done { get; set; }
        #endregion

        #region Data(s)
        #endregion

        #region Overrider(s)
        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtnCode = PRtnCode.RtnOk;

            switch ((ETrayHomeStep)Step.HomeStep)
            {
                case ETrayHomeStep.Start:
                    Log.Debug("Home start");
                    Step.HomeStep++;
                    break;
                case ETrayHomeStep.AllZAxis_HomeWait:
                    if (HomeStatus.IsAllZAxisHomeDone())
                    {
                        Step.HomeStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETrayHomeStep.YAxis_HomeSearch:
                    YAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case ETrayHomeStep.YAxis_HomeSearchWait:
                    if (YAxis.Status.IsHomeDone)
                    {
                        Log.Debug($"{YAxis} origin done");
                        YAxis.ClearPosition();
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
                            CDef.RootProcess.SetWarning($"{YAxis} home search timeout");
                        }
                    }
                    break;
                case ETrayHomeStep.End:
                    Log.Debug("Home done");
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

            switch ((ETrayToRunStep)Step.ToRunStep)
            {
                case ETrayToRunStep.Start:
                    Log.Debug("To Run Start");
                    Step.ToRunStep++;
                    break;
                case ETrayToRunStep.AllZAxis_ReadyWait:
                    if (Flags.AllZAxis_Ready)
                    {
                        Step.ToRunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETrayToRunStep.RootProcess_ModeCheck:
                    if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                    {
                        if (ManualTrayChange == true)
                        {
                            RunMode = ERunMode.Manual_TrayChange;
                        }
                        else
                        {
                            RunMode = ERunMode.Stop;
                        }
                        ProcessingStatus = EProcessingStatus.ToRunDone;
                        Step.ToRunStep = (int)ETrayToRunStep.End + 1;
                    }
                    else
                    {
                        Step.ToRunStep++;
                    }
                    break;
                case ETrayToRunStep.TrayType_Check:
                    if (Tray == ETrays.UnloadTray)
                    {
                        Step.ToRunStep = (int)ETrayToRunStep.YAxis_WorkPosition_Move;
                    }
                    else
                    {
                        Step.ToRunStep++;
                    }
                    break;
#if USPCUTTING
                case ETrayToRunStep.PressCheck:
                    if (CDef.RootProcess.LoadTrayProcess.FirstValidPressRow == -1)
                    {
                        Step.ToRunStep = (int)ETrayToRunStep.YAxis_WorkPosition_Move;
                    }
                    else
                    {
                        Step.ToRunStep++;
                    }
                    break;
                case ETrayToRunStep.YAxis_PressPosition_Move:
                    YAxis_PressPosition_Move();
                    Step.ToRunStep++;
                    break;
                case ETrayToRunStep.YAxis_PressPosition_MoveWait:
                    if (YAxis_PressPosition_MoveDone())
                    {
                        Log.Debug($"{YAxis} move Press Position done");
                        Step.ToRunStep = (int)ETrayToRunStep.End;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisHomeSearch_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{YAxis} move Press Position timeout");
                        }
                    }
                    break;
#endif
                case ETrayToRunStep.YAxis_WorkPosition_Move:
                    YAxis_WorkPosition_Move();
                    Step.ToRunStep++;
                    break;
                case ETrayToRunStep.YAxis_WorkPosition_MoveWait:
                    if (YAxis_WorkPosition_MoveDone())
                    {
                        Log.Debug($"{YAxis} move Work Position done");
                        Step.ToRunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.AxisHomeSearch_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            CDef.RootProcess.SetWarning($"{YAxis} move Work Position timeout");
                        }
                    }
                    break;
                case ETrayToRunStep.End:
                    Log.Debug("To Run End");
                    this.RunMode = CDef.RootProcess.RunMode;
                    ProcessingStatus = EProcessingStatus.ToRunDone;
                    Step.RunStep = 0;
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
                    ManualTrayChange = false;
                    if (Tray == ETrays.LoadTray)
                    {
#if USPCUTTING
                        RunMode = ERunMode.Manual_Press;
#else
                        RunMode = ERunMode.Manual_LoadVision;
#endif
                    }
                    else
                    {
                        // If all cell P&P done, set mode to inspect
                        if (CDef.CurrentUnloadingTray != null)
                        {
                            RunMode = ERunMode.Manual_Place;
                        }
                        else
                        {
                            RunMode = ERunMode.Manual_Inspect;
                        }
                    }
                    break;
#if USPCUTTING
                case ERunMode.Manual_Press:
                    Running_Press();
                    break;
#endif

                case ERunMode.Manual_LoadVision:
                    RunMode = ERunMode.Manual_Pick;
                    break;
                case ERunMode.Manual_Pick:
                    Running_PickPlace(EPickPlace.PICK);
                    break;
                case ERunMode.Manual_UnderVision:
                    Sleep(20);
                    break;
                case ERunMode.Manual_UnloadVision:
                    RunMode = ERunMode.Manual_Place;
                    break;
                case ERunMode.Manual_Place:
                    Running_PickPlace(EPickPlace.PLACE);
                    break;
                case ERunMode.Manual_Inspect:
                    Running_Inspect();
                    break;
                case ERunMode.Manual_TrayChange:
                    Running_TrayChange(Tray);
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
            switch ((ETrayPressStep)Step.RunStep)
            {
                case ETrayPressStep.Start:
                    Log.Debug("Press Start");
                    Step.RunStep++;
                    break;
                case ETrayPressStep.AllZAxis_ReadyPosition_Wait:
                    if (Flags.AllZAxis_Ready)
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayPressStep.TrayType_Check:
                    if (Tray == ETrays.UnloadTray)
                    {
                        Step.RunStep = (int)ETrayPressStep.End;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayPressStep.PressDone_Check:
                    if (CDef.RootProcess.LoadTrayProcess.FirstValidPressRow == -1)
                    {
                        Step.RunStep = (int)ETrayPressStep.YAxis_WorkPosition_Move;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayPressStep.PressStart:
                    Log.Debug("Press Start");
                    Step.RunStep++;
                    break;
                case ETrayPressStep.PressAxis_ReadyPosition_Wait:
                    if (Flags.PressAxis_Ready)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETrayPressStep.YAxis_PressPosition_Move:
                    YAxis_PressPosition_Move();
                    Step.RunStep++;
                    break;
                case ETrayPressStep.YAxis_PressPosition_Wait:
                    if (Flag_TrayPress_Position)
                    {
                        Log.Debug($"{YAxis} move Press position done");
                        Flag_SingleRow_Pressed = false;
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
                            CDef.RootProcess.SetWarning($"{YAxis} move Press Position TimeOut");
                        }
                    }
                    break;
                case ETrayPressStep.PressWait:
                    if (Flag_SingleRow_Pressed == false)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayPressStep.PressStatus_Check:
                    if (CDef.RootProcess.LoadTrayProcess.FirstValidPressRow != -1)
                    {
                        Step.RunStep = (int)ETrayPressStep.PressAxis_ReadyPosition_Wait;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayPressStep.YAxis_ChangePosition_Move:
                    YAxis_ChangePosition_Move();
                    Step.RunStep++;
                    break;
                case ETrayPressStep.YAxis_ChangePosition_MoveWait:
                    if (Flag_TrayChange_Position)
                    {
                        Log.Debug($"{YAxis} move Change Position done");
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
                            CDef.RootProcess.SetWarning($"{YAxis} move Change Position TimeOut");
                        }
                    }
                    break;
                case ETrayPressStep.Message_Display:
                    CDef.MessageViewModel.Show(Language.GetString("str_RemoveDummy") + "\n" + Language.GetString("str_PushStartButton"));
                    Step.RunStep++;
                    break;
                case ETrayPressStep.PressEnd:
                    TrayCell tray1StartCell = CDef.LoadingTray1.Cells.Last(cell => cell.CellInfo.CellStatus == TopUI.Define.ECellStatus.PrepareDone
                                                                                    || cell.CellInfo.CellStatus == TopUI.Define.ECellStatus.Processing
                                                                                    || cell.CellInfo.CellStatus == TopUI.Define.ECellStatus.Empty);
                    if (tray1StartCell != null)
                    {
                        CDef.LoadingTray1.SetSetOfCell(TopUI.Define.ECellStatus.Processing, tray1StartCell.CellInfo.CellIndex);
                    }

                    TrayCell tray2StartCell = CDef.LoadingTray2.Cells.Last(cell => cell.CellInfo.CellStatus == TopUI.Define.ECellStatus.PrepareDone
                                                                                    || cell.CellInfo.CellStatus == TopUI.Define.ECellStatus.Processing
                                                                                    || cell.CellInfo.CellStatus == TopUI.Define.ECellStatus.Empty);
                    if (tray2StartCell != null)
                    {
                        CDef.LoadingTray2.SetSetOfCell(TopUI.Define.ECellStatus.Processing, tray2StartCell.CellInfo.CellIndex);
                    }

                    CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    this.RunMode = ERunMode.Stop;
                    break;
                case ETrayPressStep.YAxis_WorkPosition_Move:
                    YAxis_WorkPosition_Move();
                    Step.RunStep++;
                    break;
                case ETrayPressStep.YAxis_WorkPosition_MoveWait:
                    if (Flag_TrayWork_Position)
                    {
                        Log.Debug($"{YAxis} move Work Position done");
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
                            CDef.RootProcess.SetWarning($"{YAxis} move Work Position TimeOut");
                        }
                    }
                    break;
                case ETrayPressStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Pick;
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

        private void Running_PickPlace(EPickPlace mode)
        {
            switch ((ETrayPickStep)Step.RunStep)
            {
                case ETrayPickStep.Start:
                    Log.Debug($"{mode} Start");
                    Step.RunStep++;
                    break;
                case ETrayPickStep.TrayType_Check:
                    if (CDef.CurrentLoadingTray == null && CDef.CurrentUnloadingTray == null)
                    {
                        RunMode = ERunMode.Manual_Inspect;
                    }
                    else if (CDef.CurrentUnloadingTray == null)
                    {
                        RunMode = ERunMode.Manual_Inspect;
                    }
                    // TODO: EXCEPTION OCCURS IN THIS LINE
                    //else if (CDef.CurrentLoadingTray == null && CDef.CurrentUnloadingTray?.WorkStartIndex <= 28)
                    //{
                    //    RunMode = ERunMode.Manual_Inspect;
                    //}
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayPickStep.NewIndex_PickPlace_Wait:
                    bool flag_PickPlaceDone_1 = Tray == ETrays.LoadTray ? HeadWorkStatus.PickDone : HeadWorkStatus.PlaceDone;

                    if (flag_PickPlaceDone_1)
                    {
                        if (Tray == ETrays.LoadTray && CDef.CurrentLoadingTray == null)
                        {
                            RunMode = ERunMode.Manual_Inspect;
                        }
                        else if (Tray == ETrays.UnloadTray && CDef.CurrentUnloadingTray == null)
                        {
                            RunMode = ERunMode.Manual_Inspect;
                        }
                        else
                        {
                            Sleep(10);
                        }
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case ETrayPickStep.HeadZ_ReadyWait:
                    if (Flags.AllHead_ZReady)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETrayPickStep.YAxis_WorkPosition_Move:
                    YAxis_WorkPosition_Move();
                    Step.RunStep++;
                    break;
                case ETrayPickStep.YAxis_WorkPosition_MoveWait:
                    if (Flag_TrayWork_Position)
                    {
                        Log.Debug($"{YAxis} move {mode} Position done");
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
                            CDef.RootProcess.SetWarning($"{YAxis} move {mode} Position TimeOut");
                        }
                    }
                    break;
                case ETrayPickStep.Pick_Wait:
                    bool flag_PickPlaceDone = Tray == ETrays.LoadTray ? HeadWorkStatus.PickDone : HeadWorkStatus.PlaceDone;

                    if (flag_PickPlaceDone)
                    {
                        Flag_Tray_PickPlace_Done = true;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case ETrayPickStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        Step.RunStep = (int)ETrayPickStep.Start;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Pick ||
                             CDef.RootProcess.RunMode == ERunMode.Manual_Place)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
            }
        }

        private void Running_TrayChange(ETrays tray)
        {
            switch ((ETrayTrayChangeStep)Step.RunStep)
            {
                case ETrayTrayChangeStep.Start:
                    Log.Debug($"{tray} tray change start.");
                    Step.RunStep++;
                    break;
                case ETrayTrayChangeStep.AllZAxis_ReadyWait:
                    if (Flags.AllHead_ZReady)
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
                            CDef.RootProcess.SetWarning($"{tray} tray change waiting Z ready timeout");
                        }
                    }
                    break;
                case ETrayTrayChangeStep.YAxis_TrayChangePosition_Move:
                    YAxis_ChangePosition_Move();
                    Step.RunStep++;
                    break;
                case ETrayTrayChangeStep.YAxis_TrayChangePosition_MoveWait:
                    if (Flag_TrayChange_Position)
                    {
                        Log.Debug($"{YAxis} move Change Position done");
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
                            CDef.RootProcess.SetWarning($"{YAxis} move Change Position TimeOut");
                        }
                    }
                    break;
                case ETrayTrayChangeStep.CheckIf_ManualTrayChange:
                    if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                    {
                        // Manual Tray change, display message and END
                        CDef.MessageViewModel.Show(Language.GetString("str_Change", $"{Tray}"));
                        if (Tray == ETrays.LoadTray)
                        {
                            if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                            {
                                foreach (var _tray in CDef.LoadingTrays)
                                {
                                    _tray.ResetCommand.Execute(null);
                                }
                            }
                        }
                        else
                        {
                            if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                            {
                                foreach (var _tray in CDef.UnloadingTrays)
                                {
                                    _tray.ResetCommand.Execute(null);
                                }
                            }
                        }
                        Step.RunStep = (int)ETrayTrayChangeStep.End;
                    }
                    else
                    {
                        // Auto Run
                        Step.RunStep++;
                    }
                    break;
                case ETrayTrayChangeStep.TrayChangeMessage_Display:
                    if (CDef.RootProcess.LoadTrayProcess.Flag_TrayChange_Position && CDef.RootProcess.UnloadTrayProcess.Flag_TrayChange_Position)
                    {
                        CDef.MessageViewModel.Show(Language.GetString("str_ChangeBothTray") + "\n" + Language.GetString("str_PushStartButton"));

                        if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                        {
                            foreach (var _tray in CDef.LoadingTrays)
                            {
                                _tray.ResetCommand.Execute(null);
                            }
                            foreach (var _tray in CDef.UnloadingTrays)
                            {
                                _tray.ResetCommand.Execute(null);
                            }
                        }

                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    else
                    {
                        CDef.MessageViewModel.ShowDialog(Language.GetString("str_Change", $"{Tray}"));
                        if (Tray == ETrays.LoadTray)
                        {
                            if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                            {
                                foreach (var _tray in CDef.LoadingTrays)
                                {
                                    _tray.ResetCommand.Execute(null);
                                }
                            }
                        }
                        else
                        {
                            if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                            {
                                foreach (var _tray in CDef.UnloadingTrays)
                                {
                                    _tray.ResetCommand.Execute(null);
                                }
                            }
                        }
                        Step.RunStep++;
                    }
                    break;
                case ETrayTrayChangeStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Stop;
//                        if (Tray == ETrays.LoadTray)
//                        {
//#if USPCUTTING
//                            this.RunMode = ERunMode.Manual_Press;
//#else
//                            this.RunMode = ERunMode.Manual_Pick;
//#endif
//                        }
//                        else
//                        {
//                            this.RunMode = ERunMode.Manual_Place;
//                        }
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_TrayChange)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Running_Inspect()
        {
            switch ((ETrayInspectStep)Step.RunStep)
            {
                case ETrayInspectStep.Start:
                    if (Tray == ETrays.UnloadTray && CDef.GlobalRecipe.SkipBallInspect == false)
                    {
                        Log.Debug($"{Tray} inspect start.");
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)ETrayInspectStep.End;
                    }
                    break;
                case ETrayInspectStep.HeadZ_ReadyWait:
                    if (Flags.AllHead_ZReady)
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
                            CDef.RootProcess.SetWarning($"{Tray} inspect waiting Z ready timeout");
                        }
                    }
                    break;
                case ETrayInspectStep.YAxis_InspectPosition_Move:
                    if (Flags.Inspect.Tray_InspectPosition)
                    {
                        if (Flags.Inspect.IsAllCellInspectDone == false)
                        {
                            Sleep(50);
                        }
                        else
                        {
                            this.RunMode = ERunMode.Manual_TrayChange;
                        }    
                    }
                    else
                    {
                        YAxis_InspectPosition_Move();
                        Step.RunStep++;
                    }
                    break;
                case ETrayInspectStep.YAxis_InspectPosition_MoveWait:
                    if (Flag_Tray_InspectPosition)
                    {
                        Log.Debug($"{YAxis} move Inspect Position done");
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
                            CDef.RootProcess.SetWarning($"{YAxis} move Change Position TimeOut");
                        }
                    }
                    break;
                case ETrayInspectStep.CheckIfInspectAllDone:
                    if (Flags.Inspect.IsAllCellInspectDone == false
                        /*&& CDef.RootProcess.RunMode == ERunMode.AutoRun*/)
                    {
                        Step.RunStep = (int)ETrayInspectStep.HeadZ_ReadyWait;
                    }
                    else
                    {
                        Step.RunStep = (int)ETrayInspectStep.End;
                    }
                    break;
                case ETrayInspectStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_TrayChange;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Inspect)
                    {
                        Log.Debug($"{Tray} inspect done");
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Moving Function(s)
#if USPCUTTING
        public void YAxis_PressPosition_Move()
        {
            YAxis.MoveAbs(TrayPress_Position);
        }

        public bool YAxis_PressPosition_MoveDone()
        {
            return YAxis.IsOnPosition(TrayPress_Position);
        }
#endif

        public void YAxis_WorkPosition_Move()
        {
            YAxis.MoveAbs(TrayWork_Position);
        }

        public bool YAxis_WorkPosition_MoveDone()
        {
            return YAxis.IsOnPosition(TrayWork_Position);
        }

        public void YAxis_InspectPosition_Move()
        {
            YAxis.MoveAbs(TrayInspect_Position);
        }

        public void YAxis_ChangePosition_Move()
        {
            if (Tray == ETrays.LoadTray)
            {
                YAxis.MoveAbs(CDef.TrayRecipe.Y1Axis_Change_Position);
            }
            else
            {
                YAxis.MoveAbs(CDef.TrayRecipe.Y2Axis_Change_Position);
            }
        }
        #endregion

        #region Privates
        private ERunMode _RunMode;
        #endregion
    }
}