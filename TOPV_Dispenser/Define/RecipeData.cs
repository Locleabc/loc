using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using TopCom;
using TopCom.Models;
using TopCom.LOG;

namespace TOPV_Dispenser.Define
{
    public interface IRecipe
    {
        void Save();
        T Load<T>();
    }

    public abstract class RecipeBase : PropertyChangedNotifier, IRecipe
    {
        private readonly object recipeReadWriteLock = new object(); 

        protected string SerializeString
        {
            get
            {
                string recipeFile, recipeFileName;

                recipeFileName = this.GetType().Name + ".tda";

                recipeFile = Path.Combine(CDef.CurrentRecipeFolder, recipeFileName);
                if (!File.Exists(recipeFile))
                {
                    using (StreamWriter sw = File.AppendText(recipeFile))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
                    }
                }

                return File.ReadAllText(recipeFile);
            }
        }

        public void Save()
        {
            lock (recipeReadWriteLock)
            {
                string recipeFile, recipeFileName, recipeFileContent;

                /*COMMON RECIPE*/
                recipeFileName = this.GetType().Name + ".tda";

                recipeFile = Path.Combine(CDef.CurrentRecipeFolder, recipeFileName);
                recipeFileContent = JsonConvert.SerializeObject(this, Formatting.Indented);

                File.WriteAllText(recipeFile, recipeFileContent);
            }
        }

        public T Load<T>()
        {
            lock (recipeReadWriteLock)
            {
                T tmp = JsonConvert.DeserializeObject<T>(SerializeString);

                if (tmp == null) UILog.Error($"File {SerializeString} does not exit or format error.\nCheck again!");

                return tmp;
            }
        }
    }

    public class CGlobalRecipe : RecipeBase
    {
        #region Properties
        public string FileThatRunWithTheApplication
        {
            get { return _FileThatRunWithTheApplication; }
            set
            {
                if (_FileThatRunWithTheApplication == value) return;

                _FileThatRunWithTheApplication = value;
                Save();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selected Options
        public bool SkipLoadVision
        {
            get { return _SkipLoadVision; }
            set
            {
                if (_SkipLoadVision == value) return;

                _SkipLoadVision = value;
                Save();
                OnPropertyChanged();
            }

        }

        public bool SkipUnloadVision
        {
            get { return _SkipUnloadVision; }
            set
            {
                if (_SkipUnloadVision == value) return;

                _SkipUnloadVision = value;
                Save();
                OnPropertyChanged();
            }

        }

        public bool SkipUnderVision
        {
            get { return _SkipUnderVision; }
            set
            {
                if (_SkipUnderVision == value) return;

                _SkipUnderVision = value;
                Save();
                OnPropertyChanged();
            }

        }

        public bool SkipBallInspect
        {
            get { return _SkipBallInpsect; }
            set
            {
                if (_SkipBallInpsect == value) return;

                _SkipBallInpsect = value;
                Save();
                OnPropertyChanged();
            }
        }

        public bool ImageSave
        {
            get { return _ImageSave; }
            set
            {
                if (_ImageSave == value) return;

                _ImageSave = value;
                Save();
                OnPropertyChanged();
            }
        }

        public bool SaveInputImage
        {
            get { return _SaveInputImage & ImageSave; }
            set
            {
                if (_SaveInputImage == value) return;

                _SaveInputImage = value;
                Save();
                OnPropertyChanged();
            }
        }

        public bool SaveProcessImage
        {
            get { return _SaveProcessImage & ImageSave; }
            set
            {
                if (_SaveProcessImage == value) return;

                _SaveProcessImage = value;
                Save();
                OnPropertyChanged();
            }
        }

        public bool SaveResultImage
        {
            get { return _SaveResultImage & ImageSave; }
            set
            {
                if (_SaveResultImage == value) return;

                _SaveResultImage = value;
                Save();
                OnPropertyChanged();
            }
        }

        public double ImageSaveDay
        {
            get { return _ImageSaveDay; }
            set
            {
                if (_ImageSaveDay == value) return;

                _ImageSaveDay = value;
                Save();
                OnPropertyChanged();
            }
        }

        public bool UseVacuumCheck
        {
            get { return _UseVacuumCheck; }
            set
            {
                if (_UseVacuumCheck == value) return;

                _UseVacuumCheck = value;
                Save();
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool UseMES
        {
            get { return CDef.MES.MESInfo.UseMES; }
            set
            {
                CDef.MES.MESInfo.UseMES = value;
                CDef.MES.MESInfo.Save();
            }
        }

        public int SelectedCulture
        {
            get { return _SelectedCulture; }
            set
            {
                if (_SelectedCulture == value) return;

                _SelectedCulture = value;
                if (_SelectedCulture < 0 || _SelectedCulture > CDef.Cultures.Count - 1) _SelectedCulture = 0;

                App.SelectCulture(CDef.Cultures[_SelectedCulture].Id);

                Save();
                OnPropertyChanged();
            }
        }

        public bool DoubleUnderVisionCheck
        {
            get { return _DoubleUnderVisionCheck; }
            set
            {
                if (_DoubleUnderVisionCheck == value) return;

                _DoubleUnderVisionCheck = value;
                Save();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Place Touch Check function
        private bool _UsePlaceTouchCheck = false;
        private double _PlaceTouchAllowGap = 0.5;
        private double _PlaceTouchCheckLimit = 0.1;

        public bool UsePlaceTouchCheck
        {
            get { return _UsePlaceTouchCheck; }
            set
            {
                if (_UsePlaceTouchCheck == value) return;

                _UsePlaceTouchCheck = value;
                Save();
                OnPropertyChanged();
            }
        }

        public double PlaceTouchAllowGap
        {
            get { return _PlaceTouchAllowGap; }
            set
            {
                if (_PlaceTouchAllowGap == value) return;

                _PlaceTouchAllowGap = value;
                Save();
                OnPropertyChanged();
            }
        }


        public double PlaceTouchCheckLimit
        {
            get { return _PlaceTouchCheckLimit; }
            set
            {
                if (_PlaceTouchCheckLimit == value) return;

                _PlaceTouchCheckLimit = value;
                Save();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private bool _SkipLoadVision;
        private bool _SkipUnloadVision;
        private bool _SkipUnderVision;

        private bool _SkipBallInpsect;

        private bool _ImageSave = true;
        private bool _SaveInputImage = true;
        private bool _SaveProcessImage = false;
        private bool _SaveResultImage = false;

        private double _ImageSaveDay = 10;

        private bool _UseVacuumCheck = true;
        private string _FileThatRunWithTheApplication = @"D:\AGENT\LinkAgent.exe";

        private int _SelectedCulture = -1;

        private bool _DoubleUnderVisionCheck;
        #endregion
    }

    public class CCommonRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Loading Tray X Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double LoadingTray_X_Pitch
        {
            get { return _LoadingTray_X_Pitch; }
            set
            {
                if (_LoadingTray_X_Pitch == value) return;

                _LoadingTray_X_Pitch = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Loading Tray Y Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double LoadingTray_Y_Pitch
        {
            get { return _LoadingTray_Y_Pitch; }
            set
            {
                if (_LoadingTray_Y_Pitch == value) return;

                _LoadingTray_Y_Pitch = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Unloading Tray X Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double UnloadingTray_X_Pitch
        {
            get { return _UnloadingTray_X_Pitch; }
            set
            {
                if (_UnloadingTray_X_Pitch == value) return;

                _UnloadingTray_X_Pitch = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Unloading Tray Y Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double UnloadingTray_Y_Pitch
        {
            get { return _UnloadingTray_Y_Pitch; }
            set
            {
                if (_UnloadingTray_Y_Pitch == value) return;

                _UnloadingTray_Y_Pitch = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "Vacuum Delay", Detail = "Sol Valve", Unit = Unit.Second)]
        public double VAC_Delay
        {
            get { return _VAC_Delay; }
            set
            {
                if (_VAC_Delay == value) return;

                _VAC_Delay = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Purge Delay", Detail = "Sol Valve", Unit = Unit.Second)]
        public double Purge_Delay
        {
            get { return _Purge_Delay; }
            set
            {
                if (_Purge_Delay == value) return;

                _Purge_Delay = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_3 { get; set; }

        [RecipeDescription(Description = "Loading Tray 1 to Tray 2 X Offset", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double LoadingTray_Offset_X
        {
            get { return _LoadingTray_Offset_X; }
            set
            {
                if (_LoadingTray_Offset_X == value) return;

                _LoadingTray_Offset_X = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Unloading Tray 1 to Tray 2 X Offset", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double UnloadingTray_Offset_X
        {
            get { return _UnloadingTray_Offset_X; }
            set
            {
                if (_UnloadingTray_Offset_X == value) return;

                _UnloadingTray_Offset_X = value;
                OnPropertyChanged();

            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_4 { get; set; }

        [RecipeDescription(Description = "Pick/ Place Timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double PickPlace_TimeOut
        {
            get { return _PickPlace_TimeOut; }
            set
            {
                if (_PickPlace_TimeOut == value) return;

                _PickPlace_TimeOut = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Picker Vision Avoid Timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double PickerVisionAvoid_TimeOut
        {
            get { return _PickerVisionAvoid_TimeOut; }
            set
            {
                if (_PickerVisionAvoid_TimeOut == value) return;

                _PickerVisionAvoid_TimeOut = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Vision Inspect Timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double VisionInspect_TimeOut
        {
            get { return _VisionInspect_TimeOut; }
            set
            {
                if (_VisionInspect_TimeOut == value) return;

                _VisionInspect_TimeOut = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Axis Home Search TimeOut", Detail = "Timeout", Unit = Unit.Second)]
        public double AxisHomeSearch_TimeOut
        {
            get { return _AxisHomeSearch_TimeOut; }
            set
            {
                if (_AxisHomeSearch_TimeOut == value) return;

                _AxisHomeSearch_TimeOut = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Axis Moving Timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double AxisMoving_TimeOut
        {
            get { return _AxisMoving_TimeOut; }
            set
            {
                if (_AxisMoving_TimeOut == value) return;

                _AxisMoving_TimeOut = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_5 { get; set; }

        [RecipeDescription(Description = "Light Comport number", Detail = "Number", Unit = Unit.ETC)]
        public uint LightComport
        {
            get { return _LightComport; }
            set
            {
                if (_LightComport == value) return;

                _LightComport = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selection Options
        #endregion

        #region Privates
        private double _LoadingTray_X_Pitch = 10;
        private double _LoadingTray_Y_Pitch = 10;
        private double _UnloadingTray_X_Pitch = 10;
        private double _UnloadingTray_Y_Pitch = 10;
        private double _VAC_Delay = 0.05;
        private double _Purge_Delay = 0.05;
        private double _LoadingTray_Offset_X;
        private double _UnloadingTray_Offset_X;
        private double _PickPlace_TimeOut = 30;
        private double _PickerVisionAvoid_TimeOut = 30;
        private double _VisionInspect_TimeOut = 30;
        private double _AxisHomeSearch_TimeOut = 30;
        private double _AxisMoving_TimeOut = 30;

        private uint _LightComport = 1;
        #endregion
    }

    public static class RecipeHelper
    {
        public static Dictionary<string, RecipeDescriptionAttribute> GetDescription<T>(T Recipe, string Property)
        {
            Dictionary<string, RecipeDescriptionAttribute> _dict = new Dictionary<string, RecipeDescriptionAttribute>();

            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    RecipeDescriptionAttribute authAttr = attr as RecipeDescriptionAttribute;
                    if (authAttr != null)
                    {
                        string propName = prop.Name;

                        if (Property == prop.Name)
                        {
                            _dict.Add(propName, authAttr);
                        }
                    }
                }
            }

            return _dict;
        }

        public static Dictionary<string, RecipeDescriptionAttribute> GetDescriptions<T>()
        {
            Dictionary<string, RecipeDescriptionAttribute> _dict = new Dictionary<string, RecipeDescriptionAttribute>();

            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    RecipeDescriptionAttribute authAttr = attr as RecipeDescriptionAttribute;
                    if (authAttr != null)
                    {
                        string propName = prop.Name;

                        _dict.Add(propName, authAttr);
                    }
                }
            }

            return _dict;
        }
    }
}
