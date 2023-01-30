using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.Processing
{
    public enum ETrayHomeStep
    {
        Start,
        Head_HomeDone_Wait,
        XAxis_HomeSearch,
        XAxis_HomeSearch_Wait,
        End
    }
}
