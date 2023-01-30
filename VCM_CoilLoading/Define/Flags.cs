using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCM_CoilLoading.Define
{
    public static class Flags
    {
        public static bool Flag_Picker_LoadVisionInspectAvoid
        {
            get
            {
                return CDef.RootProcess.PickerProcess.Flag_Picker_LoadVision_AvoidDone;
            }
        }

        public static bool Flag_Picker_UnloadVisionInspectAvoid
        {
            get
            {
                return CDef.RootProcess.PickerProcess.Flag_Picker_UnloadVision_AvoidDone;
            }
        }

        /// <summary>
        /// Use this flag to check if tray is in position to pick or inspect loading vision
        /// </summary>
        public static bool Flag_Picker_TrayPickPosition_Ready
        {
            get
            {
                return CDef.RootProcess.PickerProcess.Flag_Picker_TrayPickPosition_Ready;
            }
        }

        /// <summary>
        /// Use this flag to check if tray is in position to place or inspect unloading vision
        /// </summary>
        public static bool Flag_Picker_TrayPlacePosition_Ready
        {
            get
            {
                return CDef.RootProcess.PickerProcess.Flag_Picker_TrayPlacePosition_Ready;
            }
        }

        public static bool Flag_UpperVision_LoadInspect_Done
        {
            get { return CDef.RootProcess.UpperVisionProcess.Flag_LoadingInspect_Done; }
        }

        public static bool Flag_UpperVision_UnloadInspect_Done
        {
            get { return CDef.RootProcess.UpperVisionProcess.Flag_UnloadingInspect_Done; }
        }

        public static bool Flag_LoadVisionInspectAvoid_Request
        {
            get { return CDef.RootProcess.UpperVisionProcess.Flag_LoadVisionInspectAvoid_Request; }
        }

        public static bool Flag_UnloadVisionInspectAvoid_Request
        {
            get { return CDef.RootProcess.UpperVisionProcess.Flag_UnloadVisionInspectAvoid_Request; }
        }
    }
}
