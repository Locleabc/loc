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
using VCM_FullAssy.Define;

namespace VCM_FullAssy.MVVM.ViewModels
{
    public class RecipeViewModel : PropertyChangedNotifier
    {
        #region Properties

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
            CDef.LeftTrayRecipe = CDef.LeftTrayRecipe.Load<CLeftTrayRecipe>();
            CDef.RightTrayRecipe = CDef.RightTrayRecipe.Load<CRightTrayRecipe>();
            CDef.TransferRecipe = CDef.TransferRecipe.Load<CTransferRecipe>();
            CDef.HeadRecipe = CDef.HeadRecipe.Load<CHeadRecipe>();
            CDef.UpperVisionRecipe = CDef.UpperVisionRecipe.Load<CUpperVisionRecipe>();
            CDef.UnderVisionRecipe = CDef.UnderVisionRecipe.Load<CUnderVisionRecipe>();

            CDef.AllAxis?.Load();
        }
        #endregion

        #region Methods
        public void SaveRecipe()
        {
            CDef.GlobalRecipe.Save();
            CDef.CommonRecipe.Save();
            CDef.LeftTrayRecipe.Save();
            CDef.RightTrayRecipe.Save();
            CDef.TransferRecipe.Save();
            CDef.HeadRecipe.Save();
            CDef.UpperVisionRecipe.Save();
            CDef.UnderVisionRecipe.Save();

            CDef.AllAxis?.Save();
        }
        #endregion

        #region Privates
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
