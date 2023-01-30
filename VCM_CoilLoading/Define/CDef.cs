using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using TopCom;
using TopCom.Models;
using TopMotion.IO;
using TopUI.Models;
using TopVision.Grabbers;
using TopVision.Lights;
using VCM_CoilLoading.MES;
using VCM_CoilLoading.MVVM.ViewModels;
using VCM_CoilLoading.MVVM.Views;
using VCM_CoilLoading.Processing;

namespace VCM_CoilLoading.Define
{
    public class CDef
    {
        #region CONST(s)
        public const string RecipeInfoFile = "recipe.json";

        public static ObservableCollection<Locale> Cultures = new ObservableCollection<Locale>()
        {
            new Locale() { Id="zh", Name="Chinese" },
            new Locale() { Id="en-US", Name="English" },
            new Locale() { Id="ko-KR", Name="Korean" },
            new Locale() { Id="vi-VN", Name="Vietnamese" }
        };
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
        public static IIOBoard IO1 { get; set; } = new IOBoardFAS16000("X1 FAS16000", 1);

        public static CGlobalRecipe GlobalRecipe { get; set; }
        public static CCommonRecipe CommonRecipe { get; set; }
        public static CPickerRecipe PickerRecipe { get; set; }
        public static CUnderVisionRecipe UnderVisionRecipe { get; set; }
        public static CUpperVisionRecipe UpperVisionRecipe { get; set; }

        public static RecipeInfo CurrentRecipe
        {
            get
            {
                string recipeInitPath = Path.Combine(ProgramFolder.FolderEQRecipe, RecipeInfoFile);

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

                return currentRecipeInfo;
            }
            set
            {
                string recipeInitPath = Path.Combine(ProgramFolder.FolderEQRecipe, RecipeInfoFile);

                if (!File.Exists(recipeInitPath))
                {
                    using (StreamWriter sw = File.AppendText(recipeInitPath))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(value, Formatting.Indented));
                    }
                }
            }
        }

        public static string CurrentRecipeFolder
        {
            get
            {
                Directory.CreateDirectory(Path.Combine(ProgramFolder.FolderEQRecipe, CurrentRecipe.Name));
                return Path.Combine(ProgramFolder.FolderEQRecipe, CurrentRecipe.Name);
            }
        }

        public static string CurrentRecipeLoadVisionConfigFilePath
        {
            get { return Path.Combine(ProgramFolder.FolderEQRecipeLoadVision, @"LoadVisionProcess.json"); }
        }

        public static string CurrentRecipeUnderVisionConfigFilePath
        {
            get { return Path.Combine(ProgramFolder.FolderEQRecipeUnderVision, @"UnderVisionProcess.json"); }
        }

        public static string CurrentRecipeUnloadVisionConfigFilePath
        {
            get { return Path.Combine(ProgramFolder.FolderEQRecipeUnloadVision, @"UnloadVisionProcess.json"); }
        }

        public static CRootProcess RootProcess { get; set; } = new CRootProcess("Common", 0, 2);

        public static CAllAxis AllAxis { get; set; }

        public static ITrayModel LoadingTray { get; set; } = new TrayModelBase();

        public static ITrayModel UnloadingTray1 { get; set; } = new TrayModelBase();

        public static ITrayModel UnloadingTray2 { get; set; } = new TrayModelBase();

        public static ITrayModel CurrentLoadingTray
        {
            get
            {
                if (LoadingTray.WorkIndexInRage)
                {
                    return LoadingTray;
                }
                else
                {
                    // TODO: considering to change this, null value may cause runtime exception
                    return null;
                }
            }
        }

        public static ITrayModel CurrentUnloadingTray
        {
            get
            {
                if (UnloadingTray1.WorkIndexInRage)
                {
                    return UnloadingTray1;
                }
                else if (UnloadingTray2.WorkIndexInRage)
                {
                    return UnloadingTray2;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
