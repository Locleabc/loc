using TopVision.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision Result for <see cref="CannyEdge"/> process
    /// </summary>
    public class CannyEdgeParameter : VisionParameterBase, ICannyDetection
    {
        #region Properties
        public double CannyMaxVal
        {
            get { return _CannyMaxVal; }
            set
            {
                if (_CannyMaxVal == value) return;
                _CannyMaxVal = value;
                OnPropertyChanged();
            }
        }

        public double CannyMinVal
        {
            get { return _CannyMinVal; }
            set
            {
                if (_CannyMinVal == value) return;
                _CannyMinVal = value;
                OnPropertyChanged();
            }
        }

        public int ApertureSize
        {
            get { return _ApertureSize; }
            set
            {
                if (_ApertureSize == value) return;

                value = (value / 2) * 2 + 1;

                if (value < 3) value = 3;
                if (value > 7) value = 7;

                _ApertureSize = value;
                OnPropertyChanged();
            }
        }

        public bool L2Gradient
        {
            get { return _L2Gradient; }
            set
            {
                if (_L2Gradient == value) return;
                _L2Gradient = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _CannyMaxVal;
        private double _CannyMinVal;
        private int _ApertureSize = 3;
        private bool _L2Gradient = false;
        #endregion
    }

    /// <summary>
    /// Vision Result for <see cref="CannyEdge"/> process
    /// </summary>
    public class CannyEdgeResult : VisionResultBase
    {
    }

    public class CannyEdge : VisionProcessBase
    {
        #region Privates
        private CannyEdgeParameter ThisParameter
        {
            get { return (CannyEdgeParameter)Parameter; }
        }

        private CannyEdgeResult ThisResult
        {
            get { return (CannyEdgeResult)Result; }
        }
        #endregion

        #region Constructors
        public CannyEdge()
            : this(new CannyEdgeParameter())
        {
        }

        public CannyEdge(CannyEdgeParameter parameter)
        {
            Result = new CannyEdgeResult();
            Parameter = parameter;
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new CannyEdgeResult();

            Cv2.Canny(
                InputMat,
                OutputMat,
                ThisParameter.CannyMinVal,
                ThisParameter.CannyMaxVal,
                ThisParameter.ApertureSize,
                ThisParameter.L2Gradient
            );

            ThisResult.Judge = EVisionJudge.OK;

            return EVisionRtnCode.OK;
        }
    }
}
