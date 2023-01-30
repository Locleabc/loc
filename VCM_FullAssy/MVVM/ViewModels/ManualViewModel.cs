using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TopCom;
using TopCom.Command;
using TopCom.Models;
using TopCom.Processing;
using TopCom.LOG;
using VCM_FullAssy.Define;
using VCM_FullAssy.Processing;

namespace VCM_FullAssy.MVVM.ViewModels
{
    public class ManualViewModel : PropertyChangedNotifier
    {
        #region Properties
        public IMotion SelectedAxis
        {
            get { return _SelectedAxis; }
            set
            {
                if (_SelectedAxis == value) return;

                _SelectedAxis = value;
                OnPropertyChanged();
            }
        }

        public double Speed
        {
            get { return _Speed; }
            set
            {
                if (_Speed == value) return;

                _Speed = value;
                OnPropertyChanged();
            }
        }

        public double Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand<PositionData> SpeedDataUpdateCommand
        {
            get
            {
                return new RelayCommand<PositionData>((sender) =>
                {
                    PositionData pd = sender as PositionData;

                    if (pd.OldValue == pd.Value) return;

                    UILog.Info($"Recipe Updated: [{pd.PositionName}] {pd.OldValue} -> {pd.Value}");
                    SaveSpeed();
                });
            }
        }

        public RelayCommand MotionButtonCommand
        {
            get
            {
                return new RelayCommand((sender) =>
                {
                    string tag = "";

                    if (sender != null)
                    {
                        tag = (sender as Button).Tag.ToString();
                    }
                    UILog.Info($"Button {(sender as Button).Content} Clicked!");

                    switch (tag)
                    {
                        case "Jog-Button":
                            SelectedAxis.MoveJog(Speed, false);
                            break;
                        case "Jog+Button":
                            SelectedAxis.MoveJog(Speed, true);
                            break;
                        case "Inc-Button":
                            SelectedAxis.MoveInc(Position * -1, Speed);
                            break;
                        case "Inc+Button":
                            SelectedAxis.MoveInc(Position, Speed);
                            break;
                        case "StopButton":
                            SelectedAxis.SoftStop();
                            break;
                        case "AbsButton":
                            SelectedAxis.MoveAbs(Position, Speed);
                            break;
                        case "AlarmResetButton":
                            SelectedAxis.AlarmReset();
                            break;
                        case "ServoOnButton":
                            SelectedAxis.MotionOn();
                            break;
                        case "ServoOffButton":
                            SelectedAxis.MotionOff();
                            break;
                        case "HomeButton":
                            SelectedAxis.HomeSearch();
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public RelayCommand SubAutoButtonCommand
        {
            get
            {
                return new RelayCommand((sender) =>
                {
                    string tag = (sender as Button).Tag.ToString();
                    UILog.Info($"Button {(sender as Button).Content} Clicked!");

                    switch (tag)
                    {
                        case "SubAutoPick":
                            CDef.RootProcess.RunMode = ERunMode.Manual_Pick;
                            CDef.RootProcess.OperationCommand = OperatingMode.Run;
                            break;
                        case "SubUnderVision":
                            CDef.RootProcess.RunMode = ERunMode.Manual_UnderVision;
                            CDef.RootProcess.OperationCommand = OperatingMode.Run;
                            break;
                        case "SubAutoPlace":
                            CDef.RootProcess.RunMode = ERunMode.Manual_Place;
                            CDef.RootProcess.OperationCommand = OperatingMode.Run;
                            break;
                        case "SubUpperLoadVision":
                            CDef.RootProcess.RunMode = ERunMode.Manual_LoadVision;
                            CDef.RootProcess.OperationCommand = OperatingMode.Run;
                            break;
                        case "SubUpperUnloadVision":
                            CDef.RootProcess.RunMode = ERunMode.Manual_UnloadVision;
                            CDef.RootProcess.OperationCommand = OperatingMode.Run;
                            break;
                        case "SubLeftTrayChange":
                            CDef.RootProcess.LeftTrayProcess.InWorking = true;
                            CDef.RootProcess.RightTrayProcess.InWorking = false;

                            CDef.RootProcess.RunMode = ERunMode.Manual_TrayChange;
                            CDef.RootProcess.OperationCommand = OperatingMode.Run;
                            break;
                        case "SubRightTrayChange":
                            CDef.RootProcess.LeftTrayProcess.InWorking = false;
                            CDef.RootProcess.RightTrayProcess.InWorking = true;

                            CDef.RootProcess.RunMode = ERunMode.Manual_TrayChange;
                            CDef.RootProcess.OperationCommand = OperatingMode.Run;
                            break;
                        //case "SubLeftTrayLoadPos":
                        //    CDef.RootProcess.RunMode = ERunMode.Manual_LoadingTray_Load;
                        //    CDef.RootProcess.OperationCommand = OperatingMode.Run;
                        //    break;
                        //case "SubRightTrayLoadPos":
                        //    CDef.RootProcess.RunMode = ERunMode.Manual_UnloadingTray_Load;
                        //    CDef.RootProcess.OperationCommand = OperatingMode.Run;
                        //    break;
                        default:
                            break;
                    }
                });
            }
        }
        #endregion

        #region Constructors
        public ManualViewModel()
        {
        }
        #endregion

        #region Methods
        private void SaveSpeed()
        {
            CDef.AllAxis.Save();
        }
        #endregion

        #region Privates
        public IMotion _SelectedAxis;
        private double _Speed = 50;
        private double _Position = 10;
        #endregion
    }
}
