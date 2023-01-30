using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopMotion.IO
{
    public class IOBoardPlusR : IOBoardBase
    {
        public int SlaveId { get; set; }

        public IOBoardPlusR(string name, int portNumber, int slaveId, List<int> defaultOnInputs = null)
        {
            Name = name;
            Index = portNumber;
            SlaveId = slaveId;

            Input = new IOInputPlusR(Name, Index, slaveId, defaultOnInputs);
            Output = new IOOutputPlusR(Name, Index, slaveId);
        }
    }
}
