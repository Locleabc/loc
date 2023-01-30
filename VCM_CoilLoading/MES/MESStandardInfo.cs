using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlatform.MES
{
    public class MESStandardInfo
    {
        public static string str_Device_Code;
        public static string str_Operation_Code;
        public static string str_Equip_ID_Code;
        public static string str_Product_Type_Code;

        public static string str_Device_Name;
        public static string str_Operation_Name;
        public static string str_Equip_ID_Name;
        public static string str_Product_Type_Name;

        public MESStandardInfo()
        {
            str_Device_Code = "";
            str_Operation_Code = "";
            str_Equip_ID_Code = "";
            str_Product_Type_Code = "";

            str_Device_Name = "";
            str_Operation_Name = "";
            str_Equip_ID_Name = "";
            str_Product_Type_Name = "";
        }

        public static void LoadStandardInfoFromFile()
        {
            // Read Data to file
            StringBuilder returnString = new StringBuilder();

            GetPrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Device_Code, "", returnString, 64, MES_STANDARD_INFO_FILENAME);
            str_Device_Code = returnString.ToString();
            GetPrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Operation_Code, "", returnString, 64, MES_STANDARD_INFO_FILENAME);
            str_Operation_Code = returnString.ToString();
            GetPrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Equip_ID_Code, "", returnString, 64, MES_STANDARD_INFO_FILENAME);
            str_Equip_ID_Code = returnString.ToString();
            GetPrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Product_Type_Code, "", returnString, 64, MES_STANDARD_INFO_FILENAME);
            str_Product_Type_Code = returnString.ToString();

            GetPrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Device_Name, "", returnString, 64, MES_STANDARD_INFO_FILENAME);
            str_Device_Name = returnString.ToString();
            GetPrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Operation_Name, "", returnString, 64, MES_STANDARD_INFO_FILENAME);
            str_Operation_Name = returnString.ToString();
            GetPrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Equip_ID_Name, "", returnString, 64, MES_STANDARD_INFO_FILENAME);
            str_Equip_ID_Name = returnString.ToString();
            GetPrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Product_Type_Name, "", returnString, 64, MES_STANDARD_INFO_FILENAME);
            str_Product_Type_Name = returnString.ToString();
        }

        public static void SaveStandardInfoToFile()
        {
            WritePrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Device_Code, str_Device_Code, MES_STANDARD_INFO_FILENAME);
            WritePrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Operation_Code, str_Operation_Code, MES_STANDARD_INFO_FILENAME);
            WritePrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Equip_ID_Code, str_Equip_ID_Code, MES_STANDARD_INFO_FILENAME);
            WritePrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Product_Type_Code, str_Product_Type_Code, MES_STANDARD_INFO_FILENAME);

            WritePrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Device_Name, str_Device_Name, MES_STANDARD_INFO_FILENAME);
            WritePrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Operation_Name, str_Operation_Name, MES_STANDARD_INFO_FILENAME);
            WritePrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Equip_ID_Name, str_Equip_ID_Name, MES_STANDARD_INFO_FILENAME);
            WritePrivateProfileString(MES_STANDARD_INFO_SECTION, MES_Product_Type_Name, str_Product_Type_Name, MES_STANDARD_INFO_FILENAME);
        }

        public MESStandardInfo(string str1, string str2, string str3, string str4, string str5, string str6, string str7, string str8)
        {
            str_Device_Code = str1;
            str_Operation_Code = str2;
            str_Equip_ID_Code = str3;
            str_Product_Type_Code = str4;

            str_Device_Name = str5;
            str_Operation_Name = str6;
            str_Equip_ID_Name = str7;
            str_Product_Type_Name = str8;
        }

        public static bool IsAllFieldNull()
        {
            return string.IsNullOrEmpty(str_Device_Code) && string.IsNullOrEmpty(str_Operation_Code) && string.IsNullOrEmpty(str_Equip_ID_Code) && string.IsNullOrEmpty(str_Product_Type_Code) &&
                   string.IsNullOrEmpty(str_Device_Name) && string.IsNullOrEmpty(str_Operation_Name) && string.IsNullOrEmpty(str_Equip_ID_Name) && string.IsNullOrEmpty(str_Product_Type_Name);
        }

        public static bool IsContainNullField()
        {
            return string.IsNullOrEmpty(str_Device_Code) || string.IsNullOrEmpty(str_Operation_Code) && string.IsNullOrEmpty(str_Equip_ID_Code) && string.IsNullOrEmpty(str_Product_Type_Code) &&
                   string.IsNullOrEmpty(str_Device_Name) || string.IsNullOrEmpty(str_Operation_Name) || string.IsNullOrEmpty(str_Equip_ID_Name) || string.IsNullOrEmpty(str_Product_Type_Name);
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        #region Privates
        private static string MES_STANDARD_INFO_SECTION = "MES_STANDARD_INFO";
        private static string MES_STANDARD_INFO_FILENAME = Path.Combine(ProgramFolder.FolderTOP, "MES_LastData.ini");

        private static string MES_Device_Code = "Device_Code_key";
        private static string MES_Operation_Code = "Operation_Code_key";
        private static string MES_Equip_ID_Code = "Equip_ID_key";
        private static string MES_Product_Type_Code = "Product_Type_key";

        private static string MES_Device_Name = "Device_Name";
        private static string MES_Operation_Name = "Operation_Name";
        private static string MES_Equip_ID_Name = "Equip_ID_Name";
        private static string MES_Product_Type_Name = "Product_Type";
        #endregion
    }
}
