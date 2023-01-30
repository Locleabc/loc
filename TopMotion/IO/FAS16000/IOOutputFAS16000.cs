using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeLib = FASTECH.FasFM16000Lib;

namespace TopMotion.IO
{
    public class IOOutputFAS16000 : IOOutputBase
    {
        public IOOutputFAS16000(string name, int index)
            : base(name, index)
        {
        }

        internal override void SetOutput(int pinNumber, bool value)
        {
#if SIMULATION
            base.SetOutput(pinNumber, value);
#else
            NativeLib.FAS_SetIoBit(Index, true, pinNumber, value);
#endif
        }

        internal override bool GetOutput(int pinNumber)
        {
#if SIMULATION
            return base.GetOutput(pinNumber);
#else
            int outputStatus = 0;
            NativeLib.FAS_GetIoOutputStatus(Index, true, ref outputStatus);

            return (outputStatus & (1 << pinNumber)) == (1 << pinNumber);
#endif
        }
    }
}
