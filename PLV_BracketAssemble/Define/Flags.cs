using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Define
{
    public static class Flags
    {
        public static bool HeadHomeDone
        {
            get
            {
                return CDef.RootProcess.HeadProcess.Flag_HeadHomeDone;
            }
            set
            {
                CDef.RootProcess.HeadProcess.Flag_HeadHomeDone = value;
            }
        }
        public static bool Picker1_UnderVisionPosition
        {
            get
            {
                return CDef.RootProcess.HeadProcess.Is_Picker1_UnderVision_Position;
            }
        }
        public static bool Picker2_UnderVisionPosition
        {
            get
            {
                return CDef.RootProcess.HeadProcess.Is_Picker2_UnderVision_Position;
            }
        }

        public static bool Picker1_Pick_Done
        {
            get
            {
                return CDef.RootProcess.HeadProcess.Flag_Picker1_PickDone;
            }
        }
        public static bool Picker2_Pick_Done
        {
            get
            {
                return CDef.RootProcess.HeadProcess.Flag_Picker2_PickDone;
            }
        }
        public static bool Picker1_Place_Done 
        {
            get
            {
                return CDef.RootProcess.HeadProcess.Flag_Picker1_PlaceDone;
            } 
        }
        public static bool Picker2_Place_Done
        {
            get
            {
                return CDef.RootProcess.HeadProcess.Flag_Picker2_PlaceDone;
            }
        }
        public static bool Request_UnderVision_Start
        {
            get
            {
                return CDef.RootProcess.HeadProcess.Flag_Request_UnderVision_Start;
            }
            set
            {
                CDef.RootProcess.HeadProcess.Flag_Request_UnderVision_Start = value;
            }
        }

        public static bool UnderVision_Inspect_Picker1_Done
        {
            get
            {
                return CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_Inspect_Picker1_Done;
            }
        }
        public static bool UnderVision_Inspect_Picker2_Done
        {
            get
            {
                return CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_Inspect_Picker2_Done;
            }
        }
        public static bool UnderVision_Inspect_Done
        {
            get
            {
                return UnderVision_Inspect_Picker1_Done && UnderVision_Inspect_Picker2_Done;
            }
            set
            {
                CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_Inspect_Picker1_Done = value;
                CDef.RootProcess.UnderVisionProcess.Flag_UnderVision_Inspect_Picker2_Done = value;
            }
        }
    }
}
