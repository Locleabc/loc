using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopMotion.IO
{
    public class IOBoardPlusE : IOBoardBase
    {
        public IOBoardPlusE(string name, int index)
        {
            Name = name;
            Index = index;

            Input = new IOInputPlusE(Name, Index);
            Output = new IOOutputPlusE(Name, Index);
        }
    }
}
