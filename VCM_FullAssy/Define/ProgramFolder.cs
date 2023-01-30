using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCM_FullAssy.Define
{
    public class ProgramFolder
    {
        public static string FolderCurrent { get; } = Environment.CurrentDirectory;
        public static string FolderTOP { get; } = @"D:\TOP";
        public static string FolderEQ { get; } = @"D:\TOP\TOPVEQ";
        public static string FolderImages { get; } = @"D:\TOP\TOPVEQ\Images";
        public static string FolderEQLog { get; } = @"D:\TOP\TOPVEQ\Log";
        public static string FolderEQRecipe { get; } = @"D:\TOP\TOPVEQ\Recipe";
        public static string FolderEQRecipeLoadVision { get; } = CDef.CurrentRecipeFolder + @"\Vision\LOAD";
        public static string FolderEQRecipeUnderVision { get; } = CDef.CurrentRecipeFolder + @"\Vision\UNDER";
        public static string FolderEQRecipeUnloadVision { get; } = CDef.CurrentRecipeFolder + @"\Vision\UNLOAD";
        public static string FolderEQCount { get; } = @"D:\TOP\TOPVEQ\Count";

        public static string FolderMES { get; } = @"D:\MES";
        public static string FolderMESEquip { get; } = @"D:\MES\EQUIP";
        public static string FolderMESSet { get; } = @"D:\MES\SET";

        public static void Check()
        {
            System.IO.Directory.CreateDirectory(FolderTOP);
            System.IO.Directory.CreateDirectory(FolderEQ);
            System.IO.Directory.CreateDirectory(FolderImages);
            System.IO.Directory.CreateDirectory(FolderEQLog);
            System.IO.Directory.CreateDirectory(FolderEQRecipe);
            System.IO.Directory.CreateDirectory(FolderEQRecipeLoadVision);
            System.IO.Directory.CreateDirectory(FolderEQRecipeUnderVision);
            System.IO.Directory.CreateDirectory(FolderEQRecipeUnloadVision);
            System.IO.Directory.CreateDirectory(FolderEQCount);

            System.IO.Directory.CreateDirectory(FolderMES);
            System.IO.Directory.CreateDirectory(FolderMESEquip);
            System.IO.Directory.CreateDirectory(FolderMESSet);
        }
    }
}
