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
    /// Vision Parameter for <see cref="Smooth"/> process
    /// </summary>
    public class SmoothParameter : VisionParameterBase
    {
        #region Properties
        public int GaussianKernelSize
        {
            get { return _GaussianKernelSize; }
            set
            {
                if (_GaussianKernelSize != value)
                {
                    if (value % 2 == 1)
                    {
                        _GaussianKernelSize = value;
                    }
                    else
                    {
                        _GaussianKernelSize = value + 1;
                    }

                    OnPropertyChanged();
                }
            }
        }

        public double SigmaX
        {
            get { return _SigmaX; }
            set
            {
                _SigmaX = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private int _GaussianKernelSize = 3;
        private double _SigmaX = 0;
        #endregion
    }
    /// <summary>
    /// Vision Result for <see cref="Smooth"/> process
    /// </summary>
    public class SmoothResult : VisionResultBase
    {
    }

    public class Smooth : VisionProcessBase
    {
        #region Privates 
        private SmoothParameter ThisParameter
        {
            get { return (SmoothParameter)Parameter; }
        }

        private SmoothResult ThisResult
        {
            get { return (SmoothResult)Result; }
        }
        #endregion

        #region Constructor
        public Smooth()
            : this(new SmoothParameter())
        {
        }

        public Smooth(SmoothParameter parameter)
        {
            Result = new VisionResultBase();
            Parameter = parameter;
        }
        #endregion 

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new SmoothResult();

            Cv2.GaussianBlur(
                InputMat
                , OutputMat
                , new Size(
                    ThisParameter.GaussianKernelSize
                    , ThisParameter.GaussianKernelSize)
                , ThisParameter.SigmaX);

            return EVisionRtnCode.OK;
        }
    }
}
