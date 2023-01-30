using PLV_BracketAssemble.MVVM.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class MainContentViewModel
    {
        public MainContentView View { get; set; }

        public void ShowVisionTeachingTab()
        {
            View?.ShowVisionTeachingTab();
        }

        #region Properties
        public AutoViewModel AutoVM
        {
            get
            {
                return _AutoVM ?? (_AutoVM = new AutoViewModel());
            }
        }

        public ManualViewModel ManualVM
        {
            get
            {
                return _ManualVM ?? (_ManualVM = new ManualViewModel());
            }
        }

        public RecipeViewModel RecipeVM
        {
            get
            {
                return _RecipeVM ?? (_RecipeVM = new RecipeViewModel());
            }
        }

        public VisionTeachingViewModel VisionTeachingVM
        {
            get
            {
                return _VisionTeachingVM ?? (_VisionTeachingVM = new VisionTeachingViewModel());
            }
        }

        public StatisticViewModel StatisticVM
        {
            get
            {
                return _StatisticVM ?? (_StatisticVM = new StatisticViewModel());
            }
        }

        public VisionAutoViewModel VisionAutoVM
        {
            get
            {
                return _VisionAutoVM ?? (_VisionAutoVM = new VisionAutoViewModel());
            }
        }

        public RecipeChangeViewModel RecipeChangeVM
        {
            get
            {
                return _RecipeChangeVM ?? (_RecipeChangeVM = new RecipeChangeViewModel());
            }
        }
        #endregion

        #region Privates
        private AutoViewModel _AutoVM;
        private ManualViewModel _ManualVM;
        private RecipeViewModel _RecipeVM;
        private VisionTeachingViewModel _VisionTeachingVM;
        private StatisticViewModel _StatisticVM;
        private VisionAutoViewModel _VisionAutoVM;
        private RecipeChangeViewModel _RecipeChangeVM;
        #endregion
    }
}
