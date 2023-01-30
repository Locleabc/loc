using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TopLang
{
    public class Language
    {
        public const string ParaPattern = "{param}";

        // {para}
        public static string GetString(string key, params object[] args)
        {
            string strRtn = "";

            try
            {
                strRtn = (string)Application.Current.FindResource(key);

                for (int i = 0; i < args.Count(); i++)
                {
                    strRtn = strRtn.Replace(ParaPattern, (string)args[i]);
                }
            }
            catch { }

            return strRtn;
        }
    }
}
