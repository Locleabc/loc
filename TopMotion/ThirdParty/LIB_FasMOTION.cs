using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FASTECH
{
    partial class FasMOTIONLib
    {
        [DllImport("FAS16000Dll.dll")]
        public static extern int FAS_Open();
        
        [DllImport("FAS16000Dll.dll")]
        public static extern int FAS_Close();

        [DllImport("FAS16000Dll.dll")]
        protected static extern int FAS_ServoEnable(int iBdID, int iAxisID, int nOnOff);
        public static int FAS_ServoEnable(int iAxisID, int nOnOff)
        {
            int nRtn = 0;

            nRtn = FAS_ServoEnable(iAxisID/16 + 1, iAxisID%16, nOnOff);

            return nRtn;
        }

        [DllImport("FAS16000Dll.dll")]    //171218
        protected static extern int FAS_ServoAlarmReset(int iBdID, int iAxisID);
        public static int FAS_ServoAlarmReset(int iAxisID)
        {
            int nRtn = 0;

            nRtn = FAS_ServoAlarmReset(iAxisID / 16 + 1, iAxisID % 16);

            return nRtn;
        }

        [DllImport("FAS16000Dll.dll")]
        protected static extern int FAS_MoveVelocity(int iBdID, int iAxisID, double dVelocity, int iVelDir);
        public static int FAS_MoveVelocity(int iAxisID, int dVelocity, int iVelDir)
        {
            int nRtn = 0;

            nRtn = FAS_MoveVelocity(iAxisID / 16 + 1, iAxisID % 16, Convert.ToDouble(dVelocity), iVelDir);

            return nRtn;
        }


        [DllImport("FAS16000Dll.dll")]
        protected static extern int FAS_MoveStop(int iBdID, int iAxisID, int iEndCheck);
        public static int FAS_MoveStop(int iAxisID)
        {
            int nRtn = 0;

            nRtn = FAS_MoveStop(iAxisID / 16 + 1, iAxisID % 16, 0);

            return nRtn;
        }

        [DllImport("FAS16000Dll.dll")]
        protected static extern int FAS_MoveSingleAxisAbsPos(int iBdID, int iAxisID, double dPos, double dVel, int iEndCheck);
        public static int FAS_MoveAbsPos(int iAxisID, int nPosition, int nVelocity)
        {
            int nRtn = 0;

            nRtn = FAS_MoveSingleAxisAbsPos(iAxisID / 16 + 1, iAxisID % 16, Convert.ToDouble(nPosition), Convert.ToDouble(nVelocity), 0);

            return nRtn;
        }

        [DllImport("FAS16000Dll.dll")]
        protected static extern int FAS_MoveSingleAxisIncPos(int iBdID, int iAxisID, double dPos, double dVel, int iEndCheck);
        public static int FAS_MoveIncPos(int iAxisID, int nPosition, int nVelocity)
        {
            int nRtn = 0;

            nRtn = FAS_MoveSingleAxisIncPos(iAxisID / 16 + 1, iAxisID % 16, Convert.ToDouble(nPosition), Convert.ToDouble(nVelocity), 0);

            return nRtn;
        }
        
        [DllImport("FAS16000Dll.dll")]
        protected static extern int FAS_IsMotioning(int iBdID, int iAxisID, ref uint pMotioning);
        public static int FAS_IsMotioning(int iAxisID)
        {
            int nRtn = 0;

            uint nMotioning = 0;

            FAS_IsMotioning(iAxisID / 16 + 1, iAxisID % 16, ref nMotioning);

            if (nMotioning == 1)
            {
                nRtn = 1;
            }
            else
            {
                nRtn = 0;
            }                       

            return nRtn;
        }         

        [DllImport("FAS16000Dll.dll")]
        protected static extern int FAS_WaitSingleMoveDone(int iBdID, int iAxisID);
        public static int FAS_MotionEndCheck(int iAxisID)
        {
            int nRtn = 0;

            nRtn = FAS_WaitSingleMoveDone(iAxisID / 16 + 1, iAxisID % 16);

            return nRtn;
        }

        [DllImport("FAS16000Dll.dll")]
        protected static extern int FAS_MoveOriginSingleAxis(int iBdID, int iAxisID, int iEndCheck);
        public static int FAS_MoveOrigin(int iAxisID)
        {
            int nRtn = 0;

            nRtn = FAS_MoveOriginSingleAxis(iAxisID / 16 + 1, iAxisID % 16, 0);

            return nRtn;
        }

        [DllImport("FAS16000Dll.dll")]
        protected static extern int FAS_SetIoBit(int iBdID, int iIsLow, int iBitNo, int nOnOff);
        public static int FAS_SetIoBit(int iBitNo, int nOnOff)
        {
            int nRtn = 0;

            nRtn = FAS_SetIoBit(1, 1, iBitNo, nOnOff);

            return nRtn;
        }
    }
}
