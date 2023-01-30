using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Command;
using VCM_FullAssy.Define;

namespace VCM_FullAssy.MVVM.ViewModels
{
    public class StatisticViewModel
    {
        #region Commands
        public RelayCommand TestButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                });
            }
        }
        #endregion
    }
}
