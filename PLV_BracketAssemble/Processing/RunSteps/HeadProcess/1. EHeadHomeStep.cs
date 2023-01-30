using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum EHeadHomeStep
    {
        Start,
        AllPickerCylinderUp,
        ClampCylinder_Backward,
        AllPickerCylinderUp_Wait,
        ClampCylinder_Backward_Wait,
        Head_CM_ExistCheck, //(Exist : WARNING -> HOME FAIL)
        Head_HomeDone_Set,
        Y_XXAxisHomeSearch,
        Y_XXAxisHomeSearch_Wait,
        End
    }
}
