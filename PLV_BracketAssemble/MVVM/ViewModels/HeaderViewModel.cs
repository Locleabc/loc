using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TopCom;
using TopCom.Command;
using TopCom.LOG;
using TopCom.MES;
using PLV_BracketAssemble.Define;
using TopCom.Mqtt;
using TopCom.Define;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class HeaderViewModel : PropertyChangedNotifier
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
                    CDef.MessageViewModel.ShowDialog((string)Application.Current.FindResource("str_PushExitButton"));

                    if (CDef.MessageViewModel.Result == false)
                    {
                        return;
                    }

                    CDef.MainViewModel.SwitchToTerminate();
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
