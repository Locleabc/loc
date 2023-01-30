using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum EHeadPickStep
    {
        Start,
        Head_CM_ExistCheck,
        SetCurrentHead,
        Head_PreSet_PickDone_Status,
        Head_Move_PickPosition,
        Head_Move_PickPosition_Wait,
        Tray_PickPosition_Wait,
        Head_PickerCylinder_Down,
        Head_PickerCylinder_Down_Wait,
        Head_PickerCylinder_VacOn,
        Head_PickerCylinder_VacOn_Delay,
        Head_PickerCylinder_Up,
        Head_PickerCylinder_Up_Wait,
        Head_PostSet_PickDone_Status,
        UpdateCurrentHead,
        End
    }
}
