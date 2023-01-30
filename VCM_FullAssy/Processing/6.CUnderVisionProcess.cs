using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Processing;
using TopVision;
using TopVision.Models;
using VCM_FullAssy.Define;

namespace VCM_FullAssy.Processing
{
    public class CUnderVisionProcess : ProcessingBase
    {
        #region Properties
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

        public IVisionResult Data_UnderInspect_Result
        {
            get; set;
        } = new VisionResultBase();
        #endregion

        #region Flags
        public bool Flag_UnderVision_InspectWork_Request { get; set; }
        public bool Flag_UnderVision_InspectDone { get; set; }
        #endregion

        public CUnderVisionProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }

        public enum UnderVisionToRunStep
        {
            ToRunStart,
            ToRunDecision,
            ToRunEnd,
        }

        public override PRtnCode ProcessToRun()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch ((UnderVisionToRunStep)Step.ToRunStep)
            {
                case UnderVisionToRunStep.ToRunStart:
                    Log.Debug("ToRun start");
                    Flag_UnderVision_InspectDone = false;
                    Flag_UnderVision_InspectWork_Request = false;
                    Step.ToRunStep++;
                    break;
                case UnderVisionToRunStep.ToRunDecision:
                    RunMode = CDef.RootProcess.RunMode;
                    Step.ToRunStep++;
                    break;
                case UnderVisionToRunStep.ToRunEnd:
                    // Total, Pick, Place takt time reset
                    PTimer.TactTimeCounter = new System.Collections.ObjectModel.ObservableCollection<int> { 0, 0, 0 };

                    Log.Debug("ToRun end");
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
                case ERunMode.AutoRun:
                    Run_UnderVision();
                    break;
                case ERunMode.Manual_UnderVision:
                    Run_UnderVision();
                    break;
                case ERunMode.Stop:
                case ERunMode.Manual_LoadVision:
                case ERunMode.Manual_Pick:
                case ERunMode.Manual_UnloadVision:
                case ERunMode.Manual_Place:
                case ERunMode.Manual_TrayChange:
                default:
                    Sleep(20);
                    break;
            }

            return nRtn;
        }

        public enum UnderVisionUnderVisionStep
        {
            UnderVisionStart,
            InspectDone_FlagClearWait,
            Transfer_UnderVision_PositionWait,
            Head_UnderVision_PositionWait,
            Grab,
            Grab_Wait,
            Inspect,
            Inspect_Wait,
            ApplyVisionInspectResult,
            UnderVision_FlagUpdate,
            UnderVisionEnd,
        }

        private void Run_UnderVision()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                CDef.GlobalRecipe.SkipUnderVision)
            {
                this.RunMode = ERunMode.Manual_UnloadVision;
                return;
            }

            switch ((UnderVisionUnderVisionStep)Step.RunStep)
            {
                case UnderVisionUnderVisionStep.UnderVisionStart:
                    Step.RunStep++;
                    break;
                case UnderVisionUnderVisionStep.InspectDone_FlagClearWait:
                    if (Flag_UnderVision_InspectWork_Request == true)
                    {
                        Log.Debug("UnderVision Start");
                        // Clear request (prevent repeat)
                        Flag_UnderVision_InspectWork_Request = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(20);
                    }
                    break;
                case UnderVisionUnderVisionStep.Transfer_UnderVision_PositionWait:
                    if (Flags.Transfer_UnderVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(20);
                    }
                    break;
                case UnderVisionUnderVisionStep.Head_UnderVision_PositionWait:
                    if (Flags.Head_UnderVisionPosition_MoveDone)
                    {
                        Flags.Head_UnderVisionPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(20);
                    }
                    break;
                case UnderVisionUnderVisionStep.Grab:
                    ImageGrab(EVisionArea.UNDER);
                    Step.RunStep++;
                    break;
                case UnderVisionUnderVisionStep.Grab_Wait:
                    if (CDef.BotCamera.IsGrabDone == true)
                    {
                        if (CDef.BotCamera.GrabResult.RtnCode == TopVision.EGrabRtnCode.OK)
                        {
                            Log.Debug("Under vision image grab done");
                            Step.RunStep++;
                        }
                        else
                        {
                            Log.Warn("Under vision image grab failed");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.VisionGrab_Timeout * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Under vision image grab timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnderVisionUnderVisionStep.Inspect:
                    UnderVision_Inspect(EVisionArea.UNDER);
                    Step.RunStep++;
                    break;
                case UnderVisionUnderVisionStep.Inspect_Wait:
                    if (UnderVision_InspectDone())
                    {
                        Log.Debug("Image inspect done");
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case UnderVisionUnderVisionStep.ApplyVisionInspectResult:
                    Data_UnderInspect_Result = VisionResult();
                    Step.RunStep++;
                    break;
                case UnderVisionUnderVisionStep.UnderVision_FlagUpdate:
                    Flag_UnderVision_InspectDone = true;
                    Step.RunStep++;
                    break;
                case UnderVisionUnderVisionStep.UnderVisionEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnderVision;
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

        #region Run Functions
        public void ImageGrab(EVisionArea area)
        {
            CDef.IO.Output.LightUnder = true;
            CDef.MainViewModel.VisionAutoVM.GrabCommand.Execute(area);
        }

        public void UnderVision_Inspect(EVisionArea area)
        {
            CDef.MainViewModel.VisionAutoVM.InspectCommand.Execute(area);
            CDef.IO.Output.LightUnder = false;
        }

        public bool UnderVision_InspectDone()
        {
            return CDef.MainViewModel.VisionAutoVM.UnderVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
        }
        #endregion

        #region Privates
        private IVisionResult VisionResult()
        {
            return CDef.MainViewModel.VisionAutoVM.UnderVisionProcess.Result;
        }
        #endregion
    }
}
