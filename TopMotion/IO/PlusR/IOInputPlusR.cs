using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeLib = FASTECH.EziMOTIONPlusRLib;

namespace TopMotion.IO
{
    public class IOInputPlusR : IOInputBase
    {
        public int SlaveId { get; set; }

        private uint[] inputPinMask = new uint[] {
            (uint)EInputPin_PlusR.User_IN0,
            (uint)EInputPin_PlusR.User_IN1,
            (uint)EInputPin_PlusR.User_IN2,
            (uint)EInputPin_PlusR.User_IN3,
            (uint)EInputPin_PlusR.User_IN4,
            (uint)EInputPin_PlusR.User_IN5,
            (uint)EInputPin_PlusR.PT_A5,
            (uint)EInputPin_PlusR.PT_A6,
            (uint)EInputPin_PlusR.PT_A7,
        };

        internal override bool GetInput(int pinNumber)
        {
#if SIMULATION
            return base.GetInput(pinNumber);
#else
            uint ioInput = 0;
            NativeLib.FAS_GetIOInput((byte)Index, (byte)SlaveId, ref ioInput);

            return (ioInput & inputPinMask[pinNumber]) == inputPinMask[pinNumber];
#endif
        }

        public IOInputPlusR(string name, int portNumber, int slaveId, List<int> defaultOnInputs = null)
            : base(name, portNumber)
        {
            SlaveId = slaveId;

            if (defaultOnInputs == null) return;

            for (int i = 0; i < defaultOnInputs.Count; i++)
            {
                SimulationInputValue[defaultOnInputs[i]] = true;
            }
        }
    }
}
