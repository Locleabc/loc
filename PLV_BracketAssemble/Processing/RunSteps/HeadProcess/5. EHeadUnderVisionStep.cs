using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum EHeadUnderVisionStep
    {
        Start,
        If_UseVision,
        UnderVisionStart_Flag_Set,
        Head_Move_UnderVisionPosition1,
        Head_Move_UnderVisionPosition1_Wait,
        UnderVision_Head1_InspectDone_Wait,
        Head_Move_UnderVisionPosition2,
        Head_Move_UnderVisionPosition2_Wait,
        UnderVision_Head2_InspectDone_Wait,
        UnderVisionStart_Flag_Clear,
        End
    }
}
