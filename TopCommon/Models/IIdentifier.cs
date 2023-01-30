using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom
{
    public interface IIdentifier
    {
        string Name { get; set; }
        int Index { get; set; }
    }
}
