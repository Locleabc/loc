using Newtonsoft.Json;
using PLV_BracketAssemble.MVVM.ViewModels;
using PLV_BracketAssemble.MVVM.Views;
using PLV_BracketAssemble.Processing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TopCom;
using TopCom.Models;
using TopMotion.IO;
using TopUI.Models;
using TopVision.Grabbers;
using TopVision.Lights;

namespace PLV_BracketAssemble.Define
{
    public class CDef
    {
        #region Views & ViewModels
        public static MainWindowView MainView = new MainWindowView();
        public static MainWindowViewModel MainViewModel = new MainWindowViewModel();

        public static MessageWindowView MessageView = new MessageWindowView();
        public static MessageWindowViewModel MessageViewModel = new MessageWindowViewModel();
        #endregion

        public static bool IsSimulationMode
        {
            get
            {
#if SIMULATION
                return true;
#else
                return false;
#endif
            }
        }
        public static ICamera BotCamera { get; set; }
        public static ILightController LightController { get; set; }

        public static CRootProcess RootProcess { get; set; } = new CRootProcess("Global", 0, 1);

        #region Motion & IO
        public static CAllAxis AllAxis { get; set; }

        /// <summary>
        /// All IO
        /// </summary>
        public static CIO IO { get; set; } = new CIO();

        /// <summary>
        /// IO Board which link to IO
        /// </summary>
        public static IIOBoard IO_X { get; set; } = new IOBoardPlusR("XAxis IO", 1, 0);
        public static IIOBoard IO_Y { get; set; } = new IOBoardPlusR("YAxis IO", 1, 1);
        public static IIOBoard IO_XX { get; set; } = new IOBoardPlusR("XXAxis IO", 1, 2);
        /// <summary>
        /// Using same COM1 PORT -> no need to Connect
        /// </summary>
        public static IIOInput In_DIO3 { get; set; } = new IOInputDIOPlusR("DIO3 Input", 1, 5);
        /// <summary>
        /// Using same COM1 PORT -> no need to Connect
        /// </summary>
        public static IIOOutput Out_DIO4 { get; set; } = new IOOutputDIOPlusR("DIO4 Output", 1, 6);

        /// <summary>
        /// Tester Turn Axis COM2 PORT -> NEED to Connect on Motion Init
        /// </summary>
        public static IIOBoard IO_Tester { get; set; } = new IOBoardPlusR("Tester IO", 2, 0, new List<int> { 7, 8 });
        #endregion

        #region Recipe
        public static CGlobalRecipe GlobalRecipe { get; set; }
        public static CCommonRecipe CommonRecipe { get; set; }
        public static CTrayRecipe TrayRecipe { get; set; }
        public static CHeadRecipe HeadRecipe { get; set; }
        public static CUnderVisionRecipe UnderVisionRecipe { get; set; }

        public const string RecipeInfoFile = "recipe.json";

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

        public static string CurrentRecipeUnderVisionConfigFilePath
        {
            get { return Path.Combine(LocalFolders.FolderEQRecipeUnderVision, @"UnderVisionProcess.json"); }
        }
        #endregion
    }
}
