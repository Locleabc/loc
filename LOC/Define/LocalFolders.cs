using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOC.Define
{
    public class LocalFolders
    {
        public static string FolderEQRecipeUnderVision
        {
            get
            {
                return CDef.CurrentRecipeFolder + @"\Vision\UNDER";
            }
        }

        public static void Check()
        {
            System.IO.Directory.CreateDirectory(FolderEQRecipeUnderVision);
        }
    }
}
