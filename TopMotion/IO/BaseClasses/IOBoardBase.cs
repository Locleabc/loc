using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopMotion.IO
{
    public class IOBoardBase : IIOBoard
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public virtual IIOInput Input { get; set; }
        public virtual IIOOutput Output { get; set; }
    }
}
