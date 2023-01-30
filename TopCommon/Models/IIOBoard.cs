using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom
{
    public interface IIOBoard : IIdentifier
    {
        IIOInput Input { get; set; }
        IIOOutput Output { get; set; }
    }
}
