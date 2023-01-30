using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom.Mqtt
{
    public class ExceptionMessage
    {
        public string MachineID { get; set; } = MqttGlobal.Client.ClientID;
        public string Message { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Note { get; set; } = "";
    }
}
