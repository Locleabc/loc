using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VCM_FullAssy.Processing;

namespace VCM_FullAssy.Define
{
    public class Flags
    {
        #region Head Flags
        public static bool Head_PickDone
        {
            get { return CDef.RootProcess.HeadProcess.Flag_PickDone; }
        }

        public static bool Head_PlaceDone
        {
            get { return CDef.RootProcess.HeadProcess.Flag_PlaceDone; }
        }

        public static bool ZAxisReady
        {
            get { return CDef.RootProcess.HeadProcess.Flag_ZAxisReady; }
        }

        public static bool Head_LoadVision_ProcessDone
        {
            get { return ZAxisReady & Upper_LoadVision_InspectDone; }
        }

        public static bool Head_UnloadVision_ProcessDone
        {
            get { return ZAxisReady & Upper_UnloadVision_InspectDone; }
        }

        public static bool Head_UnderVisionPosition_MoveDone
        {
            get { return CDef.RootProcess.HeadProcess.Flag_Head_UnderVisionPosition_MoveDone; }
            set
            {
                CDef.RootProcess.HeadProcess.Flag_Head_UnderVisionPosition_MoveDone = value;
            }
        }

        public static bool Head_LoadVisionPosition_MoveDone
        {
            get { return CDef.RootProcess.HeadProcess.Flag_Head_LoadVisionPosition_MoveDone; }
            set
            {
                CDef.RootProcess.HeadProcess.Flag_Head_LoadVisionPosition_MoveDone = value;
            }
        }

        public static bool Head_UnloadVisionPosition_MoveDone
        {
            get { return CDef.RootProcess.HeadProcess.Flag_Head_UnloadVisionPosition_MoveDone; }
            set
            {
                CDef.RootProcess.HeadProcess.Flag_Head_UnloadVisionPosition_MoveDone = value;
            }
        }
        #endregion

        #region Tray Flags
        public static bool Tray_LoadVisionPosition_MoveDone
        {
            get
            {
                return CDef.RootProcess.LeftTrayProcess.InWorking ?
                    CDef.RootProcess.LeftTrayProcess.Flag_Tray_LoadVisionPosition_MoveDone :
                    CDef.RootProcess.RightTrayProcess.Flag_Tray_LoadVisionPosition_MoveDone;
            }
        }

        public static bool Tray_PickPosition_MoveDone
        {
            get
            {
                return CDef.RootProcess.LeftTrayProcess.InWorking ?
                    CDef.RootProcess.LeftTrayProcess.Flag_Tray_PickPosition_MoveDone :
                    CDef.RootProcess.RightTrayProcess.Flag_Tray_PickPosition_MoveDone;
            }
        }

        public static bool Tray_UnloadVisionPosition_MoveDone
        {
            get
            {
                return CDef.RootProcess.LeftTrayProcess.InWorking ?
                    CDef.RootProcess.LeftTrayProcess.Flag_Tray_UnloadVisionPosition_MoveDone :
                    CDef.RootProcess.RightTrayProcess.Flag_Tray_UnloadVisionPosition_MoveDone;
            }
        }

        public static bool Tray_PlacePosition_MoveDone
        {
            get
            {
                return CDef.RootProcess.LeftTrayProcess.InWorking ?
                    CDef.RootProcess.LeftTrayProcess.Flag_Tray_PlacePosition_MoveDone :
                    CDef.RootProcess.RightTrayProcess.Flag_Tray_PlacePosition_MoveDone;
            }
        }
        #endregion

        #region UpperVision Flags
        public static bool Upper_LoadVision_InspectDone
        {
            get { return CDef.RootProcess.UpperVisionProcess.Flag_Upper_LoadVision_InspectDone; }
        }

        public static bool Upper_UnloadVision_InspectDone
        {
            get { return CDef.RootProcess.UpperVisionProcess.Flag_Upper_UnloadVision_InspectDone; }
        }

        #endregion

        #region Transfer Flags
        public static bool Transfer_LoadVisionPosition_MoveDone
        {
            get { return CDef.RootProcess.TransferProcess.Flag_Transfer_LoadVisionPosition_MoveDone; }
        }

        public static bool Transfer_PickPosition_MoveDone
        {
            get { return CDef.RootProcess.TransferProcess.Flag_Transfer_PickPosition_MoveDone; }
        }

        public static bool Transfer_UnderVisionPosition_MoveDone
        {
            get { return CDef.RootProcess.TransferProcess.Flag_Transfer_UnderVisionPosition_MoveDone; }
        }

        public static bool Transfer_UnloadVisionPosition_MoveDone
        {
            get { return CDef.RootProcess.TransferProcess.Flag_Transfer_UnloadVisionPosition_MoveDone; }
        }

        public static bool Transfer_PlacePosition_MoveDone
        {
            get { return CDef.RootProcess.TransferProcess.Flag_Transfer_PlacePosition_MoveDone; }
        }
        #endregion

        #region UnderVision Flags
        // HEAD -> UNDER VISION WORK REQUEST FLAG
        public static bool UnderVision_InspectWork_Request
        {
            get { return CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_InspectWork_Request; }
            set
            {
                CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_InspectWork_Request = value;
            }
        }

        public static bool UnderVision_InspectDone
        {
            get { return CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_InspectDone; }
            set
            {
                CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_InspectDone = value;
            }
        }
        #endregion
    }
}
