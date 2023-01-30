using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeLib = FASTECH.EziMOTIONPlusELib;

namespace TopMotion.IO
{
    public class IOInputPlusE : IOInputBase
    {
        private uint[] inputPinMask = new uint[] {
            (uint)EInputPin_PlusE.User_IN0,
            (uint)EInputPin_PlusE.User_IN1,
            (uint)EInputPin_PlusE.User_IN2,
            (uint)EInputPin_PlusE.User_IN3,
            (uint)EInputPin_PlusE.User_IN4,
            (uint)EInputPin_PlusE.User_IN5,
            (uint)EInputPin_PlusE.PT_A5,
            (uint)EInputPin_PlusE.PT_A6,
            (uint)EInputPin_PlusE.PT_A7,
        };

        internal override bool GetInput(int pinNumber)
        {
#if SIMULATION
            return base.GetInput(pinNumber);
#else
            uint ioInput = 0;
            NativeLib.FAS_GetIOInput(Index, ref ioInput);

            return (ioInput & inputPinMask[pinNumber]) == inputPinMask[pinNumber];
#endif
        }

        public IOInputPlusE(string name, int index)
            : base(name, index)
        {
        }
    }
}
