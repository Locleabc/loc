using TopVision.Models;
using OpenCvSharp;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision parameter for <see cref="Binarization"/> processing
    /// </summary>
    public class BinarizationParameter : VisionParameterBase
    {
    }

    /// <summary>
    /// Vision result for <see cref="Binarization"/> processing
    /// </summary>
    public class BinarizationResult : VisionResultBase
    {
    }

    public class Binarization : VisionProcessBase
    {
        #region Privates
        private BinarizationParameter ThisParameter
        {
            get { return (BinarizationParameter)Parameter; }
        }

        private BinarizationResult ThisResult
        {
            get { return (BinarizationResult)Result; }
        }
        #endregion

        #region Constructors
        public Binarization()
            : this(new BinarizationParameter())
        {
        }

        public Binarization(BinarizationParameter parameter)
        {
            Parameter = parameter;
            Result = new BinarizationResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new BinarizationResult();

            Cv2.Threshold(InputMat, OutputMat, ThisParameter.Threshold, 255, ThresholdTypes.Binary);

            return EVisionRtnCode.OK;
        }
    }
}
