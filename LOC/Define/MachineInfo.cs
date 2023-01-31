using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOC.Define
{
    public class MachineInfo
    {
        public static string MachineName
        {
            get
            {
                return "LOC Machine";
            }
        }

        public static string SoftwareVersion
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;
                return version;
                //return "8.8.8.8";
            }
        }

        public static string VersionDescription
        {
            get
            {
                //return File.ReadAllText("BuiltInfo.txt");
                return "5.5.5.5";
            }
        }
    }
}
