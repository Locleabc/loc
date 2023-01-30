using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TopCom;
using TopCom.Command;
using TopCom.Models;
using TopCom.LOG;
using TOPV_Dispenser.Define;

namespace TOPV_Dispenser.MVVM.ViewModels
{
    public class RecipeViewModel : PropertyChangedNotifier
    {
        #region Properties
        public RecipeChangeViewModel RecipeChangeVM
        {
            get
            {
                return _RecipeChangeVM ?? (_RecipeChangeVM = new RecipeChangeViewModel());
            }
        }
        #endregion

        #region Commands
        public RelayCommand SaveRecipeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o is CheckBox)
                    {
                        UILog.Info($"Recipe Updated: [{(o as CheckBox).Content}] {!(o as CheckBox).IsChecked} -> {(o as CheckBox).IsChecked}");
                        CDef.MainViewModel.StatisticVM.StatisticHistory.AddRecord(
                            CDef.MainViewModel.StatisticVM.StatisticHistory.RecipeUpdateRecords,
                            new CRecipeUpdateRecord()
                            {
                                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Description = (o as CheckBox).Content.ToString(),
                                AxisName = null,
                                OldValue = (bool)!(o as CheckBox).IsChecked ? 1 : 0,
                                NewValue = (bool)(o as CheckBox).IsChecked ? 1 : 0,
                            }
                        );
                    }

                    SaveRecipe();
                });
            }
        }

        public RelayCommand<PositionData> DataUpdateCommand
        {
            get
            {
                return new RelayCommand<PositionData>((pos) =>
                {
                    PositionData pd = pos as PositionData;

                    if (pd.OldValue == pd.Value) return;

                    UILog.Info($"Recipe Updated: [{pd.PositionName}] {pd.OldValue} -> {pd.Value}");
                    CDef.MainViewModel.StatisticVM.StatisticHistory.AddRecord(
                        CDef.MainViewModel.StatisticVM.StatisticHistory.RecipeUpdateRecords,
                        new CRecipeUpdateRecord()
                        {
                            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Description = pd.PositionName,
                            AxisName = pd.AxisName,
                            OldValue = pd.OldValue,
                            NewValue = pd.Value,
                        }
                    );
                    SaveRecipe();
                });
            }
        }

        public RelayCommand TopCameraConnectCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.TopCamera.ConnectAsync();
                });
            }
        }

        public RelayCommand BottomCameraConnectCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.BotCamera.ConnectAsync();
                });
            }
        }
        #endregion

        #region Constructor
        public RecipeViewModel()
        {
        }
        #endregion

        #region Methods
        public void SaveRecipe()
        {
            CDef.GlobalRecipe.Save();
            CDef.CommonRecipe.Save();

            CDef.AllAxis?.Save();
        }
        #endregion

        #region Privates
        private RecipeChangeViewModel _RecipeChangeVM;
        #endregion
    }
}
