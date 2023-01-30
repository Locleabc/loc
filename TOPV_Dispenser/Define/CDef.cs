using Newtonsoft.Json;
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
using TOPV_Dispenser.MES;
using TOPV_Dispenser.MVVM.ViewModels;
using TOPV_Dispenser.MVVM.Views;
using TOPV_Dispenser.Processing;
using TopMotion;

namespace TOPV_Dispenser.Define
{
    public class CDef
    {
        #region CONST(s)
        public const string RecipeInfoFile = "recipe.json";

        public static ObservableCollection<Locale> Cultures
        {
            get
            {
                return new ObservableCollection<Locale>()
                {
                    new Locale() { Id="zh", Name="Chinese" },
                    new Locale() { Id="en-US", Name="English" },
                    new Locale() { Id="ko-KR", Name="Korean" },
                    new Locale() { Id="vi-VN", Name="Vietnamese" }
                };
            }
        }
        #endregion

        #region Views & ViewModels
        public static MainWindowView MainView = new MainWindowView();
        public static MainWindowViewModel MainViewModel = new MainWindowViewModel();

        public static MessageWindowView MessageView = new MessageWindowView();
        public static MessageWindowViewModel MessageViewModel = new MessageWindowViewModel();
        #endregion

        public static ICamera TopCamera { get; set; }
        public static ICamera BotCamera { get; set; }

        public static ILightController LightController { get; set; }

        public static CMES MES { get; set; } = new CMES();

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

        /// <summary>
        /// All IO
        /// </summary>
        public static CIO IO { get; set; } = new CIO();

        /// <summary>
        /// IO Board which link to IO
        /// </summary>
        public static IIOBoard IO1 { get; set; } = new IOBoardFAS16000("X1 FAS16000", 1, new List<int> { 6, 7, 12 });
        public static IIOBoard IO2 { get; set; } = new IOBoardPlusE("Y1 PlusE", 8);

        public static CGlobalRecipe GlobalRecipe { get; set; }
        public static CCommonRecipe CommonRecipe { get; set; }
#if USPCUTTING
        public static CPressRecipe PressRecipe { get; set; }
#endif

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

        public static string CurrentRecipeLoadVisionConfigFilePath
        {
            get { return Path.Combine(LocalFolders.FolderEQRecipeLoadVision, @"LoadVisionProcess.json"); }
        }

        public static string CurrentRecipeUnderVisionConfigFilePath
        {
            get { return Path.Combine(LocalFolders.FolderEQRecipeUnderVision, @"UnderVisionProcess.json"); }
        }

        public static string CurrentRecipeUnloadVisionConfigFilePath
        {
            get { return Path.Combine(LocalFolders.FolderEQRecipeUnloadVision, @"UnloadVisionProcess.json"); }
        }

        public static string CurrentRecipeInspectVisionConfigFilePath
        {
            get { return Path.Combine(LocalFolders.FolderEQRecipeInspectVision, @"InspectVisionProcess.json"); }
        }

        public static CRootProcess RootProcess { get; set; } = new CRootProcess("Common", 0, 1);

        public static BoardAjinAXL AjinBoard { get; set; } = new BoardAjinAXL();

        public static CAllAxis AllAxis { get; set; }

        public static ObservableCollection<TrayModelBase> LoadingTrays { get; set; } = new ObservableCollection<TrayModelBase>();
        public static ObservableCollection<TrayModelBase> UnloadingTrays { get; set; } = new ObservableCollection<TrayModelBase>();

        public static ITrayModel CurrentLoadingTray
        {
            get
            {
                try
                {
                    return LoadingTrays.First(tray => tray.WorkIndexInRage && tray.IsEnable);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static ITrayModel CurrentUnloadingTray
        {
            get
            {
                try
                {
                    return UnloadingTrays.First(tray => tray.WorkIndexInRage && tray.IsEnable);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
