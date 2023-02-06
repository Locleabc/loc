using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOC.MVVM.ViewModels
{
    public class RecipeViewModel
    {
        public ManualControlMotionViewModel ManualControlMotionVM
        {
            get
            {
                return _ManualControlMotionVM ?? (_ManualControlMotionVM = new ManualControlMotionViewModel());
            }
        }
        private ManualControlMotionViewModel _ManualControlMotionVM;
    }
}
