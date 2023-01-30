using log4net;
using log4net.Config;
using System;
using System.IO;

namespace TopCom.Define
{
    public static class LogFactory
    {
        public const string ConfigFile = "log4net.config";

        public static void Configure(string configFileName = ConfigFile)
        {
            Type type = typeof(LogFactory);
            FileInfo configFile = new FileInfo(configFileName);
            XmlConfigurator.ConfigureAndWatch(configFile);
        }
    }
}
