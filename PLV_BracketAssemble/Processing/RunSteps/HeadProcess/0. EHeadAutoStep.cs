using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum EHeadAutoStep
    {
        Start,
        CheckIf_HeadIsReadyPosition,
        CheckIf_CMExistOnPicker,

        Tray_CheckStatus,
        End
    }
}
