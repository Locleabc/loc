using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TopCom;
using TopCom.Processing;
using TopMotion;
using TopUI.Models;
using TOPV_Dispenser.Processing;

namespace TOPV_Dispenser.Define
{
    public static class MachineInfor
    {
        public static bool IsUSPCutting
        {
            get
            {
#if USPCUTTING
                return true;
#else
                return false;
#endif
            }
        }

        public static string MachineName
        {
            get
            {
#if USPCUTTING
                return "PLBT USP Cutting M/C";
#else
                return "OIS ACT Sub1+Sub2 Assemble";
#endif
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
            }
        }

        public static string VersionDescription
        {
            get
            {
                return File.ReadAllText("BuiltInfo.txt");
            }
        }
    }
}
