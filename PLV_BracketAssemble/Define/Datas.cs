using PLV_BracketAssemble.Define.WorkData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Models;

namespace PLV_BracketAssemble.Define
{
    public class Datas
    {
        public static List<IVisionResult> Data_UnderInspect_Result
        {
            get { return CDef.RootProcess.UnderVisionProcess.Data_UnderVisionInspect_Result; }
        }
        public static CWorkData WorkData { get; set; } = new CWorkData();
    }
}
