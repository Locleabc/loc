using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Command;
using VCM_CoilLoading.Define;

namespace VCM_CoilLoading.MVVM.ViewModels
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
