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
    /// Vision parameter for <see cref="Bitwise"/> process
    /// </summary>
    public class BitwiseParameter : VisionParameterBase
    {
    }

    /// <summary>
    /// Vision Result for <see cref="Bitwise"/> process
    /// </summary>
    public class BitwiseResult : VisionResultBase
    {
    }

    public class Bitwise : VisionProcessBase
    {
        #region Privates
        private BitwiseParameter ThisParameter
        {
            get { return (BitwiseParameter)Parameter; }
        }

        private BitwiseResult ThisResult
        {
            get { return (BitwiseResult)Result; }
        }
        #endregion

        #region Constructors
        public Bitwise()
            : this(new BitwiseParameter())
        {
        }

        public Bitwise(BitwiseParameter parameter)
        {
            Parameter = parameter;
            Result = new BitwiseResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new BitwiseResult();

            Cv2.BitwiseNot(InputMat, OutputMat);

            return EVisionRtnCode.OK;
        }
    }
}
