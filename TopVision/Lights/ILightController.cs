using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Models;

namespace TopVision.Lights
{
    public interface ILightController : ILoggable
    {
        // Number of channel with-in the controller (start with 1)
        int NumberOfChannel { get; set; }

        string Name { get; set; }

        /// <summary>
        /// Light controller connect function
        /// </summary>
        /// <returns>true: connect success; fall: connect fail</returns>
        bool Open();
        void Close();

        void SetLightLevel(int channel, int value);
        void SetLightStatus(int channel, bool bOnOff);

        int GetLightLevel(int channel);
        bool GetLightStatus(int channel);
    }
}
