using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Models
{
    public interface IVisionParameter
    {
        double Threshold { get; set; }
        ObservableCollection<CRectangle> ROIs { get; set; }
        COutputMatOption OutputMatOption { get; set; }
        bool IsUseInputImageAsInput { get; set; }
        double ThetaAdjust { get; set; }

        bool UseFixtureAlign { get; set; }
        /// <summary>
        /// Pre-align offset, using this to align ROI(s); unit [mm]
        /// </summary>
        XYTOffset TeachingFixtureOffset { get; set; }
        /// <summary>
        /// Unit [mm]
        /// </summary>
        XYTOffset RuntimeFixtureOffset { get; set; }

        bool UseOffsetLimit { get; set; }
        XYTOffset OffsetLimit { get; set; }
    }
}
