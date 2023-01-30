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
    /// Vision Parameter for <see cref="ColorConvertToGray"/> process
    /// </summary>
    public class ColorConvertToGrayParameter : VisionParameterBase
    {
    }

    /// <summary>
    /// Vision Result for <see cref="ColorConvertToGray"/> process
    /// </summary>
    public class ColorConvertToGrayResult : VisionResultBase
    {
    }

    public class ColorConvertToGray : VisionProcessBase
    {
        #region Privates
        private ColorConvertToGrayParameter ThisParameter
        {
            get { return (ColorConvertToGrayParameter)Parameter; }
        }

        private ColorConvertToGrayResult ThisResult
        {
            get { return (ColorConvertToGrayResult)Result; }
        }
        #endregion

        #region Constructor
        public ColorConvertToGray()
            : this(new ColorConvertToGrayParameter())
        {
        }

        public ColorConvertToGray(ColorConvertToGrayParameter parameter)
        {
            Parameter = parameter;
            Result = new ColorConvertToGrayResult();
        }
        #endregion 

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new ColorConvertToGrayResult();

            if (InputMat.Channels() != 1)
            {
                Cv2.CvtColor(InputMat, OutputMat, ColorConversionCodes.BGR2GRAY);
            }
            else
            {
                OutputMat = InputMat.Clone();
            }

            return EVisionRtnCode.OK;
        }
    }
}
