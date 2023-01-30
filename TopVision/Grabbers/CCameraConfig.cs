using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Grabbers
{
    public class CCameraConfig
    {
        public int PacketSize { get; set; } = 9000;
        public int InterPacketDelay { get; set; } = 0;
    }
}
