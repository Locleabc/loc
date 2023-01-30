using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum ETrayPlaceStep
    {
        Start,
        Tray_Move_PlacePosition1,
        Tray_Move_PlacePosition1_Wait,
        Head_PickerCylinder1_PlaceDone_Wait,
        Tray_Move_PlacePosition2,
        Tray_Move_PlacePosition2_Wait,
        Head_PickerCylinder2_PlaceDone_Wait,
        End
    }
}
