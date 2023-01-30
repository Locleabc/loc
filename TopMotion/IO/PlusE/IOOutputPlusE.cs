using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeLib = FASTECH.EziMOTIONPlusELib;

namespace TopMotion.IO
{
    public class IOOutputPlusE : IOOutputBase
    {
        public IOOutputPlusE(string name, int index)
            : base(name, index)
        {
        }

        internal override void SetOutput(int pinNumber, bool value)
        {
#if SIMULATION
            base.SetOutput(pinNumber, value);
#else
            uint setMask = 0;
            uint clearMask = 0;

            if (value == true)
            {
                setMask = (uint)((int)EOutputPin_PlusE.User_OUT0 << pinNumber);
            }
            else
            {
                clearMask = (uint)((int)EOutputPin_PlusE.User_OUT0 << pinNumber);
            }

            NativeLib.FAS_SetIOOutput(Index, setMask, clearMask);
#endif
        }

        internal override bool GetOutput(int pinNumber)
        {
#if SIMULATION
            return base.GetOutput(pinNumber);
#else
            uint ioOutput = 0;
            NativeLib.FAS_GetIOOutput(Index, ref ioOutput);

            uint pinBitMask = (uint)((int)EOutputPin_PlusE.User_OUT0 << pinNumber);

            return (ioOutput & pinBitMask) == pinBitMask;
#endif
        }
    }
}
