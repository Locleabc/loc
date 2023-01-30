using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum EUnderVisionUnderVisionStep
    {
        Start,
        If_UseVision,
        UnderVisionStart_Flag_Wait,
        SetCurrentHead,
        Head_Move_UnderVisionPosition_Wait,
        UnderVision_ImageGrab,
        UnderVision_ImageGrab_Wait,
        UnderVision_Inspect,
        UnderVision_Inspect_Wait,
        UnderVision_RetryCheck,
        UnderVision_ResultUpdate,
        UpdateCurrentHead,
        End
    }
}
