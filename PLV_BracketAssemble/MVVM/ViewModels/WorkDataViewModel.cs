using PLV_BracketAssemble.Define;
using PLV_BracketAssemble.Define.WorkData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom.Command;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class WorkDataViewModel
    {
        public RelayCommand ResetCountDataButtonCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    Datas.WorkData.Reset();
                });
            }
        }
    }
}
