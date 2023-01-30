using Newtonsoft.Json;
using System.Windows;
using TopCom;
using TopCom.Define;
using TopCom.Command;
using TopCom.LOG;
using TopCom.MES;
using VCM_FullAssy.Define;

namespace VCM_FullAssy.MVVM.ViewModels
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
                        string loadTray1 = JsonConvert.SerializeObject(CTray.LoadTray1, Formatting.Indented);
                        string loadTray2 = JsonConvert.SerializeObject(CTray.LoadTray2, Formatting.Indented);
                        string loadTray3 = JsonConvert.SerializeObject(CTray.LoadTray3, Formatting.Indented);
                        string loadTray4 = JsonConvert.SerializeObject(CTray.LoadTray4, Formatting.Indented);
                        string unloadTray1 = JsonConvert.SerializeObject(CTray.UnloadTray1, Formatting.Indented);
                        string unloadTray2 = JsonConvert.SerializeObject(CTray.UnloadTray2, Formatting.Indented);
                        string unloadTray3 = JsonConvert.SerializeObject(CTray.UnloadTray3, Formatting.Indented);
                        string unloadTray4 = JsonConvert.SerializeObject(CTray.UnloadTray4, Formatting.Indented);

                        FileWriter.WriteAllText("LoadTray1.json", loadTray1);
                        FileWriter.WriteAllText("LoadTray2.json", loadTray2);
                        FileWriter.WriteAllText("LoadTray3.json", loadTray3);
                        FileWriter.WriteAllText("LoadTray4.json", loadTray4);
                        FileWriter.WriteAllText("UnloadTray1.json", unloadTray1);
                        FileWriter.WriteAllText("UnloadTray2.json", unloadTray2);
                        FileWriter.WriteAllText("UnloadTray3.json", unloadTray3);
                        FileWriter.WriteAllText("UnloadTray4.json", unloadTray4);

                        Datas.WorkData.Save();

                        CDef.MainViewModel.RecipeVM.SaveRecipe();

                        CDef.TopCamera?.DisconnectAsync();
                        CDef.BotCamera?.DisconnectAsync();

                        //20211109 Off Light Camera when Exits Program
                        CDef.IO.Output.LightUpper = false;
                        CDef.IO.Output.LightUnder = false;

                        CDef.MES.Send_EquipStatus(EMESEqpStatus.DISCONNECT);

                        // Send ExitCode 100 for preventing re-call ExitCommand
                        Application.Current.Shutdown((int)EExitCode.UserTerminatedAppication);
                        UILog.Info("Program End!");
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
