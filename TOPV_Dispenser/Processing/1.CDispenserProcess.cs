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
using TOPV_Dispenser.Define;

namespace TOPV_Dispenser.Processing
{
    public class CDispenserProcess : ProcessingBase
    {
        #region Constructor(s)
        public CDispenserProcess(IProcessing _Parent, string _Name, int _MessageCodeStartIndex, int _IntervalTimeMs, EHead head)
            : base(_Parent, _Name, _MessageCodeStartIndex, _IntervalTimeMs)
        {
            Head = head;
        }
        #endregion

        #region Properties
        #region Motion(s)
        public MotionAjinAXL XAxis
        {
            get
            {
                if (Head == EHead.HEAD_Left)
                {
                    return (MotionAjinAXL)CDef.AllAxis.X1Axis;
                }
                else
                {
                    return (MotionAjinAXL)CDef.AllAxis.X2Axis;
                }
            }
        }

        public MotionAjinAXL YAxis
        {
            get
            {
                if (Head == EHead.HEAD_Left)
                {
                    return (MotionAjinAXL)CDef.AllAxis.Y1Axis;
                }
                else
                {
                    return (MotionAjinAXL)CDef.AllAxis.Y2Axis;
                }
            }
        }

        public MotionAjinAXL ZAxis
        {
            get
            {
                if (Head == EHead.HEAD_Left)
                {
                    return (MotionAjinAXL)CDef.AllAxis.Z1Axis;
                }
                else
                {
                    return (MotionAjinAXL)CDef.AllAxis.Z2Axis;
                }
            }
        }
        #endregion

        public EHead Head { get; private set; }

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
        #endregion

        #region Data(s)
        #endregion

        #region Overrider(s)
        public override PRtnCode ProcessOrigin()
        {
            PRtnCode nRtnCode = PRtnCode.RtnOk;

            switch ((EDispenserHomeStep)Step.HomeStep)
            {
                case EDispenserHomeStep.Start:
                    Log.Debug("Home start");
                    Step.HomeStep++;
                    break;
                case EDispenserHomeStep.ZAxis_HomeSearch:
                    ZAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case EDispenserHomeStep.ZAxis_HomeWait:
                    if (ZAxis.Status.IsHomeDone)
                    {
                        Log.Debug($"{ZAxis} origin done");
                        ZAxis.ClearPosition();
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
                            CDef.RootProcess.SetWarning($"{ZAxis} home search timeout");
                        }
                    }
                    break;
                case EDispenserHomeStep.XAxisYAxis_HomeSearch:
                    XAxis.HomeSearch();
                    YAxis.HomeSearch();
                    Step.HomeStep++;
                    break;
                case EDispenserHomeStep.XAxisYAxis_HomeSearchWait:
                    if (XAxis.Status.IsHomeDone && YAxis.Status.IsHomeDone)
                    {
                        Log.Debug($"{XAxis} and {YAxis} origin done");
                        XAxis.ClearPosition();
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
                            if (XAxis.Status.IsHomeDone)
                            { 
                                CDef.RootProcess.SetWarning($"{XAxis} home search timeout");
                            }
                            else
                            {
                                CDef.RootProcess.SetWarning($"{YAxis} home search timeout");
                            }
                        }
                    }
                    break;
                case EDispenserHomeStep.End:
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

            switch ((EDispenserToRunStep)Step.ToRunStep)
            {
                case EDispenserToRunStep.Start:
                    Log.Debug("To Run start");
                    Step.ToRunStep++;
                    break;
                case EDispenserToRunStep.ZAxis_ReadyPosition_Move:
                    ZAxis_ReadyPosition_Move();
                    Step.ToRunStep++;
                    break;
                case EDispenserToRunStep.ZAxis_ReadyPosition_MoveWait:
                    if (ZAxis_ReadyPosition_MoveDone())
                    {
                        Log.Debug($"{ZAxis} move Ready Position Done");
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
                            CDef.RootProcess.SetWarning($"{ZAxis} move Ready Position timeout");
                        }
                    }
                    break;
                case EDispenserToRunStep.XAxisYAxis_VisionPosition_Move:
                    XAxisYAxis_VisionPosition_Move();
                    Step.ToRunStep++;
                    break;
                case EDispenserToRunStep.XAxisYAxis_VisionPosition_MoveWait:
                    if (XAxisYAxis_VisionPosition_MoveDone())
                    {
                        Log.Debug($"{XAxis} and {YAxis} move Vision Position Done");
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
                            CDef.RootProcess.SetWarning($"{XAxis} and {YAxis} move Vision Position time out");
                        }
                    }
                    break;
                case EDispenserToRunStep.End:
                    Log.Debug("To Run done");
                    ProcessingStatus = EProcessingStatus.ToRunDone;
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

            return nRtnCode;
        }
        #endregion

        #region Run Function(s)
        #endregion

        #region Moving Function(s)
        private void XAxisYAxis_VisionPosition_Move()
        {

        }

        private bool XAxisYAxis_VisionPosition_MoveDone()
        {
            return true;
        }

        private void ZAxis_ReadyPosition_Move()
        {

        }

        private bool ZAxis_ReadyPosition_MoveDone()
        {
            return true;
        }
        #endregion

        #region Privates
        private ERunMode _RunMode;
        #endregion
    }
}