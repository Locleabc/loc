using TopVision.Models;
using OpenCvSharp;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision parameter for <see cref="Erode"/> processing
    /// </summary>
    public class ErodeParameter : VisionParameterBase
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

        public int Interations {
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
    public class ErodeResult : VisionResultBase
    {
    }

    public class Erode : VisionProcessBase
    {
        #region Privates
        private ErodeParameter ThisParameter
        {
            get { return (ErodeParameter)Parameter; }
        }

        private ErodeResult ThisResult
        {
            get { return (ErodeResult)Result; }
        }
        #endregion

        #region Constructors
        public Erode()
            : this(new ErodeParameter())
        {
        }

        public Erode(ErodeParameter parameter)
        {
            Parameter = parameter;
            Result = new ErodeResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new BinarizationResult();

            OutputMat = new Mat(InputMat.Size(), InputMat.Type());

            foreach (CRectangle roi in ThisParameter.ROIs)
            {
                using (Mat roiInput = InputMat.SubMat(roi.OCvSRect))
                {
                    using (Mat roiOutput = new Mat(roiInput.Size(), roiInput.Type()))
                    {
                        Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(ThisParameter.KernelSize, ThisParameter.KernelSize));
                        Cv2.Erode(roiInput, roiOutput, Kernel, new OpenCvSharp.Point(-1, -1), ThisParameter.Interations);

                        roiOutput.CopyTo(OutputMat.SubMat(roi.OCvSRect));
                    }
                }
            }
            //Cv2.Erode(InputMat, OutputMat, null);
            return EVisionRtnCode.OK;
        }
    }
}
