using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopMotion.IO
{
    public class IOBoardFAS16000 : IOBoardBase
    {
        public IOBoardFAS16000(string name, int index, List<int> defaultOnInputs = null)
        {
            Name = name;

            Input = new IOInputFAS16000(name, index, defaultOnInputs);
            Output = new IOOutputFAS16000(name, index);
        }
    }
}
