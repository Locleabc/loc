using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopVision.ViewModels
{
    public class TemplateMaskingWindowViewModel : PropertyChangedNotifier
    {
        #region Properties
        public TemplateMaskingViewModel TemplateMaskingVM
        {
			get { return _TemplateMaskingVM ?? (_TemplateMaskingVM = new TemplateMaskingViewModel()); }
		}
        #endregion

        #region Privates
		private TemplateMaskingViewModel _TemplateMaskingVM;
        #endregion
    }
}
