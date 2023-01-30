using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPV_Dispenser.Define
{
    public class LocalFolders
    {
        public static string FolderEQRecipeLoadVision
        {
            get
            {
                return CDef.CurrentRecipeFolder + @"\Vision\LOAD";
            }
        }
        public static string FolderEQRecipeUnderVision
        {
            get
            {
                return CDef.CurrentRecipeFolder + @"\Vision\UNDER";
            }
        }
        public static string FolderEQRecipeUnloadVision
        {
            get
            {
                return CDef.CurrentRecipeFolder + @"\Vision\UNLOAD";
            }
        }
        public static string FolderEQRecipeInspectVision
        {
            get
            {
                return CDef.CurrentRecipeFolder + @"\Vision\INSPECT";
            }
        }

        public static void Check()
        {
            System.IO.Directory.CreateDirectory(FolderEQRecipeLoadVision);
            System.IO.Directory.CreateDirectory(FolderEQRecipeUnderVision);
            System.IO.Directory.CreateDirectory(FolderEQRecipeUnloadVision);
            System.IO.Directory.CreateDirectory(FolderEQRecipeInspectVision);
        }
    }
}
