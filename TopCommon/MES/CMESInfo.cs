using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopCom.MES
{
    public class CMESDeviceInfo : PropertyChangedNotifier
    {
        #region Properties
        public string Device
        {
            get { return _Device; }
            set
            {
                if (_Device == value) return;

                _Device = value;
                OnPropertyChanged();
            }
        }

        public string Operation
        {
            get { return _Operation; }
            set
            {
                if (_Operation == value) return;

                _Operation = value;
                OnPropertyChanged();
            }
        }

        public string EQPName
        {
            get { return _EQPName; }
            set
            {
                if (_EQPName == value) return;

                _EQPName = value;
                OnPropertyChanged();
            }
        }

        public string ProductType
        {
            get { return _ProductType; }
            set
            {
                if (_ProductType == value) return;

                _ProductType = value;
                OnPropertyChanged();
            }
        }

        public string DeviceCode { get; set; }
        public string OperationCode { get; set; }
        public string EQPCode { get; set; }
        public string ProductTypeCode { get; set; }
        #endregion

        #region Privates
        // 공유메모리 변수 선언
        private string _Device;
        private string _Operation;
        private string _EQPName;
        private string _ProductType;
        #endregion
    }

    public class CMESInfo : PropertyChangedNotifier
    {
        #region Properties
        public CMESDeviceInfo Head1
        {
            get { return _Head1; }
            set
            {
                _Head1 = value;
                OnPropertyChanged();
            }
        }

        public CMESDeviceInfo Head2
        {
            get { return _Head2; }
            set
            {
                _Head2 = value;
                OnPropertyChanged();
            }
        }

        public bool UseMES
        {
            get { return _UseMES; }
            set
            {
                if (_UseMES == value) return;

                _UseMES = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        private static string BackupFile { get; set; }
        #endregion

        #region Constructors
        public CMESInfo()
            : this(@"D:\TOP\MESInfo.json")
        {
        }

        public CMESInfo(string _BackupFile)
        {
            BackupFile = _BackupFile;
        }
        #endregion

        #region Methods
        public static CMESInfo Load()
        {
            if (File.Exists(BackupFile) == false) return new CMESInfo();

            try
            {
                CMESInfo tmp = JsonConvert.DeserializeObject<CMESInfo>(File.ReadAllText(BackupFile));
                if (tmp == null) return new CMESInfo();
                else return tmp;
            }
            catch
            {
                return new CMESInfo();
            }
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(BackupFile));
            FileWriter.WriteAllText(BackupFile, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        #endregion

        #region Privates
        private bool _UseMES = true;
        private CMESDeviceInfo _Head1 = new CMESDeviceInfo();
        private CMESDeviceInfo _Head2 = new CMESDeviceInfo();
        #endregion
    }
}
