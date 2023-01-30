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
using PLV_BracketAssemble.Define;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class RecipeChangeViewModel : PropertyChangedNotifier
    {
        #region Properties
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

        #region Constructors
        public RecipeChangeViewModel()
        {
        }
        #endregion

        #region Command
        public RelayCommand OpenButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    System.Diagnostics.Process.Start(GlobalFolders.FolderEQRecipe);
                });
            }
        }

        public RelayCommand RefreshButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    LoadListRecipe();
                });
            }
        }

        public RelayCommand ChangeButtonCommand
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

                        CDef.MainViewModel.MainContentVM.VisionAutoVM.InitVisionProcess();
                    }
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
        #endregion

        #region Methods
        public void LoadListRecipe()
        {
            ListRecipe = new ObservableCollection<RecipeInfo>();


            List<string> ListRecipeName = Directory.GetDirectories(GlobalFolders.FolderEQRecipe)
                                              .Select(Path.GetFileName)
                                              .ToList();

            foreach (string a in ListRecipeName)
            {
                ListRecipe.Add(new RecipeInfo { Name = a, Index = ListRecipe.Count });
            }
        }
        #endregion

        #region Privates
        private ObservableCollection<RecipeInfo> _ListRecipe;
        private RecipeInfo _SelectedRecipeItem;
        #endregion
    }
}
