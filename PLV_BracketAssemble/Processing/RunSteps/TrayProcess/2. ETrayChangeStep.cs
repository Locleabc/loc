using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum ETrayChangeStep
    {
        Start,
        Tray_Move_ReadyPosition,
        Tray_Move_ReadyPosition_Wait,
        HeadReadyPosition_Wait,
        End,
    }
}
