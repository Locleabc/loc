using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeLib = FASTECH.FasFM16000Lib;

namespace TopMotion.IO
{
    public class IOInputFAS16000 : IOInputBase
    {
        List<int> DefaultOnInputs = new List<int>();

        public IOInputFAS16000(string name, int index, List<int> defaultOnInputs = null)
            : base(name, index)
        {
            DefaultOnInputs = defaultOnInputs;

            if (defaultOnInputs == null) return;

            for (int i = 0; i < DefaultOnInputs.Count; i++)
            {
                SimulationInputValue[DefaultOnInputs[i]] = true;
            }
        }

        public override bool[] SimulationInputValue { get; set; } = new bool[16];

        internal override bool GetInput(int pinNumber)
        {
#if SIMULATION
            return base.GetInput(pinNumber);
#else
            int ioInput = 0;
            NativeLib.FAS_GetIo(Index, true, ref ioInput);

            return (ioInput & (1 << pinNumber)) == (1 << pinNumber);
#endif
        }
    }
}
