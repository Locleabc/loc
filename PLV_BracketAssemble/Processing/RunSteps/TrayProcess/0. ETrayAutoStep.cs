using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum ETrayAutoStep
    {
        Start,

        CheckIf_TrayIsReadyPosition,
        CheckIf_TrayExist,

        CheckIf_CMExistOnBothPicker,
        Tray_CheckStatus,

        End,
    }
}
