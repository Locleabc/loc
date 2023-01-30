using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Models
{
    public interface IThetaOffset : IOffset
    {
        double Theta { get; set; }
    }
}
