using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using TopCom;
using TopCom.Models;

namespace VCM_CoilLoading.Define
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

        [RecipeDescription(Description = "Loading Tray Head 2 Additional Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double LoadingTray_Head2_Additional_Pitch
        {
            get { return _LoadingTray_Head2_Additional_Pitch; }
            set
            {
                if (_LoadingTray_Head2_Additional_Pitch == value) return;

                _LoadingTray_Head2_Additional_Pitch = value;
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

        [RecipeDescription(Description = "Unloading Tray Head 2 Additional Pitch", Detail = "Pitch", Unit = Unit.mm)]
        public double UnloadingTray_Head2_Additional_Pitch
        {
            get { return _UnloadingTray_Head2_Additional_Pitch; }
            set
            {
                if (_UnloadingTray_Head2_Additional_Pitch == value) return;

                _UnloadingTray_Head2_Additional_Pitch = value;
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

        [JsonIgnore]
        public bool NULL_SPACE_4 { get; set; }

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
        public bool NULL_SPACE_5 { get; set; }

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

        [RecipeDescription(Description = "Servo Home Search Timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double ServoHomeSearch_TimeOut
        {
            get { return _ServoHomeSearch_TimeOut; }
            set
            {
                if (_ServoHomeSearch_TimeOut == value) return;

                _ServoHomeSearch_TimeOut = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Servo Moving Timeout", Detail = "Timeout", Unit = Unit.Second)]
        public double ServoMoving_TimeOut
        {
            get { return _ServoMoving_TimeOut; }
            set
            {
                if (_ServoMoving_TimeOut == value) return;

                _ServoMoving_TimeOut = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_6 { get; set; }

        [RecipeDescription(Description = "Load Tray X Count", Detail = "Count", Unit = Unit.EA)]
        public double LoadTray_XCount
        {
            get { return _LoadTray_XCount; }
            set
            {
                if (_LoadTray_XCount == value) return;

                _LoadTray_XCount = value;
                OnPropertyChanged();

                File.Delete("TrayLoading.json");
                if (CDef.MainViewModel.InitVM.InitCompleted) CDef.MainViewModel.AutoVM.InitAllTray();
            }
        }

        [RecipeDescription(Description = "Load Tray Y Count", Detail = "Count", Unit = Unit.EA)]
        public double LoadTray_YCount
        {
            get { return _LoadTray_YCount; }
            set
            {
                if (_LoadTray_YCount == value) return;

                _LoadTray_YCount = value;
                OnPropertyChanged();

                File.Delete("TrayLoading.json");
                if (CDef.MainViewModel.InitVM.InitCompleted) CDef.MainViewModel.AutoVM.InitAllTray();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_7 { get; set; }

        [RecipeDescription(Description = "Unload Tray X Count", Detail = "Count", Unit = Unit.EA)]
        public double UnloadTray_XCount
        {
            get { return _UnloadTray_XCount; }
            set
            {
                if (_UnloadTray_XCount == value) return;

                _UnloadTray_XCount = value;
                OnPropertyChanged();

                File.Delete("TrayUnloading1.json");
                File.Delete("TrayUnloading2.json");
                if (CDef.MainViewModel.InitVM.InitCompleted) CDef.MainViewModel.AutoVM.InitAllTray();
            }
        }

        [RecipeDescription(Description = "Unload Tray Y Count", Detail = "Count", Unit = Unit.EA)]
        public double UnloadTray_YCount
        {
            get { return _UnloadTray_YCount; }
            set
            {
                if (_UnloadTray_YCount == value) return;

                _UnloadTray_YCount = value;
                OnPropertyChanged();

                File.Delete("TrayUnloading1.json");
                File.Delete("TrayUnloading2.json");
                if (CDef.MainViewModel.InitVM.InitCompleted) CDef.MainViewModel.AutoVM.InitAllTray();
            }
        }
        #endregion

        #region Selection Options
        #endregion

        #region Privates
        private double _LoadingTray_X_Pitch;
        private double _LoadingTray_Y_Pitch;
        private double _LoadingTray_Head2_Additional_Pitch;
        private double _UnloadingTray_X_Pitch;
        private double _UnloadingTray_Y_Pitch;
        private double _UnloadingTray_Head2_Additional_Pitch;
        private double _VAC_Delay;
        private double _Purge_Delay;
        private double _LoadingTray_Offset_X;
        private double _UnloadingTray_Offset_X;
        private double _PickPlace_TimeOut;
        private double _PickerVisionAvoid_TimeOut;
        private double _VisionInspect_TimeOut;
        private double _ServoHomeSearch_TimeOut;
        private double _ServoMoving_TimeOut;

        private double _LoadTray_XCount = 10;
        private double _LoadTray_YCount = 10;
        private double _UnloadTray_XCount = 6;
        private double _UnloadTray_YCount = 5;
        #endregion
    }

    public class CPickerRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "Y1 Axis Loading Position", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double Y1_LoadPosition
        {
            get { return _Y1_LoadPosition; }
            set
            {
                if (_Y1_LoadPosition == value) return;

                _Y1_LoadPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y1 Axis Working Position (Tray 1)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double Y1_WorkPosition_Tray1
        {
            get { return _Y1_WorkPosition_Tray1; }
            set
            {
                if (_Y1_WorkPosition_Tray1 == value) return;

                _Y1_WorkPosition_Tray1 = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y1 Axis Working Position (Tray 2)", Axis = AxisName.Y1Axis, Unit = Unit.mm)]
        public double Y1_WorkPosition_Tray2
        {
            get { return _Y1_WorkPosition_Tray2; }
            set
            {
                if (Y1_WorkPosition_Tray2 == value) return;

                _Y1_WorkPosition_Tray2 = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Y2 Axis Loading Position", Axis = AxisName.Y2Axis, Unit = Unit.mm)]
        public double Y2_LoadPosition
        {
            get { return _Y2_LoadPosition; }
            set
            {
                if (_Y2_LoadPosition == value) return;

                _Y2_LoadPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y2 Axis Working Position (Tray 1)", Axis = AxisName.Y2Axis, Unit = Unit.mm)]
        public double Y2_WorkPosition_Tray1
        {
            get { return _Y2_WorkPosition_Tray1; }
            set
            {
                if (_Y2_WorkPosition_Tray1 == value) return;

                _Y2_WorkPosition_Tray1 = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Y2 Axis Working Position (Tray 2)", Axis = AxisName.Y2Axis, Unit = Unit.mm)]
        public double Y2_WorkPosition_Tray2
        {
            get { return _Y2_WorkPosition_Tray2; }
            set
            {
                if (Y2_WorkPosition_Tray2 == value) return;

                _Y2_WorkPosition_Tray2 = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "Z1 Axis Waiting Position", Axis = AxisName.Z1Axis, Unit = Unit.mm)]
        public double Z1_WaitPosition
        {
            get { return _Z1_WaitPosition; }
            set
            {
                if (_Z1_WaitPosition == value) return;

                _Z1_WaitPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z2 Axis Waiting Position", Axis = AxisName.Z2Axis, Unit = Unit.mm)]
        public double Z2_WaitPosition
        {
            get { return _Z2_WaitPosition; }
            set
            {
                if (_Z2_WaitPosition == value) return;

                _Z2_WaitPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_3 { get; set; }

        [RecipeDescription(Description = "X1 Axis Picking Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1_PickPosition
        {
            get { return _X1_PickPosition; }
            set
            {
                if (_X1_PickPosition == value) return;

                _X1_PickPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "XX Axis Picking Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XX_PickPosition
        {
            get { return _XX_PickPosition; }
            set
            {
                if (_XX_PickPosition == value) return;

                _XX_PickPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY1 Axis Picking Position", Axis = AxisName.YY1Axis, Unit = Unit.mm)]
        public double YY1_PickPosition
        {
            get { return _YY1_PickPosition; }
            set
            {
                if (_YY1_PickPosition == value) return;

                _YY1_PickPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY2 Axis Picking Position", Axis = AxisName.YY2Axis, Unit = Unit.mm)]
        public double YY2_PickPosition
        {
            get { return _YY2_PickPosition; }
            set
            {
                if (_YY2_PickPosition == value) return;
                _YY2_PickPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T1 Axis Picking Position", Axis = AxisName.T1Axis, Unit = Unit.mm)]
        public double T1_PickPosition
        {
            get { return _T1_PickPosition; }
            set
            {
                if (_T1_PickPosition == value) return;
                _T1_PickPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T2 Axis Picking Position", Axis = AxisName.T2Axis, Unit = Unit.mm)]
        public double T2_PickPosition
        {
            get { return _T2_PickPosition; }
            set
            {
                if (_T2_PickPosition == value) return;

                _T2_PickPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z1 Axis Picking Position", Axis = AxisName.Z1Axis, Unit = Unit.mm)]
        public double Z1_PickPosition
        {
            get { return _Z1_PickPosition; }
            set
            {
                if (_Z1_PickPosition == value) return;
                _Z1_PickPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z2 Axis Picking Position", Axis = AxisName.Z2Axis, Unit = Unit.mm)]
        public double Z2_PickPosition
        {
            get { return _Z2_PickPosition; }
            set
            {
                if (_Z2_PickPosition == value) return;

                _Z2_PickPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z Axis Picking Speed", Detail = "Speed", Unit = Unit.mmPerSecond)]
        public double Z_PickSpeed
        {
            get { return _Z_PickSpeed; }
            set
            {
                if (_Z_PickSpeed == value) return;

                _Z_PickSpeed = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_4 { get; set; }

        [RecipeDescription(Description = "X1 Axis Place Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1_PlacePosition
        {
            get { return _X1_PlacePosition; }
            set
            {
                if (_X1_PlacePosition == value) return;

                _X1_PlacePosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "XX Axis Place Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XX_PlacePosition
        {
            get { return _XX_PlacePosition; }
            set
            {
                if (_XX_PlacePosition == value) return;

                _XX_PlacePosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY1 Axis Place Position", Axis = AxisName.YY1Axis, Unit = Unit.mm)]
        public double YY1_PlacePosition
        {
            get { return _YY1_PlacePosition; }
            set
            {
                if (_YY1_PlacePosition == value) return;

                _YY1_PlacePosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY2 Axis Place Position", Axis = AxisName.YY2Axis, Unit = Unit.mm)]
        public double YY2_PlacePosition
        {
            get { return _YY2_PlacePosition; }
            set
            {
                if (_YY2_PlacePosition == value) return;

                _YY2_PlacePosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T1 Axis Place Position", Axis = AxisName.T1Axis, Unit = Unit.mm)]
        public double T1_PlacePosition
        {
            get { return _T1_PlacePosition; }
            set
            {
                if (_T1_PlacePosition == value) return;

                _T1_PlacePosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T2 Axis Place Position", Axis = AxisName.T2Axis, Unit = Unit.mm)]
        public double T2_PlacePosition
        {
            get { return _T2_PlacePosition; }
            set
            {
                if (_T2_PlacePosition == value) return;

                _T2_PlacePosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z1 Axis Place Position", Axis = AxisName.Z1Axis, Unit = Unit.mm)]
        public double Z1_PlacePosition
        {
            get { return _Z1_PlacePosition; }
            set
            {
                if (_Z1_PlacePosition == value) return;

                _Z1_PlacePosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z2 Axis Place Position", Axis = AxisName.Z2Axis, Unit = Unit.mm)]
        public double Z2_PlacePosition
        {
            get { return _Z2_PlacePosition; }
            set
            {
                if (_Z2_PlacePosition == value) return;

                _Z2_PlacePosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z Axis Placing Speed", Detail = "Speed", Unit = Unit.mmPerSecond)]
        public double Z_PlaceSpeed
        {
            get { return _Z_PlaceSpeed; }
            set
            {
                if (_Z_PlaceSpeed == value) return;

                _Z_PlaceSpeed = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _Y1_LoadPosition;
        private double _Y1_WorkPosition_Tray1;
        private double _Y1_WorkPosition_Tray2;
        private double _Y2_LoadPosition;
        private double _Y2_WorkPosition_Tray1;
        private double _Y2_WorkPosition_Tray2;
        private double _Z1_WaitPosition;
        private double _Z2_WaitPosition;
        private double _X1_PickPosition;
        private double _XX_PickPosition;
        private double _YY1_PickPosition;
        private double _YY2_PickPosition;
        private double _T1_PickPosition;
        private double _T2_PickPosition;
        private double _Z1_PickPosition;
        private double _Z2_PickPosition;
        private double _Z_PickSpeed;
        private double _X1_PlacePosition;
        private double _XX_PlacePosition;
        private double _YY1_PlacePosition;
        private double _YY2_PlacePosition;
        private double _T1_PlacePosition;
        private double _T2_PlacePosition;
        private double _Z1_PlacePosition;
        private double _Z2_PlacePosition;
        private double _Z_PlaceSpeed;
        #endregion
    }

    public class CUnderVisionRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "X1 Axis Under Vision Position HEAD 1", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1_UnderVisionPosition_Head1
        {
            get { return _X1_UnderVisionPosition_Head1; }
            set
            {
                if (_X1_UnderVisionPosition_Head1 == value) return;

                _X1_UnderVisionPosition_Head1 = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "X1 Axis Under Vision Position HEAD 2", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1_UnderVisionPosition_Head2
        {
            get { return _X1_UnderVisionPosition_Head2; }
            set
            {
                if (_X1_UnderVisionPosition_Head2 == value) return;

                _X1_UnderVisionPosition_Head2 = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "XX Axis Under Vision Position", Axis = AxisName.XXAxis, Unit = Unit.mm)]
        public double XX_UnderVisionPosition
        {
            get { return _XX_UnderVisionPosition; }
            set
            {
                if (_XX_UnderVisionPosition == value) return;

                _XX_UnderVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY1 Axis Under Vision Position", Axis = AxisName.YY1Axis, Unit = Unit.mm)]
        public double YY1_UnderVisionPosition
        {
            get { return _YY1_UnderVisionPosition; }
            set
            {
                if (_YY1_UnderVisionPosition == value) return;

                _YY1_UnderVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "YY2 Axis Under Vision Position", Axis = AxisName.YY2Axis, Unit = Unit.mm)]
        public double YY2_UnderVisionPosition
        {
            get { return _YY2_UnderVisionPosition; }
            set
            {
                if (_YY2_UnderVisionPosition == value) return;

                _YY2_UnderVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T1 Axis Under Vision Position", Axis = AxisName.T1Axis, Unit = Unit.mm)]
        public double T1_UnderVisionPosition
        {
            get { return _T1_UnderVisionPosition; }
            set
            {
                if (_T1_UnderVisionPosition == value) return;
                _T1_UnderVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "T2 Axis Under Vision Position", Axis = AxisName.T2Axis, Unit = Unit.mm)]
        public double T2_UnderVisionPosition
        {
            get { return _T2_UnderVisionPosition; }
            set
            {
                if (_T2_UnderVisionPosition == value) return;

                _T2_UnderVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "Z1 Axis Under Vision Position", Axis = AxisName.Z1Axis, Unit = Unit.mm)]
        public double Z1_UnderVisionPosition
        {
            get { return _Z1_UnderVisionPosition; }
            set
            {
                if (_Z1_UnderVisionPosition == value) return;

                _Z1_UnderVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z2 Axis Under Vision Position", Axis = AxisName.Z2Axis, Unit = Unit.mm)]
        public double Z2_UnderVisionPosition
        {
            get { return _Z2_UnderVisionPosition; }
            set
            {
                if (_Z2_UnderVisionPosition == value) return;

                _Z2_UnderVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

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
        public bool NULL_SPACE_3 { get; set; }

        [RecipeDescription(Description = "Under Vision Exposure Time", Detail = "Time", Unit = Unit.Percentage)]
        public double UnderVision_ExposureTime
        {
            get { return _UnderVision_ExposureTime; }
            set
            {
                if (_UnderVision_ExposureTime == value) return;

                _UnderVision_ExposureTime = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _X1_UnderVisionPosition_Head1;
        private double _X1_UnderVisionPosition_Head2;
        private double _XX_UnderVisionPosition;
        private double _YY1_UnderVisionPosition;
        private double _YY2_UnderVisionPosition;
        private double _T1_UnderVisionPosition;
        private double _T2_UnderVisionPosition;
        private double _Z1_UnderVisionPosition;
        private double _Z2_UnderVisionPosition;
        private double _UnderVision_PixelSize = 6.012;
        private double _UnderVision_ExposureTime;
        #endregion

        #region Selection Options
        #endregion
    }

    public class CUpperVisionRecipe : RecipeBase
    {
        #region Properties
        [RecipeDescription(Description = "X2 Axis Loading Vision Position HEAD 2", Axis = AxisName.X2Axis, Unit = Unit.mm)]
        public double X2_LoadingVisionPosition
        {
            get { return _X2_LoadingVisionPosition; }
            set
            {
                if (_X2_LoadingVisionPosition == value) return;

                _X2_LoadingVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z3 Axis Loading Vision Position", Axis = AxisName.Z3Axis, Unit = Unit.mm)]
        public double Z3_LoadingVisionPosition
        {
            get { return _Z3_LoadingVisionPosition; }
            set
            {
                if (_Z3_LoadingVisionPosition == value) return;

                _Z3_LoadingVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_1 { get; set; }

        [RecipeDescription(Description = "X2 Axis Unloading Vision Position HEAD 2", Axis = AxisName.X2Axis, Unit = Unit.mm)]
        public double X2_UnloadingVisionPosition
        {
            get { return _X2_UnloadingVisionPosition; }
            set
            {
                if (_X2_UnloadingVisionPosition == value) return;

                _X2_UnloadingVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Z3 Axis Unloading Vision Position", Axis = AxisName.Z3Axis, Unit = Unit.mm)]
        public double Z3_UnloadingVisionPosition
        {
            get { return _Z3_UnloadingVisionPosition; }
            set
            {
                if (_Z3_UnloadingVisionPosition == value) return;

                _Z3_UnloadingVisionPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_2 { get; set; }

        [RecipeDescription(Description = "X1 Axis Loading Vision Avoid Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1_LoadingVision_AvoidPosition
        {
            get { return _X1_LoadingVision_AvoidPosition; }
            set
            {
                if (_X1_LoadingVision_AvoidPosition == value) return;

                _X1_LoadingVision_AvoidPosition = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "X1 Axis Unloading Vision Avoid Position", Axis = AxisName.X1Axis, Unit = Unit.mm)]
        public double X1_UnloadingVision_AvoidPosition
        {
            get { return _X1_UnloadingVision_AvoidPosition; }
            set
            {
                if (_X1_UnloadingVision_AvoidPosition == value) return;

                _X1_UnloadingVision_AvoidPosition = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_3 { get; set; }

        [RecipeDescription(Description = "Loading Vision Pixel Size", Detail = "Pixel Size", Unit = Unit.Micrometer)]
        public double LoadingVision_PixelSize
        {
            get { return _LoadingVision_PixelSize; }
            set
            {
                if (_LoadingVision_PixelSize == value) return;

                _LoadingVision_PixelSize = value;
                OnPropertyChanged();
            }
        }

        [RecipeDescription(Description = "Unloading Vision Pixel Size", Detail = "Pixel Size", Unit = Unit.Micrometer)]
        public double UnloadingVision_PixelSize
        {
            get { return _UnloadingVision_PixelSize; }
            set
            {
                if (_UnloadingVision_PixelSize == value) return;

                _UnloadingVision_PixelSize = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool NULL_SPACE_4 { get; set; }

        [RecipeDescription(Description = "Loading Vision Exposure Time", Detail = "Time", Unit = Unit.Percentage)]
        public double LoadingVision_ExposureTime
        {
            get { return _LoadingVision_ExposureTime; }
            set
            {
                if (_LoadingVision_PixelSize == value) return;

                _LoadingVision_ExposureTime = value;
                OnPropertyChanged();
            }

        }

        [RecipeDescription(Description = "Unloading Vision Exposure Time", Detail = "Time", Unit = Unit.Percentage)]
        public double UnloadingVision_ExposureTime
        {
            get { return _UnloadingVision_ExposureTime; }
            set
            {
                if (_UnloadingVision_ExposureTime == value) return;

                _UnloadingVision_ExposureTime = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _X2_LoadingVisionPosition;
        private double _Z3_LoadingVisionPosition;
        private double _X2_UnloadingVisionPosition;
        private double _Z3_UnloadingVisionPosition;
        private double _X1_LoadingVision_AvoidPosition;
        private double _X1_UnloadingVision_AvoidPosition;
        private double _LoadingVision_PixelSize = 6.012;
        private double _UnloadingVision_PixelSize = 6.012;
        private double _LoadingVision_ExposureTime;
        private double _UnloadingVision_ExposureTime;
        #endregion

        #region Selection Options
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
