using System.Collections.Generic;
using System.Collections.ObjectModel;
using TopCom;
using TopCom.Processing;
using TopMotion;
using TopUI.Models;
using VCM_FullAssy.Processing;

namespace VCM_FullAssy.Define
{
    public static class MachineInfor
    {
        public static string MachineName
        {
            get
            {
                return "PLBT Full Assemble M/C";
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
                return "Vision Upgrade - Reverse";
            }
        }
    }
}
