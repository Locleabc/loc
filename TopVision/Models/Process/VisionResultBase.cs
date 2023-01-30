using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Models
{
    public class VisionResultBase : IVisionResult
    {
        // Set defaul vision result OK in case vision skip option is actived
        public EVisionJudge Judge { get; internal set; } = EVisionJudge.OK;
        public string NGMessage { get; set; } = "";
        public double Score { get; internal set; }
        public double Cost { get; set; }
        public XYTOffset DetectedOffset { get; set; } = new XYTOffset();
        public override string ToString()
        {
            return $"[{Judge}] Cost: {Cost:0.###}ms, Score: {Score * 100:0.##}%\nOffset: {DetectedOffset}";
        }
    }
}
