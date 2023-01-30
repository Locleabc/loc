using PLV_BracketAssemble.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TopCom;
using TopCom.Define;
using TopCom.LOG;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class TerminateViewModel : PropertyChangedNotifier
    {
        #region Properties
        public string TerminateStatus
        {
            get { return _TerminateStatus; }
            set
            {
                if (_TerminateStatus == value) return;

                _TerminateStatus = value;
                TerminateStatusDetail = "";
                OnPropertyChanged();
            }
        }

        public string TerminateStatusDetail
        {
            get { return _TerminateStatusDetail; }
            set
            {
                _TerminateStatusDetail = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async Task TerminateTask()
        {
            await Task.Run(() =>
            {
                UILog.Info("Shutting down program");
                TerminateStatus = "Terminate Started.";
                Thread.Sleep(1000);

                DisconnectCamera();
                Thread.Sleep(200);

                BackupData();
                Thread.Sleep(200);

                TerminateStatus = "Terminate Done.";
                Thread.Sleep(300);
            });
        }

        public async Task Terminate()
        {
            UILog.Info("Terminating program!");

            await TerminateTask();

            Application.Current.Shutdown((int)EExitCode.UserTerminatedAppication);
            UILog.Info("Program End!");
        }
        #endregion

        private void DisconnectCamera()
        {
            TerminateStatus = "Disconnecting Camera...";

            bool result = false;

            try
            {
                result = CDef.BotCamera.DisconnectAsync().Wait(5000);
            }
            catch (Exception ex)
            {
                UILog.Debug(ex.Message);
            }

            if (result)
            {
                TerminateStatusDetail = $"{CDef.BotCamera} Disconnected!";
            }
            else
            {
                TerminateStatusDetail = $"{CDef.BotCamera} Disconnect fail!";
            }
        }

        private void BackupData()
        {
            TerminateStatus = "Backup Data...";

            bool result = false;

            try
            {
                Datas.WorkData.Save();
                CDef.AllAxis.Save();
                CDef.MainViewModel.MainContentVM.RecipeVM.SaveRecipe();

                CDef.MainViewModel.MainContentVM.AutoVM.InputTrayVM.BackupTrayToFile();
                CDef.MainViewModel.MainContentVM.AutoVM.OutputTrayVM.BackupTrayToFile();
            }
            catch (Exception ex)
            {
                UILog.Debug(ex.Message);
            }

            if (result)
            {
                TerminateStatusDetail = $"Backup Data Successed!";
            }
            else
            {
                TerminateStatusDetail = $"Backup Data fail!";
            }
        }

        #region Privates
        private string _TerminateStatus;
        private string _TerminateStatusDetail;
        #endregion
    }
}
