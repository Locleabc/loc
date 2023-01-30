using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Models
{
    public interface IXYOffset : IOffset
    {
        /// <summary>
        /// Millimeter (mm)
        /// </summary>
        double X { get; set; }
        /// <summary>
        /// Millimeter (mm)
        /// </summary>
        double Y { get; set; }
    }
}
