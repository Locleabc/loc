using LOC.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TopCom.Command;

namespace LOC.MVVM.ViewModels
{
    public class HeaderViewModel
    {
        #region Properties
        #endregion

        #region Commands
        public RelayCommand ExitCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.messageViewModel.ShowDialog((string)Application.Current.FindResource("str_PushExitButton"));

                    if (CDef.messageViewModel.Result == false)
                    {
                        return;
                    }

                    CDef.mainViewModel.SwitchToTerminate();
                });
            }
        }
        #endregion

        #region Constructors
        public HeaderViewModel()
        {
        }
        #endregion

        #region Privates
        #endregion
    }
}
