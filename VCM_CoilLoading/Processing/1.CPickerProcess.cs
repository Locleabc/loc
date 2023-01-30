using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TopCom.Processing;
using TopMotion;
using TopVision;
using TopVision.Models;
using VCM_CoilLoading.Define;
using VCM_CoilLoading.MVVM.Views;

namespace VCM_CoilLoading.Processing
{
    public class CPickerProcess : ProcessingBase
    {
        enum HomeStep : uint
        {
            HomeZ1Z2Axis = 0,
            WaitZ1Z2AxisHomeDone,
            WaitAllZAxisHomeDone,
            HomeAnotherAxis,
            WaitAnotherAxisHomeDone,
            HomeDone
        }

        enum PickStep : int
        {
            Picker_PickStart,
            Picker_MovePosition_ZWaiting,
            Picker_WaitPosition_ZWaiting,
            Picker_MovePosition_TrayPick,
            Picker_WaitPosition_TrayPick,
            Picker_SetFlag_LoadVisionPrePrepareDone,

            Picker_GetFlag_VisionStatus,   // DONE -> Picker_Get_VisionResult, NOT DONE -> Picker_GetFlag_LoadVisionInspectAvoid_Request
            Picker_GetFlag_LoadVisionInspectAvoid_Request,  // Require -> Picker_MovePosition_LoadVisionInspectAvoid, Not require: Picker_GetFlag_VisionStatus
            Picker_MovePosition_LoadVisionInspectAvoid,
            Picker_WaitPosition_LoadVisionInspectAvoid, // => Back to: Picker_GetFlag_VisionStatus

            Picker_Get_VisionResult,    // TODO: Making decision to pick or read vision again

            Picker_MovePosition_XYTPick,
            Picker_WaitPosition_XYTPick,
            Picker_MovePosition_ZSubPickDown,
            Picker_WaitPosition_ZSubPickDown,
            Picker_MovePosition_ZPickDown,
            Picker_WaitPosition_ZPickDown,
            Picker_VAC_On,
            Picker_VAC_Delay,
            Picker_MovePosition_ZPickUp,
            Picker_WaitPosition_ZPickUp,
            Picker_PickStatusUpdate,
            Picker_PickDone,
        }

        enum PlaceStep : int
        {
            Picker_PlaceStart,
            Picker_MovePosition_ZWaiting,
            Picker_WaitPosition_ZWaiting,
            Picker_MovePosition_TrayPlace,
            Picker_WaitPosition_TrayPlace,
            Picker_SetFlag_UnloadVisionPrePrepareDone,

            Picker_GetFlag_VisionStatus,   // DONE -> Picker_Get_VisionResult, NOT DONE -> Picker_GetFlag_LoadVisionInspectAvoid_Request
            Picker_GetFlag_UnloadVisionInspectAvoid_Request,  // Require -> Picker_MovePosition_LoadVisionInspectAvoid, Not require: Picker_GetFlag_VisionStatus
            Picker_MovePosition_UnloadVisionInspectAvoid,
            Picker_WaitPosition_UnloadVisionInspectAvoid, // => Back to: Picker_GetFlag_VisionStatus

            Picker_Get_VisionResult,    // TODO: Making decision to pick or read vision again

            Picker_MovePosition_XYTPlace,
            Picker_WaitPosition_XYTPlace,
            Picker_MovePosition_ZSubPlaceDown,
            Picker_WaitPosition_ZSubPlaceDown,
            Picker_MovePosition_ZPlaceDown,
            Picker_WaitPosition_ZPlaceDown,
            Picker_Purge_On,
            Picker_Purge_Delay,
            Picker_MovePosition_ZPlaceUp,
            Picker_WaitPosition_ZPlaceUp,

            Picker_TrayStatusUpdate,
            Picker_WorkDataUpdate,
            Picker_CheckIfTrayChangeRequired,
            Picker_PlaceDone,
        }

        enum UnderVisionStep : int
        {
            Picker_UnderVisionStart,
            Picker_MovePosition_ZUnderVision,
            Picker_WaitPosition_ZUnderVision,

            Picker_MovePosition_XYTUnderVision,
            Picker_WaitPosition_XYTUnderVision,

            ImageGrab,
            ImageGrabResult,
            StartVisionInspect,
            WaitVisionInspect,
            AplyVisionInspectResult,

            Picker_UnderVisionDone
        }

        enum TrayChangeStep : int
        {
            Picker_TrayChangeStart,
            Picker_MovePosition_ZWaiting,
            Picker_WaitPosition_ZWaiting,
            Picker_TrayChange_DecisionMaker,

            Picker_LoadingTray_TrayChangeStart,
            Picker_MovePosition_LoadingTrayChange,
            Picker_WaitPosition_LoadingTrayChange,
            Picker_LoadingTray_CheckChangeCondition,
            Picker_LoadingTray_TrayChangeMessage,
            Picker_LoadingTray_TrayChangeDone,

            Picker_UnloadingTray_TrayChangeStart,
            Picker_MovePosition_UnloadingTrayChange,
            Picker_WaitPosition_UnloadingTrayChange,
            Picker_UnloadingTray_CheckChangeCondition,
            Picker_UnloadingTray_TrayChangeMessage,
            Picker_UnloadingTray_TrayChangeDone,

            Picker_BothTray_TrayChangeStart,
            Picker_MovePosition_BothTrayChange,
            Picker_WaitPosition_BothTrayChange,
            Picker_BothTray_CheckChangeCondition,
            Picker_BothTray_TrayChangeMessage,
            Picker_BothTray_TrayChangeDone,
        }

        enum ToRunStep : int
        {
            Move_ZWaitPosition,
            Wait_ZWaitPosition,
            CheckIfAutoRunMode,
            Check_AllTrayStatus,
            Move_TrayWork_VisionAvoid_Position,
            Wait_TrayWork_VisionAvoid_Position,
            ToRunDone,

            MAX,    // set to MAX to swtich to default
        }

        private ERunMode runMode;
        public ERunMode RunMode
        {
            get
            {
                return runMode;
            }
            set
            {
                if (value != runMode)
                {
                    runMode = value;
                    Step.RunStep = 0;
                    OnPropertyChanged("RunMode");
                    ModeDetail = value.ToString();
                }
            }
        }

        #region DATA
        public List<IVisionResult> Data_UnderInspect_Result
        {
            get; set;
        } = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
        #endregion

        #region FLAGS
        public bool Flag_Picker_LoadVision_AvoidDone
        {
            get { return X1Axis.Status.ActualPosition >= CDef.UpperVisionRecipe.X1_LoadingVision_AvoidPosition - 0.1; }
        }

        public bool Flag_Picker_UnloadVision_AvoidDone
        {
            get { return X1Axis.Status.ActualPosition <= CDef.UpperVisionRecipe.X1_UnloadingVision_AvoidPosition + 0.1; }
        }

        public bool Flag_Picker_TrayPickPosition_Ready
        {
            get
            {
                double offsetY = CDef.CurrentLoadingTray.WorkStartIndex_Y * CDef.CommonRecipe.LoadingTray_Y_Pitch;

                return Y1Axis.IsOnPosition(CDef.PickerRecipe.Y1_WorkPosition_Tray1 - offsetY);
            }
        }

        public bool Flag_Picker_TrayPlacePosition_Ready
        {
            get
            {
                double offsetY = CDef.CurrentUnloadingTray.WorkStartIndex_Y * CDef.CommonRecipe.UnloadingTray_Y_Pitch;

                double Y2_WorkingPosition = CDef.PickerRecipe.Y2_WorkPosition_Tray1;
                if (CDef.CurrentUnloadingTray == CDef.UnloadingTray2)
                {
                    Y2_WorkingPosition = CDef.PickerRecipe.Y2_WorkPosition_Tray2;
                }

                return Y2Axis.IsOnPosition(Y2_WorkingPosition - offsetY);
            }
        }
        #endregion

        #region MOTIONS
        public MotionFas16000 ZZ1Enc
        {
            get
            {
                return (MotionFas16000)CDef.AllAxis.ZZ1Enc;
            }
        }

        public MotionFas16000 ZZ2Enc
        {
            get
            {
                return (MotionFas16000)CDef.AllAxis.ZZ2Enc;
            }
        }

        public MotionFas16000 X1Axis
        {
            get
            {
                return (MotionFas16000)CDef.AllAxis.X1Axis;
            }
        }

        public MotionPlusE Y1Axis
        {
            get { return (MotionPlusE)CDef.AllAxis.Y1Axis; }
        }

        public MotionPlusE Y2Axis
        {
            get { return (MotionPlusE)CDef.AllAxis.Y2Axis; }
        }

        public MotionPlusE Z1Axis
        {
            get
            {
                return (MotionPlusE)CDef.AllAxis.Z1Axis;
            }
        }

        public MotionPlusE Z2Axis
        {
            get
            {
                return (MotionPlusE)CDef.AllAxis.Z2Axis;
            }
        }

        public MotionPlusE XXAxis
        {
            get
            {
                return (MotionPlusE)CDef.AllAxis.XXAxis;
            }
        }

        public MotionPlusE T1Axis
        {
            get
            {
                return (MotionPlusE)CDef.AllAxis.T1Axis;
            }
        }

        public MotionPlusE T2Axis
        {
            get
            {
                return (MotionPlusE)CDef.AllAxis.T2Axis;
            }
        }

        public MotionPlusE YY1Axis
        {
            get
            {
                return (MotionPlusE)CDef.AllAxis.YY1Axis;
            }
        }

        public MotionPlusE YY2Axis
        {
            get
            {
                return (MotionPlusE)CDef.AllAxis.YY2Axis;
            }
        }
        #endregion

        public CPickerProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }

        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch (Step.HomeStep)
            {
                case (int)HomeStep.HomeZ1Z2Axis:
                    Log.Debug("Start search Z1/Z2 home position");
                    Z1Axis.HomeSearch();
                    Z2Axis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case (int)HomeStep.WaitZ1Z2AxisHomeDone:
                    if (Z1Axis.Status.IsHomeDone && Z2Axis.Status.IsHomeDone)
                    {
                        Log.Debug("Z1/Z2 home done");
                        HomeStatus.Z1Done = true;
                        HomeStatus.Z2Done = true;
                        Step.HomeStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoHomeSearch_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            string strAxis = Z1Axis.Status.IsHomeDone ? "" : " Z1 Axis ";
                            strAxis += Z2Axis.Status.IsHomeDone ? "" : " Z2 Axis ";

                            Log.Warn($"Home search timeout:{strAxis}");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case (int)HomeStep.WaitAllZAxisHomeDone:
                    if (HomeStatus.IsAllZAxisHomeDone())
                    {
                        Step.HomeStep++;
                    }
                    else
                    {
                        // TODO: this check may not necessary, warning may throw at another process
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoHomeSearch_TimeOut * 1000)
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
                case (int)HomeStep.HomeAnotherAxis:
                    Y1Axis.HomeSearch();
                    Y2Axis.HomeSearch();
                    X1Axis.HomeSearch();
                    XXAxis.HomeSearch();
                    T1Axis.HomeSearch();
                    T2Axis.HomeSearch();
                    YY1Axis.HomeSearch();
                    YY2Axis.HomeSearch();
                    Log.Debug("Start search X1/XX/Y1/Y2/T1/T2/YY1/YY2 home position");
                    Step.HomeStep++;
                    break;
                case (int)HomeStep.WaitAnotherAxisHomeDone:
                    if (Y1Axis.Status.IsHomeDone && Y2Axis.Status.IsHomeDone &&
                        X1Axis.Status.IsHomeDone && XXAxis.Status.IsHomeDone &&
                        T1Axis.Status.IsHomeDone && T2Axis.Status.IsHomeDone &&
                        YY1Axis.Status.IsHomeDone && YY2Axis.Status.IsHomeDone)
                    {
                        Log.Debug("X1/XX/Y1/Y2/T1/T2/YY1/YY2 home done");
                        Step.HomeStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoHomeSearch_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            string strAxis = Y1Axis.Status.IsHomeDone ? "" : " Y1 Axis ";
                            strAxis += Y2Axis.Status.IsHomeDone ? "" : " Y2 Axis ";
                            strAxis += X1Axis.Status.IsHomeDone ? "" : " X1 Axis ";
                            strAxis += XXAxis.Status.IsHomeDone ? "" : " XX Axis ";
                            strAxis += T1Axis.Status.IsHomeDone ? "" : " T1 Axis ";
                            strAxis += T2Axis.Status.IsHomeDone ? "" : " T2 Axis ";
                            strAxis += YY1Axis.Status.IsHomeDone ? "" : " YY1 Axis ";
                            strAxis += YY2Axis.Status.IsHomeDone ? "" : " YY2 Axis ";

                            Log.Warn($"Home search timeout:{strAxis}");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case (int)HomeStep.HomeDone:
                    ProcessingStatus = EProcessingStatus.OriginDone;
                    break;
                default:
                    Sleep(10);
                    break;
            }

            return nRtn;
        }

        public override PRtnCode ProcessToRun()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch ((ToRunStep)Step.ToRunStep)
            {
                case ToRunStep.Move_ZWaitPosition:
                    Picker_MoveZWaitPosition();
                    Step.ToRunStep++;
                    break;
                case ToRunStep.Wait_ZWaitPosition:
                    if (Picker_MoveZWaitPosition_Done())
                    {
                        Step.ToRunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Z Wait Position moving timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case ToRunStep.CheckIfAutoRunMode:
                    // In AutoRun mode, to reduce worktime move tray to work position and move picker to avoid position
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        Step.ToRunStep++;
                    }
                    else
                    {
                        Step.ToRunStep = (int)ToRunStep.ToRunDone;
                    }
                    break;
                case ToRunStep.Check_AllTrayStatus:
                    // Make sure all tray is avaiable
                    if (CDef.CurrentLoadingTray == null && CDef.CurrentUnloadingTray == null)
                    {
                        Log.Debug(">>>> Full All Tray");
                        CDef.MessageViewModel.Show("Full All Tray, Change tray first");
                        ProcessingStatus = EProcessingStatus.ToRunFail;
                        this.RunMode = ERunMode.Stop;
                        Step.ToRunStep = (int)ToRunStep.MAX;
                        return nRtn;
                    }
                    else if (CDef.CurrentLoadingTray == null)
                    {
                        Log.Debug(">>>> Full Loading Tray");
                        CDef.MessageViewModel.Show("Full Loading Tray, Change tray first");
                        ProcessingStatus = EProcessingStatus.ToRunFail;
                        this.RunMode = ERunMode.Stop;
                        Step.ToRunStep = (int)ToRunStep.MAX;
                        return nRtn;
                    }
                    else if (CDef.CurrentUnloadingTray == null)
                    {
                        Log.Debug(">>>> Full Unloading Tray");
                        CDef.MessageViewModel.Show("Full Unloading Tray, Change tray first");
                        ProcessingStatus = EProcessingStatus.ToRunFail;
                        this.RunMode = ERunMode.Stop;
                        Step.ToRunStep = (int)ToRunStep.MAX;
                        return nRtn;
                    }
                    else
                    {
                        Step.ToRunStep++;
                    }
                    break;
                case ToRunStep.Move_TrayWork_VisionAvoid_Position:
                    Picker_MoveTrayPickPosition();
                    Picker_MoveTrayPlacePosition();
                    Picker_MoveLoadingInspectAvoidPosition();
                    Step.ToRunStep++;
                    break;
                case ToRunStep.Wait_TrayWork_VisionAvoid_Position:
                    if (Flag_Picker_TrayPickPosition_Ready
                        && Flag_Picker_TrayPlacePosition_Ready
                        && Picker_MoveLoadingInspectAvoidPosition_Done())
                    {
                        Step.ToRunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            string strAxis = Flag_Picker_TrayPickPosition_Ready ? "" : " Loading Tray ";
                            strAxis += Flag_Picker_TrayPlacePosition_Ready ? "" : " Unloading Tray ";

                            if (string.IsNullOrEmpty(strAxis) == false)
                            {
                                Log.Warn($"Tray Working Position moving timeout:{strAxis}");
                            }
                            else
                            {
                                Log.Warn("Moving Loading Inspect Avoid timeout");
                            }
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case ToRunStep.ToRunDone:
                    switch (CDef.RootProcess.RunMode)
                    {
                        case ERunMode.AutoRun:
                            if (CDef.IO.HeadStatus.HeadOccupied)
                            {
                                this.RunMode = ERunMode.Manual_UnderVision_Inspect_1;
                            }
                            else
                            {
                                this.RunMode = ERunMode.Manual_Picker_Pick;
                            }
                            break;
                        case ERunMode.Manual_Picker_Pick:
                            if (CDef.IO.HeadStatus.HeadOccupied)
                            {
                                CDef.MessageViewModel.Show("Head is not empty");
                                ProcessingStatus = EProcessingStatus.ToRunFail;
                                this.RunMode = ERunMode.Stop;
                                Step.ToRunStep++;
                                return nRtn;
                            }
                            this.RunMode = ERunMode.Manual_Picker_Pick;
                            break;
                        case ERunMode.Manual_Picker_Place:
                            this.RunMode = ERunMode.Manual_Picker_Place;
                            break;
                        case ERunMode.Manual_UnderVision_Inspect_1:
                            this.RunMode = ERunMode.Manual_UnderVision_Inspect_1;
                            break;
                        case ERunMode.Manual_UnderVision_Inspect_2:
                            this.RunMode = ERunMode.Manual_UnderVision_Inspect_2;
                            break;
                        case ERunMode.Manual_UpperVision_Load_Inspect:
                            this.RunMode = ERunMode.Manual_UpperVision_Load_Inspect;
                            break;
                        case ERunMode.Manual_UpperVision_Unload_Inspect:
                            this.RunMode = ERunMode.Manual_UpperVision_Unload_Inspect;
                            break;
                        case ERunMode.Manual_LoadingTray_Change:
                            this.RunMode = ERunMode.Manual_LoadingTray_Change;
                            break;
                        case ERunMode.Manual_UnloadingTray_Change:
                            this.RunMode = ERunMode.Manual_UnloadingTray_Change;
                            break;
                        case ERunMode.Manual_AllTray_Change:
                            this.RunMode = ERunMode.Manual_AllTray_Change;
                            break;
                        case ERunMode.Manual_LoadingTray_Load:
                            this.RunMode = ERunMode.Manual_LoadingTray_Load;
                            break;
                        case ERunMode.Manual_UnloadingTray_Load:
                            this.RunMode = ERunMode.Manual_UnloadingTray_Load;
                            break;
                        case ERunMode.Manual_AllTray_Load:
                            this.RunMode = ERunMode.Manual_AllTray_Load;
                            break;
                        default:
                            this.RunMode = ERunMode.Stop;
                            break;
                    }

                    // Clear all takt time counter in start (or restart)
                    PTimer.ClearTactTimeCounter();
                    Step.ToRunStep++;
                    return base.ProcessToRun();
                default:
                    Sleep(20);
                    break;
            }

            return nRtn;
        }

        public override PRtnCode ProcessToStop()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (Step.RunStep != 0)
            {
                ProcessRun();
                return RtnCode;
            }
            else
            {
                return base.ProcessToStop();
            }
        }

        public override PRtnCode ProcessToWarning()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (Step.RunStep != 0)
            {
                ProcessRun();
                return RtnCode;
            }
            else
            {
                return base.ProcessToWarning();
            }
        }

        public override PRtnCode ProcessRun()
        {
            PRtnCode RtnCode = PRtnCode.RtnOk;

            if (PickerWorkStatus.PickDone)
            {
                CDef.CurrentLoadingTray.WorkStartIndex += 2;
            }

            if (PickerWorkStatus.PlaceDone)
            {
                CDef.CurrentUnloadingTray.WorkStartIndex += 2;
            }

            switch (this.RunMode)
            {
                case ERunMode.Manual_Picker_Pick:
                    Running_Picker_Pick();
                    break;
                case ERunMode.Manual_Picker_Place:
                    Running_Picker_Place();
                    break;
                case ERunMode.Manual_UnderVision_Inspect_1:
                    Running_UnderVision_Inspect_1();
                    break;
                case ERunMode.Manual_UnderVision_Inspect_2:
                    Running_UnderVision_Inspect_2();
                    break;
                case ERunMode.Manual_UpperVision_Load_Inspect:
                    Running_Picker_LoadingVision();
                    break;
                case ERunMode.Manual_UpperVision_Unload_Inspect:
                    Running_Picker_UnloadingVision();
                    break;
                case ERunMode.Manual_LoadingTray_Change:
                    Running_Tray_Change(TargetTray.LoadingTray);
                    break;
                case ERunMode.Manual_UnloadingTray_Change:
                    Running_Tray_Change(TargetTray.UnloadingTray);
                    break;
                case ERunMode.Manual_AllTray_Change:
                    Running_Tray_Change(TargetTray.Both);
                    break;
                case ERunMode.Manual_LoadingTray_Load:
                    Running_Tray_Change(TargetTray.LoadingTray, isMoveToLoadingPositionOnly: true);
                    break;
                case ERunMode.Manual_UnloadingTray_Load:
                    Running_Tray_Change(TargetTray.UnloadingTray, isMoveToLoadingPositionOnly: true);
                    break;
                case ERunMode.Manual_AllTray_Load:
                    Running_Tray_Change(TargetTray.Both, isMoveToLoadingPositionOnly: true);
                    break;
                default:
                    Sleep(20);
                    break;
            }

            return RtnCode;
        }

        public void Running_Picker_LoadingVision()
        {
            switch ((PickStep)Step.RunStep)
            {
                case PickStep.Picker_PickStart:
                    Log.Debug(">>>>>>>>>>>>> Picker_PickStart");
                    // Clear total sequence takt time
                    PTimer.TactTimeCounter[0] = PTimer.Now;

                    // Clear picking takt time
                    PTimer.TactTimeCounter[1] = PTimer.Now;

                    Step.RunStep++;
                    break;
                case PickStep.Picker_MovePosition_ZWaiting:
                    Picker_MoveZWaitPosition();
                    Step.RunStep++;
                    break;
                case PickStep.Picker_WaitPosition_ZWaiting:
                    if (Picker_MoveZWaitPosition_Done())
                    {
                        Log.Debug("Picker_MoveWaitingPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Z Wait Position Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PickStep.Picker_MovePosition_TrayPick:
                    Picker_MoveTrayPickPosition();
                    Step.RunStep++;
                    break;
                case PickStep.Picker_WaitPosition_TrayPick:
                    if (Flag_Picker_TrayPickPosition_Ready)
                    {
                        Log.Debug("Picker_MoveTrayPickPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Tray Pick Position Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PickStep.Picker_SetFlag_LoadVisionPrePrepareDone:
                    // Watching Vision Inspec timeout
                    PTimer.SpareTimer = PTimer.Now;
                    Step.RunStep++;
                    break;
                case PickStep.Picker_GetFlag_VisionStatus:
                    if (Flags.Flag_UpperVision_LoadInspect_Done == true)
                    {
                        Log.Debug("Flag_UpperVision_LoadInspect_Done");
                        Step.RunStep = (int)PickStep.Picker_Get_VisionResult;
                    }
                    else
                    {
                        // Stop waiting to Vision Inspecting while machine is going to be stop
                        if (Parent.Mode == ProcessingMode.ModeToStop)
                        {
                            Step.RunStep = 0;
                            base.ProcessToStop();
                            break;
                        }
                        else if (Parent.Mode == ProcessingMode.ModeToWarning)
                        {
                            Step.RunStep = 0;
                            base.ProcessToWarning();
                            break;
                        }
                        // TODO: Add ToAlarm (if neccessary)

                        // TODO: Report timeout only in vision process?
                        if (PTimer.Now - PTimer.SpareTimer < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                            Step.RunStep++;
                        }
                        else
                        {
                            Log.Warn("Loading Vision Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PickStep.Picker_GetFlag_LoadVisionInspectAvoid_Request:
                    if (Flags.Flag_LoadVisionInspectAvoid_Request == true)
                    {
                        if (Flag_Picker_LoadVision_AvoidDone == false)
                        {
                            Step.RunStep = (int)PickStep.Picker_MovePosition_LoadVisionInspectAvoid;
                            break;
                        }
                        else
                        {
                            Step.RunStep = (int)PickStep.Picker_GetFlag_VisionStatus;
                        }
                    }
                    else
                    {
                        Step.RunStep = (int)PickStep.Picker_GetFlag_VisionStatus;
                    }
                    break;
                case PickStep.Picker_MovePosition_LoadVisionInspectAvoid:
                    Picker_MoveLoadingInspectAvoidPosition();
                    Step.RunStep++;
                    break;
                case PickStep.Picker_WaitPosition_LoadVisionInspectAvoid:
                    if (Picker_MoveLoadingInspectAvoidPosition_Done())
                    {
                        Log.Debug("Picker_MoveLoadingInspectAvoidPosition_Done");
                        Step.RunStep = (int)PickStep.Picker_GetFlag_VisionStatus;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.PickerVisionAvoid_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Loading Tray Vision Avoid Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PickStep.Picker_Get_VisionResult:
                    // TODO: Handle get vision result here

                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun || CDef.RootProcess.RunMode == ERunMode.Manual_Picker_Pick)
                    {
                        // Continue to pick sequence
                        Step.RunStep++;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UpperVision_Load_Inspect)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public void Running_Picker_Pick()
        {
            Running_Picker_LoadingVision();
            switch ((PickStep)Step.RunStep)
            {
                case PickStep.Picker_MovePosition_XYTPick:
                    Picker_MoveXYTPickPosition();
                    Step.RunStep++;
                    break;
                case PickStep.Picker_WaitPosition_XYTPick:
                    if (Picker_MoveXYTPickPosition_Done())
                    {
                        Log.Debug("Picker_MoveTrayPickPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("XYTPickPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PickStep.Picker_MovePosition_ZSubPickDown:
                    Picker_MoveZPick_SubPosition();
                    Step.RunStep++;
                    break;
                case PickStep.Picker_WaitPosition_ZSubPickDown:
                    if (Picker_MoveZPick_SubPosition_Done())
                    {
                        Log.Debug("Picker_MoveZPick_SubPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("MoveZPick_SubPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PickStep.Picker_MovePosition_ZPickDown:
                    Picker_MoveZPickPosition();
                    Step.RunStep++;
                    break;
                case PickStep.Picker_WaitPosition_ZPickDown:
                    if (Picker_MoveZPickPosition_Done())
                    {
                        Log.Debug("Picker_MoveZPickPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("ZPickPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PickStep.Picker_VAC_On:
                    Log.Info("Turn ON VAC1 / VAC2");
                    CDef.IO.Output.Head1VAC = true;
                    CDef.IO.Output.Head2VAC = true;
                    PTimer.DelayTimer = PTimer.Now;
                    Step.RunStep++;
                    break;
                case PickStep.Picker_VAC_Delay:
                    if (PTimer.Now - PTimer.DelayTimer < CDef.CommonRecipe.VAC_Delay * 1000)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        Log.Info($"VAC Delayed for {PTimer.Now - PTimer.DelayTimer} [ms]");
                        Sleep(100);
                        Step.RunStep++;
                    }
                    break;
                case PickStep.Picker_MovePosition_ZPickUp:
                    Picker_MoveZWaitPosition();
                    Step.RunStep++;
                    break;
                case PickStep.Picker_WaitPosition_ZPickUp:
                    if (Picker_MoveZWaitPosition_Done())
                    {
                        Log.Debug("Picker_MoveZWaitPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("ZWaitPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PickStep.Picker_PickStatusUpdate:
                    // Important: Log before changing WorkStartIndex
                    Log.Info($"Pick Index {CDef.CurrentLoadingTray.WorkStartIndex} and {CDef.CurrentLoadingTray.WorkStartIndex + 1} DONE");
                    CDef.CurrentLoadingTray.SetSetOfCell(TopUI.Define.ECellStatus.OK, CDef.CurrentLoadingTray.WorkStartIndex);

                    Datas.WorkData.TaktTime.Pick = 1.0 * (PTimer.Now - PTimer.TactTimeCounter[1]) / 1000;

                    Step.RunStep++;
                    break;
                case PickStep.Picker_PickDone:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        // Move tray to pick position after tray status updated
                        if (CDef.CurrentLoadingTray != null)
                        {
                            Picker_MoveTrayPickPosition();
                        }

                        this.RunMode = ERunMode.Manual_UnderVision_Inspect_2;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Picker_Pick)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public void Running_UnderVision_Inspect_1()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                CDef.GlobalRecipe.SkipUnderVision)
            {
                this.RunMode = ERunMode.Manual_Picker_Place;
                return;
            }

            switch ((UnderVisionStep)Step.RunStep)
            {
                case UnderVisionStep.Picker_UnderVisionStart:
                    Log.Debug(">>>>>>>>>>>>> UnderVision_InspectStart");

                    PickerWorkStatus.Head1UnderInspectDone = false;

                    Step.RunStep++;
                    break;
                case UnderVisionStep.Picker_MovePosition_ZUnderVision:
                    Picker_MoveZUnderVisionPosition();
                    Step.RunStep++;
                    break;
                case UnderVisionStep.Picker_WaitPosition_ZUnderVision:
                    if (Picker_MoveZUnderVisionPosition_Done())
                    {
                        Log.Debug("Picker_MoveZUnderVisionPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("ZUnderVisionPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnderVisionStep.Picker_MovePosition_XYTUnderVision:
                    Picker_MoveXYTUnderVisionPosition_1();
                    Step.RunStep++;
                    break;
                case UnderVisionStep.Picker_WaitPosition_XYTUnderVision:
                    if (Picker_MoveXYTUnderVisionPosition_1_Done())
                    {
                        Log.Debug("Picker_MoveXYTUnderVisionPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Picker_MoveXYTUnderVisionPosition_Done Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnderVisionStep.ImageGrab:
                    ImageGrab(EVisionArea.UNDER);
                    Step.RunStep++;
                    break;
                case UnderVisionStep.ImageGrabResult:
                    if (CDef.BotCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Under Vision Grab Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                        break;
                    }

                    if (ImageGrabResult("Under1") == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Log.Warn("Vision Image Grab failed");
                        CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                    }
                    break;
                case UnderVisionStep.StartVisionInspect:
                    Log.Debug("StartVisionInspect HEAD 1");
                    StartVisionInspect(EVisionArea.UNDER);

                    Step.RunStep++;
                    break;
                case UnderVisionStep.WaitVisionInspect:
                    if (IsVisionInspectDone("Under1"))
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Under Vision Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnderVisionStep.AplyVisionInspectResult:
                    Log.Info(VisionResult("Under1"));
                    Data_UnderInspect_Result[0] = VisionResult("Under1");

                    Datas.WorkData.TaktTime.Vision = 1.0 * (PTimer.Now - PTimer.TactTimeCounter[2]) / 1000;

                    Step.RunStep++;
                    break;

                case UnderVisionStep.Picker_UnderVisionDone:
                    Log.Debug("UnderVision_Inspect Done");
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Picker_Place;

                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UnderVision_Inspect_1)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;

                default:
                    break;
            }
        }

        public void Running_UnderVision_Inspect_2()
        {
            if (CDef.RootProcess.RunMode == ERunMode.AutoRun &&
                CDef.GlobalRecipe.SkipUnderVision)
            {
                this.RunMode = ERunMode.Manual_Picker_Place;
                return;
            }

            switch ((UnderVisionStep)Step.RunStep)
            {
                case UnderVisionStep.Picker_UnderVisionStart:
                    Log.Debug(">>>>>>>>>>>>> UnderVision_InspectStart");

                    PickerWorkStatus.Head2UnderInspectDone = false;
                    // Clear Under vision takt time counter
                    PTimer.TactTimeCounter[2] = PTimer.Now;

                    Step.RunStep++;
                    break;
                case UnderVisionStep.Picker_MovePosition_ZUnderVision:
                    Picker_MoveZUnderVisionPosition();
                    Step.RunStep++;
                    break;
                case UnderVisionStep.Picker_WaitPosition_ZUnderVision:
                    if (Picker_MoveZUnderVisionPosition_Done())
                    {
                        Log.Debug("Picker_MoveZUnderVisionPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("ZUnderVisionPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnderVisionStep.Picker_MovePosition_XYTUnderVision:
                    Picker_MoveXYTUnderVisionPosition_2();
                    Step.RunStep++;
                    break;
                case UnderVisionStep.Picker_WaitPosition_XYTUnderVision:
                    if (Picker_MoveXYTUnderVisionPosition_2_Done())
                    {
                        Log.Debug("Picker_MoveXYTUnderVisionPosition_2_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Picker_MoveXYTUnderVisionPosition_2_Done Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnderVisionStep.ImageGrab:
                    ImageGrab(EVisionArea.UNDER);
                    Step.RunStep++;
                    break;
                case UnderVisionStep.ImageGrabResult:
                    if (CDef.BotCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Under Vision Grab Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                        break;
                    }

                    if (ImageGrabResult("Under2") == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Log.Warn("Vision Image Grab failed");
                        CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                    }
                    break;
                case UnderVisionStep.StartVisionInspect:
                    Log.Debug("StartVisionInspect HEAD 2");
                    StartVisionInspect(EVisionArea.UNDER);

                    Step.RunStep++;
                    break;
                case UnderVisionStep.WaitVisionInspect:
                    if (IsVisionInspectDone("Under2"))
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Under Vision Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnderVisionStep.AplyVisionInspectResult:
                    Log.Info(VisionResult("Under2"));
                    Data_UnderInspect_Result[1] = VisionResult("Under2");

                    Step.RunStep++;
                    break;
                case UnderVisionStep.Picker_UnderVisionDone:
                    Log.Debug("UnderVision_Inspect Done");
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_UnderVision_Inspect_1;

                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UnderVision_Inspect_2)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public void Running_Picker_UnloadingVision()
        {
            switch ((PlaceStep)Step.RunStep)
            {
                case PlaceStep.Picker_PlaceStart:
                    Log.Debug(">>>>>>>>>>>>> Picker_PlaceStart Done");

                    // Clear place takt time
                    PTimer.TactTimeCounter[3] = PTimer.Now;

                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_MovePosition_ZWaiting:
                    Picker_MoveZWaitPosition();
                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_WaitPosition_ZWaiting:
                    if (Picker_MoveZWaitPosition_Done())
                    {
                        Log.Debug("Picker_MoveZWaitPosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("ZWaitPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PlaceStep.Picker_MovePosition_TrayPlace:
                    Picker_MoveTrayPlacePosition();
                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_WaitPosition_TrayPlace:
                    if (Flag_Picker_TrayPlacePosition_Ready)
                    {
                        Log.Debug("Picker_MoveTrayPlacePosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("TrayPlacePosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PlaceStep.Picker_SetFlag_UnloadVisionPrePrepareDone:
                    // Watching Vision Inspec timeout
                    PTimer.SpareTimer = PTimer.Now;
                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_GetFlag_VisionStatus:
                    if (Flags.Flag_UpperVision_UnloadInspect_Done)
                    {
                        Log.Debug("Flag_UpperVision_UnloadInspect_Done");
                        Step.RunStep = (int)PlaceStep.Picker_Get_VisionResult;
                    }
                    else
                    {
                        // Stop waiting to Vision Inspecting while machine is going to be stop
                        if (Parent.Mode == ProcessingMode.ModeToStop)
                        {
                            Step.RunStep = 0;
                            base.ProcessToStop();
                            break;
                        }
                        else if (Parent.Mode == ProcessingMode.ModeToWarning)
                        {
                            Step.RunStep = 0;
                            base.ProcessToWarning();
                            break;
                        }
                        // TODO: Add ToAlarm (if neccessary)

                        // TODO: Report timeout only in vision process?
                        if (PTimer.Now - PTimer.SpareTimer < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                            Step.RunStep++;
                        }
                        else
                        {
                            Log.Warn("Unloading Vision Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PlaceStep.Picker_GetFlag_UnloadVisionInspectAvoid_Request:
                    if (Flags.Flag_UnloadVisionInspectAvoid_Request == true)
                    {
                        if (Flag_Picker_UnloadVision_AvoidDone == false)
                        {
                            Step.RunStep = (int)PlaceStep.Picker_MovePosition_UnloadVisionInspectAvoid;
                            break;
                        }
                        else
                        {
                            Step.RunStep = (int)PlaceStep.Picker_GetFlag_VisionStatus;
                        }
                    }
                    else
                    {
                        Step.RunStep = (int)PlaceStep.Picker_GetFlag_VisionStatus;
                    }
                    break;
                case PlaceStep.Picker_MovePosition_UnloadVisionInspectAvoid:
                    Picker_MoveUnloadingInspectAvoidPosition();
                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_WaitPosition_UnloadVisionInspectAvoid:
                    if (Picker_MoveUnloadingInspectAvoidPosition_Done())
                    {
                        Log.Debug("Picker_MoveUnloadingInspectAvoidPosition_Done");
                        Step.RunStep = (int)PlaceStep.Picker_GetFlag_VisionStatus;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("UnloadingInspectAvoid Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PlaceStep.Picker_Get_VisionResult:
                    // TODO: Handle get vision result here

                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun || CDef.RootProcess.RunMode == ERunMode.Manual_Picker_Place)
                    {
                        // Continue to place sequence
                        Step.RunStep++;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UpperVision_Unload_Inspect)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public void Running_Picker_Place()
        {
            Running_Picker_UnloadingVision();
            switch ((PlaceStep)Step.RunStep)
            {
                case PlaceStep.Picker_MovePosition_XYTPlace:
                    Picker_MoveXYTPlacePosition();
                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_WaitPosition_XYTPlace:
                    if (Picker_MoveXYTPlacePosition_Done())
                    {
                        Log.Debug("Picker_MoveXYTPlacePosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("XYTPlacePosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PlaceStep.Picker_MovePosition_ZSubPlaceDown:
                    Picker_MoveZPlace_SubPosition();
                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_WaitPosition_ZSubPlaceDown:
                    if (Picker_MoveZPlace_SubPosition_Done())
                    {
                        Log.Debug("Picker_MoveZPlacePosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("ZPlacePosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PlaceStep.Picker_MovePosition_ZPlaceDown:
                    Picker_MoveZPlacePosition();
                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_WaitPosition_ZPlaceDown:
                    if (Picker_MoveZPlacePosition_Done())
                    {
                        Log.Debug("Picker_MoveZPlacePosition_Done");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("ZPlacePosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PlaceStep.Picker_Purge_On:
                    Log.Info("Turning Purge ON");
                    CDef.IO.Output.Head1VAC = false;
                    CDef.IO.Output.Head2VAC = false;

                    CDef.IO.Output.Head1Purge = true;
                    CDef.IO.Output.Head2Purge = true;

                    PTimer.DelayTimer = PTimer.Now;
                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_Purge_Delay:
                    if (PTimer.Now - PTimer.DelayTimer < CDef.CommonRecipe.Purge_Delay * 1000)
                    {
                        Sleep(10);
                    }
                    else
                    {
                        Log.Info($"Purge Delayed for {PTimer.Now - PTimer.DelayTimer} [ms]");
                        Step.RunStep++;
                        CDef.IO.Output.Head1Purge = false;
                        CDef.IO.Output.Head2Purge = false;
                    }
                    break;
                case PlaceStep.Picker_MovePosition_ZPlaceUp:
                    Picker_MoveZWaitPosition();
                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_WaitPosition_ZPlaceUp:
                    if (Picker_MoveZWaitPosition_Done())
                    {
                        Console.WriteLine(this.ProcessName + " " + "Picker_MoveZWaitPosition_Done");

                        Step.RunStep++;
                        Picker_MoveTrayPlacePosition();
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("ZWaitPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case PlaceStep.Picker_TrayStatusUpdate:
                    // Important: Log before changing WorkStartIndex
                    Log.Info($"Place Index {CDef.CurrentUnloadingTray.WorkStartIndex} and {CDef.CurrentUnloadingTray.WorkStartIndex + 1} DONE");
                    CDef.CurrentUnloadingTray.SetSetOfCell(TopUI.Define.ECellStatus.OK, CDef.CurrentUnloadingTray.WorkStartIndex);

                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_WorkDataUpdate:
                    Datas.WorkData.CountData.Total += 2;
                    Datas.WorkData.CountData.OK += (uint)Datas.Data_UnderInspect_Result.Count(r => r.Judge == EVisionJudge.OK);
                    Datas.WorkData.CountData.LoadVisionNG[0] += (uint)(Datas.Data_LoadingInspect_Result[0].Judge == EVisionJudge.NG ? 1 : 0);
                    Datas.WorkData.CountData.LoadVisionNG[1] += (uint)(Datas.Data_LoadingInspect_Result[1].Judge == EVisionJudge.NG ? 1 : 0);
                    Datas.WorkData.CountData.UnderVisionNG[0] += (uint)(Datas.Data_UnderInspect_Result[0].Judge == EVisionJudge.NG ? 1 : 0);
                    Datas.WorkData.CountData.UnderVisionNG[1] += (uint)(Datas.Data_UnderInspect_Result[1].Judge == EVisionJudge.NG ? 1 : 0);
                    Datas.WorkData.CountData.UnloadVisionNG[0] += (uint)(Datas.Data_UnloadingInspect_Result[0].Judge == EVisionJudge.NG ? 1 : 0);
                    Datas.WorkData.CountData.UnloadVisionNG[1] += (uint)(Datas.Data_UnloadingInspect_Result[1].Judge == EVisionJudge.NG ? 1 : 0);

                    Datas.WorkData.TaktTime.Total = 1.0 * (PTimer.Now - PTimer.TactTimeCounter[0]) / 1000;
                    Datas.WorkData.TaktTime.Place = 1.0 * (PTimer.Now - PTimer.TactTimeCounter[3]) / 1000;
                    Datas.WorkData.TaktTime.LoadVision[0].ProcessTime = Datas.Data_LoadingInspect_Result[0].Cost / 1000;
                    Datas.WorkData.TaktTime.LoadVision[1].ProcessTime = Datas.Data_LoadingInspect_Result[1].Cost / 1000;
                    Datas.WorkData.TaktTime.BotVision[0].ProcessTime = Datas.Data_UnderInspect_Result[0].Cost / 1000;
                    Datas.WorkData.TaktTime.BotVision[1].ProcessTime = Datas.Data_UnderInspect_Result[1].Cost / 1000;
                    Datas.WorkData.TaktTime.UnloadVision[0].ProcessTime = Datas.Data_UnloadingInspect_Result[0].Cost / 1000;
                    Datas.WorkData.TaktTime.UnloadVision[1].ProcessTime = Datas.Data_UnloadingInspect_Result[1].Cost / 1000;

                    CDef.MES.SavePD(0);
                    CDef.MES.SavePD(1);
                    CDef.MES.SaveTT(0);
                    CDef.MES.SaveTT(1);

                    // Reset inspect data for next sequence
                    // Datas.ResetInspectData(); => [BUG] This reset make in-work TOP Vision result be reset

                    Step.RunStep++;
                    break;
                case PlaceStep.Picker_CheckIfTrayChangeRequired:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        if (CDef.CurrentLoadingTray == null && CDef.CurrentUnloadingTray == null)
                        {
                            Log.Debug(">>>> Full All Tray");
                            this.RunMode = ERunMode.Manual_AllTray_Change;
                            break;
                        }
                        else if (CDef.CurrentLoadingTray == null)
                        {
                            Log.Debug(">>>> Full Loading Tray");
                            this.RunMode = ERunMode.Manual_LoadingTray_Change;
                            break;
                        }
                        else if (CDef.CurrentUnloadingTray == null)
                        {
                            Log.Debug(">>>> Full Unloading Tray");
                            this.RunMode = ERunMode.Manual_UnloadingTray_Change;
                            break;
                        }
                    }
                    Step.RunStep++;

                    break;
                case PlaceStep.Picker_PlaceDone:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        this.RunMode = ERunMode.Manual_Picker_Pick;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_Picker_Place)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                default:
                    break;
            }
        }

        public void Running_Tray_Change(TargetTray tray, bool isMoveToLoadingPositionOnly = false)
        {
            switch ((TrayChangeStep)Step.RunStep)
            {
                case TrayChangeStep.Picker_TrayChangeStart:
                    Log.Debug("Picker_TrayChangeStart");

                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_MovePosition_ZWaiting:
                    Picker_MoveZWaitPosition();
                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_WaitPosition_ZWaiting:
                    if (Picker_MoveZWaitPosition_Done())
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("ZWaitPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayChangeStep.Picker_TrayChange_DecisionMaker:
                    if (tray == TargetTray.LoadingTray)
                    {
                        Step.RunStep = (int)TrayChangeStep.Picker_LoadingTray_TrayChangeStart;
                    }
                    else if (tray == TargetTray.UnloadingTray)
                    {
                        Step.RunStep = (int)TrayChangeStep.Picker_UnloadingTray_TrayChangeStart;
                    }
                    else
                    {
                        Step.RunStep = (int)TrayChangeStep.Picker_BothTray_TrayChangeStart;
                    }
                    break;
                #region LOADING TRAY CHANGE
                case TrayChangeStep.Picker_LoadingTray_TrayChangeStart:
                    Log.Debug("Picker_LoadingTray_TrayChangeStart");

                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_MovePosition_LoadingTrayChange:
                    LoadingTray_MoveLoadingPosition();
                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_WaitPosition_LoadingTrayChange:
                    if (LoadingTray_MoveLoadingPosition_Done())
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Loading Tray Loading Position Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayChangeStep.Picker_LoadingTray_CheckChangeCondition:
                    if (CDef.RootProcess.RunMode == ERunMode.Manual_LoadingTray_Load)
                    {
                        Step.RunStep = (int)TrayChangeStep.Picker_LoadingTray_TrayChangeDone;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case TrayChangeStep.Picker_LoadingTray_TrayChangeMessage:
                    CDef.MessageViewModel.Show("Change Loading Tray Please!");
                    CDef.LoadingTray.ResetCommand.Execute(null);

                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_LoadingTray_TrayChangeDone:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        Log.Debug("Picker_LoadingTray_TrayChange");
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_LoadingTray_Change ||
                             CDef.RootProcess.RunMode == ERunMode.Manual_LoadingTray_Load)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                #endregion

                #region UNLOADING TRAY CHANGE
                case TrayChangeStep.Picker_UnloadingTray_TrayChangeStart:
                    Log.Debug("Picker_UnloadingTray_TrayChangeStart");

                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_MovePosition_UnloadingTrayChange:
                    UnloadingTray_MoveLoadingPosition();
                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_WaitPosition_UnloadingTrayChange:
                    if (UnloadingTray_MoveLoadingPosition_Done())
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Unloading Tray Loading Position Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayChangeStep.Picker_UnloadingTray_CheckChangeCondition:
                    if (CDef.RootProcess.RunMode == ERunMode.Manual_UnloadingTray_Load)
                    {
                        Step.RunStep = (int)TrayChangeStep.Picker_UnloadingTray_TrayChangeDone;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case TrayChangeStep.Picker_UnloadingTray_TrayChangeMessage:
                    CDef.MessageViewModel.Show("Change Unloading Tray Please!");
                    CDef.UnloadingTray2.ResetCommand.Execute(null);
                    CDef.UnloadingTray1.ResetCommand.Execute(null);

                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_UnloadingTray_TrayChangeDone:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        Log.Debug("Manual_UnloadingTray_Change");
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                        break;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UnloadingTray_Change ||
                             CDef.RootProcess.RunMode == ERunMode.Manual_UnloadingTray_Load)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                #endregion

                #region BOTH TRAY CHANGE
                case TrayChangeStep.Picker_BothTray_TrayChangeStart:
                    Log.Debug("Picker_BothTray_TrayChangeStart");

                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_MovePosition_BothTrayChange:
                    LoadingTray_MoveLoadingPosition();
                    UnloadingTray_MoveLoadingPosition();
                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_WaitPosition_BothTrayChange:
                    if (LoadingTray_MoveLoadingPosition_Done() && UnloadingTray_MoveLoadingPosition_Done())
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.ServoMoving_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Both Tray Loading Position Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case TrayChangeStep.Picker_BothTray_CheckChangeCondition:
                    if (CDef.RootProcess.RunMode == ERunMode.Manual_AllTray_Load)
                    {
                        Step.RunStep = (int)TrayChangeStep.Picker_BothTray_TrayChangeDone;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case TrayChangeStep.Picker_BothTray_TrayChangeMessage:
                    CDef.MessageViewModel.Show("Change All Tray Please!");
                    CDef.LoadingTray.ResetCommand.Execute(null);
                    CDef.UnloadingTray1.ResetCommand.Execute(null);
                    CDef.UnloadingTray2.ResetCommand.Execute(null);

                    Step.RunStep++;
                    break;
                case TrayChangeStep.Picker_BothTray_TrayChangeDone:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        Log.Debug("End Manual_AllTray_Change");
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_AllTray_Change ||
                             CDef.RootProcess.RunMode == ERunMode.Manual_AllTray_Load)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                        this.RunMode = ERunMode.Stop;
                    }
                    break;
                #endregion
                default:
                    break;
            }
        }

        #region Functions Position Move or Check

        #region __COMMON__
        private void Picker_MoveZWaitPosition()
        {
            Z1Axis.MoveAbs(CDef.PickerRecipe.Z1_WaitPosition);
            Z2Axis.MoveAbs(CDef.PickerRecipe.Z2_WaitPosition);
        }

        private bool Picker_MoveZWaitPosition_Done()
        {
            return Z1Axis.IsOnPosition(CDef.PickerRecipe.Z1_WaitPosition)
                && Z2Axis.IsOnPosition(CDef.PickerRecipe.Z2_WaitPosition);
        }
        #endregion

        #region __PICK__
        private void Picker_MoveLoadingInspectAvoidPosition()
        {
            X1Axis.MoveAbs(CDef.UpperVisionRecipe.X1_LoadingVision_AvoidPosition);
        }

        private bool Picker_MoveLoadingInspectAvoidPosition_Done()
        {
            return X1Axis.IsOnPosition(CDef.UpperVisionRecipe.X1_LoadingVision_AvoidPosition);
        }

        private void Picker_MoveTrayPickPosition()
        {
            //TODO: add switch for tray1/tray2
            double offsetY = CDef.CurrentLoadingTray.WorkStartIndex_Y * CDef.CommonRecipe.LoadingTray_Y_Pitch;

            Y1Axis.MoveAbs(CDef.PickerRecipe.Y1_WorkPosition_Tray1 - offsetY);
            Console.WriteLine(this.ProcessName + " " + "Picker_MoveTrayPickPosition");
        }

        private void Picker_MoveXYTPickPosition()
        {
            double offsetX = CDef.CurrentLoadingTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;
            double[] visionOffsetX = new double[] { Datas.Data_LoadingInspect_Result[0].DetectedOffset.X * CDef.UpperVisionRecipe.LoadingVision_PixelSize / 1000,
                                                    Datas.Data_LoadingInspect_Result[1].DetectedOffset.X * CDef.UpperVisionRecipe.LoadingVision_PixelSize / 1000};
            double[] visionOffsetY = new double[] { Datas.Data_LoadingInspect_Result[0].DetectedOffset.Y * CDef.UpperVisionRecipe.LoadingVision_PixelSize / 1000,
                                                    Datas.Data_LoadingInspect_Result[1].DetectedOffset.Y * CDef.UpperVisionRecipe.LoadingVision_PixelSize / 1000};

            X1Axis.MoveAbs(CDef.PickerRecipe.X1_PickPosition - offsetX - visionOffsetX[0]);
            XXAxis.MoveAbs(CDef.PickerRecipe.XX_PickPosition + (visionOffsetX[0] * 10) - (visionOffsetX[1] * 10));
            YY1Axis.MoveAbs(CDef.PickerRecipe.YY1_PickPosition - visionOffsetY[0]);
            YY2Axis.MoveAbs(CDef.PickerRecipe.YY2_PickPosition - visionOffsetY[1]);
            T1Axis.MoveAbs(CDef.PickerRecipe.T1_PickPosition
                         + Datas.Data_LoadingInspect_Result[0].DetectedOffset.Theta * 10.0 / 360.0);
            T2Axis.MoveAbs(CDef.PickerRecipe.T2_PickPosition
                         + Datas.Data_LoadingInspect_Result[1].DetectedOffset.Theta * 10.0 / 360.0);

            Console.WriteLine(this.ProcessName + " " + "Picker_MoveXYTPickPosition");
        }

        private bool Picker_MoveXYTPickPosition_Done()
        {
            double offsetX = CDef.CurrentLoadingTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;
            double[] visionOffsetX = new double[] { Datas.Data_LoadingInspect_Result[0].DetectedOffset.X * CDef.UpperVisionRecipe.LoadingVision_PixelSize / 1000,
                                                    Datas.Data_LoadingInspect_Result[1].DetectedOffset.X * CDef.UpperVisionRecipe.LoadingVision_PixelSize / 1000};
            double[] visionOffsetY = new double[] { Datas.Data_LoadingInspect_Result[0].DetectedOffset.Y * CDef.UpperVisionRecipe.LoadingVision_PixelSize / 1000,
                                                    Datas.Data_LoadingInspect_Result[1].DetectedOffset.Y * CDef.UpperVisionRecipe.LoadingVision_PixelSize / 1000};

            return X1Axis.IsOnPosition(CDef.PickerRecipe.X1_PickPosition - offsetX - visionOffsetX[0])
                && XXAxis.IsOnPosition(CDef.PickerRecipe.XX_PickPosition + (visionOffsetX[0] * 10) - (visionOffsetX[1] * 10))
                && YY1Axis.IsOnPosition(CDef.PickerRecipe.YY1_PickPosition - visionOffsetY[0])
                && YY2Axis.IsOnPosition(CDef.PickerRecipe.YY2_PickPosition - visionOffsetY[1])
                && T1Axis.IsOnPosition(CDef.PickerRecipe.T1_PickPosition
                                     + Datas.Data_LoadingInspect_Result[0].DetectedOffset.Theta * 10.0 / 360.0)
                && T2Axis.IsOnPosition(CDef.PickerRecipe.T2_PickPosition
                                     + Datas.Data_LoadingInspect_Result[1].DetectedOffset.Theta * 10.0 / 360.0);
        }

        private void Picker_MoveZPick_SubPosition()
        {
            Z1Axis.MoveAbs(CDef.PickerRecipe.Z1_PickPosition - 2 /* mm */);
            Z2Axis.MoveAbs(CDef.PickerRecipe.Z2_PickPosition - 2 /* mm */);
        }

        private bool Picker_MoveZPick_SubPosition_Done()
        {
            return Z1Axis.IsOnPosition(CDef.PickerRecipe.Z1_PickPosition - 2 /* mm */)
                && Z2Axis.IsOnPosition(CDef.PickerRecipe.Z2_PickPosition - 2 /* mm */);
        }

        private void Picker_MoveZPickPosition()
        {
            Z1Axis.MoveAbs(CDef.PickerRecipe.Z1_PickPosition, CDef.PickerRecipe.Z_PickSpeed);
            Z2Axis.MoveAbs(CDef.PickerRecipe.Z2_PickPosition, CDef.PickerRecipe.Z_PickSpeed);
        }

        private bool Picker_MoveZPickPosition_Done()
        {
            return Z1Axis.IsOnPosition(CDef.PickerRecipe.Z1_PickPosition)
                && Z2Axis.IsOnPosition(CDef.PickerRecipe.Z2_PickPosition);
        }

        private void LoadingTray_MoveLoadingPosition()
        {
            Y1Axis.MoveAbs(CDef.PickerRecipe.Y1_LoadPosition);
            Console.WriteLine(this.ProcessName + " " + "LoadingTray_MoveLoadingPosition");
        }

        private bool LoadingTray_MoveLoadingPosition_Done()
        {
            return Y1Axis.IsOnPosition(CDef.PickerRecipe.Y1_LoadPosition);
        }

        private void UnloadingTray_MoveLoadingPosition()
        {
            Y2Axis.MoveAbs(CDef.PickerRecipe.Y2_LoadPosition);
            Console.WriteLine(this.ProcessName + " " + "UnloadingTray_MoveLoadingPosition");
        }

        private bool UnloadingTray_MoveLoadingPosition_Done()
        {
            return Y2Axis.IsOnPosition(CDef.PickerRecipe.Y2_LoadPosition);
        }
        #endregion

        #region __UNDER_VISION__
        private void Picker_MoveZUnderVisionPosition()
        {
            Z1Axis.MoveAbs(CDef.UnderVisionRecipe.Z1_UnderVisionPosition);
            Z2Axis.MoveAbs(CDef.UnderVisionRecipe.Z2_UnderVisionPosition);
        }

        private bool Picker_MoveZUnderVisionPosition_Done()
        {
            return Z1Axis.IsOnPosition(CDef.UnderVisionRecipe.Z1_UnderVisionPosition) &&
                   Z2Axis.IsOnPosition(CDef.UnderVisionRecipe.Z2_UnderVisionPosition);
        }

        private void Picker_MoveXYTUnderVisionPosition_1()
        {
            X1Axis.MoveAbs(CDef.UnderVisionRecipe.X1_UnderVisionPosition_Head1);
            YY1Axis.MoveAbs(CDef.UnderVisionRecipe.YY1_UnderVisionPosition);
            T1Axis.MoveAbs(CDef.UnderVisionRecipe.T1_UnderVisionPosition);
        }

        private bool Picker_MoveXYTUnderVisionPosition_1_Done()
        {
            return X1Axis.IsOnPosition(CDef.UnderVisionRecipe.X1_UnderVisionPosition_Head1) &&
                   YY1Axis.IsOnPosition(CDef.UnderVisionRecipe.YY1_UnderVisionPosition) &&
                   T1Axis.IsOnPosition(CDef.UnderVisionRecipe.T1_UnderVisionPosition);
        }

        private void Picker_MoveXYTUnderVisionPosition_2()
        {
            X1Axis.MoveAbs(CDef.UnderVisionRecipe.X1_UnderVisionPosition_Head2);
            XXAxis.MoveAbs(CDef.UnderVisionRecipe.XX_UnderVisionPosition);
            YY2Axis.MoveAbs(CDef.UnderVisionRecipe.YY2_UnderVisionPosition);
            T2Axis.MoveAbs(CDef.UnderVisionRecipe.T2_UnderVisionPosition);
        }

        private bool Picker_MoveXYTUnderVisionPosition_2_Done()
        {
            return X1Axis.IsOnPosition(CDef.UnderVisionRecipe.X1_UnderVisionPosition_Head2) &&
                   XXAxis.IsOnPosition(CDef.UnderVisionRecipe.XX_UnderVisionPosition) &&
                   YY2Axis.IsOnPosition(CDef.UnderVisionRecipe.YY2_UnderVisionPosition) &&
                   T2Axis.IsOnPosition(CDef.UnderVisionRecipe.T2_UnderVisionPosition);
        }

        private void Picker_MoveXUnderVisionPosition_Head2()
        {
            X1Axis.MoveAbs(CDef.UnderVisionRecipe.X1_UnderVisionPosition_Head2);
        }

        private bool Picker_MoveXUnderVisionPosition_Head2_Done()
        {
            return X1Axis.IsOnPosition(CDef.UnderVisionRecipe.X1_UnderVisionPosition_Head2);
        }

        private void ImageGrab(EVisionArea visionArea)
        {
            CDef.IO.Output.LightUnder = true;

            Sleep(50);

            CDef.MainViewModel.VisionAutoVM.GrabCommand.Execute(visionArea);
        }

        private EGrabRtnCode ImageGrabResult(string Mode)
        {
            return CDef.BotCamera.GrabResult.RtnCode;
        }

        private void StartVisionInspect(EVisionArea visionArea)
        {
            CDef.MainViewModel.VisionAutoVM.InspectCommand.Execute(visionArea);
            CDef.IO.Output.LightUnder = false;
        }

        private bool IsVisionInspectDone(string Mode = null)
        {
            return CDef.MainViewModel.VisionAutoVM.UnderVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
        }

        private IVisionResult VisionResult(string Mode = null)
        {
            return CDef.MainViewModel.VisionAutoVM.UnderVisionProcess.Result;
        }
        #endregion

        #region __PLACE__
        private void Picker_MoveUnloadingInspectAvoidPosition()
        {
            X1Axis.MoveAbs(CDef.UpperVisionRecipe.X1_UnloadingVision_AvoidPosition);
        }

        private bool Picker_MoveUnloadingInspectAvoidPosition_Done()
        {
            return X1Axis.IsOnPosition(CDef.UpperVisionRecipe.X1_UnloadingVision_AvoidPosition);
        }

        private void Picker_MoveTrayPlacePosition()
        {
            double offsetY = CDef.CurrentUnloadingTray.WorkStartIndex_Y * CDef.CommonRecipe.UnloadingTray_Y_Pitch;

            double Y2_WorkingPosition = CDef.PickerRecipe.Y2_WorkPosition_Tray1;
            if (CDef.CurrentUnloadingTray == CDef.UnloadingTray2)
            {
                Y2_WorkingPosition = CDef.PickerRecipe.Y2_WorkPosition_Tray2;
            }

            Y2Axis.MoveAbs(Y2_WorkingPosition - offsetY);

            Console.WriteLine(this.ProcessName + " " + "Picker_MoveTrayPlacePosition");
        }

        private void Picker_MoveXYTPlacePosition()
        {
            double offsetX = CDef.CurrentUnloadingTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;

            if (CDef.CurrentUnloadingTray == CDef.UnloadingTray2)
            {
                offsetX -= CDef.CommonRecipe.UnloadingTray_Offset_X;
            }

            double[] visionOffsetX = new double[] { Datas.Data_UnderInspect_Result[0].DetectedOffset.X * CDef.UnderVisionRecipe.UnderVision_PixelSize / 1000,
                                                    Datas.Data_UnderInspect_Result[1].DetectedOffset.X * CDef.UnderVisionRecipe.UnderVision_PixelSize / 1000};
            double[] visionOffsetY = new double[] { Datas.Data_UnderInspect_Result[0].DetectedOffset.Y * CDef.UnderVisionRecipe.UnderVision_PixelSize / 1000,
                                                    Datas.Data_UnderInspect_Result[1].DetectedOffset.Y * CDef.UnderVisionRecipe.UnderVision_PixelSize / 1000};

            X1Axis.MoveAbs(CDef.PickerRecipe.X1_PlacePosition - offsetX - visionOffsetX[0]);
            XXAxis.MoveAbs(CDef.PickerRecipe.XX_PlacePosition + (visionOffsetX[0] * 10) - (visionOffsetX[1] * 10));
            YY1Axis.MoveAbs(CDef.PickerRecipe.YY1_PlacePosition - visionOffsetY[0]);
            YY2Axis.MoveAbs(CDef.PickerRecipe.YY2_PlacePosition - visionOffsetY[1]);
            T1Axis.MoveAbs(CDef.PickerRecipe.T1_PlacePosition);
            T2Axis.MoveAbs(CDef.PickerRecipe.T2_PlacePosition);

            Console.WriteLine(this.ProcessName + " " + "Picker_MoveXYTPlacePosition");
        }

        private bool Picker_MoveXYTPlacePosition_Done()
        {
            double offsetX = CDef.CurrentUnloadingTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;

            if (CDef.CurrentUnloadingTray == CDef.UnloadingTray2)
            {
                offsetX -= CDef.CommonRecipe.UnloadingTray_Offset_X;
            }

            double[] visionOffsetX = new double[] { Datas.Data_UnderInspect_Result[0].DetectedOffset.X * CDef.UnderVisionRecipe.UnderVision_PixelSize / 1000,
                                                    Datas.Data_UnderInspect_Result[1].DetectedOffset.X * CDef.UnderVisionRecipe.UnderVision_PixelSize / 1000};
            double[] visionOffsetY = new double[] { Datas.Data_UnderInspect_Result[0].DetectedOffset.Y * CDef.UnderVisionRecipe.UnderVision_PixelSize / 1000,
                                                    Datas.Data_UnderInspect_Result[1].DetectedOffset.Y * CDef.UnderVisionRecipe.UnderVision_PixelSize / 1000};

            return X1Axis.IsOnPosition(CDef.PickerRecipe.X1_PlacePosition - offsetX - visionOffsetX[0])
                && XXAxis.IsOnPosition(CDef.PickerRecipe.XX_PlacePosition + (visionOffsetX[0] * 10) - (visionOffsetX[1] * 10))
                && YY1Axis.IsOnPosition(CDef.PickerRecipe.YY1_PlacePosition - visionOffsetY[0])
                && YY2Axis.IsOnPosition(CDef.PickerRecipe.YY2_PlacePosition - visionOffsetY[1])
                && T1Axis.IsOnPosition(CDef.PickerRecipe.T1_PlacePosition)
                && T2Axis.IsOnPosition(CDef.PickerRecipe.T2_PlacePosition);
        }

        private void Picker_MoveZPlace_SubPosition()
        {
            Z1Axis.MoveAbs(CDef.PickerRecipe.Z1_PlacePosition - 2 /* mm */);
            Z2Axis.MoveAbs(CDef.PickerRecipe.Z2_PlacePosition - 2 /* mm */);

            Console.WriteLine(this.ProcessName + " " + "Picker_MoveZPlacePosition");
        }

        private bool Picker_MoveZPlace_SubPosition_Done()
        {
            return Z1Axis.IsOnPosition(CDef.PickerRecipe.Z1_PlacePosition - 2 /* mm */)
                && Z2Axis.IsOnPosition(CDef.PickerRecipe.Z2_PlacePosition - 2 /* mm */);
        }

        private void Picker_MoveZPlacePosition()
        {
            Z1Axis.MoveAbs(CDef.PickerRecipe.Z1_PlacePosition, CDef.PickerRecipe.Z_PlaceSpeed);
            Z2Axis.MoveAbs(CDef.PickerRecipe.Z2_PlacePosition, CDef.PickerRecipe.Z_PlaceSpeed);

            Console.WriteLine(this.ProcessName + " " + "Picker_MoveZPlacePosition");
        }

        private bool Picker_MoveZPlacePosition_Done()
        {
            return Z1Axis.IsOnPosition(CDef.PickerRecipe.Z1_PlacePosition)
                && Z2Axis.IsOnPosition(CDef.PickerRecipe.Z2_PlacePosition);
        }
        #endregion

        #endregion
    }
}
