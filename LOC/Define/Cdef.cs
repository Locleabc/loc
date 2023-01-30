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
using Formatting = Newtonsoft.Json.Formatting;

namespace LOC.Define
{
    public class Cdef
    {
        public const string RecipeInfoFile = "recipe.json";


        public static MessageView massageView = new MessageView();
        public static MessageViewModel messageViewModel = new MessageViewModel();

        public static MainView mainView= new MainView();
        public static MainViewModel mainViewModel = new MainViewModel();


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
