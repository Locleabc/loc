using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum EHeadPreAlignStep
    {
        Start,
        If_UsePreAlgin,
        Head_Move_PreAlginPosition,
        Head_Move_PreAlginPosition_Wait,
        ClampCylinder_Backward_First,
        ClampCylinder_Backward_Wait_First,
        Head_PickerCylinder_Down,
        Head_PickerCylinder_Down_Wait,
        ClampCylinder_Forward,
        ClampCylinder_Forward_Wait,
        ClampCylinder_Backward,
        ClampCylinder_Backward_Wait,
        Head_PickerCylinder_Up,
        Head_PickerCylinder_Up_Wait,
        End
    }
}
