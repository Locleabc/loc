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
using TOPV_Dispenser.Define;
using TopCom.Mqtt;
using TopCom.Define;

namespace TOPV_Dispenser.MVVM.ViewModels
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
                    CDef.MessageViewModel.ShowDialog((string)Application.Current.FindResource("str_PushExitButton"));

                    if (CDef.MessageViewModel.Result == false)
                    {
                        return;
                    }

                    try
                    {
                        string loadingTrays = JsonConvert.SerializeObject(CDef.LoadingTrays, Formatting.Indented);
                        string unloadingTrays = JsonConvert.SerializeObject(CDef.UnloadingTrays, Formatting.Indented);

                        FileWriter.WriteAllText("TrayLoadings.json", loadingTrays);
                        FileWriter.WriteAllText("TrayUnloadings.json", unloadingTrays);

                        Datas.WorkData.Save();

                        CDef.MainViewModel.RecipeVM.SaveRecipe();

                        CDef.TopCamera?.DisconnectAsync();
                        CDef.BotCamera?.DisconnectAsync();

                        CDef.AllAxis.Save();

                        //20211109 Off Light Camera, Ion after Exits Program
                        CDef.IO.Output.AllLightOff();
                        CDef.IO.Output.IonizerSol = false;

                        CDef.IO.Output.VacPurgeOff();

                        CDef.MES.Send_EquipStatus(EMESEqpStatus.DISCONNECT);

                        MqttGlobal.Client.Terminate();
                    }
                    catch (Exception ex)
                    {
                        CDef.MessageViewModel.ShowDialog($"Exception threw while application shutdown.\n\"{ex.Message}\"");
                        UILog.Error($"Exception threw while application shutdown.\n\"{ex.Message}\"");
                    }
                    finally
                    {
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
