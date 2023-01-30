using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace VCM_PickAndPlace.Define
{
    public class CWorkData : PropertyChangedNotifier
    {
        #region Properties
        [JsonIgnore]
        public CTaktTime TaktTime
        {
            get { return _TaktTime; }
            set
            {
                if (_TaktTime == value) return;

                _TaktTime = value;
                OnPropertyChanged();
            }
        }

        public CCountData CountData
        {
            get { return _CountData; }
            set
            {
                if (_CountData == value) return;

                _CountData = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public void Reset()
        {
            Save();
            CreateNextWorkDataFile();

            CountData = new CCountData();

            CountData.LoadVisionNG = new System.Collections.ObjectModel.ObservableCollection<uint> { 0, 0 };
            CountData.UnderVisionNG = new System.Collections.ObjectModel.ObservableCollection<uint> { 0, 0 };
            CountData.UnloadVisionNG = new System.Collections.ObjectModel.ObservableCollection<uint> { 0, 0 };

            TaktTime = new CTaktTime();
        }

        public void Load()
        {
            CWorkData loadedWorkData = new CWorkData();

            if (File.Exists(WorkDataFilePath) != false)
            {
                try
                {
                    loadedWorkData = JsonConvert.DeserializeObject<CWorkData>(File.ReadAllText(WorkDataFilePath));
                }
                catch { }
                finally
                {
                    if (loadedWorkData == null)
                    {
                        loadedWorkData = new CWorkData();
                    }
                }
            }

            CountData = loadedWorkData.CountData;
            if (CountData.LoadVisionNG == null || CountData.UnderVisionNG == null || CountData.UnloadVisionNG == null)
            {
                CountData.LoadVisionNG = new System.Collections.ObjectModel.ObservableCollection<uint> { 0, 0 };
                CountData.UnderVisionNG = new System.Collections.ObjectModel.ObservableCollection<uint> { 0, 0 };
                CountData.UnloadVisionNG = new System.Collections.ObjectModel.ObservableCollection<uint> { 0, 0 };
            }
        }

        public void Save()
        {
            FileWriter.WriteAllText(WorkDataFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
            return;
        }

        public void CreateNextWorkDataFile()
        {
            File.Create(DefaulWorkDataFile.Replace(".json", $"_{WorkDataFileIndex + 1}.json")).Close();
        }
        #endregion

        #region Privates
        // Basic format of work data back up file
        private string DefaulWorkDataFile
        {
            get
            {
                return Path.Combine(GlobalFolders.FolderEQCount, $"{DateTime.Now:yyyyMM}", $"Count_{DateTime.Now:yyyyMMdd}.json");
            }
        }
        private int WorkDataFileIndex
        {
            get
            {
                int i = 1;
                while (true)
                {
                    if (File.Exists(DefaulWorkDataFile.Replace(".json", $"_{i}.json")) == false)
                    {
                        break;
                    }
                    i++;
                }

                return i - 1;
            }
        }

        // Return lastest WorkDataFilePath
        private string WorkDataFilePath
        {
            get
            {
                // Create directory if work data back up file
                Directory.CreateDirectory(Path.Combine(GlobalFolders.FolderEQCount, $"{DateTime.Now:yyyyMM}"));

                string resultFile = DefaulWorkDataFile.Replace(".json", $"_{WorkDataFileIndex}.json");

                if (File.Exists(resultFile) == false)
                {
                    File.Create(resultFile).Close();
                }

                return resultFile;
            }
        }

        private CTaktTime _TaktTime = new CTaktTime();
        private CCountData _CountData = new CCountData();
        #endregion
    }
}
