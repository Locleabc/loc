using TopVision.Models;
using OpenCvSharp;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision parameter for <see cref="AdaptiveThreshold"/> processing
    /// </summary>
    public class AdaptiveThresholdParameter : VisionParameterBase
    {
        #region Properties
        public int NeighbourhoodArea
        {
            get { return _NeighbourhoodArea; }
            set
            {
                if (_NeighbourhoodArea == value) return;

                _NeighbourhoodArea = value / 2 * 2 + 1;
                OnPropertyChanged();
            }
        }

        public double Constant
        {
            get { return _Constant; }
            set
            {
                if (_Constant == value) return;

                _Constant = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Privates
        private int _NeighbourhoodArea = 5;
        private double _Constant = 2;
        #endregion
    }

    /// <summary>
    /// Vision result for <see cref="AdaptiveThreshold"/> processing
    /// </summary>
    public class AdaptiveThresholdResult : VisionResultBase
    {
    }

    public class AdaptiveThreshold : VisionProcessBase
    {
        #region Privates
        private AdaptiveThresholdParameter ThisParameter
        {
            get { return (AdaptiveThresholdParameter)Parameter; }
        }

        private AdaptiveThresholdResult ThisResult
        {
            get { return (AdaptiveThresholdResult)Result; }
        }
        #endregion

        #region Constructors
        public AdaptiveThreshold()
            : this(new AdaptiveThresholdParameter())
        {
        }

        public AdaptiveThreshold(AdaptiveThresholdParameter parameter)
        {
            Parameter = parameter;
            Result = new AdaptiveThresholdResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new AdaptiveThresholdResult();

            OutputMat = new Mat(InputMat.Size(), InputMat.Type());
            OutputMat.SetTo(255);

            foreach (CRectangle roi in ThisParameter.ROIs)
            {
                using (Mat roiInput = InputMat.SubMat(roi.OCvSRect))
                {
                    using (Mat roiOutput = new Mat(roiInput.Size(), roiInput.Type()))
                    {
                        Cv2.AdaptiveThreshold(roiInput, roiOutput, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, ThisParameter.NeighbourhoodArea, ThisParameter.Constant);
                        roiOutput.CopyTo(OutputMat.SubMat(roi.OCvSRect));
                    }
                }
            }

            return EVisionRtnCode.OK;
        }
    }
}
