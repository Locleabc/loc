using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum EHeadPlaceStep
    {
        Start,
        Head_CM_ExistCheck,
        Head_PickerCylinder_PlaceDone_Clear,
        SetCurrentHead,
        Head_Move_PlacePosition,
        Head_Move_PlacePosition_Wait,
        Tray_PlacePosition_Wait,
        Head_PickerCylinder_Down,
        Head_PickerCylinder_Down_Wait,
        Head_PickerCylinder_PurgeOn,
        Head_PickerCylinder_PurgeOn_Delay,
        Head_PickerCylinder_Up,
        Head_PickerCylinder_Up_Wait,
        Head_PickerCylinder_PlaceDone_Set,
        UpdateCurrentHead,
        UpdateWorkData,
        End
    }
}
