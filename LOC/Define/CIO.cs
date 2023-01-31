using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOC.Define
{
    public class CIO
    {
        public CInput Input { get; set; } = new CInput();
        public COutput Output { get; set; } = new COutput();
        public CHeadStatus HeadStatus { get; set; } = new CHeadStatus();
    }
}
