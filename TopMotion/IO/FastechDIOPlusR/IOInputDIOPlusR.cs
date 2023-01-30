using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeLib = FASTECH.EziMOTIONPlusRLib;

namespace TopMotion.IO
{
    public class IOInputDIOPlusR : IOInputBase
    {
        public int SlaveId { get; set; }

        private uint[] inputPinMask = new uint[] {
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

        internal override bool GetInput(int pinNumber)
        {
#if SIMULATION
            return base.GetInput(pinNumber);
#else
            uint ioInput = 0;
            uint ioLatch = 0;
            NativeLib.FAS_GetInput((byte)Index, (byte)SlaveId, ref ioInput, ref ioLatch);

            return (ioInput & inputPinMask[pinNumber]) == inputPinMask[pinNumber];
#endif
        }

        public IOInputDIOPlusR(string name, int portNumber, int slaveId)
            : base(name, portNumber)
        {
            SlaveId = slaveId;
        }
    }
}
