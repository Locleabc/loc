using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TopCom;
using TopCom.Command;
using TopCom.LOG;
using TopCom.Models;
using TOPV_Dispenser.Define;

namespace TOPV_Dispenser.MVVM.ViewModels
{
    public class CHistory : PropertyChangedNotifier
    {
        #region Properties
        public ObservableCollection<CRecipeUpdateRecord> RecipeUpdateRecords
        {
            get { return _RecipeUpdateRecords; }
            set
            {
                if (_RecipeUpdateRecords == value) return;

                _RecipeUpdateRecords = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CEventRecord> AlarmRecords
        {
            get { return _AlarmRecords; }
            set
            {
                if (_AlarmRecords == value) return;

                _AlarmRecords = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CEventRecord> WarningRecords
        {
            get { return _WarningRecords; }
            set
            {
                if (_WarningRecords == value) return;

                _WarningRecords = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public void AddRecord<T>(ObservableCollection<T> recordList, T record)
        {
            // Prevent changed in different thread (not UI thread)
            // Changing this recordList will make UI changed
            try
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    recordList.Insert(0, record);

                    while (recordList.Count > MaxRecord)
                    {
                        recordList.RemoveAt(recordList.Count - 1);
                    }

                    for (int i = 0; i < recordList.Count; i++)
                    {
                        try
                        {
                            ((IIndexer)recordList[i]).Index = i + 1;
                        }
                        catch { }
                    }
                });

                // Save file after changed
                Save(recordList);
            }
            catch { }
        }

        public void Save(object obj)
        {
            try
            {
                string strRecords = JsonConvert.SerializeObject(obj, Formatting.Indented);
                if (obj == RecipeUpdateRecords)
                {
                    File.WriteAllText(RecipeUpdateHistoryFile, strRecords);
                }
                else if (obj == AlarmRecords)
                {
                    File.WriteAllText(AlarmEventHistoryFile, strRecords);
                }
                else if (obj == WarningRecords)
                {
                    File.WriteAllText(WarningEventHistoryFile, strRecords);
                }
            }
            catch (Exception ex)
            {
                UILog.Error(ex.Message);
            }
        }

        public void Load()
        {
            try
            {
                string strRecipeUpdateRecords = File.ReadAllText(RecipeUpdateHistoryFile);
                RecipeUpdateRecords = JsonConvert.DeserializeObject<ObservableCollection<CRecipeUpdateRecord>>(strRecipeUpdateRecords);
            }
            catch
            {
                UILog.Error($"Loading recipe update records from {RecipeUpdateHistoryFile} failed");
                RecipeUpdateRecords = new ObservableCollection<CRecipeUpdateRecord>();
            }

            try
            {
                string strAlarmRecords = File.ReadAllText(AlarmEventHistoryFile);
                AlarmRecords = JsonConvert.DeserializeObject<ObservableCollection<CEventRecord>>(strAlarmRecords);
            }
            catch
            {
                UILog.Error($"Loading alarm records from {AlarmEventHistoryFile} failed");
                AlarmRecords = new ObservableCollection<CEventRecord>();
            }

            try
            {
                string strWarningRecords = File.ReadAllText(WarningEventHistoryFile);
                WarningRecords = JsonConvert.DeserializeObject<ObservableCollection<CEventRecord>>(strWarningRecords);
            }
            catch
            {
                UILog.Error($"Loading warning records from {WarningEventHistoryFile} failed");
                WarningRecords = new ObservableCollection<CEventRecord>();
            }
        }
        #region Privates
        private const int MaxRecord = 200;

        private ObservableCollection<CRecipeUpdateRecord> _RecipeUpdateRecords = new ObservableCollection<CRecipeUpdateRecord>();
        private ObservableCollection<CEventRecord> _AlarmRecords = new ObservableCollection<CEventRecord>();
        private ObservableCollection<CEventRecord> _WarningRecords = new ObservableCollection<CEventRecord>();

        public string RecipeUpdateHistoryFile
        {
            get
            {
                return Path.Combine(GlobalFolders.FolderEQLogRecipeUpdate, "DataUpdate.json");
            }
        }
        public string AlarmEventHistoryFile
        {
            get
            {
                return Path.Combine(GlobalFolders.FolderEQLogRecipeUpdate, "Alarm.json");
            }
        }
        public string WarningEventHistoryFile
        {
            get
            {
                return Path.Combine(GlobalFolders.FolderEQLogRecipeUpdate, "Warning.json");
            }
        }

        #endregion
    }

    public class StatisticViewModel : PropertyChangedNotifier
    {
        public CHistory StatisticHistory
        {
            get { return _StatisticHistory; }
            set
            {
                if (_StatisticHistory == value) return;

                _StatisticHistory = value;
                OnPropertyChanged();
            }
        }


        public StatisticViewModel()
        {
            StatisticHistory.Load();
        }

        #region Commands
        #endregion

        #region Privates
        private CHistory _StatisticHistory = new CHistory();
        #endregion
    }
}
