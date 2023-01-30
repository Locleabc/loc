using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;
using TopCom.Command;
using TopCom.MES;
using VCM_FullAssy.Define;
using VCM_FullAssy.MES;

namespace VCM_FullAssy.MVVM.ViewModels
{
    public class MESPMStatusViewModel : PropertyChangedNotifier
    {
        #region Commands
        public RelayCommand MESEquipEventChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o.GetType() != typeof(EPMStatus)) return;

                    CDef.MES.Send_PMStatus((EPMStatus)o);
                });
            }
        }

        public RelayCommand SavePDCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.MES.SavePD();

                    CDef.MES.SaveTT();
                });
            }
        }
        #endregion
    }
}
