using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Models;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision Parameter for <see cref="Masking"/> process
    /// </summary>
    public class MaskingParameter : VisionParameterBase
    {
    }

    /// <summary>
    /// Vision Result for <see cref="Masking"/> process
    /// </summary>
    public class MaskingResult : VisionResultBase
    {
    }

    public class Masking : VisionProcessBase
    {
        #region Constructors
        public Masking()
            : this(new MaskingParameter())
        {
        }

        public Masking(MaskingParameter parameter)
        {
            Parameter = parameter;
            Result = new MaskingResult();
        }

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            OutputMat = InputMat;

            return EVisionRtnCode.OK;
        }
        #endregion
    }
}
