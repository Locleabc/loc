using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOC.MVVM.ViewModels
{
    public class AutoViewModel
    {
        public WorkDataViewModel WorkDataVM
        {
            get
            {
                return _WorkDataVM ?? (_WorkDataVM = new WorkDataViewModel());
            }
        }
        public TrayViewModel InputTrayVM
        {
            get
            {
                return _InputTrayVM ?? (_InputTrayVM = new TrayViewModel("Input Tray"));
            }
        }
        public TrayViewModel OutputTrayVM
        {
            get
            {
                return _OutputTrayVM ?? (_OutputTrayVM = new TrayViewModel("Output Tray"));
            }
        }
        private WorkDataViewModel _WorkDataVM;
        private TrayViewModel _InputTrayVM;
        private TrayViewModel _OutputTrayVM;
    }
}
