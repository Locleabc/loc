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
using VCM_CoilLoading.Define;

namespace VCM_CoilLoading.MVVM.ViewModels
{
    public class RecipeViewModel : PropertyChangedNotifier
    {
        #region Properties
        public Locale SelectedCulture
        {
            get { return _SelectedCulture; }
            set
            {
                if (_SelectedCulture == value) return;

                _SelectedCulture = value;
                App.SelectCulture(value.Id);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Locale> Cultures
        {
            get { return _Cultures; }
            set
            {
                _Cultures = value;
                OnPropertyChanged();
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
                    SaveRecipe();
                });
            }
        }
        #endregion

        #region Constructor
        public RecipeViewModel()
        {
            //LoadRecipe();
        }

        public void LoadRecipe()
        {
            CDef.CommonRecipe = CDef.CommonRecipe.Load<CCommonRecipe>();
            CDef.PickerRecipe = CDef.PickerRecipe.Load<CPickerRecipe>();
            CDef.UnderVisionRecipe = CDef.UnderVisionRecipe.Load<CUnderVisionRecipe>();
            CDef.UpperVisionRecipe = CDef.UpperVisionRecipe.Load<CUpperVisionRecipe>();

            CDef.AllAxis?.Load();
        }
        #endregion

        #region Methods
        public void SaveRecipe()
        {
            CDef.GlobalRecipe.Save();
            CDef.CommonRecipe.Save();
            CDef.PickerRecipe.Save();
            CDef.UnderVisionRecipe.Save();
            CDef.UpperVisionRecipe.Save();

            CDef.AllAxis?.Save();
        }
        #endregion

        #region Privates
        private Locale _SelectedCulture;
        private ObservableCollection<Locale> _Cultures = new ObservableCollection<Locale>()
        {
            new Locale() { Id="zh", Name="Chinese" },
            new Locale() { Id="en-US", Name="English" },
            new Locale() { Id="ko-KR", Name="Korean" },
            new Locale() { Id="vi-VN", Name="Vietnamese" }
        };
        #endregion
    }
}
