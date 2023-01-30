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

namespace PLV_BracketAssemble.Define
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
        #endregion

        #region Selected Options
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

        public bool SkipPreAlign
        {
            get { return _SkipPreAlign; }
            set
            {
                if (_SkipPreAlign == value) return;

                _SkipPreAlign = value;
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
        #endregion

        #region Privates
        private bool _SkipUnderVision;
        private bool _SkipPreAlign;
        private bool _ImageSave = true;
        private bool _SaveInputImage = true;
        private bool _SaveProcessImage = false;
        private bool _SaveResultImage = false;

        private double _ImageSaveDay = 10;

        private bool _UseVacuumCheck = true;
        #endregion
    }

    public class CCommonRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Input Tray X Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double InputTray_X_Pitch
        {
            get { return _InputTray_X_Pitch; }
            set
            {
                if (_InputTray_X_Pitch == value) return;

                _InputTray_X_Pitch = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Input Tray Y Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double InputTray_Y_Pitch
        {
            get { return _InputTray_Y_Pitch; }
            set
            {
                if (_InputTray_Y_Pitch == value) return;

                _InputTray_Y_Pitch = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Output Tray X Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double OutputTray_X_Pitch
        {
            get { return _OutputTray_X_Pitch; }
            set
            {
                if (_OutputTray_X_Pitch == value) return;

                _OutputTray_X_Pitch = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Output Tray Y Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double OutputTray_Y_Pitch
        {
            get { return _OutputTray_Y_Pitch; }
            set
            {
                if (_OutputTray_Y_Pitch == value) return;

                _OutputTray_Y_Pitch = value;
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
        #endregion

        #region Selection Options
        #endregion

        #region Privates
        private double _InputTray_X_Pitch = 10;
        private double _InputTray_Y_Pitch = 10;
        private double _OutputTray_X_Pitch = 10;
        private double _OutputTray_Y_Pitch = 10;
        private double _VAC_Delay = 0.05;
        private double _Purge_Delay = 0.05;
        private double _PickPlace_TimeOut = 30;
        private double _VisionInspect_TimeOut = 30;
        private double _AxisHomeSearch_TimeOut = 30;
        private double _AxisMoving_TimeOut = 30;
        #endregion
    }

    public class CTrayRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Change Position", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double XAxis_Change_Position
        {
            get { return _XAxis_Change_Position; }
            set
            {
                if (_XAxis_Change_Position == value) return;

                _XAxis_Change_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "Tray Picker #1 Pick First Position", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double XAxis_Tray_Picker1_Pick_First_Position
        {
            get { return _XAxis_Tray_Picker1_Pick_First_Position; }
            set
            {
                if (_XAxis_Tray_Picker1_Pick_First_Position == value) return;

                _XAxis_Tray_Picker1_Pick_First_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Tray Picker #1 Place First Position", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double XAxis_Tray_Picker1_Place_First_Position
        {
            get { return _XAxis_Tray_Picker1_Place_First_Position; }
            set
            {
                if (_XAxis_Tray_Picker1_Place_First_Position == value) return;

                _XAxis_Tray_Picker1_Place_First_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Tray Picker #2 Pick First Position", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double XAxis_Tray_Picker2_Pick_First_Position
        {
            get { return _XAxis_Tray_Picker2_Pick_First_Position; }
            set
            {
                if (_XAxis_Tray_Picker2_Pick_First_Position == value) return;

                _XAxis_Tray_Picker2_Pick_First_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Tray Picker #2 Place First Position", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double XAxis_Tray_Picker2_Place_First_Position
        {
            get { return _XAxis_Tray_Picker2_Place_First_Position; }
            set
            {
                if (_XAxis_Tray_Picker2_Place_First_Position == value) return;

                _XAxis_Tray_Picker2_Place_First_Position = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _XAxis_Change_Position;

        private double _XAxis_Tray_Picker1_Pick_First_Position;
        private double _XAxis_Tray_Picker1_Place_First_Position;
        
        private double _XAxis_Tray_Picker2_Pick_First_Position;
        private double _XAxis_Tray_Picker2_Place_First_Position;
        #endregion
    }

    public class CHeadRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "XX Axis Change Position", Axis = AxisName.YAxis, Unit = Unit.mm)]
        public double XXAxis_Change_Position
        {
            get { return _XXAxis_Change_Position; }
            set
            {
                if (_XXAxis_Change_Position == value) return;

                _XXAxis_Change_Position = value;
                OnPropertyChanged();
            }
        }
        [RecipeDescription(Description = "Y Axis Change Position", Axis = AxisName.YAxis, Unit = Unit.mm)]
        public double YAxis_Change_Position
        {
            get { return _YAxis_Change_Position; }
            set
            {
                if (_YAxis_Change_Position == value) return;

                _YAxis_Change_Position = value;
                OnPropertyChanged();
            }
        }
        
        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "XX Axis Pre Align Clamp Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XXAxis_PreAlign_Position
        {
            get { return _XXAxis_PreAlign_Position; }
            set
            {
                if (_XXAxis_PreAlign_Position == value) return;

                _XXAxis_PreAlign_Position = value;
                OnPropertyChanged();
            }
        }
        [RecipeDescription(Description = "Y Axis Pre Align Clamp Position", Axis = AxisName.YAxis, Unit = Unit.mm)]
        public double YAxis_PreAlign_Position
        {
            get { return _YAxis_PreAlign_Position; }
            set
            {
                if (_YAxis_PreAlign_Position == value) return;

                _YAxis_PreAlign_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "XX Axis Pick First Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XXAxis_Pick_First_Position
        {
            get { return _XXAxis_Pick_First_Position; }
            set
            {
                if (_XXAxis_Pick_First_Position == value) return;

                _XXAxis_Pick_First_Position = value;
                OnPropertyChanged();
            }
        }
        [RecipeDescription(Description = "XX Axis Picker #1 Under Vision Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XXAxis_Picker1_UnderVision_Position
        {
            get { return _XXAxis_Picker1_UnderVision_Position; }
            set
            {
                if (_XXAxis_Picker1_UnderVision_Position == value) return;

                _XXAxis_Picker1_UnderVision_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "XX Axis Picker #2 Under Vision Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XXAxis_Picker2_UnderVision_Position
        {
            get { return _XXAxis_Picker2_UnderVision_Position; }
            set
            {
                if (_XXAxis_Picker2_UnderVision_Position == value) return;

                _XXAxis_Picker2_UnderVision_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "XX Axis Place First Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XXAxis_Place_First_Position
        {
            get { return _XXAxis_Place_First_Position; }
            set
            {
                if (_XXAxis_Place_First_Position == value) return;

                _XXAxis_Place_First_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "Y Axis Picker #1 Pick First Position", Axis = AxisName.YAxis, Unit = Unit.mm)]
        public double YAxis_Picker1_Pick_First_Position
        {
            get { return _YAxis_Picker1_Pick_First_Position; }
            set
            {
                if (_YAxis_Picker1_Pick_First_Position == value) return;

                _YAxis_Picker1_Pick_First_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y Axis Picker #1 Under Vision Position", Axis = AxisName.YAxis, Unit = Unit.mm)]
        public double YAxis_Picker1_UnderVision_Position
        {
            get { return _YAxis_Picker1_UnderVision_Position; }
            set
            {
                if (_YAxis_Picker1_UnderVision_Position == value) return;

                _YAxis_Picker1_UnderVision_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y Axis Picker #1 Place First Position", Axis = AxisName.YAxis, Unit = Unit.mm)]
        public double YAxis_Picker1_Place_First_Position
        {
            get { return _YAxis_Picker1_Place_First_Position; }
            set
            {
                if (_YAxis_Picker1_Place_First_Position == value) return;

                _YAxis_Picker1_Place_First_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_3 { get; set; }

        [RecipeDescription(Description = "Y Axis Picker #2 Pick First Position", Axis = AxisName.YAxis, Unit = Unit.mm)]
        public double YAxis_Picker2_Pick_First_Position
        {
            get { return _YAxis_Picker2_Pick_First_Position; }
            set
            {
                if (_YAxis_Picker2_Pick_First_Position == value) return;

                _YAxis_Picker2_Pick_First_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y Axis Picker #2 Under Vision Position", Axis = AxisName.YAxis, Unit = Unit.mm)]
        public double YAxis_Picker2_UnderVision_Position
        {
            get { return _YAxis_Picker2_UnderVision_Position; }
            set
            {
                if (_YAxis_Picker2_UnderVision_Position == value) return;

                _YAxis_Picker2_UnderVision_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y Axis Picker #2 Place First Position", Axis = AxisName.YAxis, Unit = Unit.mm)]
        public double YAxis_Picker2_Place_First_Position
        {
            get { return _YAxis_Picker2_Place_First_Position; }
            set
            {
                if (_YAxis_Picker2_Place_First_Position == value) return;

                _YAxis_Picker2_Place_First_Position = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _YAxis_Change_Position;
        private double _XXAxis_Change_Position;

        private double _YAxis_PreAlign_Position;
        private double _XXAxis_PreAlign_Position;

        private double _XXAxis_Pick_First_Position;
        private double _XXAxis_Place_First_Position;


        private double _YAxis_Picker1_Pick_First_Position;
        private double _YAxis_Picker1_UnderVision_Position;
        private double _YAxis_Picker1_Place_First_Position;

        private double _XXAxis_Picker1_UnderVision_Position;
        private double _XXAxis_Picker2_UnderVision_Position;

        private double _YAxis_Picker2_Pick_First_Position;
        private double _YAxis_Picker2_UnderVision_Position;
        private double _YAxis_Picker2_Place_First_Position;


        #endregion
    }

    public class CUnderVisionRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Under Camera Exposure Time", Detail = "Time", Unit = Unit.MicroSecond)]
        public double UnderCamera_ExposureTime
        {
            get { return _UnderCamera_ExposureTime; }
            set
            {
                if (_UnderCamera_ExposureTime == value) return;

                _UnderCamera_ExposureTime = value;

                try
                {
                    if (CDef.BotCamera != null)
                    {
                        CDef.BotCamera.ExposureTime = value;
                    }
                }
                catch { }
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Under Vision Light Level", Detail = "Number", Unit = Unit.ETC)]
        public int UnderVision_LightLevel
        {
            get { return _UnderVision_LightLevel; }
            set
            {
                if (_UnderVision_LightLevel == value) return;

                if (CDef.LightController != null)
                {
                    CDef.LightController.SetLightLevel(1, value);
                }
                _UnderVision_LightLevel = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "Under Vision Pixel Size", Detail = "Pixel Size", Unit = Unit.Micrometer)]
        public double UnderVision_PixelSize
        {
            get { return _UnderVision_PixelSize; }
            set
            {
                if (_UnderVision_PixelSize == value) return;

                _UnderVision_PixelSize = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Under Vision Retry Count", Detail = "Count", Unit = Unit.ETC)]
        public int RetryCount
        {
            get { return _RetryCount; }
            set
            {
                if (_RetryCount == value) return;

                _RetryCount = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

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

        #region Selected Options
        #endregion

        #region Privates
        private double _UnderCamera_ExposureTime = 5000;
        private int _UnderVision_LightLevel = 100;

        private double _UnderVision_PixelSize = 5;

        private int _RetryCount = 2;

        private uint _LightComport = 3;
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
