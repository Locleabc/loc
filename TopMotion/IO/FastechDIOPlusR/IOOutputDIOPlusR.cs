using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeLib = FASTECH.EziMOTIONPlusRLib;

namespace TopMotion.IO
{
    public class IOOutputDIOPlusR : IOOutputBase
    {
        public int SlaveId { get; set; }

        public IOOutputDIOPlusR(string name, int portNumber, int slaveId)
            : base(name, portNumber)
        {
            SlaveId = slaveId;
        }

        private uint[] outputPinMask = new uint[] {
            0x0001,
            0x0002,
            0x0004,
            0x0008,
            0x0010,
            0x0020,
            0x0040,
            0x0080,
            0x0100,
            0x0200,
            0x0400,
            0x0800,
            0x1000,
            0x2000,
            0x4000,
            0x8000,
        };

        internal override void SetOutput(int pinNumber, bool value)
        {
#if SIMULATION
            base.SetOutput(pinNumber, value);
#else
            uint setMask = 0;
            uint clearMask = 0;

            if (value == true)
            {
                setMask = outputPinMask[pinNumber];
            }
            else
            {
                clearMask = outputPinMask[pinNumber];
            }

            NativeLib.FAS_SetOutput((byte)Index, (byte)SlaveId, setMask, clearMask);
#endif
        }

        internal override bool GetOutput(int pinNumber)
        {
#if SIMULATION
            return base.GetOutput(pinNumber);
#else
            uint ioOutput = 0;
            uint ioStatus = 0;
            NativeLib.FAS_GetOutput((byte)Index, (byte)SlaveId, ref ioOutput, ref ioStatus);

            return (ioOutput & outputPinMask[pinNumber]) == outputPinMask[pinNumber];
#endif
        }
    }
}
