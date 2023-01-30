using LOC.MVVM.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOC.MVVM.ViewModels
{
    public class MainTabControlViewModel
    {
        public MainTabControlView View { get; set; }

        //public void ShowVisionTeachingTab()  Chú ý
        //{
        //    View?.ShowVisionTeachingTab();
        //}

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

        //public VisionTeachingViewModel VisionTeachingVM                       chú ý
        //{
        //    get
        //    {
        //        return _VisionTeachingVM ?? (_VisionTeachingVM = new VisionTeachingViewModel());
        //    }
        //}

        public StatisticViewModel StatisticVM
        {
            get
            {
                return _StatisticVM ?? (_StatisticVM = new StatisticViewModel());
            }
        }

        public VisionViewModel VisionVM
        {
            get
            {
                return _VisionAutoVM ?? (_VisionAutoVM = new VisionViewModel());
            }
        }

        //public RecipeChangeViewModel RecipeChangeVM                                           chú ý
        //{
        //    get
        //    {
        //        return _RecipeChangeVM ?? (_RecipeChangeVM = new RecipeChangeViewModel());
        //    }
        //}
        #endregion

        #region Privates
        private AutoViewModel _AutoVM;
        private ManualViewModel _ManualVM;
        private RecipeViewModel _RecipeVM;
        //private VisionTeachingViewModel _VisionTeachingVM;
        private StatisticViewModel _StatisticVM;
        private VisionViewModel _VisionAutoVM;
        //private RecipeChangeViewModel _RecipeChangeVM;
        #endregion
    }
}
