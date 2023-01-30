using TopVision.Models;
using OpenCvSharp;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision parameter for <see cref="Dilate"/> processing
    /// </summary>
    public class DilateParameter : VisionParameterBase
    {
        #region Properties

        public int KernelSize
        {
            get { return _KernelSize; }
            set
            {
                if (_KernelSize == value || value < 3)
                {
                    return;
                }
                else
                {
                    _KernelSize = value;
                }

                OnPropertyChanged();
            }
        }

        public int Interations
        {
            get { return _Interations; }
            set
            {
                if (_Interations == value || value < 1)
                {
                    return;
                }
                else
                {
                    _Interations = value;
                }

                OnPropertyChanged();
            }
        }

        #endregion
        #region Privates
        private int _KernelSize = 3;
        private int _Interations = 1;
        #endregion
    }

    /// <summary>
    /// Vision result for <see cref="Erode"/> processing
    /// </summary>
    public class DilateResult : VisionResultBase
    {
    }

    public class Dilate : VisionProcessBase
    {
        #region Privates
        private DilateParameter ThisParameter
        {
            get { return (DilateParameter)Parameter; }
        }

        private DilateResult ThisResult
        {
            get { return (DilateResult)Result; }
        }
        #endregion

        #region Constructors
        public Dilate()
            : this(new DilateParameter())
        {
        }

        public Dilate(DilateParameter parameter)
        {
            Parameter = parameter;
            Result = new DilateResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new BinarizationResult();

            //Cv2.Erode(InputMat, OutputMat, null);
            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(ThisParameter.KernelSize, ThisParameter.KernelSize));
            Cv2.Dilate(InputMat, OutputMat, Kernel, new OpenCvSharp.Point(-1, -1), ThisParameter.Interations);
            return EVisionRtnCode.OK;
        }
    }
}
