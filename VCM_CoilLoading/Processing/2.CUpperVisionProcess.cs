using System;
using System.Collections.Generic;
using System.Windows;
using TopCom.Processing;
using TopMotion;
using TopUI.Models;
using TopVision;
using TopVision.Models;
using VCM_CoilLoading.Define;
using VCM_CoilLoading.MVVM.ViewModels;

namespace VCM_CoilLoading.Processing
{
    public class VisionInspectEventArgs : EventArgs
    {
        public VisionInspectEventArgs(string mode)
        {
            Mode = mode;
        }

        public string Mode { get; set; }
    }

    public class CUpperVisionProcess : ProcessingBase
    {
        enum HomeStep : int
        {
            HomeZVisionAxis,
            WaitZVisionAxisHomeDone,
            WaitAllZAxisHomeDone,
            HomeAnotherAxis,
            WaitAnotherAxisHomeDone,
            HomeDone
        }

        enum LoadVisionStep : int
        {
            UpperVisionStart,
            CheckInspectDoneStatus,

            Move_InspectingPosition_Head1,
            Wait_InspectingPosition_Head1,

            Waiting_Pick_Place,
            Set_Flag_VisionInspectAvoid,
            Get_Flag_VisionInspectAvoid,
            Get_Flag_TrayReadyPosition,

            ImageGrab_Head1,
            ImageGrabResult_Head1,
            StartVisionInspect_Head1,
            WaitVisionInspect_Head1,
            AplyVisionInspectResult_Head1,

            Move_InspectingPosition_Head2,
            Wait_InspectingPosition_Head2,

            ImageGrab_Head2,
            ImageGrabResult_Head2,
            StartVisionInspect_Head2,
            WaitVisionInspect_Head2,
            AplyVisionInspectResult_Head2,

            VisionResultUpdate,
            UpperVisionDone,
        }

        enum UnloadVisionStep : int
        {
            UpperVisionStart,
            CheckInspectDoneStatus,

            Move_InspectingPosition_Head1,
            Wait_InspectingPosition_Head1,

            Waiting_Pick_Place,
            Set_Flag_VisionInspectAvoid,
            Get_Flag_VisionInspectAvoid,
            Get_Flag_TrayReadyPosition,

            ImageGrab_Head1,
            ImageGrabResult_Head1,
            StartVisionInspect_Head1,
            WaitVisionInspect_Head1,
            AplyVisionInspectResult_Head1,

            Move_InspectingPosition_Head2,
            Wait_InspectingPosition_Head2,

            ImageGrab_Head2,
            ImageGrabResult_Head2,
            StartVisionInspect_Head2,
            WaitVisionInspect_Head2,
            AplyVisionInspectResult_Head2,

            VisionResultUpdate,
            UpperVisionDone,
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

        #region Data
        public List<IVisionResult> Data_LoadingInspect_Result
        {
            get; set;
        } = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };

        public List<IVisionResult> Data_UnloadingInspect_Result
        {
            get; set;
        } = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
        #endregion

        #region FLAGS
        public bool Flag_LoadingInspect_Done
        {
            get
            {
                bool result = true;

                if (CDef.GlobalRecipe.SkipLoadVision) return true;

                ITrayModel tmpTray = CDef.CurrentLoadingTray;

                if (tmpTray != null)
                {
                    int startCell = tmpTray.WorkStartIndex - tmpTray.FirstIndex;
                    startCell /= tmpTray.HeadCount;
                    startCell *= tmpTray.HeadCount;
                    startCell += tmpTray.FirstIndex;

                    for (int i = 0; i < tmpTray.HeadCount; i++)
                    {
                        result &= tmpTray.GetCellStatus(startCell + i) > TopUI.Define.ECellStatus.Processing;
                    }
                }
                else
                {
                    result = false;
                }

                return result;
            }
        }

        public bool Flag_UnloadingInspect_Done
        {
            get
            {
                bool result = true;

                if (CDef.GlobalRecipe.SkipUnloadVision) return true;

                ITrayModel tmpTray = CDef.CurrentUnloadingTray;

                if (tmpTray != null)
                {
                    int startCell = tmpTray.WorkStartIndex - tmpTray.FirstIndex;
                    startCell /= tmpTray.HeadCount;
                    startCell *= tmpTray.HeadCount;
                    startCell += tmpTray.FirstIndex;

                    for (int i = 0; i < tmpTray.HeadCount; i++)
                    {
                        result &= tmpTray.GetCellStatus(startCell + i) > TopUI.Define.ECellStatus.Processing;
                    }
                }
                else
                {
                    result = false;
                }

                return result;
            }
        }

        public bool Flag_LoadVisionInspectAvoid_Request
        {
            get; set;
        }

        public bool Flag_UnloadVisionInspectAvoid_Request
        {
            get; set;
        }
        #endregion

        #region MOTIONS
        public MotionFas16000 X2Axis
        {
            get
            {
                return (MotionFas16000)CDef.AllAxis.X2Axis;
            }
        }

        public MotionPlusE Z3Axis
        {
            get
            {
                return (MotionPlusE)CDef.AllAxis.Z3Axis;
            }
        }
        #endregion

        Random random = new Random();

        public CUpperVisionProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
        }

        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch (Step.HomeStep)
            {
                case (int)HomeStep.HomeZVisionAxis:
                    Z3Axis.HomeSearch();
                    Step.HomeStep++;
                    Log.Debug("Start search Z3 home position");
                    break;
                case (int)HomeStep.WaitZVisionAxisHomeDone:
                    if (Z3Axis.Status.IsHomeDone)
                    {
                        HomeStatus.Z3Done = true;
                        Log.Debug("Z3 home done");
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
                            Log.Warn("Home search timeout: Z3 Axis");
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
                    X2Axis.HomeSearch();
                    Log.Debug("Start search X2 home position");
                    Step.HomeStep++;
                    break;
                case (int)HomeStep.WaitAnotherAxisHomeDone:
                    if (X2Axis.Status.IsHomeDone)
                    {
                        Log.Debug("X2 home done");
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
                            Log.Warn("Home search timeout: X2 Axis");
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

            switch (Step.ToRunStep)
            {
                case 0:
                    Console.WriteLine(this.ProcessName + " " + "Upper Vision ToRUN");
                    Step.ToRunStep++;
                    break;
                case 1:
                    Step.ToRunStep++;
                    break;
                case 2:
                    switch (CDef.RootProcess.RunMode)
                    {
                        case ERunMode.AutoRun:
                            if (!Flag_LoadingInspect_Done && !Flag_UnloadingInspect_Done)
                            {
                                if (CDef.IO.HeadStatus.HeadOccupied)
                                {
                                    this.RunMode = ERunMode.Manual_UpperVision_Unload_Inspect;
                                }
                                else
                                {
                                    this.RunMode = ERunMode.Manual_UpperVision_Load_Inspect;
                                }
                            }
                            else if (!Flag_LoadingInspect_Done)
                            {
                                this.RunMode = ERunMode.Manual_UpperVision_Load_Inspect;
                            }
                            else if (!Flag_UnloadingInspect_Done)
                            {
                                this.RunMode = ERunMode.Manual_UpperVision_Unload_Inspect;
                            }
                            else
                            {
                                //TODO: Review this (Should wait for either pick or place first??)
                                this.RunMode = ERunMode.Manual_UpperVision_Load_Inspect;
                            }
                            break;
                        case ERunMode.Manual_Picker_Pick:
                            this.RunMode = ERunMode.Manual_UpperVision_Load_Inspect;
                            break;
                        case ERunMode.Manual_Picker_Place:
                            this.RunMode = ERunMode.Manual_UpperVision_Unload_Inspect;
                            break;
                        case ERunMode.Manual_UpperVision_Load_Inspect:
                            this.RunMode = ERunMode.Manual_UpperVision_Load_Inspect;
                            break;
                        case ERunMode.Manual_UpperVision_Unload_Inspect:
                            this.RunMode = ERunMode.Manual_UpperVision_Unload_Inspect;
                            break;
                        default:
                            this.RunMode = ERunMode.Stop;
                            break;
                    }

                    Step.ToRunStep++;
                    return base.ProcessToRun();
                default:
                    Sleep(20);
                    break;
            }

            return nRtn;
        }

        public override PRtnCode ProcessRun()
        {
            PRtnCode nRtn = PRtnCode.RtnOk;

            switch (this.RunMode)
            {
                case ERunMode.Manual_UpperVision_Load_Inspect:
                    Running_UpperVision_Load_Inspect();
                    break;
                case ERunMode.Manual_UpperVision_Unload_Inspect:
                    Running_UpperVision_Unload_Inspect();
                    break;
                default:
                    break;
            }

            return nRtn;
        }

        private void Running_UpperVision_Load_Inspect()
        {
            switch ((LoadVisionStep)Step.RunStep)
            {
                case LoadVisionStep.UpperVisionStart:
                    Log.Debug("UpperVision_Load_Inspect");
                    Step.RunStep++;
                    break;
                case LoadVisionStep.CheckInspectDoneStatus:
                    if (Flags.Flag_UpperVision_UnloadInspect_Done && Flags.Flag_UpperVision_LoadInspect_Done)
                    {
                        Sleep(20);
                    }
                    else if (Flags.Flag_UpperVision_LoadInspect_Done)
                    {
                        this.RunMode = ERunMode.Manual_UpperVision_Unload_Inspect;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case LoadVisionStep.Move_InspectingPosition_Head1:
                    if (CDef.CurrentLoadingTray == null)
                    {
                        Sleep(10); // Waiting for tray change
                    }
                    else
                    {
                        Upper_Move_LoadingInspectingPosition_Head1();
                        Step.RunStep++;
                    }
                    break;
                case LoadVisionStep.Wait_InspectingPosition_Head1:
                    if (Upper_Move_LoadingInspectingPosition_Head1_Done())
                    {
                        Log.Debug("Upper_Move_LoadingInspectingPosition_Head1_Done");
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
                            Log.Warn("LoadingInspectingPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case LoadVisionStep.Waiting_Pick_Place:
                    if (CDef.CurrentLoadingTray.GetCellStatus(CDef.CurrentLoadingTray.WorkStartIndex + CDef.CurrentLoadingTray.HeadCount - 2)
                        > TopUI.Define.ECellStatus.Processing)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.PickPlace_TimeOut * 1000)
                        {
                            Sleep(10); // Waiting for tray pick done
                        }
                        else
                        {
                            Log.Warn("Tray pick timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case LoadVisionStep.Set_Flag_VisionInspectAvoid:
                    Flag_LoadVisionInspectAvoid_Request = true;
                    Step.RunStep++;
                    break;
                case LoadVisionStep.Get_Flag_VisionInspectAvoid:
                    if (Flags.Flag_Picker_LoadVisionInspectAvoid)
                    {
                        Log.Debug("Flag_Picker_LoadVisionInspectAvoid check");
                        Flag_LoadVisionInspectAvoid_Request = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.PickerVisionAvoid_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Load Vision Avoid timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case LoadVisionStep.Get_Flag_TrayReadyPosition:
                    if (Flags.Flag_Picker_TrayPickPosition_Ready)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(20);
                    }
                    break;
                case LoadVisionStep.ImageGrab_Head1:
                    ImageGrab(EVisionArea.LOAD);
                    Step.RunStep++;
                    break;
                case LoadVisionStep.ImageGrabResult_Head1:
                    if (CDef.TopCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Load Vision Grab Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                        break;
                    }

                    if (ImageGrabResult("Loading1") == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Log.Warn("Vision Image Grab failed");
                        CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                    }
                    break;
                case LoadVisionStep.StartVisionInspect_Head1:
                    Log.Debug("Flag_LoadingInspectting HEAD 1");
                    StartVisionInspect(EVisionArea.LOAD);

                    Step.RunStep++;
                    break;
                case LoadVisionStep.WaitVisionInspect_Head1:
                    if (IsVisionInspectDone("Loading1"))
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
                            Log.Warn("Load Vision Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case LoadVisionStep.AplyVisionInspectResult_Head1:
                    Log.Info(VisionResult("Loading1"));
                    Data_LoadingInspect_Result[0] = VisionResult("Loading1");
                    if (Data_LoadingInspect_Result[0].Judge == EVisionJudge.OK)
                    {
                        CDef.CurrentLoadingTray.SetSingleCell(TopUI.Define.ECellStatus.OKVision, CDef.CurrentLoadingTray.WorkStartIndex + 1);
                    }
                    else
                    {
                        CDef.CurrentLoadingTray.SetSingleCell(TopUI.Define.ECellStatus.NGVision, CDef.CurrentLoadingTray.WorkStartIndex + 1);
                    }
                    Step.RunStep++;
                    break;
                case LoadVisionStep.Move_InspectingPosition_Head2:
                    Upper_Move_LoadingInspectingPosition_Head2();
                    Step.RunStep++;
                    break;
                case LoadVisionStep.Wait_InspectingPosition_Head2:
                    if (Upper_Move_LoadingInspectingPosition_Head2_Done())
                    {
                        Log.Debug("Upper_Move_LoadingInspectingPosition_Head2_Done");
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
                            Log.Warn("LoadingInspectingPosition_Head2 Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case LoadVisionStep.ImageGrab_Head2:
                    ImageGrab(EVisionArea.LOAD);
                    Step.RunStep++;
                    break;
                case LoadVisionStep.ImageGrabResult_Head2:
                    if (CDef.TopCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Load Vision Grab Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                        break;
                    }

                    if (ImageGrabResult("Loading2") == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Log.Warn("Vision Image Grab failed");
                        CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                    }
                    break;
                case LoadVisionStep.StartVisionInspect_Head2:
                    Log.Debug("Flag_LoadingInspectting HEAD 2");
                    StartVisionInspect(EVisionArea.LOAD);

                    Step.RunStep++;
                    break;
                case LoadVisionStep.WaitVisionInspect_Head2:
                    if (IsVisionInspectDone("Loading2"))
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
                            Log.Warn("Load Vision Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case LoadVisionStep.AplyVisionInspectResult_Head2:
                    Log.Info(VisionResult("Loading2"));
                    Data_LoadingInspect_Result[1] = VisionResult("Loading2");

                    if (Data_LoadingInspect_Result[1].Judge == EVisionJudge.OK)
                    {
                        CDef.CurrentLoadingTray.SetSingleCell(TopUI.Define.ECellStatus.OKVision, CDef.CurrentLoadingTray.WorkStartIndex);
                    }
                    else
                    {
                        CDef.CurrentLoadingTray.SetSingleCell(TopUI.Define.ECellStatus.NGVision, CDef.CurrentLoadingTray.WorkStartIndex);
                    }

                    Step.RunStep++;
                    break;
                case LoadVisionStep.VisionResultUpdate:
                    Log.Info($"Flag_LoadingInspect_Done {CDef.CurrentLoadingTray.WorkStartIndex}");
                    Step.RunStep++;
                    break;
                case LoadVisionStep.UpperVisionDone:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        if (Flags.Flag_UpperVision_UnloadInspect_Done && Flags.Flag_UpperVision_LoadInspect_Done)
                        {
                            break;
                        }
                        this.RunMode = ERunMode.Manual_UpperVision_Unload_Inspect;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UpperVision_Load_Inspect)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    }
                    break;
                default:
                    Sleep(10);
                    break;
            }
        }

        private void Running_UpperVision_Unload_Inspect()
        {
            switch ((UnloadVisionStep)Step.RunStep)
            {
                case UnloadVisionStep.UpperVisionStart:
                    Log.Debug("UpperVision_Unload_Inspect");
                    Console.WriteLine(this.ProcessName + " " + "UpperVision_Unload_Inspect Start");
                    Step.RunStep++;
                    break;
                case UnloadVisionStep.CheckInspectDoneStatus:
                    if (Flags.Flag_UpperVision_UnloadInspect_Done && Flags.Flag_UpperVision_LoadInspect_Done)
                    {
                        Sleep(20);
                    }
                    else if (Flags.Flag_UpperVision_UnloadInspect_Done)
                    {
                        this.RunMode = ERunMode.Manual_UpperVision_Load_Inspect;
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case UnloadVisionStep.Move_InspectingPosition_Head1:
                    if (CDef.CurrentUnloadingTray == null)
                    {
                        Sleep(10); // Waiting for tray change
                    }
                    else
                    {
                        Upper_Move_UnloadingInspectingPosition_Head1();
                        Step.RunStep++;
                    }
                    break;
                case UnloadVisionStep.Wait_InspectingPosition_Head1:
                    if (Upper_Move_UnloadingInspectingPosition_Head1_Done())
                    {
                        Log.Debug("Upper_Move_UnloadingInspectingPosition_Head1_Done");
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
                            Log.Warn("UnloadingInspectingPosition Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnloadVisionStep.Waiting_Pick_Place:
                    if (CDef.CurrentUnloadingTray.GetCellStatus(CDef.CurrentUnloadingTray.WorkStartIndex + CDef.CurrentUnloadingTray.HeadCount - 2)
                            > TopUI.Define.ECellStatus.Processing)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.PickPlace_TimeOut * 1000)
                        {
                            Sleep(10); // Waiting for tray place done
                        }
                        else
                        {
                            Log.Warn("Tray place timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    else
                    {
                        Step.RunStep++;
                    }
                    break;
                case UnloadVisionStep.Set_Flag_VisionInspectAvoid:
                    Flag_UnloadVisionInspectAvoid_Request = true;
                    Step.RunStep++;
                    break;
                case UnloadVisionStep.Get_Flag_VisionInspectAvoid:
                    if (Flags.Flag_Picker_UnloadVisionInspectAvoid)
                    {
                        Log.Debug("Flag_Picker_UnloadVisionInspectAvoid Done");
                        Flag_UnloadVisionInspectAvoid_Request = false;
                        Step.RunStep++;
                    }
                    else
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.PickerVisionAvoid_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Unload Vision Avoid timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnloadVisionStep.Get_Flag_TrayReadyPosition:
                    if (Flags.Flag_Picker_TrayPlacePosition_Ready)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Sleep(20);
                    }
                    break;
                case UnloadVisionStep.ImageGrab_Head1:
                    ImageGrab(EVisionArea.UNLOAD);
                    Step.RunStep++;
                    break;
                case UnloadVisionStep.ImageGrabResult_Head1:
                    if (CDef.TopCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Unload Vision Grab Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                        break;
                    }

                    if (ImageGrabResult("Unloading1") == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Log.Warn("Vision Image Grab failed");
                        CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                    }
                    break;
                case UnloadVisionStep.StartVisionInspect_Head1:
                    Log.Debug("Flag_LoadingInspectting HEAD 1");
                    StartVisionInspect(EVisionArea.UNLOAD);

                    Step.RunStep++;
                    break;
                case UnloadVisionStep.WaitVisionInspect_Head1:
                    if (IsVisionInspectDone("Unloading1"))
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
                            Log.Warn("Unload Vision Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnloadVisionStep.AplyVisionInspectResult_Head1:
                    Log.Info(VisionResult("Unloading1"));
                    Data_UnloadingInspect_Result[0] = VisionResult("Unloading1");
                    if (Data_UnloadingInspect_Result[0].Judge == EVisionJudge.OK)
                    {
                        CDef.CurrentUnloadingTray.SetSingleCell(TopUI.Define.ECellStatus.OKVision, CDef.CurrentUnloadingTray.WorkStartIndex + 1);
                    }
                    else
                    {
                        CDef.CurrentUnloadingTray.SetSingleCell(TopUI.Define.ECellStatus.NGVision, CDef.CurrentUnloadingTray.WorkStartIndex + 1);
                    }
                    Step.RunStep++;
                    break;
                case UnloadVisionStep.Move_InspectingPosition_Head2:
                    Upper_Move_UnloadingInspectingPosition_Head2();
                    Step.RunStep++;
                    break;
                case UnloadVisionStep.Wait_InspectingPosition_Head2:
                    if (Upper_Move_UnloadingInspectingPosition_Head2_Done())
                    {
                        Log.Debug("Upper_Move_UnloadingInspectingPosition_Head2_Done");
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
                            Log.Warn("UnloadingInspectingPosition_Head2 Moving Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnloadVisionStep.ImageGrab_Head2:
                    ImageGrab(EVisionArea.UNLOAD);
                    Step.RunStep++;
                    break;
                case UnloadVisionStep.ImageGrabResult_Head2:
                    if (CDef.TopCamera.IsGrabDone == false)
                    {
                        if (PTimer.Now - PTimer.StepTimeoutWatcher < CDef.CommonRecipe.VisionInspect_TimeOut * 1000)
                        {
                            Sleep(10);
                        }
                        else
                        {
                            Log.Warn("Unload Vision Grab Timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                        break;
                    }

                    if (ImageGrabResult("Unloading2") == EGrabRtnCode.OK)
                    {
                        Step.RunStep++;
                    }
                    else
                    {
                        Log.Warn("Vision Image Grab failed");
                        CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                    }
                    break;
                case UnloadVisionStep.StartVisionInspect_Head2:
                    Log.Debug("Flag_UnloadingInspectting HEAD 2");
                    StartVisionInspect(EVisionArea.UNLOAD);

                    Step.RunStep++;
                    break;
                case UnloadVisionStep.WaitVisionInspect_Head2:
                    if (IsVisionInspectDone("Unloading2"))
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
                            Log.Warn("Unload Vision Inspect timeout");
                            CDef.RootProcess.Mode = ProcessingMode.ModeToWarning;
                        }
                    }
                    break;
                case UnloadVisionStep.AplyVisionInspectResult_Head2:
                    Log.Info(VisionResult("Unloading2"));
                    Data_UnloadingInspect_Result[1] = VisionResult("Unloading2");

                    if (Data_UnloadingInspect_Result[1].Judge == EVisionJudge.OK)
                    {
                        CDef.CurrentUnloadingTray.SetSingleCell(TopUI.Define.ECellStatus.OKVision, CDef.CurrentUnloadingTray.WorkStartIndex);
                    }
                    else
                    {
                        CDef.CurrentUnloadingTray.SetSingleCell(TopUI.Define.ECellStatus.NGVision, CDef.CurrentUnloadingTray.WorkStartIndex);
                    }

                    Step.RunStep++;
                    break;
                case UnloadVisionStep.VisionResultUpdate:
                    Log.Info($"Flag_UnloadingInspect_Done {CDef.CurrentUnloadingTray.WorkStartIndex}");
                    Step.RunStep++;
                    break;
                case UnloadVisionStep.UpperVisionDone:
                    if (CDef.RootProcess.RunMode == ERunMode.AutoRun)
                    {
                        if (Flags.Flag_UpperVision_UnloadInspect_Done && Flags.Flag_UpperVision_LoadInspect_Done)
                        {
                            break;
                        }
                        this.RunMode = ERunMode.Manual_UpperVision_Load_Inspect;
                    }
                    else if (CDef.RootProcess.RunMode == ERunMode.Manual_UpperVision_Unload_Inspect)
                    {
                        CDef.RootProcess.Mode = ProcessingMode.ModeToStop;
                    }
                    break;
                default:
                    Sleep(10);
                    break;
            }
        }

        #region Functions
        private void Upper_Move_LoadingInspectingPosition_Head2()
        {
            double offsetX = CDef.CurrentLoadingTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;

            X2Axis.MoveAbs(CDef.UpperVisionRecipe.X2_LoadingVisionPosition - offsetX);
            Z3Axis.MoveAbs(CDef.UpperVisionRecipe.Z3_LoadingVisionPosition);
        }

        private bool Upper_Move_LoadingInspectingPosition_Head2_Done()
        {
            double offsetX = CDef.CurrentLoadingTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;

            return X2Axis.IsOnPosition(CDef.UpperVisionRecipe.X2_LoadingVisionPosition - offsetX) &&
                   Z3Axis.IsOnPosition(CDef.UpperVisionRecipe.Z3_LoadingVisionPosition);
        }

        private void Upper_Move_LoadingInspectingPosition_Head1()
        {
            double offsetX = CDef.CurrentLoadingTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;
            offsetX += CDef.CurrentLoadingTray.ColumnCount / CDef.CurrentLoadingTray.HeadCount * CDef.CommonRecipe.LoadingTray_X_Pitch;
            offsetX += CDef.CommonRecipe.LoadingTray_Head2_Additional_Pitch;

            X2Axis.MoveAbs(CDef.UpperVisionRecipe.X2_LoadingVisionPosition - offsetX);
            Z3Axis.MoveAbs(CDef.UpperVisionRecipe.Z3_LoadingVisionPosition);
        }

        private bool Upper_Move_LoadingInspectingPosition_Head1_Done()
        {
            double offsetX = CDef.CurrentLoadingTray.WorkStartIndex_X * CDef.CommonRecipe.LoadingTray_X_Pitch;
            offsetX += (CDef.CurrentLoadingTray.ColumnCount / CDef.CurrentLoadingTray.HeadCount) * CDef.CommonRecipe.LoadingTray_X_Pitch;
            offsetX += CDef.CommonRecipe.LoadingTray_Head2_Additional_Pitch;

            return X2Axis.IsOnPosition(CDef.UpperVisionRecipe.X2_LoadingVisionPosition - offsetX) &&
                   Z3Axis.IsOnPosition(CDef.UpperVisionRecipe.Z3_LoadingVisionPosition);
        }

        private void Upper_Move_UnloadingInspectingPosition_Head2()
        {
            double offsetX = CDef.CurrentUnloadingTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;

            X2Axis.MoveAbs(CDef.UpperVisionRecipe.X2_UnloadingVisionPosition - offsetX);
            Z3Axis.MoveAbs(CDef.UpperVisionRecipe.Z3_UnloadingVisionPosition);
        }

        private bool Upper_Move_UnloadingInspectingPosition_Head2_Done()
        {
            double offsetX = CDef.CurrentUnloadingTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;

            return X2Axis.IsOnPosition(CDef.UpperVisionRecipe.X2_UnloadingVisionPosition - offsetX) &&
                   Z3Axis.IsOnPosition(CDef.UpperVisionRecipe.Z3_UnloadingVisionPosition);
        }

        private void Upper_Move_UnloadingInspectingPosition_Head1()
        {
            double offsetX = CDef.CurrentUnloadingTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;
            offsetX += (CDef.CurrentUnloadingTray.ColumnCount / CDef.CurrentUnloadingTray.HeadCount) * CDef.CommonRecipe.UnloadingTray_X_Pitch;
            offsetX += CDef.CommonRecipe.UnloadingTray_Head2_Additional_Pitch;

            X2Axis.MoveAbs(CDef.UpperVisionRecipe.X2_UnloadingVisionPosition - offsetX);
            Z3Axis.MoveAbs(CDef.UpperVisionRecipe.Z3_UnloadingVisionPosition);
        }

        private bool Upper_Move_UnloadingInspectingPosition_Head1_Done()
        {
            double offsetX = CDef.CurrentUnloadingTray.WorkStartIndex_X * CDef.CommonRecipe.UnloadingTray_X_Pitch;
            offsetX += (CDef.CurrentUnloadingTray.ColumnCount / CDef.CurrentUnloadingTray.HeadCount) * CDef.CommonRecipe.UnloadingTray_X_Pitch;
            offsetX += CDef.CommonRecipe.UnloadingTray_Head2_Additional_Pitch;

            return X2Axis.IsOnPosition(CDef.UpperVisionRecipe.X2_UnloadingVisionPosition - offsetX) &&
                   Z3Axis.IsOnPosition(CDef.UpperVisionRecipe.Z3_UnloadingVisionPosition);
        }

        private void ImageGrab(EVisionArea visionArea)
        {
            CDef.IO.Output.LightUpper = true;

            Sleep(50);

            CDef.MainViewModel.VisionAutoVM.GrabCommand.Execute(visionArea);
        }

        private EGrabRtnCode ImageGrabResult(string Mode)
        {
            return CDef.TopCamera.GrabResult.RtnCode;
        }

        private void StartVisionInspect(EVisionArea visionArea)
        {
            CDef.MainViewModel.VisionAutoVM.InspectCommand.Execute(visionArea);
            CDef.IO.Output.LightUpper = false;
        }

        private bool IsVisionInspectDone(string Mode = null)
        {
            if (Mode.Contains("Loading"))
            {
                return CDef.MainViewModel.VisionAutoVM.LoadVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
            }
            else
            {
                return CDef.MainViewModel.VisionAutoVM.UnloadVisionProcess.Status == EVisionProcessStatus.PROCESS_DONE;
            }
        }

        private IVisionResult VisionResult(string Mode = null)
        {
            if (Mode.Contains("Loading"))
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
