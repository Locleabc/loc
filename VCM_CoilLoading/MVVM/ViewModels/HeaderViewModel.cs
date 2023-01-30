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
using TopCom.Define;
using TopCom.LOG;
using TopCom.MES;
using VCM_CoilLoading.Define;

namespace VCM_CoilLoading.MVVM.ViewModels
{
    public class HeaderViewModel : PropertyChangedNotifier
    {
        #region Properties
        public MESViewModel MESVM
        {
            get
            {
                return _MESVM ?? (_MESVM = new MESViewModel());
            }
        }

        public string MachineStatus
        {
            get { return _MachineStatus; }
            set
            {
                _MachineStatus = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand ExitCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.MessageViewModel.ShowDialog("Do you want to exit?");
                    //MessageBoxResult result = MessageBox.Show("Do you want to exit?", "Confirm", MessageBoxButton.YesNo);

                    if (CDef.MessageViewModel.Result == true)
                    {
                        string loadingTray = JsonConvert.SerializeObject(CDef.LoadingTray, Formatting.Indented);
                        string unloadingTray1 = JsonConvert.SerializeObject(CDef.UnloadingTray1, Formatting.Indented);
                        string unloadingTray2 = JsonConvert.SerializeObject(CDef.UnloadingTray2, Formatting.Indented);

                        FileWriter.WriteAllText("TrayLoading.json", loadingTray);
                        FileWriter.WriteAllText("TrayUnloading1.json", unloadingTray1);
                        FileWriter.WriteAllText("TrayUnloading2.json", unloadingTray2);

                        Datas.WorkData.Save();

                        CDef.MainViewModel.RecipeVM.SaveRecipe();

                        //20211109 Off Light Camera,Ion when Exits Program
                        CDef.IO.Output.LightUpper = false;
                        CDef.IO.Output.LightUnder = false;
                        CDef.IO.Output.IonizerSol = false;

                        CDef.TopCamera?.DisconnectAsync();
                        CDef.BotCamera?.DisconnectAsync();

                        CDef.MES.Send_EquipStatus(EMESEqpStatus.DISCONNECT);
                        UILog.Info("Program End!");

                        // Send ExitCode 100 for preventing re-call ExitCommand
                        Application.Current.Shutdown((int)EExitCode.UserTerminatedAppication);
                    }
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
        private MESViewModel _MESVM;
        private string _MachineStatus;
        #endregion
    }
}
