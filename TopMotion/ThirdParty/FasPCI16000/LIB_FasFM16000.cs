using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FASTECH
{
	/// <summary>
	/// Library for Fastech PCI 16000 Motion Board
	/// </summary>
    public class FasFM16000Lib
    {
		public const int MAX_AXIS = 16;

		// Referred by FM_16000x.h
		// typedef enum _FMM_ERROR
		public const int FMM_OK = 0;

		public const int FMM_OPEN_FAIL = 1;
		public const int FMM_CLOSE_FAIL = 2;
		public const int FMM_NOT_OPEN = 3;
		public const int FMM_INVALID_BD_NUM = 4;
		public const int FMM_INVALID_AXIS_NUM = 5;
		public const int FMM_INVALID_PARAMETER_NUM = 6;
		public const int FMM_PARAMETER_RANGE_ERROR = 7;
		public const int FMM_ZERO_RETURN_ERROR = 8;
		public const int FMM_TIMEOUT_ERROR = 9;
		public const int FMM_UNKNOWN_ERROR = 10;

		public const int DIR_INC = 1;
		public const int DIR_DEC = 0;

		public const int ENDCHECK_ENABLE = 1;
		public const int ENDCHECK_DISABLE = 0;

		//------------------------------------------------------------------
		//                 Flag Defines.
		//------------------------------------------------------------------
		public const uint FFLAG_ERRORALL = 0x00000001;
		public const uint FFLAG_ERRSERVOALARM = 0x00000002;
		public const uint FFLAG_INPTIMEOUT = 0x00000004;
		public const uint FFLAG_HWPOSILMT = 0x00000008;
		public const uint FFLAG_HWNEGALMT = 0x00000010;
		public const uint FFLAG_SWPOGILMT = 0x00000020;
		public const uint FFLAG_SWNEGALMT = 0x00000040;
		public const uint FFLAG_ORGTIMEOUT = 0x00000080;
		public const uint FFLAG_POSTRACKING = 0x00000100;
		public const uint FFLAG_POSCNTOVER = 0x00000200;
		public const uint FFLAG_EMGSTOP = 0x00000400;
		public const uint FFLAG_ERRSETDATA = 0x00000800;
		public const uint FFLAG_ERRQFULL = 0x00001000;
		public const uint FFLAG_ERRIOQFULL = 0x00002000;
		public const uint FFLAG_ENCFEEDBACK = 0x00004000;
		public const uint FFLAG_ERRHANDWHEEL = 0x00008000;
		public const uint FFLAG_START = 0x00010000;
		public const uint FFLAG_SLOWSTOP = 0x00020000;
		public const uint FFLAG_ZERORETURNING = 0x00040000;
		public const uint FFLAG_INPOSITION = 0x00080000;
		public const uint FFLAG_SERVOON = 0x00100000;
		public const uint FFLAG_ALARMRESET = 0x00200000;
		public const uint FFLAG_COUNTERCLR = 0x00400000;
		public const uint FFLAG_HOMESENSOR = 0x00800000;
		public const uint FFLAG_ZPULSE = 0x01000000;
		public const uint FFLAG_ZERORETOK = 0x02000000;
		public const uint FFLAG_MOTIONDIR = 0x04000000;
		public const uint FFLAG_MOTIONING = 0x08000000;
		public const uint FFLAG_MOTIONPAUSE = 0x10000000;
		public const uint FFLAG_MOTIONACCEL = 0x20000000;
		public const uint FFLAG_MOTIONDECEL = 0x40000000;
		public const uint FFLAG_MOTIONCONST = 0x80000000;

		//------------------------------------------------------------------
		//                        Initialized Functions.
		//------------------------------------------------------------------
		[DllImport("FAS16000Dll.dll")]
		public static extern bool FAS_Open();

		[DllImport("FAS16000Dll.dll")]
		public static extern bool FAS_Close();

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_InitMotion();

		[DllImport("FAS16000Dll.dll")]
		public static extern bool  FAS_IsExist(int iBdID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_IsAlive(int iBdID, ref bool bStatus);

		//------------------------------------------------------------------------------
		//             Parameter Functions
		//------------------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SaveLocalPara(int iBdID, int nAxisID, long lLParaNo);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SaveGlobalPara(int iBdID, long lGParaNo);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetLocalPara(int iBdID, int nAxisID, long lLParaNo, long lLParaValue);
        public static int FAS_SetLocalPara(int nAxisID, long lLParaNo, long lLParaValue)
        {
            int nRtn = 0;

            FAS_SetLocalPara(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, 10, lLParaValue); ;

            return nRtn;
        }

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetGlobalPara(int iBdID, long lGParaNo, long lGParaValue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetLocalPara(int iBdID, int nAxisID, long lLParaNo, ref long lGetLocalPara);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetGlobalPara(int iBdID, long lGParaNo, ref long lGetGlobalPara);

		//------------------------------------------------------------------------------
		//             Servo Driver Control Functions
		//------------------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ServoEnable(int iBdID, int nAxisID, bool bOnOff);
		public static int FAS_ServoEnable(int nAxisID, bool bOnOff)
        {
			int nRtn = FMM_OK;

			nRtn = FAS_ServoEnable(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, bOnOff);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ServoAlarmReset(int iBdID, int nAxisID);
		public static int FAS_ServoAlarmReset(int nAxisID)
        {
			int nRtn = FMM_OK;

			nRtn = FAS_ServoAlarmReset(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ServoErrorCounterClear(int iBdID, int nAxisID);

		//------------------------------------------------------------------------------
		//            IO Functions
		//------------------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetIo(int iBdID, bool bIsLow, int dwIoValue);
		public static int FAS_SetIo(int nAxisID, int bIsLow, int dwIoValue)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_SetIo(nAxisID / MAX_AXIS + 1, true, dwIoValue);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetIoBit(int iBdID, bool bIsLow, int wBitNo, bool bOnOrOff);
		public static int FAS_SetIoBit(int nAxisID, int wBitNo, bool bOnOrOff)
        {
			int nRtn = FMM_OK;

			nRtn = FAS_SetIoBit(nAxisID / MAX_AXIS + 1, true, wBitNo, bOnOrOff);

			return nRtn;
        }

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetIo(int iBdID, bool bIsLow, ref int dwGetIo);
		public static int FAS_GetIo(int nAxisID, int bIsLow, ref int dwGetIo)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_GetIo(nAxisID / MAX_AXIS + 1, true, ref dwGetIo);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetIoBit(int iBdID, bool bIsLow, int wBitNo, ref bool bGetIoBit);
		public static int FAS_GetIoBit(int nAxisID, int wBitNo, ref bool bGetIoBit)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_GetIoBit(nAxisID / MAX_AXIS + 1, true, wBitNo, ref bGetIoBit);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetIoOutputStatus(int iBdID, bool bIsLow, ref int dwIoOutputStatus);

		//------------------------------------------------------------------------------
		//            Read Position
		//------------------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetCommandPos(int iBdID, int nAxisID, double dSetCmdPos);
		public static int FAS_SetCommandPos(int nAxisID, double dSetCmdPos)
        {
			int nRtn = FMM_OK;

			nRtn = FAS_SetCommandPos(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, dSetCmdPos);

			return nRtn;
        }

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetActualPos(int iBdID, int nAxisID, double dSetActualPos);
		public static int FAS_SetActualPos(int nAxisID, double dSetActualPos)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_SetActualPos(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, dSetActualPos);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetCommandPos(int iBdID, int nAxisID, ref double dGetCmdPos);
		public static int FAS_GetCommandPos(int nAxisID, ref double dGetCmdPos)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_GetCommandPos(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, ref dGetCmdPos);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetActualPos(int iBdID, int nAxisID, ref double dGetActualPos);
		public static int FAS_GetActualPos(int nAxisID, ref double dGetActualPos)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_GetActualPos(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, ref dGetActualPos);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetPosError(int iBdID, int nAxisID, ref double dGetPosError);
		public static int FAS_GetPosError(int nAxisID, ref double dGetPosError)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_GetPosError(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, ref dGetPosError);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetTrackingError(int iBdID, int nAxisID, ref double dGetTrackingError);
		public static int FAS_GetTrackingError(int nAxisID, ref double dGetTrackingError)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_GetTrackingError(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, ref dGetTrackingError);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetActualVel(int iBdID, int nAxisID, ref double dGetActualVel);
		public static int FAS_GetActualVel(int nAxisID, ref double dGetActualVel)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_GetActualVel(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, ref dGetActualVel);

			return nRtn;
		}

		//------------------------------------------------------------------
		//                About Queue Functions.
		//------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetFreePosCmdQueue(int iBdID, ref int wFreeCmdQueue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetFreePosIoQueue(int iBdID, ref int wFreeIoQueue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetFreeArcQueue(int iBdID, ref int wFreeArcQueue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetFreeArcMotionQueue(int iBdID, ref int wFreeArcMotionQueue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetFreeLinearQueue(int iBdID, ref int wFreeLinearQueue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetFreeContLinearQueue(int iBdID, ref int wFreeContLinearQueue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetFreeCircleQueue(int iBdID, ref int wFreeCircleQueue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetFreeAxisQueue(int iBdID, int nAxisID, ref int wFreeAxisQueue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetFreeDropQueue(int iBdID, ref int wFreeDropQueue);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ResetArcQueue(int iBdID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ResetArcMotionQueue(int iBdID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ResetLinearQueue(int iBdID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ResetContLinearQueue(int iBdID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ResetCircleQueue(int iBdID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ResetAxisQueue(int iBdID, int nAxisID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ResetDropQueue(int iBdID);

		//------------------------------------------------------------------------------
		//			     About D/A Functions.
		//------------------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetDacOutput(int iBdID, int wChannel, int iMVolt);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetDaOffset(int iBdID, int wChannel, int iMVolt);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetDacOutput(int iBdID, int wChannel, int iMVolt, ref int wGetDaOutput);

		//------------------------------------------------------------------
		//                About AD Functions.
		//------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetAdResult(int iBdID, int wChannel, ref long lGetAdResult);

		//------------------------------------------------------------------
		//                About Laser Senser Functions.	 (2005/03/28)
		//------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetLaserSenser(int iBdID, int wChannel, ref long lGetLaser);

		//------------------------------------------------------------------
		//            Axis Status
		//------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetAxisStatus(int iBdID, int nAxisID, ref int dwAxisStatus);
		public static int FAS_GetAxisStatus(int nAxisID, ref int dwAxisStatus)
        {
			int nRtn = FMM_OK;

			nRtn = FAS_GetAxisStatus(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, ref dwAxisStatus);

			return nRtn;
        }

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_IsMotioning(int iBdID, int nAxisID, ref bool bMotioning);
		/// <summary>
		/// Check if axis is motioning, ignore library function return code
		/// </summary>
		/// <param name="nAxisID">Axis ID</param>
		/// <returns>isMotioning state</returns>
		public static bool FAS_IsMotioning(int nAxisID)
        {
			bool bMotioning = false;

			FAS_IsMotioning(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, ref bMotioning);

			return bMotioning;
		}
		/// <summary>
		/// Check if axis is motioning, passing result to parameter. Prefer using this function
		/// </summary>
		/// <param name="nAxisID">Axis ID</param>
		/// <param name="bMotioning"></param>
		/// <returns>library function return code</returns>
		public static int FAS_IsMotioning(int nAxisID, ref bool bMotioning)
		{
			int nRtn = FMM_OK;

			nRtn = FAS_IsMotioning(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, ref bMotioning);

			return nRtn;
		}

		//------------------------------------------------------------------
		//                Clear Functions.
		//------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ClearErrorCounter(int iBdID, int nAxisID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ClearErrorStatus(int iBdID, int nAxisID);

		//------------------------------------------------------------------
		//                Motion Functions.
		//------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveJog(int iBdID, int nAxisID, int iJogDir);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveStop(int iBdID, int nAxisID, int wEndCheck);
		public static int FAS_MoveStop(int nAxisID)
        {
			int nRtn = FMM_OK;

			nRtn = FAS_MoveStop(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, 0);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_EmergencyStop(int iBdID, int nAxisID, int wEndCheck);
		public static int FAS_EmergencyStop(int nAxisID)
        {
			int nRtn = FMM_OK;

			nRtn = FAS_EmergencyStop(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, 0);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveOriginSingleAxis(int iBdID, int nAxisID, int wEndCheck);
		public static int FAS_MoveOriginSingleAxis(int nAxisID)
        {
			int nRtn = 0;

			nRtn = FAS_MoveOriginSingleAxis(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, 0);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveSingleAxisAbsPos(int iBdID, int nAxisID, double dAbsPos, double dVelocity, int wEndCheck);
		public static int FAS_MoveSingleAxisAbsPos(int nAxisID, double dAbsPos, double dVelocity)
        {
			int nRtn = 0;

			nRtn = FAS_MoveSingleAxisAbsPos(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, dAbsPos, dVelocity, 0);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveSingleAxisIncPos(int iBdID, int nAxisID, double dIncPos, double dVelocity, int wEndCheck);
		public static int FAS_MoveSingleAxisIncPos(int nAxisID, double dIncPos, double dVelocity)
        {
			int nRtn = 0;

			nRtn = FAS_MoveSingleAxisIncPos(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, dIncPos, dVelocity, 0);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveMultiAxisAbsPos(int iBdID, int wNoOfAxis, ref int piAxis, ref double pdAbsPos, ref double pdVelocity, int wEndCheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveMultiAxisIncPos(int iBdID, int wNoOfAxis, ref int piAxis, ref double pdIncPos, ref double pdVelocity, int wEndCheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveLinearIncPos(int iBdID, int wNoOfAxis, ref int piAxis, ref double pdIncPos, double dFeedrate, int wEndCheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveLinearAbsPos(int iBdID, int wNoOfAxis, ref int piAxis, ref double pdAbsPos, double dFeedrate, int wEndCheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveContLinearAbsPos(int iBdID, int wNoOfAxis, ref int piAxis, ref double pdAbsPos, double dFeedrate, int wStartOrEnd);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveContLinearIncPos(int iBdID, int wNoOfAxis, ref int piAxis, ref double pdIncPos, double dFeedrate, int wStartOrEnd);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveCircleAbsPos(int iBdID, int wNoOfAxis, ref int piCirAxis, ref double pdCirEndPosAbs, ref double pdCirCenterAbs, double dFeedrate, int iDirection, int wEndCheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveCircleIncPos(int iBdID, int wNoOfAxis, ref int piCirAxis, ref double pdCirEndPosInc, ref double pdCirCenterInc, double dFeedrate, int iDirection, int wEndCheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveContCircleAbsPos(int iBdID, int wNoOfAxis, ref int piCirAxis, ref double pdCirEndPosAbs, ref double pdCirCenterAbs, double dFeedrate, int iDirection, int wStartOrEnd);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveContCircleIncPos(int iBdID, int wNoOfAxis, ref int piCirAxis, ref double pdCirEndPosInc, ref double pdCirCenterInc, double dFeedrate, int iDirection, int wStartOrEnd);


		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_VelocityOverride(int iBdID, int nAxisID, double dVelocity);


		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_PositionIncOverride(int iBdID, int nAxisID, long lOverridePos);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_PositionAbsOverride(int iBdID, int nAxisID, long lOverridePos);


		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveArchMultiAbsPos(int iBdID, int wNoOfAxis, ref int piAxis, ref double pdAbsPos, double dFeedrate, int wEndCheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveArchMultiIncPos(int iBdID, int wNoOfAxis, ref int piAxis, ref double pdIncPos, double dFeedrate, int wEndCheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveOrgZpulse(int iBdID, int nAxisID);


		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveToLimit(int iBdID, int nAxisID, double dSpeed, int iLimitDir, int wEndcheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_EscapeFromLimit(int iBdID, int nAxisID, double dSpeed, int iLimitDir, int wEndcheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveToZPulse(int iBdID, int nAxisID, double dSpeed, int iZPulseDir, int wEndcheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveOnePulse(int iBdID, int nAxisID, int iOnePulseDir);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveToOrgPoint(int iBdID, int nAxisID, double dSpeed, int wEndcheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveVelocity(int iBdID, int nAxisID, double dSpeed, int iVelDir);
		public static int FAS_MoveVelocity(int nAxisID, double dSpeed, int iVelDir)
        {
			int nRtn = FMM_OK;

			nRtn = FAS_MoveVelocity(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS, dSpeed, iVelDir);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MovePause(int iBdID, int nAxisID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveRestart(int iBdID, int nAxisID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_WaitUntilIoSet(int iBdID, int wIoBitNo, bool bOnOff);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_Dwell(int iBdID, long lMsec);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SoftResetMotion(int iBdID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_WaitSingleMoveDone(int iBdID, int nAxisID);
		public static int FAS_MotionEndCheck(int nAxisID)
        {
			int nRtn = FMM_OK;

			nRtn = FAS_WaitSingleMoveDone(nAxisID / MAX_AXIS + 1, nAxisID % MAX_AXIS);

			return nRtn;
		}

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_WaitMultiMoveDone(int iBdID, int iAxes);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveAbsSynchronous(int iBdID, ref int piAxis, long lPosition, long lVelocity, int wEndCheck);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveIncSynchronous(int iBdID, ref int piAxis, long lPosition, long lVelocity, int wEndCheck);

		//------------------------------------------------------------------
		//                Special Motion Functions.
		//------------------------------------------------------------------

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveArcAbsPos(int iBdID, int NoOfAxis, ref int Axis, ref double AbsPos, double D_Feedrate, double D_CirFeedrate, double CornerDistance, int D_StartEndPoint);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveArcIncPos(int iBdID, int NoOfAxis, ref int Axis, ref double IncPos, double D_Feedrate, double D_CirFeedrate, double CornerDistance, int D_StartEndPoint);

		// 2004/10/21 WORD wOffValue를 DWORD dwOffValue로 변경함.
		//            I/O의 Low/High를 알기 위해서 flag를 인자로 하나 더 추가.

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveArcAbsPosWithSealOff(int iBdID, int NoOfAxis, ref int Axis, ref double AbsPos,
						double D_Feedrate, double D_CirFeedrate, double CornerDistance, int D_StartEndPoint,
						double dUpDistancePos, int wOffNoOfAxis, ref int piOffAxis, ref double pdOffIncPos, ref double pdOffVelocity,
						double dMusashiOffDistancePos, int dwOffValue, bool bIsLow);


		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveArcSingleAbsPosWithSealOff(int iBdID, int nAxisID, double AbsPos, double Velocity,
						double dUpDistancePos, int wOffNoOfAxis, ref int piOffAxis, ref double pdOffIncPos, ref double pdOffVelocity,
						double dMusashiOffDistancePos, int dwOffValue, bool bIsLow);


		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveArcSingleIncPosWithSealOff(int iBdID, int nAxisID, double IncPos, double Velocity,
						double dUpDistancePos, int wOffNoOfAxis, ref int piOffAxis, ref double pdOffIncPos, ref double pdOffVelocity,
						double dMusashiOffDistancePos, int dwOffValue, bool bIsLow);


		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveLineDropAbs(int iBdID, int wStageNoOfAxis, ref int wStageAxis, ref double pdStageAbsPos, double dStageFeedrate, double dStageStartSpeed, int wStartOrEnd,
						int wNoOfAxis, ref int piAxis, ref double pdIncPos, ref double pdVelocity, double dDropStartPos);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveLineDropInc(int iBdID, int wStageNoOfAxis, ref int wStageAxis, ref double pdStageIncPos, double dStageFeedrate, double dStageStartSpeed, int wStartOrEnd,
						int wNoOfAxis, ref int piAxis, ref double pdIncPos, ref double pdVelocity, double dDropStartPos);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveDropSyncAbs(int iBdID, int wStageNoOfAxis, ref int wStageAxis, ref double pdStageAbsPos, double dStageFeedrate, double dStageStartSpeed, int wStartOrEnd,
						int wNoOfAxis, ref int piAxis, ref double pdIncPos, ref double pdVelocity, double dDropStartPos);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveDropSyncInc(int iBdID, int wStageNoOfAxis, ref int wStageAxis, ref double pdStageIncPos, double dStageFeedrate, double dStageStartSpeed, int wStartOrEnd,
						int wNoOfAxis, ref int piAxis, ref double pdIncPos, ref double pdVelocity, double dDropStartPos);

		////////////////////////////////////////////////////////////////////// 2003.12.27
		// Seal

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GapControlEnable(int iBdID, int nAxisID, bool bOnOff);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GapControlEnableMulti(int iBdID, int wMultAxis);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GapControlDisableMulti(int iBdID, int wMultiAxis);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetGapError(int iBdID, int nAxisID, ref bool bGapError);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ClearGapError(int iBdID, int nAxisID);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetGapErrorCount(int iBdID, int nAxisID, ref int iGapErrorCount);

		//////////////////////////////////////////////////////////////////////
		// LC
		//////////////////////////////////////////////////////////////////////
		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveDropSyncAbsP6(int iBdID, int wStageNoOfAxis, ref int wStageAxis, ref double pdStageAbsPos, double dStageFeedrate, double dStageStartSpeed, int wStartOrEnd,
						int wSyncAxis, int wNoOfAxis, ref int piAxis, ref double pdIncPos, ref double pdVelocity, double dDropStartPos);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_MoveLineDropAbsP6(int iBdID, int wStageNoOfAxis, ref int wStageAxis, ref double pdStageAbsPos, double dStageFeedrate, double dStageStartSpeed, int wStartOrEnd,
						int wSyncAxis, int wNoOfAxis, ref int piAxis, ref double pdIncPos, ref double pdVelocity, double dDropStartPos);

		// Add 2004/1003

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetSyncSlaveAxes(int iBdID, int iAxes);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_SetSyncMasterAxis(int iBdID, int iAxes);

		// Add 2004/1015

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_CornerCompEnable(int iBdID, int nAxisID, bool bOnOff);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_CornerCompEnableMulti(int iBdID, int wMultAxis);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_CornerCompDisableMulti(int iBdID, int wMultiAxis);

		// by song 2004/10/27

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_ResetSyncMasterAxes(int iBdID);

		// Added by Jaehak 2011/10/07

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_GetEmergencyStopStatus(int iBdID, ref bool bStatus);

		// 2012 01 30

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_CheckInposition(int iBdID, int nAxisID, ref bool bInposition);

		[DllImport("FAS16000Dll.dll")]
		public static extern int FAS_CheckMotioning(int iBdID, int nAxisID, ref bool bMotioning);
	}
}
