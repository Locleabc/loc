using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopMotion.IO
{
    public class IOBoardDIOPlusR : IOBoardBase
    {
        public int SlaveId { get; set; }

        public IOBoardDIOPlusR(string name, int portNumber, int slaveId)
        {
            Name = name;
            Index = portNumber;
            SlaveId = slaveId;

            Input = new IOInputDIOPlusR(Name, Index, slaveId);
            Output = new IOOutputDIOPlusR(Name, Index, slaveId);
        }
    }
}
