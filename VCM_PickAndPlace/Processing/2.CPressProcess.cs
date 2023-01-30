using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Processing;
using TopMotion;
using VCM_PickAndPlace.Define;

#if USPCUTTING
namespace VCM_PickAndPlace.Processing
{
    public class CPressProcess : ProcessingBase
    {
#region Constructor(s)
        public CPressProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }
#endregion

#region Properties
#region Motion(s)
        public MotionPlusE PAxis
        {
            get { return (MotionPlusE)CDef.AllAxis.PAxis; }
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
        public bool Flag_PAxisReady
        {
            get
            {
                return PAxis.Status.ActualPosition <= CDef.PressRecipe.PAxis_ReadyPosition + 0.01;
            }
        }
#endregion

#region Data(s)
#endregion

#region Overrider(s)
        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nPRtn = PRtnCode.RtnOk;

            switch ((EPressHomeStep)Step.HomeStep)
            {
                case EPressHomeStep.Start:
                    Log.Debug("Home Start");
                    Step.HomeStep++;
                    break;
                case EPressHomeStep.PAxis_HomeSearch:
                    PAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case EPressHomeStep.PAxis_HomeSearchWait:
                    if (PAxis.Status.IsHomeDone)
                    {
                        Log.Debug($"{PAxis} origin done");
                        HomeStatus.PAxisDone = true;
                        PAxis.ClearPosition();
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
                            CDef.RootProcess.SetWarning($"{PAxis} home search timeout");
                        }
                    }
                    break;
                case EPressHomeStep.End:
                    Log.Debug("Home done");
                    ProcessingStatus = EProcessingStatus.OriginDone;
                    Step.HomeStep++;
                    break;
                default:
                    Sleep(20);
                    break;
            }

            return nPRtn;
        }

        public override PRtnCode ProcessToRun()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;
            switch ((EPressToRunStep)Step.ToRunStep)
            {
                case EPressToRunStep.Start:
                    Log.Debug("To Run Start");
                    Step.ToRunStep++;
                    break;
                case EPressToRunStep.PAxis_ReadyPosition_Move:
                    PAxis_ReadyPosition_Move();
                    Step.ToRunStep++;
                    break;
                case EPressToRunStep.PAxis_ReadyPosition_MoveWait:
                    if (PAxis_ReadyPosition_MoveDone())
                    {
                        Log.Debug($"{PAxis} move Ready_POS DONE!");
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
                            CDef.RootProcess.SetWarning($"{PAxis} moving Ready_POS timeout");
                        }
                    }
                    break;
                case EPressToRunStep.End:
                    Log.Debug("To Run End");
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
                    RunMode = ERunMode.Manual_Press;
                    break;
                case ERunMode.Manual_Press:
                    Running_Press();
                    break;
                case ERunMode.Manual_LoadVision:
                    Sleep(20);
                    break;
                case ERunMode.Manual_Pick:
                    Sleep(20);
                    break;
                case ERunMode.Manual_UnderVision:
                    Sleep(20);
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
                    Sleep(20);
                    break;
            }

            return nRtnCode;
        }
#endregion

#region Run Function(s)
        private void Running_Press()
        {
            switch ((EPressPressStep)Step.RunStep)
            {
                case EPressPressStep.Start:
                    Step.RunStep++;
                    break;
                case EPressPressStep.PressDone_Check:
                    if (CDef.RootProcess.LoadTrayProcess.FirstValidPressRow == -1)
                    {
                        // All row Press done
                        Step.RunStep = (int)EPressPressStep.End;
                    }
                    else
                    {
                        Log.Debug("Press Start");
                        PTimer.TactTimeCounter[0] = PTimer.Now;
                        Step.RunStep++;
                    }
                    break;
                case EPressPressStep.PAxis_ReadyPosition_Check:
                    if (Flag_PAxisReady == false)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Step.RunStep = (int)EPressPressStep.PressStart;
                    }
                    break;
                case EPressPressStep.PAxis_ReadyPosition_PrepareMove:
                    PAxis_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case EPressPressStep.PAxis_ReadyPosition_PrepareMoveWait:
                    if (PAxis_ReadyPosition_MoveDone())
                    {
                        Log.Debug($"{PAxis} move Ready Position Done");
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
                            CDef.RootProcess.SetWarning($"{PAxis} moving Ready Position timeout");
                        }
                    }
                    break;
                case EPressPressStep.PressStart:
                    string TrayName = CDef.RootProcess.LoadTrayProcess.FirstValidPressRow <= 5 ? "Tray 1" : "Tray 2";
                    int rowIndex = (CDef.RootProcess.LoadTrayProcess.FirstValidPressRow - 1) % 5 + 1;
                    Log.Info($"Start press {TrayName} row #{rowIndex}");
                    Step.RunStep++;
                    break;
                case EPressPressStep.Tray_PressPosition_Wait:
                    if (Flags.TrayPress_Position)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case EPressPressStep.PAxis_PressPosition1_Move:
                    PAxis_Press1Position_Move();
                    Step.RunStep++;
                    break;
                case EPressPressStep.PAxis_PressPosition1_MoveWait:
                    if (PAxis_Press1Position_MoveDone())
                    {
                        Log.Debug($"{PAxis} move Press 1 Position Done");
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
                            CDef.RootProcess.SetWarning($"{PAxis} moving Press 1 Position timeout");
                        }
                    }
                    break;
                case EPressPressStep.PAxis_PressPosition2_Move:
                    PAxis_Press2Position_Move();
                    Step.RunStep++;
                    break;
                case EPressPressStep.PAxis_PressPosition2_MoveWait:
                    if (PAxis_Press2Position_MoveDone())
                    {
                        Log.Debug($"{PAxis} move Press 2 Position Done");
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
                            CDef.RootProcess.SetWarning($"{PAxis} moving Press 2 Position timeout");
                        }
                    }
                    break;
                case EPressPressStep.PAxis_ReadyPosition_Move:
                    PAxis_ReadyPosition_Move();
                    Step.RunStep++;
                    break;
                case EPressPressStep.PAxis_ReadyPosition_MoveWait:
                    if (PAxis_ReadyPosition_MoveDone())
                    {
                        Log.Debug($"{PAxis} move Ready Position Done");
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
                            CDef.RootProcess.SetWarning($"{PAxis} moving Ready Position timeout");
                        }
                    }
                    break;
                case EPressPressStep.PressStatus_Update:
                    int currentRow = CDef.RootProcess.LoadTrayProcess.FirstValidPressRow;

                    if (currentRow < 6)
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                            CDef.LoadingTray1.SetSingleCell(TopUI.Define.ECellStatus.PrepareDone, (currentRow - 1) % 5 * 6 + i);
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                            CDef.LoadingTray2.SetSingleCell(TopUI.Define.ECellStatus.PrepareDone, (currentRow - 1) % 5 * 6 + i);

                        }
                    }

                    CDef.RootProcess.LoadTrayProcess.Flag_SingleRow_Pressed = true;

                    if (CDef.RootProcess.LoadTrayProcess.FirstValidPressRow != -1)
                    {
                        Step.RunStep = (int)EPressPressStep.PressStart;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case EPressPressStep.PressEnd:
                    Log.Debug("Press End");
                    Datas.WorkData.TaktTime.Press = 1.0 * (PTimer.Now - PTimer.TactTimeCounter[0]) / 1000;
                    Step.RunStep++;
                    break;
                case EPressPressStep.End:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Stop;
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

        private void Running_LoadVision()
        {
            switch ((EPressLoadVisionStep)Step.RunStep)
            {
                case EPressLoadVisionStep.Start:
                    Step.RunStep++;
                    break;
                case EPressLoadVisionStep.End:
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
                    Sleep(10);
                    break;
            }
        }

        private void Running_Pick()
        {
            switch ((EPressPickStep)Step.RunStep)
            {
                case EPressPickStep.Start:
                    Log.Debug("Pick Start");
                    Step.RunStep++;
                    break;
                case EPressPickStep.End:
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
            switch ((EPressUnderVisionStep)Step.RunStep)
            {
                case EPressUnderVisionStep.Start:
                    Log.Debug("Under Vision Start");
                    Step.RunStep++;
                    break;
                case EPressUnderVisionStep.End:
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

        }

        private void Running_Place()
        {

        }

        private void Running_TrayChange()
        {

        }
#endregion

#region Moving Function(s)
        public void PAxis_ReadyPosition_Move()
        {
            PAxis.MoveAbs(CDef.PressRecipe.PAxis_ReadyPosition);
        }

        public bool PAxis_ReadyPosition_MoveDone()
        {
            SimSleep(300);
            return PAxis.IsOnPosition(CDef.PressRecipe.PAxis_ReadyPosition);
        }

        public void PAxis_Press1Position_Move()
        {
            PAxis.MoveAbs(CDef.PressRecipe.PAxis_Press1_Position);
        }

        public bool PAxis_Press1Position_MoveDone()
        {
            SimSleep(300);
            return PAxis.IsOnPosition(CDef.PressRecipe.PAxis_Press1_Position);
        }

        public void PAxis_Press2Position_Move()
        {
            PAxis.MoveAbs(CDef.PressRecipe.PAxis_Press2_Position);
        }

        public bool PAxis_Press2Position_MoveDone()
        {
            SimSleep(300);
            return PAxis.IsOnPosition(CDef.PressRecipe.PAxis_Press2_Position);
        }
#endregion
    }
}
#endif