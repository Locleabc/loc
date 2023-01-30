using System.Collections.Generic;
using System.Collections.ObjectModel;
using TopCom;
using TopCom.Processing;
using TopMotion;
using TopUI.Models;
using VCM_CoilLoading.Processing;

namespace VCM_CoilLoading.Define
{
    public static class MachineInfor
    {
        public static string MachineName
        {
            get
            {
                return "PLBT Coil Loading M/C";
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
                return "Move tray when pick done / MES Button Color / New Update";
            }
        }
    }
}
