using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision
{
    internal interface ICircleDetection
    {
        double Radius { get; set; }
        int RadiusThreshold { get; set; }
    }
}
