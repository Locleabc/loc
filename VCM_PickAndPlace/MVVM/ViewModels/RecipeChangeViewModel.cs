using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom;
using TopCom.Command;
using TopCom.Models;
using VCM_PickAndPlace.Define;

namespace VCM_PickAndPlace.MVVM.ViewModels
{
    public class RecipeChangeViewModel : PropertyChangedNotifier
    {
        #region Properties
        public List<string> ListRecipeName;

        public ObservableCollection<RecipeInfo> ListRecipe
        {
            get { return _ListRecipe; }
            set
            {
                _ListRecipe = value;
                OnPropertyChanged();
            }
        }

        public RecipeInfo SelectedRecipeItem
        {
            get { return _SelectedRecipeItem; }
            set
            {
                _SelectedRecipeItem = value;
                OnPropertyChanged();
            }
        }
        public string NameOfCurrentRecipe
        {
            get
            {
                return CDef.CurrentRecipe.Name;
            }
        }
        #endregion

        #region Constructor
        public RecipeChangeViewModel()
        {
            //UpdateRecipeListCommand.Execute(null);
        }
        #endregion

        #region Commands
        public RelayCommand UpdateRecipeListCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    ListRecipeName = Directory.GetDirectories(GlobalFolders.FolderEQRecipe)
                                              .Select(Path.GetFileName)
                                              .ToList();

                    ListRecipe = new ObservableCollection<RecipeInfo>();

                    foreach (string a in ListRecipeName)
                    {
                        ListRecipe.Add(new RecipeInfo { Name = a });
                    }
                });
            }
        }

        public RelayCommand RecipeChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (SelectedRecipeItem == null)
                    {
                        CDef.MessageViewModel.Show("Select recipe to change!", isAlarm: true, caption: "Notice");
                        return;
                    }    
                    if (SelectedRecipeItem.Name == CDef.CurrentRecipe.Name)
                    {
                        CDef.MessageViewModel.Show("This recipe is being used!", isAlarm: true, caption: "Notice");
                        return;
                    }

                    CDef.MessageViewModel.ShowDialog($"Do you want change recipe: \n {CDef.CurrentRecipe.Name} => {SelectedRecipeItem.Name}");

                    if (CDef.MessageViewModel.Result == true)
                    {
                        CDef.CurrentRecipe = SelectedRecipeItem;
                        
                        OnPropertyChanged("NameOfCurrentRecipe");

                        Thread.Sleep(200);

                        CDef.MainViewModel.InitVM.InitRecipe();

                        Thread.Sleep(200);

                        CDef.MainViewModel.VisionAutoVM.InitVisionProcess();
                    }
                });
            }
        }

        public RelayCommand CopyButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (SelectedRecipeItem == null)
                    {
                        CDef.MessageViewModel.Show("Please select Recipe! ");
                        return;
                    }

                    CDef.MessageViewModel.ShowDialog($"Do you want Copy Recipe:{SelectedRecipeItem.Name} ?");

                    if (CDef.MessageViewModel.Result != true) return;

                    string currentRecipe = SelectedRecipeItem.Name;
                    string currentRecipePath = Path.Combine(GlobalFolders.FolderEQRecipe, currentRecipe);
                    string newRecipe = $"{currentRecipe} - Copy";
                    int folderIndex = 2;
                    while (Directory.Exists(Path.Combine(GlobalFolders.FolderEQRecipe, newRecipe)))
                    {
                        newRecipe = $"{currentRecipe} - Copy ({folderIndex++})";
                    }
                    string newRecipePath = Path.Combine(GlobalFolders.FolderEQRecipe, newRecipe);

                    CopyDirectory(sourceDir: currentRecipePath, destinationDir: newRecipePath, recursive: true);

                    UpdateRecipeListCommand.Execute(null);
                    SelectedRecipeItem = ListRecipe.First(r => r.Name == newRecipe);
                    RecipeChangeCommand.Execute(o);
                });
            }
        }

        public RelayCommand DeleteButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (SelectedRecipeItem == null) return;

                    if (SelectedRecipeItem.Name.Equals(CDef.CurrentRecipe.Name))
                    {
                        CDef.MessageViewModel.Show("This recipe is being used!", caption: "Warning");
                        return;
                    }

                    if (ListRecipe.Count == 1) return;

                    CDef.MessageViewModel.ShowDialog($"Do you want Delete recipe: \" {SelectedRecipeItem.Name} \" ? ");

                    if (CDef.MessageViewModel.Result == true)
                    {
                        string pathDelete = Path.Combine(GlobalFolders.FolderEQRecipe, SelectedRecipeItem.Name);

                        try
                        {
                            Directory.Delete(pathDelete, true);
                        }
                        catch { }

                        ListRecipe.Remove(SelectedRecipeItem);
                    }
                });
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
        #endregion

        #region Privates
        private ObservableCollection<RecipeInfo> _ListRecipe;

        private RecipeInfo _SelectedRecipeItem;
        #endregion
    }
}
