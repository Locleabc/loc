﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;
using TopVision.Models;

namespace VCM_PickAndPlace.Define
{
    public class Datas
    {
        //public static void ResetInspectData()
        //{
        //    CDef.RootProcess.UpperVisionProcess.Data_LoadingInspect_Result = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
        //    CDef.RootProcess.UpperVisionProcess.Data_UnloadingInspect_Result = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
        //    CDef.RootProcess.PickerProcess.Data_UnderInspect_Result = new List<IVisionResult> { new VisionResultBase(), new VisionResultBase() };
        //}
        public static bool[] PickResult { get; set; } = new bool[2]; // Head1, Head2
        public static bool[] PlaceResult { get; set; } = new bool[2]; // Head1, Head2

        public static List<IVisionResult> Data_LoadingInspect_Result
        {
            get { return CDef.RootProcess.UpperVisionProcess.Data_LoadingInspect_Result; }
        }

        public static List<IVisionResult> Data_UnderInspect_Result
        {
            get { return CDef.RootProcess.UnderVisionProcess.Data_UnderVisionInspect_Result; }
        }

        public static List<IVisionResult> Data_UnloadingInspect_Result
        {
            get { return CDef.RootProcess.UpperVisionProcess.Data_UnloadingInspect_Result; }
        }

        public static CWorkData WorkData { get; set; } = new CWorkData();
    }
}
