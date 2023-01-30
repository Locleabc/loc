using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TopCom;
using TopCom.Command;
using TopCom.Models;
using TopCom.Processing;
using TopLang;
using TopCom.LOG;
using PLV_BracketAssemble.Define;
using PLV_BracketAssemble.Processing;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class ManualViewModel : PropertyChangedNotifier
    {
        #region Properties
        public IOViewModel IOVM
        {
            get
            {
                return _IOVM ?? (_IOVM = new IOViewModel());
            }
        }

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
                            CDef.MessageViewModel.ShowDialog(Language.GetString("str_OnlyHomeButtonNoitfication", SelectedAxis.AxisName));
                            if (CDef.MessageViewModel.Result == true)
                            {
                                SelectedAxis.HomeSearch();
                            }
                            break;
                        case "ClearPositionButton":
                            SelectedAxis.ClearPosition();
                            break;
                        case "ConnectButton":
                            SelectedAxis.Connect();
                            break;
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
        private IOViewModel _IOVM;
        public IMotion _SelectedAxis;
        private double _Speed = 10;
        private double _Position = 2;
        #endregion
    }
}
