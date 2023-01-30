using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom.Mqtt
{
    public class Config
    {
#if SIMULATION
        internal static string Host = "192.168.161.254";
#else
        internal static string Host = "172.16.161.254";
#endif
        internal static uint Port = 1883;
        internal static string ClientID = Guid.NewGuid().ToString();
        internal static string Username = "topveq";
        internal static string Password = "topv2020@@";
        internal static bool Tls = false;
    }

    /// <summary>
    /// Most used mqtt topics define</br>
    /// tp: topic</br>
    /// eq: equip</br>
    /// rm: remote</br>
    /// Example #1: tp/<publisher>/<topic_name>
    /// Example #2: tp/<publisher>/<target>/<topic_name>
    /// </summary>
    public class Topics
    {
        public static string EquipException = "tp/eq/exeception";
        public static string EquipPing = "tp/eq/ping";

        public static string RemoteMessageDisplay = "tp/rm/{0}/messagedisplay";
    }
}
