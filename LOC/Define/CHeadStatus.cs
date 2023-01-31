using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOC.Define
{
    public class CHeadStatus
    {
        public bool HeadOccupied
        {
            get
            {
                return CDef.IO.Input.Picker1_VacDetect | CDef.IO.Input.Picker2_VacDetect;
            }
        }
    }
}
