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

namespace VCM_FullAssy.Define
{
    public interface IRecipe
    {
        void Save();
        T Load<T>();
    }

    public abstract class RecipeBase : PropertyChangedNotifier, IRecipe
    {
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
            string recipeFile, recipeFileName, recipeFileContent;

            /*COMMON RECIPE*/
            recipeFileName = this.GetType().Name + ".tda";

            recipeFile = Path.Combine(CDef.CurrentRecipeFolder, recipeFileName);
            recipeFileContent = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (!File.Exists(recipeFile))
            {
                using (StreamWriter sw = File.AppendText(recipeFile))
                {
                    sw.WriteLine(recipeFileContent);
                }
            }

            FileWriter.WriteAllText(recipeFile, recipeFileContent);
        }

        public T Load<T>()
        {
            return JsonConvert.DeserializeObject<T>(SerializeString);
        }
    }

    public class CGlobalRecipe : RecipeBase
    {
        #region Properties
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

        #region Selection Options
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


        #endregion

        #region Privates
        private int _SelectedCulture;

        private bool _SkipLoadVision;
        private bool _SkipUnloadVision;
        private bool _SkipUnderVision;

        private string _FileThatRunWithTheApplication = "";
        #endregion
    }

    public class CCommonRecipe : RecipeBase
    {
        #region Properti
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
        public bool NULL_SPACE_0 { get; set; }

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
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Vision To Picker X Offset", Detail = "Offset", Unit = Unit.mm)]
        public double VisionToPicker_XOffset
        {
            get { return _VisionToPicker_XOffset; }
            set
            {
                _VisionToPicker_XOffset = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Vision To Picker Y Offset", Detail = "Offset", Unit = Unit.mm)]
        public double VisionToPicker_YOffset
        {
            get { return _VisionToPicker_YOffset; }
            set
            {
                _VisionToPicker_YOffset = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "Axis home search timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double AxisHomeSearch_TimeOut
        {
            get { return _AxisHomeSearch_TimeOut; }
            set
            {
                _AxisHomeSearch_TimeOut = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Axis moving timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double AxisMoving_TimeOut
        {
            get { return _AxisMoving_TimeOut; }
            set
            {
                _AxisMoving_TimeOut = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_3 { get; set; }

        [RecipeDescription(Description = "Vacuum Delay", Detail = "Delay", Unit = Unit.Second)]
        public double Vacuum_Delay
        {
            get { return _Vacuum_Delay; }
            set
            {
                _Vacuum_Delay = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Purge Delay", Detail = "Delay", Unit = Unit.Second)]
        public double Purge_Delay
        {
            get { return _Purge_Delay; }
            set
            {
                _Purge_Delay = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_4 { get; set; }

        [RecipeDescription(Description = "Vision Grab_Timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double VisionGrab_Timeout
        {
            get { return _VisionGrab_Timeout; }
            set
            {
                _VisionGrab_Timeout = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Vision Inspect_Timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double VisionInspect_Timeout
        {
            get { return _VisionInspect_Timeout; }
            set
            {
                _VisionInspect_Timeout = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selection Options
        #endregion

        #region Privates
        private double _LoadingTray_X_Pitch;
        private double _LoadingTray_Y_Pitch;

        private double _UnloadingTray_X_Pitch;
        private double _UnloadingTray_Y_Pitch;


        private double _VisionToPicker_XOffset;
        private double _VisionToPicker_YOffset;

        private double _AxisHomeSearch_TimeOut = 60;
        private double _AxisMoving_TimeOut = 30;

        private double _Vacuum_Delay = 0.1;
        private double _Purge_Delay = 0.05;

        private double _VisionGrab_Timeout = 30;
        private double _VisionInspect_Timeout = 30;
        #endregion
    }

    public class CLeftTrayRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Y1 Axis Tray Change Position", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double TrayChange_Position
        {
            get { return _TrayChange_Position; }
            set
            {
                _TrayChange_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "Y1 Axis Front-Load-Tray First Position (Vision)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double FrontLoadTray_FirstPosition
        {
            get { return _FrontLoadTray_FirstPosition; }
            set
            {
                _FrontLoadTray_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y1 Axis Rear-Load-Tray First Position (Vision)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double RearLoadTray_FirstPosition
        {
            get { return _RearLoadTray_FirstPosition; }
            set
            {
                _RearLoadTray_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Y1 Axis Front-Unload-Tray First Position (Vision)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double FrontUnloadTray_FirstPosition
        {
            get { return _FrontUnloadTray_FirstPosition; }
            set
            {
                _FrontUnloadTray_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y1 Axis Rear-Unload-Tray First Position (Vision)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double RearUnloadTray_FirstPosition
        {
            get { return _RearUnloadTray_FirstPosition; }
            set
            {
                _RearUnloadTray_FirstPosition = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selection Options
        #endregion

        #region Privates
        private double _TrayChange_Position;
        private double _FrontLoadTray_FirstPosition;
        private double _RearLoadTray_FirstPosition;
        private double _FrontUnloadTray_FirstPosition;
        private double _RearUnloadTray_FirstPosition;
        #endregion
    }

    public class CRightTrayRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Y2 Axis Tray Change Position", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double TrayChange_Position
        {
            get { return _TrayChange_Position; }
            set
            {
                _TrayChange_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "Y2 Axis Front-Load-Tray First Position (Vision)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double FrontLoadTray_FirstPosition
        {
            get { return _FrontLoadTray_FirstPosition; }
            set
            {
                _FrontLoadTray_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y2 Axis Rear-Load-Tray First Position (Vision)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double RearLoadTray_FirstPosition
        {
            get { return _RearLoadTray_FirstPosition; }
            set
            {
                _RearLoadTray_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Y2 Axis Front-Unload-Tray First Position (Vision)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double FrontUnloadTray_FirstPosition
        {
            get { return _FrontUnloadTray_FirstPosition; }
            set
            {
                _FrontUnloadTray_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y2 Axis Rear-Unload-Tray First Position (Vision)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double RearUnloadTray_FirstPosition
        {
            get { return _RearUnloadTray_FirstPosition; }
            set
            {
                _RearUnloadTray_FirstPosition = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selection Options
        #endregion

        #region Privates
        private double _TrayChange_Position;
        private double _FrontLoadTray_FirstPosition;
        private double _RearLoadTray_FirstPosition;
        private double _FrontUnloadTray_FirstPosition;
        private double _RearUnloadTray_FirstPosition;
        #endregion
    }

    public class CTransferRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "X Axis Left-Front-Load Tray Work First Position (Vision)", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double LeftFrontLoadTrayWork_FirstPosition
        {
            get { return _LeftFrontLoadTrayWork_FirstPosition; }
            set
            {
                _LeftFrontLoadTrayWork_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "X Axis Left-Rear-Load Tray Work First Position (Vision)", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double LeftRearLoadTrayWork_FirstPosition
        {
            get { return _LeftRearLoadTrayWork_FirstPosition; }
            set
            {
                _LeftRearLoadTrayWork_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "X Axis Left-Front-Unload Tray Work First Position (Vision)", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double LeftFrontUnloadTrayWork_FirstPosition
        {
            get { return _LeftFrontUnloadTrayWork_FirstPosition; }
            set
            {
                _LeftFrontUnloadTrayWork_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "X Axis Left-Rear-Unload Tray Work First Position (Vision)", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double LeftRearUnloadTrayWork_FirstPosition
        {
            get { return _LeftRearUnloadTrayWork_FirstPosition; }
            set
            {
                _LeftRearUnloadTrayWork_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "X Axis Right-Front-Load Tray Work First Position (Vision)", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double RightFrontLoadTrayWork_FirstPosition
        {
            get { return _RightFrontLoadTrayWork_FirstPosition; }
            set
            {
                _RightFrontLoadTrayWork_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "X Axis Right-Rear-Load Tray Work First Position (Vision)", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double RightRearLoadTrayWork_FirstPosition
        {
            get { return _RightRearLoadTrayWork_FirstPosition; }
            set
            {
                _RightRearLoadTrayWork_FirstPosition = value;
                OnPropertyChanged();
            }
        }


        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "X Axis Right-Front-Unload Tray Work First Position(Vision)", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double RightFrontUnloadTrayWork_FirstPosition
        {
            get { return _RightFrontUnloadTrayWork_FirstPosition; }
            set
            {
                _RightFrontUnloadTrayWork_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "X Axis Right-Rear-Unload Tray Work First Position(Vision)", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double RightRearUnloadTrayWork_FirstPosition
        {
            get { return _RightRearUnloadTrayWork_FirstPosition; }
            set
            {
                _RightRearUnloadTrayWork_FirstPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_3 { get; set; }

        [RecipeDescription(Description = "X Axis Under Vision Work Position", Axis = AxisName.XAxis, Unit = Unit.mm)]
        public double UnderVisionWork_Position
        {
            get { return _UnderVisionWork_Position; }
            set
            {
                _UnderVisionWork_Position = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selected Option
        #endregion

        #region Privates
        private double _LeftFrontLoadTrayWork_FirstPosition;
        private double _LeftRearLoadTrayWork_FirstPosition;
        private double _LeftFrontUnloadTrayWork_FirstPosition;
        private double _LeftRearUnloadTrayWork_FirstPosition;
        private double _RightFrontLoadTrayWork_FirstPosition;
        private double _RightRearLoadTrayWork_FirstPosition;
        private double _RightFrontUnloadTrayWork_FirstPosition;
        private double _RightRearUnloadTrayWork_FirstPosition;
        private double _UnderVisionWork_Position;
        #endregion
    }

    public class CHeadRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Z Axis Ready Position", Axis = AxisName.ZAxis, Unit = Unit.mm)]
        public double ZAxisReady_Position
        {
            get { return _ZAxisReady_Position; }
            set
            {
                _ZAxisReady_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "Z Axis Pick Position", Axis = AxisName.ZAxis, Unit = Unit.mm)]
        public double ZAxisPick_Position
        {
            get { return _ZAxisPick_Position; }
            set
            {
                _ZAxisPick_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z Axis Pick Speed", Detail = "Speed", Unit = Unit.mmPerSecond)]
        public double ZAxisPick_Speed
        {
            get { return _ZAxisPick_Speed; }
            set
            {
                _ZAxisPick_Speed = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Z Axis Place Position", Axis = AxisName.ZAxis, Unit = Unit.mm)]
        public double ZAxisPlace_Position
        {
            get { return _ZAxisPlace_Position; }
            set
            {
                _ZAxisPlace_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z Axis Place Speed", Detail = "Speed", Unit = Unit.mmPerSecond)]
        public double ZAxisPlace_Speed
        {
            get { return _ZAxisPlace_Speed; }
            set
            {
                _ZAxisPlace_Speed = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "Z Axis Load Vision Position", Axis = AxisName.ZAxis, Unit = Unit.mm)]
        public double ZAxisLoadVision_Position
        {
            get { return _ZAxisLoadVision_Position; }
            set
            {
                _ZAxisLoadVision_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z Axis Under Vision Position", Axis = AxisName.ZAxis, Unit = Unit.mm)]
        public double ZAxisUnderVision_Position
        {
            get { return _ZAxisUnderVision_Position; }
            set
            {
                _ZAxisUnderVision_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z Axis Unload Vision Position", Axis = AxisName.ZAxis, Unit = Unit.mm)]
        public double ZAxisUnloadVision_Position
        {
            get { return _ZAxisUnloadVision_Position; }
            set
            {
                _ZAxisUnloadVision_Position = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selected Options
        #endregion

        #region Privates
        private double _ZAxisReady_Position;

        private double _ZAxisPick_Position;
        private double _ZAxisPick_Speed;

        private double _ZAxisPlace_Position;
        private double _ZAxisPlace_Speed;

        private double _ZAxisLoadVision_Position;
        private double _ZAxisUnderVision_Position;
        private double _ZAxisUnloadVision_Position;
        #endregion
    }

    public class CUpperVisionRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Load Vision Exposure Time", Detail = "Time", Unit = Unit.Percentage)]
        public double LoadVision_ExposureTime
        {
            get { return _LoadVision_ExposureTime; }
            set
            {
                _LoadVision_ExposureTime = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Unload Vision Exposure Time", Detail = "Time", Unit = Unit.Percentage)]
        public double UnloadVision_ExposureTime
        {
            get { return _UnloadVision_ExposureTime; }
            set
            {
                _UnloadVision_ExposureTime = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }


        [RecipeDescription(Description = "Load Vision Pixel Size", Detail = "Pixel Size", Unit = Unit.Micrometer)]
        public double LoadVision_PixelSize
        {
            get { return _LoadVision_PixelSize; }
            set
            {
                if (_LoadVision_PixelSize == value) return;

                _LoadVision_PixelSize = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Unload Vision Pixel Size", Detail = "Pixel Size", Unit = Unit.Micrometer)]
        public double UnloadVision_PixelSize
        {
            get { return _UnloadVision_PixelSize; }
            set
            {
                if (_UnloadVision_PixelSize == value) return;

                _UnloadVision_PixelSize = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Selected Options
        #endregion

        #region Privates
        private double _LoadVision_ExposureTime;
        private double _UnloadVision_ExposureTime;

        private double _LoadVision_PixelSize;
        private double _UnloadVision_PixelSize;

        #endregion
    }

    public class CUnderVisionRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Under Vision Exposure Time", Detail = "Time", Unit = Unit.Percentage)]
        public double UnderVision_ExposureTime
        {
            get { return _UnderVision_ExposureTime; }
            set
            {
                _UnderVision_ExposureTime = value;
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

        #endregion

        #region Selected Options
        #endregion

        #region Privates
        private double _UnderVision_ExposureTime;

        private double _UnderVision_PixelSize;
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
