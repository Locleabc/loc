using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;
using TopVision.Models;

namespace VCM_FullAssy.Define
{
    public class Datas
    {
        public static CWorkData WorkData { get; set; } = new CWorkData();

        public static IVisionResult Data_LoadingInspect_Result
        {
            get { return CDef.RootProcess.UpperVisionProcess.Data_LoadingInspect_Result; }
        }

        public static IVisionResult Data_UnloadingInspect_Result
        {
            get { return CDef.RootProcess.UpperVisionProcess.Data_UnloadingInspect_Result; }
        }

        public static IVisionResult Data_UnderInspect_Result
        {
            get { return CDef.RootProcess.UnderVisionProcess.Data_UnderInspect_Result; }
        }
    }
}
