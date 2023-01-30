using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TopCom
{
    public class FileWriter
    {
        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes
        //  the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string to write to the file.</param>
        public static void WriteAllText(string path, string contents)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path);
                //Write content
                sw.Write(contents);
                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                LOG.UILog.Error("WriteAllText Exception: " + e.Message);
            }
        }
    }
}
