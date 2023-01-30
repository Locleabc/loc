using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Models;

namespace TopVision.Algorithms
{
    public class MultiContourCenterDetectionParameter : VisionParameterBase
    {
    }

    public class MultiContourCenterDetectionResult : VisionResultBase
    {
    }

    public class MultiContourCenterDetection : VisionProcessBase
    {
        private MultiContourCenterDetectionParameter ThisParameter;

        public MultiContourCenterDetection(MultiContourCenterDetectionParameter parameter)
        {
            Parameter = parameter;
            Result = new MultiContourCenterDetectionResult();

            ThisParameter = Parameter as MultiContourCenterDetectionParameter;
        }

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            EVisionRtnCode rtnCode = EVisionRtnCode.OK;



            return rtnCode;
        }
    }
}
