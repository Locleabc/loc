using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom
{
    public interface IIOInput : IIdentifier
    {
        /// <summary>
        /// Simulation Value
        /// </summary>
        bool[] SimulationInputValue { get; set; }

        bool this[int index]
        {
            get;
        }
    }
}
