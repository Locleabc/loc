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

namespace VCM_PickAndPlace.Define
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

#if USPCUTTING
    public class CPressRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Press Axis Ready Position", Axis = AxisName.PAxis, Unit = Unit.mm)]
        public double PAxis_ReadyPosition
        {
            get { return _PAxis_ReadyPosition; }
            set
            {
                if (_PAxis_ReadyPosition == value) return;

                _PAxis_ReadyPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Press Axis Press 1 Position", Axis = AxisName.PAxis, Unit = Unit.mm)]
        public double PAxis_Press1_Position
        {
            get { return _PAxis_Press1_Position; }
            set
            {
                if (_PAxis_Press1_Position == value) return;

                _PAxis_Press1_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Press Axis Press 2 Position", Axis = AxisName.PAxis, Unit = Unit.mm)]
        public double PAxis_Press2_Position
        {
            get { return _PAxis_Press2_Position; }
            set
            {
                if (_PAxis_Press2_Position == value) return;

                _PAxis_Press2_Position = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _PAxis_ReadyPosition;
        private double _PAxis_Press1_Position;
        private double _PAxis_Press2_Position;
        #endregion
    }
#endif

    public class CTrayRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Load Tray Change Position", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double Y1Axis_Change_Position
        {
            get { return _Y1Axis_Change_Position; }
            set
            {
                if (_Y1Axis_Change_Position == value) return;

                _Y1Axis_Change_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Load Tray #1 Work Position (Align, Pick)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double Y1Axis_Tray1_Work_Position
        {
            get { return _Y1Axis_Tray1_Work_Position; }
            set
            {
                if (_Y1Axis_Tray1_Work_Position == value) return;

                _Y1Axis_Tray1_Work_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Load Tray #2 Work Position (Align, Pick)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double Y1Axis_Tray2_Work_Position
        {
            get { return _Y1Axis_Tray2_Work_Position; }
            set
            {
                if (_Y1Axis_Tray2_Work_Position == value) return;

                _Y1Axis_Tray2_Work_Position = value;
                OnPropertyChanged();
            }
        }

#if USPCUTTING
        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "Tray #1 Press Position", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double Y1Axis_Tray1_Press_Position
        {
            get { return _Y1Axis_Tray1_Press_Position; }
            set
            {
                if (_Y1Axis_Tray1_Press_Position == value) return;

                _Y1Axis_Tray1_Press_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Tray #2 Press Position", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double Y1Axis_Tray2_Press_Position
        {
            get { return _Y1Axis_Tray2_Press_Position; }
            set
            {
                if (_Y1Axis_Tray2_Press_Position == value) return;

                _Y1Axis_Tray2_Press_Position = value;
                OnPropertyChanged();
            }
        }
#endif

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Unload Tray Change Position", Axis = AxisName.Y2Axis, Unit = Unit.mm)]
        public double Y2Axis_Change_Position
        {
            get { return _Y2Axis_Change_Position; }
            set
            {
                if (_Y2Axis_Change_Position == value) return;

                _Y2Axis_Change_Position = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Tray work position (align vision, place, inspect)
        /// </summary>
        [RecipeDescription(Description = "Unload Tray #1 Work Position (Align, Place, Inspect)", Axis = AxisName.Y2Axis, Unit = Unit.mm)]
        public double Y2Axis_Tray1_Work_Position
        {
            get { return _Y2Axis_Tray1_Work_Position; }
            set
            {
                if (_Y2Axis_Tray1_Work_Position == value) return;

                _Y2Axis_Tray1_Work_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Unload Tray #2 Work Position (Align, Place, Inspect)", Axis = AxisName.Y2Axis, Unit = Unit.mm)]
        public double Y2Axis_Tray2_Work_Position
        {
            get { return _Y2Axis_Tray2_Work_Position; }
            set
            {
                if (_Y2Axis_Tray2_Work_Position == value) return;

                _Y2Axis_Tray2_Work_Position = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _Y1Axis_Change_Position;
        private double _Y1Axis_Tray1_Work_Position;
        private double _Y1Axis_Tray2_Work_Position;

#if USPCUTTING
        private double _Y1Axis_Tray1_Press_Position;
        private double _Y1Axis_Tray2_Press_Position;
#endif

        private double _Y2Axis_Change_Position;
        private double _Y2Axis_Tray1_Work_Position;
        private double _Y2Axis_Tray2_Work_Position;
#endregion
    }

    public class CTransferRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "X1 Axis Load Vision Avoid Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1Axis_LoadVS_Avoid_Position
        {
            get { return _X1Axis_LoadVS_Avoid_Position; }
            set
            {
                if (_X1Axis_LoadVS_Avoid_Position == value) return;

                _X1Axis_LoadVS_Avoid_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "X1 Axis UnLoad Vision Avoid Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1Axis_UnloadVS_Avoid_Position
        {
            get { return _X1Axis_UnloadVS_Avoid_Position; }
            set
            {
                if (_X1Axis_UnloadVS_Avoid_Position == value) return;

                _X1Axis_UnloadVS_Avoid_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "X1 Axis Pick Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1Axis_Pick_Position
        {
            get { return _X1Axis_Pick_Position; }
            set
            {
                if (_X1Axis_Pick_Position == value) return;

                _X1Axis_Pick_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "XX Axis Pick Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XXAxis_Pick_Position
        {
            get { return _XXAxis_Pick_Position; }
            set
            {
                if (_XXAxis_Pick_Position == value) return;

                _XXAxis_Pick_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "X1 Axis Under Vision Head 1 Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1Axis_UnderVS1_Position
        {
            get { return _X1Axis_UnderVS1_Position; }
            set
            {
                if (_X1Axis_UnderVS1_Position == value) return;

                _X1Axis_UnderVS1_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "X1 Axis Under Vision Head 2 Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1Axis_UnderVS2_Position
        {
            get { return _X1Axis_UnderVS2_Position; }
            set
            {
                if (_X1Axis_UnderVS2_Position == value) return;

                _X1Axis_UnderVS2_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "XX Axis Under Vision Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XXAxis_UnderVision_Position
        {
            get { return _XXAxis_UnderVision_Position; }
            set
            {
                if (_XXAxis_UnderVision_Position == value) return;

                _XXAxis_UnderVision_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "X1 Axis Place Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1Axis_Place_Position
        {
            get { return _X1Axis_Place_Position; }
            set
            {
                if (_X1Axis_Place_Position == value) return;

                _X1Axis_Place_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "XX Axis Place Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XXAxis_Place_Position
        {
            get { return _XXAxis_Place_Position; }
            set
            {
                if (_XXAxis_Place_Position == value) return;

                _XXAxis_Place_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_3 { get; set; }
        [RecipeDescription(Description = "X1 Axis Inspect Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1Axis_Inspect_Position
        {
            get { return _X1Axis_Inspect_Position; }
            set
            {
                if (_X1Axis_Inspect_Position == value) return;

                _X1Axis_Inspect_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "XX Axis Inspect Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XXAxis_Inspect_Position
        {
            get { return _XXAxis_Inspect_Position; }
            set
            {
                if (_XXAxis_Inspect_Position == value) return;

                _XXAxis_Inspect_Position = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _X1Axis_LoadVS_Avoid_Position;
        private double _X1Axis_UnloadVS_Avoid_Position;

        private double _X1Axis_Pick_Position;
        private double _XXAxis_Pick_Position;

        private double _X1Axis_UnderVS1_Position;
        private double _X1Axis_UnderVS2_Position;
        private double _XXAxis_UnderVision_Position;

        private double _X1Axis_Place_Position;
        private double _XXAxis_Place_Position;

        private double _X1Axis_Inspect_Position;
        private double _XXAxis_Inspect_Position;
        #endregion

    }

    public class CHeadRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Z1 Axis-HEAD1 Pick Position", Axis = AxisName.Z1Axis, Unit = Unit.mm)]
        public double Z1Axis_Pick_Position
        {
            get { return _Z1Axis_Pick_Position; }
            set
            {
                if (_Z1Axis_Pick_Position == value) return;

                _Z1Axis_Pick_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY1 Axis-HEAD1 Pick Position", Axis = AxisName.YY1Axis, Unit = Unit.mm)]
        public double YY1Axis_Pick_Position
        {
            get { return _YY1Axis_Pick_Position; }
            set
            {
                if (_YY1Axis_Pick_Position == value) return;

                _YY1Axis_Pick_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T1 Axis-HEAD1 Pick Vision Position", Axis = AxisName.T1Axis, Unit = Unit.mm)]
        public double T1Axis_Pick_Position
        {
            get { return _T1Axis_Pick_Position; }
            set
            {
                if (_T1Axis_Pick_Position == value) return;

                _T1Axis_Pick_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "Z2 Axis-HEAD2 Pick Position", Axis = AxisName.Z2Axis, Unit = Unit.mm)]
        public double Z2Axis_Pick_Position
        {
            get { return _Z2Axis_Pick_Position; }
            set
            {
                if (_Z2Axis_Pick_Position == value) return;

                _Z2Axis_Pick_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY2 Axis-HEAD2 Pick Position", Axis = AxisName.YY2Axis, Unit = Unit.mm)]
        public double YY2Axis_Pick_Position
        {
            get { return _YY2Axis_Pick_Position; }
            set
            {
                if (_YY2Axis_Pick_Position == value) return;

                _YY2Axis_Pick_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T2 Axis-HEAD2 Pick Position", Axis = AxisName.T2Axis, Unit = Unit.mm)]
        public double T2Axis_Pick_Position
        {
            get { return _T2Axis_Pick_Position; }
            set
            {
                if (_T2Axis_Pick_Position == value) return;

                _T2Axis_Pick_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Z1 Axis-HEAD1 Under Vision Position", Axis = AxisName.Z1Axis, Unit = Unit.mm)]
        public double Z1Axis_UnderVS_Position
        {
            get { return _Z1Axis_UnderVS_Position; }
            set
            {
                if (_Z1Axis_UnderVS_Position == value) return;

                _Z1Axis_UnderVS_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY1 Axis-HEAD1 Under Vision Position", Axis = AxisName.YY1Axis, Unit = Unit.mm)]
        public double YY1Axis_UnderVS_Position
        {
            get { return _YY1Axis_UnderVS_Position; }
            set
            {
                if (_YY1Axis_UnderVS_Position == value) return;

                _YY1Axis_UnderVS_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T1 Axis-HEAD1 Under Vision Position", Axis = AxisName.T1Axis, Unit = Unit.mm)]
        public double T1Axis_UnderVS_Position
        {
            get { return _T1Axis_UnderVS_Position; }
            set
            {
                if (_T1Axis_UnderVS_Position == value) return;

                _T1Axis_UnderVS_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "Z2 Axis-HEAD2 Under Vision Position", Axis = AxisName.Z2Axis, Unit = Unit.mm)]
        public double Z2Axis_UnderVS_Position
        {
            get { return _Z2Axis_UnderVS_Position; }
            set
            {
                if (_Z2Axis_UnderVS_Position == value) return;

                _Z2Axis_UnderVS_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY2 Axis-HEAD2 Under Vision Position", Axis = AxisName.YY2Axis, Unit = Unit.mm)]
        public double YY2Axis_UnderVS_Position
        {
            get { return _YY2Axis_UnderVS_Position; }
            set
            {
                if (_YY2Axis_UnderVS_Position == value) return;

                _YY2Axis_UnderVS_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T2 Axis-HEAD2 Under Vision Position", Axis = AxisName.T2Axis, Unit = Unit.mm)]
        public double T2Axis_UnderVS_Position
        {
            get { return _T2Axis_UnderVS_Position; }
            set
            {
                if (_T2Axis_UnderVS_Position == value) return;

                _T2Axis_UnderVS_Position = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public bool NULL_SPACE_3 { get; set; }

        [RecipeDescription(Description = "Z1 Axis-HEAD1 Place Position", Axis = AxisName.Z1Axis, Unit = Unit.mm)]
        public double Z1Axis_Place_Position
        {
            get { return _Z1Axis_Place_Position; }
            set
            {
                if (_Z1Axis_Place_Position == value) return;

                _Z1Axis_Place_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY1 Axis-HEAD1 Place Position", Axis = AxisName.YY1Axis, Unit = Unit.mm)]
        public double YY1Axis_Place_Position
        {
            get { return _YY1Axis_Place_Position; }
            set
            {
                if (_YY1Axis_Place_Position == value) return;

                _YY1Axis_Place_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T1 Axis-HEAD1 Place Position", Axis = AxisName.T1Axis, Unit = Unit.mm)]
        public double T1Axis_Place_Position
        {
            get { return _T1Axis_Place_Position; }
            set
            {
                if (_T1Axis_Place_Position == value) return;

                _T1Axis_Place_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_4 { get; set; }

        [RecipeDescription(Description = "Z2 Axis-HEAD2 Place Position", Axis = AxisName.Z2Axis, Unit = Unit.mm)]
        public double Z2Axis_Place_Position
        {
            get { return _Z2Axis_Place_Position; }
            set
            {
                if (_Z2Axis_Place_Position == value) return;

                _Z2Axis_Place_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY2 Axis-HEAD2 Place Position", Axis = AxisName.YY2Axis, Unit = Unit.mm)]
        public double YY2Axis_Place_Position
        {
            get { return _YY2Axis_Place_Position; }
            set
            {
                if (_YY2Axis_Place_Position == value) return;

                _YY2Axis_Place_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T2 Axis-HEAD2 Place Position", Axis = AxisName.T2Axis, Unit = Unit.mm)]
        public double T2Axis_Place_Position
        {
            get { return _T2Axis_Place_Position; }
            set
            {
                if (_T2Axis_Place_Position == value) return;

                _T2Axis_Place_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_5 { get; set; }

        [RecipeDescription(Description = "Z1 Axis-HEAD1 Ready Position", Axis = AxisName.Z1Axis, Unit = Unit.mm)]
        public double Z1Axis_Ready_Position
        {
            get { return _Z1Axis_Ready_Position; }
            set
            {
                if (_Z1Axis_Ready_Position == value) return;

                _Z1Axis_Ready_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z2 Axis-HEAD2 Ready Position", Axis = AxisName.Z2Axis, Unit = Unit.mm)]
        public double Z2Axis_Ready_Position
        {
            get { return _Z2Axis_Ready_Position; }
            set
            {
                if (_Z2Axis_Ready_Position == value) return;

                _Z2Axis_Ready_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_6 { get; set; }
        [RecipeDescription(Description = "YYAxis place override distance", Detail = "Distance", Unit = Unit.mm)]
        public double YYAxis_Place_OverrideDistance
        {
            get { return _YYAxis_Place_OverrideDistance; }
            set
            {
                if (_YYAxis_Place_OverrideDistance == value) return;

                _YYAxis_Place_OverrideDistance = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "ZAxis pick/place override height", Detail = "Distance", Unit = Unit.mm)]
        public double ZAxis_PickPlace_OverrideHeight
        {
            get { return _ZAxis_PickPlace_OverrideHeight; }
            set
            {
                if (_ZAxis_PickPlace_OverrideHeight == value) return;

                _ZAxis_PickPlace_OverrideHeight = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "ZAxis pick/place override position moving speed rate", Detail = "Rate", Unit = Unit.Percentage)]
        public double ZAxis_PickPlace_OverrideSpeed
        {
            get { return _ZAxis_PickPlace_OverrideSpeed; }
            set
            {
                if (_ZAxis_PickPlace_OverrideSpeed == value) return;

                _ZAxis_PickPlace_OverrideSpeed = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _Z1Axis_Pick_Position;
        private double _YY1Axis_Pick_Position;
        private double _T1Axis_Pick_Position;

        private double _Z2Axis_Pick_Position;
        private double _YY2Axis_Pick_Position;
        private double _T2Axis_Pick_Position;

        private double _Z1Axis_Place_Position;
        private double _YY1Axis_Place_Position;
        private double _T1Axis_Place_Position;

        private double _Z2Axis_Place_Position;
        private double _YY2Axis_Place_Position;
        private double _T2Axis_Place_Position;

        private double _Z1Axis_UnderVS_Position;
        private double _YY1Axis_UnderVS_Position;
        private double _T1Axis_UnderVS_Position;

        private double _Z2Axis_UnderVS_Position;
        private double _YY2Axis_UnderVS_Position;
        private double _T2Axis_UnderVS_Position;

        private double _Z1Axis_Ready_Position;
        private double _Z2Axis_Ready_Position;
        
        private double _YYAxis_Place_OverrideDistance = 0.3;
        private double _ZAxis_PickPlace_OverrideHeight = 5;
        private double _ZAxis_PickPlace_OverrideSpeed = 50;
        #endregion
    }

    public class CUpperVisionRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Z3 Axis Ready Position", Axis = AxisName.Z3Axis, Unit = Unit.mm)]
        public double Z3Axis_Ready_Position
        {
            get { return _Z3Axis_Ready_Position; }
            set
            {
                if (_Z3Axis_Ready_Position == value) return;

                _Z3Axis_Ready_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_0 { get; set; }

        [RecipeDescription(Description = "X2 Axis Load Vision Position", Axis = AxisName.X2Axis, Unit = Unit.mm)]
        public double X2Axis_LoadVision_Position
        {
            get { return _X2Axis_LoadVision_Position; }
            set
            {
                if (_X2Axis_LoadVision_Position == value) return;

                _X2Axis_LoadVision_Position = value;
                OnPropertyChanged();
            }
        }


        [RecipeDescription(Description = "Z3 Axis Load Vision Position", Axis = AxisName.Z3Axis, Unit = Unit.mm)]
        public double Z3Axis_LoadVision_Position
        {
            get { return _Z3Axis_LoadVision_Position; }
            set
            {
                if (_Z3Axis_LoadVision_Position == value) return;

                _Z3Axis_LoadVision_Position = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "X2 Axis Unload Vision Position (+ Inspect)", Axis = AxisName.X2Axis, Unit = Unit.mm)]
        public double X2Axis_UnLoadVision_Position
        {
            get { return _X2Axis_UnLoadVision_Position; }
            set
            {
                if (_X2Axis_UnLoadVision_Position == value) return;

                _X2Axis_UnLoadVision_Position = value;
                OnPropertyChanged();
            }
        }


        [RecipeDescription(Description = "Z3 Axis Unload Vision Position", Axis = AxisName.Z3Axis, Unit = Unit.mm)]
        public double Z3Axis_UnLoadVision_Position
        {
            get { return _Z3Axis_UnLoadVision_Position; }
            set
            {
                if (_Z3Axis_UnLoadVision_Position == value) return;

                _Z3Axis_UnLoadVision_Position = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z3 Axis Inspect Vision Position", Axis = AxisName.Z3Axis, Unit = Unit.mm)]
        public double Z3Axis_InspectVision_Position
        {
            get { return _Z3Axis_InspectVision_Position; }
            set
            {
                if (_Z3Axis_InspectVision_Position == value) return;

                _Z3Axis_InspectVision_Position = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "Upper Camera Exposure Time", Detail = "Time", Unit = Unit.MicroSecond)]
        public double UpperCamera_ExposureTime
        {
            get { return _UpperCamera_ExposureTime; }
            set
            {
                if (_UpperCamera_ExposureTime == value) return;

                _UpperCamera_ExposureTime = value;
                try
                {
                    if (CDef.TopCamera != null)
                    {
                        CDef.TopCamera.ExposureTime = value;
                    }
                } catch { }
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Load Vision Light Level", Detail = "Number", Unit = Unit.ETC)]
        public int LoadVision_LightLevel
        {
            get { return _LoadVision_LightLevel; }
            set
            {
                if (_LoadVision_LightLevel == value) return;

                _LoadVision_LightLevel = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Unload Vision Light Level", Detail = "Number", Unit = Unit.ETC)]
        public int UnloadVision_LightLevel
        {
            get { return _UnloadVision_LightLevel; }
            set
            {
                if (_UnloadVision_LightLevel == value) return;

                _UnloadVision_LightLevel = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Inspect Light Level", Detail = "Number", Unit = Unit.ETC)]
        public int Inspect_LightLevel
        {
            get { return _Inspect_LightLevel; }
            set
            {
                if (_Inspect_LightLevel == value) return;

                _Inspect_LightLevel = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]

        public bool NULL_SPACE_3 { get; set; }

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
        private double _Z3Axis_Ready_Position;
        private double _X2Axis_LoadVision_Position;

        private double _X2Axis_UnLoadVision_Position;

        private double _Z3Axis_LoadVision_Position;
        private double _Z3Axis_UnLoadVision_Position;
        private double _Z3Axis_InspectVision_Position;

        private double _UpperCamera_ExposureTime = 5000;

        private int _LoadVision_LightLevel = 100;
        private int _UnloadVision_LightLevel = 100;
        private int _Inspect_LightLevel = 100;

        private double _LoadVision_PixelSize = 5;
        private double _UnloadVision_PixelSize = 5;
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

        #endregion

        #region Selected Options
        #endregion

        #region Privates
        private double _UnderCamera_ExposureTime = 5000;
        private int _UnderVision_LightLevel = 100;
        private double _UnderVision_PixelSize = 5;
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
