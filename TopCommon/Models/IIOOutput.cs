using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom
{
    public interface IIOOutput : IIdentifier
    {
        bool this[int index]
        {
            get;
            set;
        }
    }
}
