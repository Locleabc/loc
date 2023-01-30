using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom.Mqtt
{
    public class AliveMessage
    {
        public string MachineID { get; set; } = MqttGlobal.Client.ClientID;
        public bool Status { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
