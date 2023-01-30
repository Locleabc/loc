using TopVision.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision Parameter for <see cref="Histogram"/> process
    /// </summary>
    public class HistogramParameter : VisionParameterBase
    {
    }

    /// <summary>
    /// Vision Result for <see cref="Histogram"/> process
    /// </summary>
    public class HistogramResult : VisionResultBase
    {
    }

    public class Histogram : VisionProcessBase
    {
        #region Privates
        private HistogramParameter ThisPamameter
        {
            get { return (HistogramParameter)Parameter; }
        }

        private HistogramResult ThisResult
        {
            get { return (HistogramResult)Result; }
        }
        #endregion

        #region Constructor
        public Histogram()
            : this(new HistogramParameter())
        {
        }

        public Histogram(HistogramParameter parameter)
        {
            Parameter = parameter;
            Result = new HistogramResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new HistogramResult();

            Cv2.EqualizeHist(InputMat, OutputMat);

            return EVisionRtnCode.OK;
        }
    }
}
