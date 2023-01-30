using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom
{
    public class GlobalFolders
    {
        public static string CurrentDate
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd"); }
        }
        public static string CurrentTime
        {
            get { return DateTime.Now.ToString("HHmmss_fff"); }
        }
        public static string FolderCurrent { get; } = Environment.CurrentDirectory;
        public static string FolderTOP { get; } = @"D:\TOP";
        public static string FolderEQ { get; } = @"D:\TOP\TOPVEQ";
        public static string FolderEQCount { get; } = @"D:\TOP\TOPVEQ\Count";
        public static string FolderEQConfig { get; } = @"D:\TOP\TOPVEQ\Config";
        public static string FolderImages { get; } = @"D:\TOP\TOPVEQ\Images";
        public static string FolderEQLog { get; } = @"D:\TOP\TOPVEQ\Log";
        public static string FolderEQLogRecipeUpdate { get; } = @"D:\TOP\TOPVEQ\Log\RecipeUpdate";
        public static string FolderEQRecipe { get; } = @"D:\TOP\TOPVEQ\Recipe";
        
        public static string FolderMES { get; } = @"D:\MES";
        public static string FolderMESEquip { get; } = @"D:\MES\EQUIP";
        public static string FolderMESSet { get; } = @"D:\MES\SET";

        public static void Check()
        {
            System.IO.Directory.CreateDirectory(FolderTOP);
            System.IO.Directory.CreateDirectory(FolderEQ);
            System.IO.Directory.CreateDirectory(FolderImages);
            System.IO.Directory.CreateDirectory(FolderEQLog);
            System.IO.Directory.CreateDirectory(FolderEQLogRecipeUpdate);
            System.IO.Directory.CreateDirectory(FolderEQRecipe);
            System.IO.Directory.CreateDirectory(FolderEQCount);

            System.IO.Directory.CreateDirectory(FolderMES);
            System.IO.Directory.CreateDirectory(FolderMESEquip);
            System.IO.Directory.CreateDirectory(FolderMESSet);
        }
    }
}
