using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Grabbers
{
    public enum ECameraWorkState
    {
        ERROR = -1,
        NotReady = 0,
        IDLE,
        GRAB,
        LIVE,
    }
}
