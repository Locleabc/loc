using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum ETrayPickStep
    {
        Start,

        PreSet_CurrentPicker,

        CheckIf_Picker_PickDone,

        Tray_CheckStatus,
        Tray_Move_PickPosition,
        Tray_Move_PickPosition_Wait,
        Head_PickerCylinder_PickDone_Wait,

        PostSet_CurrentPicker,

        End,
    }
}
