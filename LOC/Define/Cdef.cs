using LOC.MVVM.ViewModels;
using LOC.MVVM.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TopCom;
using TopCom.Models;
using TopMotion.IO;
using Formatting = Newtonsoft.Json.Formatting;

namespace LOC.Define
{
    public class CDef
    {
        public const string RecipeInfoFile = "recipe.json";


        public static MessageView massageView = new MessageView();
        public static MessageViewModel messageViewModel = new MessageViewModel();

        public static MainView mainView= new MainView();
        public static MainViewModel mainViewModel = new MainViewModel();

        public static CIO IO { get; set; } = new CIO();
        public static IIOBoard IO_X { get; set; } = new IOBoardPlusR("XAxis IO", 1, 0);
        public static IIOBoard IO_Y { get; set; } = new IOBoardPlusR("YAxis IO", 1, 1);
        public static IIOBoard IO_XX { get; set; } = new IOBoardPlusR("XXAxis IO", 1, 2);

        public static IIOInput In_DIO3 { get; set; } = new IOInputDIOPlusR("DIO3 Input", 1, 5);
        /// <summary>
        /// Using same COM1 PORT -> no need to Connect
        /// </summary>
        public static IIOOutput Out_DIO4 { get; set; } = new IOOutputDIOPlusR("DIO4 Output", 1, 6);

        /// <summary>
        /// Tester Turn Axis COM2 PORT -> NEED to Connect on Motion Init
        /// </summary>
        public static IIOBoard IO_Tester { get; set; } = new IOBoardPlusR("Tester IO", 2, 0, new List<int> { 7, 8 });


        public static string CurrentRecipeFolder
        {
            get
            {
                if (!Directory.Exists(Path.Combine(GlobalFolders.FolderEQRecipe, CurrentRecipe.Name)))
                {
                    Directory.CreateDirectory(Path.Combine(GlobalFolders.FolderEQRecipe, CurrentRecipe.Name));
                }
                return Path.Combine(GlobalFolders.FolderEQRecipe, CurrentRecipe.Name);
            }
        }

        public static RecipeInfo CurrentRecipe
        {
            get
            {
                string recipeInitPath = Path.Combine(GlobalFolders.FolderEQRecipe, RecipeInfoFile);

                if (!File.Exists(recipeInitPath))
                {
                    RecipeInfo defaultRecipeInfo = new RecipeInfo();

                    Directory.CreateDirectory(Path.GetDirectoryName(recipeInitPath));
                    using (StreamWriter sw = File.AppendText(recipeInitPath))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(defaultRecipeInfo, Formatting.Indented));
                    }
                }

                string currentRecipeInfoString = File.ReadAllText(recipeInitPath);
                RecipeInfo currentRecipeInfo = JsonConvert.DeserializeObject<RecipeInfo>(currentRecipeInfoString);

                if (currentRecipeInfo == null)
                {
                    currentRecipeInfo = new RecipeInfo();
                }

                return currentRecipeInfo;
            }
            set
            {
                string recipeInitPath = Path.Combine(GlobalFolders.FolderEQRecipe, RecipeInfoFile);

                if (!File.Exists(recipeInitPath))
                {
                    using (StreamWriter sw = File.AppendText(recipeInitPath))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(value, Formatting.Indented));
                    }
                }
                File.WriteAllText(recipeInitPath, JsonConvert.SerializeObject(value, Formatting.Indented));
            }
        }
    }
}
