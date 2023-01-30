using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision
{
    internal interface ICannyDetection
    {
        double CannyMaxVal { get; set; }
        double CannyMinVal { get; set; }
    }
}
