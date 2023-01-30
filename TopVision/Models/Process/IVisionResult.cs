using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Models
{
    public interface IVisionResult
    {
        EVisionJudge Judge { get; }
        string NGMessage { get; set; }
        double Score { get; }
        /// <summary>
        /// Process cost time, in ms
        /// </summary>
        double Cost { get; set; }
        XYTOffset DetectedOffset { get; set; }
    }
}
