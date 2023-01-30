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
    public class CUpperVisionProcess : ProcessingBase
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
        public IVisionResult Data_LoadingInspect_Result
        {
            get; set;
        } = new VisionResultBase();

        public IVisionResult Data_UnloadingInspect_Result
        {
            get; set;
        } = new VisionResultBase();
        #endregion

        #region Flags
        public bool Flag_Upper_LoadVision_InspectDone
        {
            get
            {
                return CDef.RootProcess.CurrentLoadTray == null
                    ? false
                    : CDef.RootProcess.CurrentLoadTray.GetCellStatus(CDef.RootProcess.CurrentLoadTray.WorkStartIndex) >= TopUI.Define.ECellStatus.NGVision;
            }
        }

        public bool Flag_Upper_UnloadVision_InspectDone
        {
            get
            {
                return CDef.RootProcess.CurrentUnloadTray == null
                    ? false
                    : CDef.RootProcess.CurrentUnloadTray.GetCellStatus(CDef.RootProcess.CurrentUnloadTray.WorkStartIndex) >= TopUI.Define.ECellStatus.NGVision;
            }
        }
        #endregion
        public CUpperVisionProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }

        public enum UpperVisionToRunStep
        {
            ToRunStart,
            ToRunDecision,
            ToRunEnd,
        }

        public override PRtnCode ProcessToRun()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch ((UpperVisionToRunStep)Step.ToRunStep)
            {
                case UpperVisionToRunStep.ToRunStart:
                    Log.Debug("ToRun start");
                    Step.ToRunStep++;
                    break;
                case UpperVisionToRunStep.ToRunDecision:
                    RunMode = CDef.RootProcess.RunMode;
                    Step.ToRunStep++;
                    break;
                case UpperVisionToRunStep.ToRunEnd:
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
                default:
                    break;
            }
            return nRtn;
        }

        #region Run Functions
        public enum UpperVisionLoadVisionStep
        {
            LoadVisionStart,
            CheckTrayStatus,
            Tray_LoadVisionPosition_MoveWait,
            Transfer_LoadVisionPosition_MoveWait,
            Head_LoadVisionPosition_MoveWait,
            LoadVision_GrabImage,
            LoadVision_GrabImageWait,
            LoadVision_Inspect,
            LoadVision_InspecWait,
            ApplyVisionInspectResult,
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

            switch ((UpperVisionLoadVisionStep)Step.RunStep)
            {
                case UpperVisionLoadVisionStep.LoadVisionStart:
                    Step.RunStep++;
                    break;
                case UpperVisionLoadVisionStep.CheckTrayStatus:
                    if (Flags.Upper_LoadVision_InspectDone == false)
                    {
                        Log.Debug("UpperVision Load Vision start");
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(20);
                    }
                    break;
                case UpperVisionLoadVisionStep.Tray_LoadVisionPosition_MoveWait:
                    if (Flags.Tray_LoadVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case UpperVisionLoadVisionStep.Transfer_LoadVisionPosition_MoveWait:
                    if (Flags.Transfer_LoadVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case UpperVisionLoadVisionStep.Head_LoadVisionPosition_MoveWait:
                    if (Flags.Head_LoadVisionPosition_MoveDone)
                    {
                        Flags.Head_LoadVisionPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case UpperVisionLoadVisionStep.LoadVision_GrabImage:
                    ImageGrab(EVisionArea.LOAD);
                    Step.RunStep++;
                    break;
                case UpperVisionLoadVisionStep.LoadVision_GrabImageWait:
                    if (CDef.TopCamera.IsGrabDone == true)
                    {
                        if (CDef.TopCamera.GrabResult.RtnCode == TopVision.EGrabRtnCode.OK)
                        {
                            Log.Debug("Grab Image Load Vision Done");
                            Step.RunStep++;
                        }
                        else
                        {
                            Log.Warn("Vision Image Grab failed");
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
                            Log.Warn("Vision Image Grab Load Vision timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UpperVisionLoadVisionStep.LoadVision_Inspect:
                    LoadVision_Inspect(EVisionArea.LOAD);
                    Step.RunStep++;
                    break;
                case UpperVisionLoadVisionStep.LoadVision_InspecWait:
                    if (LoadVision_InspectDone(EVisionArea.LOAD))
                    {
                        Log.Debug("Inspect Image Load Vision Done");
                        CDef.RootProcess.CurrentLoadTray.SetSetOfCell(TopUI.Define.ECellStatus.OKVision, CDef.RootProcess.CurrentLoadTray.WorkStartIndex);
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.VisionInspect_Timeout * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Vision Image Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UpperVisionLoadVisionStep.ApplyVisionInspectResult:
                    Data_LoadingInspect_Result = VisionResult(EVisionArea.LOAD);
                    Step.RunStep++;
                    break;
                case UpperVisionLoadVisionStep.LoadVisionEnd:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnderVision;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_LoadVision)
                    {
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
            }
        }

        public enum UpperVisionPickStep
        {
            PickDecision,
        }
        public void Running_Pick()
        {
            switch ((UpperVisionPickStep)Step.RunStep)
            {
                case UpperVisionPickStep.PickDecision:
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

        public enum UpperVisionUnderVisionStep
        {
            UnderVisionDecision,
        }
        public void Running_UnderVision()
        {
            switch ((UpperVisionUnderVisionStep)Step.RunStep)
            {
                case UpperVisionUnderVisionStep.UnderVisionDecision:
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

        public enum UpperVisionUnloadVisionStep
        {
            UnloadVisionStart,
            CheckTrayStatus,
            Tray_UnloadVisionPosition_MoveWait,
            Transfer_UnloadVisionPosition_MoveWait,
            Head_UnloadVisionPosition_MoveWait,
            Grab_Image_UnloadVision,
            Grab_Image_UnloadVision_Wait,
            Inspect_Image_UnloadVision,
            Inspect_Image_UnloadVision_Wait,
            ApplyVisionInspectResult,
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

            switch ((UpperVisionUnloadVisionStep)Step.RunStep)
            {
                case UpperVisionUnloadVisionStep.UnloadVisionStart:
                    Step.RunStep++;
                    break;
                case UpperVisionUnloadVisionStep.CheckTrayStatus:
                    if (Flags.Upper_UnloadVision_InspectDone == false)
                    {
                        Log.Debug("UpperVision Unload Vision start");
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(20);
                    }
                    break;
                case UpperVisionUnloadVisionStep.Tray_UnloadVisionPosition_MoveWait:
                    if (Flags.Tray_UnloadVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case UpperVisionUnloadVisionStep.Transfer_UnloadVisionPosition_MoveWait:
                    if (Flags.Transfer_UnloadVisionPosition_MoveDone)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case UpperVisionUnloadVisionStep.Head_UnloadVisionPosition_MoveWait:
                    if (Flags.Head_UnloadVisionPosition_MoveDone)
                    {
                        Flags.Head_UnloadVisionPosition_MoveDone = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;
                case UpperVisionUnloadVisionStep.Grab_Image_UnloadVision:
                    ImageGrab(EVisionArea.UNLOAD);
                    Step.RunStep++;
                    break;
                case UpperVisionUnloadVisionStep.Grab_Image_UnloadVision_Wait:
                    if (CDef.TopCamera.IsGrabDone == true)
                    {
                        if (CDef.TopCamera.GrabResult.RtnCode == EGrabRtnCode.OK)
                        {
                            Log.Debug("Grab Image Unload Vision Done");
                            Step.RunStep++;
                        }
                        else
                        {
                            Log.Warn("Unload Vision Image Grab failed");
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
                            Log.Warn("Unload Vision Image Grab timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UpperVisionUnloadVisionStep.Inspect_Image_UnloadVision:
                    LoadVision_Inspect(EVisionArea.UNLOAD);
                    Step.RunStep++;
                    break;
                case UpperVisionUnloadVisionStep.Inspect_Image_UnloadVision_Wait:
                    if (LoadVision_InspectDone(EVisionArea.UNLOAD))
                    {
                        Log.Debug("Inspect Image Vision_Unload Vision Done");
                        CDef.RootProcess.CurrentUnloadTray.SetSetOfCell(TopUI.Define.ECellStatus.OKVision, CDef.RootProcess.CurrentUnloadTray.WorkStartIndex);
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.StepLeadTime < CDef.CommonRecipe.VisionGrab_Timeout * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Vision Image Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UpperVisionUnloadVisionStep.ApplyVisionInspectResult:
                    Data_UnloadingInspect_Result = VisionResult(EVisionArea.UNLOAD);
                    Step.RunStep++;
                    break;
                case UpperVisionUnloadVisionStep.UnloadVisionEnd:
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
            }
        }

        public enum UpperVisionPlaceStep
        {
            PlaceDecision,
        }
        public void Running_Place()
        {
            switch ((UpperVisionPlaceStep)Step.RunStep)
            {
                case UpperVisionPlaceStep.PlaceDecision:
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
            }
        }
        #endregion

        #region Methods
        public void ImageGrab(EVisionArea area)
        {
            CDef.IO.Output.LightUpper = true;
            CDef.MainViewModel.VisionAutoVM.GrabCommand.Execute(area);
        }

        public void LoadVision_Inspect(EVisionArea area)
        {
            CDef.MainViewModel.VisionAutoVM.InspectCommand.Execute(area);
            CDef.IO.Output.LightUpper = false;
        }

        public bool LoadVision_InspectDone(EVisionArea area)
        {
            if (area == EVisionArea.LOAD)
            {
                return CDef.MainViewModel.VisionAutoVM.LoadVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
            }
            else
            {
                return CDef.MainViewModel.VisionAutoVM.UnloadVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
            }
        }

        #endregion

        #region Privates
        private IVisionResult VisionResult(EVisionArea area)
        {
            if (area == EVisionArea.LOAD)
            {
                return CDef.MainViewModel.VisionAutoVM.LoadVisionProcess.Result;
            }
            else
            {
                return CDef.MainViewModel.VisionAutoVM.UnloadVisionProcess.Result;
            }
        }
        #endregion
    }
}
