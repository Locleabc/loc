using PLV_BracketAssemble.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;
using TopCom.Command;
using TopCom.Models;
using TopMotion;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class ManualControlMotionViewModel : PropertyChangedNotifier
    {
        #region Properties

        #region Control Motor Options 
        public double Percent_Velocity
        {
            get { return _Percent_Velocity; }
            set
            {
                _Percent_Velocity = value;
                OnPropertyChanged();
            }
        }
        public double XAxisVelocity
        {
            get
            {
                return Percent_Velocity * CDef.AllAxis.XAxis.Speed / 100;
            }
        }
        public double YAxisVelocity
        {
            get
            {
                return Percent_Velocity * CDef.AllAxis.YAxis.Speed / 100;
            }
        }
        public double XXAxisVelocity
        {
            get
            {
                return Percent_Velocity * CDef.AllAxis.XXAxis.Speed / 100;
            }
        }

        public double PositionControl
        {
            get { return _PositionControl; }
            set
            {
                _PositionControl = value;
                OnPropertyChanged();
            }
        }
        public bool IsModeJogControl
        {
            get { return _IsModeJogControl; }
            set
            {
                _IsModeJogControl = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ModeControlToString));
            }
        }
        public string ModeControlToString
        {
            get
            {
                return IsModeJogControl ? "Jog" : "Relative";
            }
        }
        #endregion

        #region Picker Cylinders 
        public CPickerCylinder PickerCylinder1 
        { 
            get
            {
                return (CPickerCylinder)CDef.RootProcess.HeadProcess.PickerCylinder1;
            }
        }

        public CPickerCylinder PickerCylinder2
        {
            get
            {
                return (CPickerCylinder)CDef.RootProcess.HeadProcess.PickerCylinder2;
            }
        }
        #endregion

        #region Motion
        public MotionPlusR X_Axis
        {
            get
            {
                return (MotionPlusR)CDef.AllAxis.XAxis;
            }
        }
        public MotionPlusR Y_Axis
        {
            get
            {
                return (MotionPlusR)CDef.AllAxis.YAxis;
            }
        }
        public MotionPlusR XX_Axis
        {
            get
            {
                return (MotionPlusR)CDef.AllAxis.XXAxis;
            }
        }
        #endregion

        #region Update Position Status
        public bool Is_Tray_Change_Position
        {
            get
            {
                return X_Axis.IsOnPosition(CDef.TrayRecipe.XAxis_Change_Position);
            }
        }
        public bool Is_Head_Change_Position
        {
            get
            {
                return XX_Axis.IsOnPosition(CDef.HeadRecipe.XXAxis_Change_Position) && Y_Axis.IsOnPosition(CDef.HeadRecipe.YAxis_Change_Position);
            }
        }

        public bool Is_Head_PreAlign_Position
        {
            get
            {
                return CDef.RootProcess.HeadProcess.Is_Head_PreAlign_Position;
            }
        }

        public bool Is_Tray_Picker1_Pick_First_Position
        {
            get
            {
                return X_Axis.IsOnPosition(CDef.TrayRecipe.XAxis_Tray_Picker1_Pick_First_Position);
            }
        }
        public bool Is_Tray_Picker1_Place_First_Position
        {
            get
            {
                return X_Axis.IsOnPosition(CDef.TrayRecipe.XAxis_Tray_Picker1_Place_First_Position);
            }
        }
        public bool Is_Tray_Picker2_Pick_First_Position
        {
            get
            {
                return X_Axis.IsOnPosition(CDef.TrayRecipe.XAxis_Tray_Picker2_Pick_First_Position);
            }
        }
        public bool Is_Tray_Picker2_Place_First_Position
        {
            get
            {
                return X_Axis.IsOnPosition(CDef.TrayRecipe.XAxis_Tray_Picker2_Place_First_Position);
            }
        }

        public bool Is_Head_Picker1_Pick_First_Position
        {
            get
            {
                return XX_Axis.IsOnPosition(CDef.HeadRecipe.XXAxis_Pick_First_Position) && Y_Axis.IsOnPosition(CDef.HeadRecipe.YAxis_Picker1_Pick_First_Position);
            }
        }
        
        public bool Is_Head_Picker1_UnderVision_Position
        {
            get
            {
                return XX_Axis.IsOnPosition(CDef.HeadRecipe.XXAxis_Picker1_UnderVision_Position) && Y_Axis.IsOnPosition(CDef.HeadRecipe.YAxis_Picker1_UnderVision_Position);
            }
        }
        public bool Is_Head_Picker1_Place_First_Position
        {
            get
            {
                return XX_Axis.IsOnPosition(CDef.HeadRecipe.XXAxis_Place_First_Position) && Y_Axis.IsOnPosition(CDef.HeadRecipe.YAxis_Picker1_Place_First_Position);
            }
        }

        public bool Is_Head_Picker2_Pick_First_Position
        {
            get
            {
                return XX_Axis.IsOnPosition(CDef.HeadRecipe.XXAxis_Pick_First_Position) && Y_Axis.IsOnPosition(CDef.HeadRecipe.YAxis_Picker2_Pick_First_Position);
            }
        }
        public bool Is_Head_Picker2_UnderVision_Position
        {
            get
            {
                return XX_Axis.IsOnPosition(CDef.HeadRecipe.XXAxis_Picker2_UnderVision_Position) && Y_Axis.IsOnPosition(CDef.HeadRecipe.YAxis_Picker2_UnderVision_Position);
            }
        }
        public bool Is_Head_Picker2_Place_First_Position
        {
            get
            {
                return XX_Axis.IsOnPosition(CDef.HeadRecipe.XXAxis_Place_First_Position) && Y_Axis.IsOnPosition(CDef.HeadRecipe.YAxis_Picker2_Place_First_Position);
            }
        }
        #endregion

        #endregion

        #region Constructor
        public ManualControlMotionViewModel()
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += (s, e) =>
            {
                OnPropertyChanged(nameof(Is_Tray_Change_Position));
                OnPropertyChanged(nameof(Is_Head_Change_Position));
                OnPropertyChanged(nameof(Is_Head_PreAlign_Position));

                OnPropertyChanged(nameof(Is_Tray_Picker1_Pick_First_Position));
                OnPropertyChanged(nameof(Is_Tray_Picker1_Place_First_Position));
                OnPropertyChanged(nameof(Is_Tray_Picker2_Pick_First_Position));
                OnPropertyChanged(nameof(Is_Tray_Picker2_Place_First_Position));


                OnPropertyChanged(nameof(Is_Head_Picker1_Pick_First_Position));
                OnPropertyChanged(nameof(Is_Head_Picker1_UnderVision_Position));
                OnPropertyChanged(nameof(Is_Head_Picker1_Place_First_Position));

                OnPropertyChanged(nameof(Is_Head_Picker2_Pick_First_Position));
                OnPropertyChanged(nameof(Is_Head_Picker2_UnderVision_Position));
                OnPropertyChanged(nameof(Is_Head_Picker2_Place_First_Position));
            };
            timer.Start();
        }
        #endregion

        #region Commands 
        public RelayCommand ChangeModeControl
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    IsModeJogControl = !IsModeJogControl;
                });
            }
        }

        public RelayCommand AxisControlButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (!PickerCylinder1.IsBackward && !PickerCylinder2.IsBackward)
                    {
                        CDef.MessageViewModel.Show("Please Both Piker Cylinder Up", caption: "Warning");
                        return;
                    }
                    if (!PickerCylinder1.IsBackward)
                    {
                        CDef.MessageViewModel.Show("Please Picker 1 Up", caption: "Warning");
                        return;
                    }

                    if (!PickerCylinder2.IsBackward)
                    {
                        CDef.MessageViewModel.Show("Please Picker 2 Up", caption: "Warning");
                        return;
                    }

                    if (IsModeJogControl) return;
                    
                    //If Motion is moving => return
                    if (!X_Axis.Status.IsMotionDone || !XX_Axis.Status.IsMotionDone || !Y_Axis.Status.IsMotionDone) return;
                    
                    //Check Picker Status
                    

                    switch ((string)o)
                    {
                        case "XAxis_Left":
                            X_Axis.MoveInc(PositionControl * -1, XAxisVelocity);
                            break;
                        case "XAxis_Right":
                            X_Axis.MoveInc(PositionControl, XAxisVelocity);
                            break;

                        case "YAxis_Backward":
                            Y_Axis.MoveInc(PositionControl * -1, YAxisVelocity);
                            break;
                        case "YAxis_Forward":
                            Y_Axis.MoveInc(PositionControl, YAxisVelocity);
                            break;

                        case "XXAxis_Left":
                            XX_Axis.MoveInc(PositionControl* -1,XXAxisVelocity);
                            break;
                        case "XXAxis_Right":
                            XX_Axis.MoveInc(PositionControl, XXAxisVelocity);
                            break;
                    }
                });
            }
        }

        public RelayCommand ControlPickerButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    //If Motion is moving => return
                    if (!X_Axis.Status.IsMotionDone || !XX_Axis.Status.IsMotionDone || !Y_Axis.Status.IsMotionDone) return;

                    switch ((string)o)
                    {
                        case "Picker1_Up":
                            PickerCylinder1.MoveBackward();
                            break;
                        case "Picker1_Down":
                            PickerCylinder1.MoveForward();
                            break;
                        case "Picker1_Vacuum":
                            if (CDef.IO.Output.Picker1_Purge)
                            {
                                CDef.IO.Output.Picker1_Purge = false;
                            }
                            CDef.IO.Output.Picker1_Vac = !CDef.IO.Output.Picker1_Vac;
                            break;
                        case "Picker1_Purge":
                            if (CDef.IO.Output.Picker1_Vac)
                            {
                                CDef.IO.Output.Picker1_Vac = false;
                            }
                            CDef.IO.Output.Picker1_Purge = !CDef.IO.Output.Picker1_Purge;
                            break;

                        case "Picker2_Up":
                            PickerCylinder2.MoveBackward();
                            break;
                        case "Picker2_Down":
                            PickerCylinder2.MoveForward();
                            break;
                        case "Picker2_Vacuum":
                            if (CDef.IO.Output.Picker2_Purge)
                            {
                                CDef.IO.Output.Picker2_Purge = false;
                            }
                            CDef.IO.Output.Picker2_Vac = !CDef.IO.Output.Picker2_Vac;
                            break;
                        case "Picker2_Purge":
                            if (CDef.IO.Output.Picker2_Vac)
                            {
                                CDef.IO.Output.Picker2_Vac = false;
                            }
                            CDef.IO.Output.Picker2_Purge = !CDef.IO.Output.Picker2_Purge;
                            break;
                    }
                });
            }
        }

        public RelayCommand MovePositionButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    //If Motion is moving => return
                    if (!X_Axis.Status.IsMotionDone || !XX_Axis.Status.IsMotionDone || !Y_Axis.Status.IsMotionDone) return;

                    //Check Picker Status
                    if (!PickerCylinder1.IsBackward && !PickerCylinder2.IsBackward)
                    {
                        CDef.MessageViewModel.Show("Please Both Piker Cylinder Up", caption: "Warning");
                        return;
                    }

                    if (!PickerCylinder1.IsBackward)
                    {
                        CDef.MessageViewModel.Show("Please Picker Cylinder 1 Up", caption: "Warning");
                        return;
                    }

                    if (!PickerCylinder2.IsBackward)
                    {
                        CDef.MessageViewModel.Show("Please Picker Cylinder 2 Up", caption: "Warning");
                        return;
                    }

                    CDef.MessageViewModel.ShowDialog($"Do you want move: \'{o}\'?");
                    if (CDef.MessageViewModel.Result == false)
                    {
                        return;
                    }

                    switch ((string)o)
                    {
                        case "Tray_ChangePosition":
                            X_Axis.MoveAbs(CDef.TrayRecipe.XAxis_Change_Position, XAxisVelocity);
                            break;
                        case "Head_ChangePosition":
                            XX_Axis.MoveAbs(CDef.HeadRecipe.XXAxis_Change_Position, XXAxisVelocity);
                            Y_Axis.MoveAbs(CDef.HeadRecipe.YAxis_Change_Position, YAxisVelocity);
                            break;
                        case "Head_PreAlignPosition":
                            XX_Axis.MoveAbs(CDef.HeadRecipe.XXAxis_PreAlign_Position, XXAxisVelocity);
                            Y_Axis.MoveAbs(CDef.HeadRecipe.YAxis_PreAlign_Position, YAxisVelocity);
                            break;

                        case "Tray_Piker1PickFirstPosition":
                            X_Axis.MoveAbs(CDef.TrayRecipe.XAxis_Tray_Picker1_Pick_First_Position, XAxisVelocity);
                            break;
                        case "Tray_Piker1PlaceFirstPosition":
                            X_Axis.MoveAbs(CDef.TrayRecipe.XAxis_Tray_Picker1_Place_First_Position, XAxisVelocity);
                            break;
                        case "Tray_Piker2PickFirstPosition":
                            X_Axis.MoveAbs(CDef.TrayRecipe.XAxis_Tray_Picker2_Pick_First_Position, XAxisVelocity);
                            break;
                        case "Tray_Piker2PlaceFirstPosition":
                            X_Axis.MoveAbs(CDef.TrayRecipe.XAxis_Tray_Picker2_Place_First_Position, XAxisVelocity);
                            break;

                        case "Picker1_PickFirstPosition":
                            XX_Axis.MoveAbs(CDef.HeadRecipe.XXAxis_Pick_First_Position, XXAxisVelocity);
                            Y_Axis.MoveAbs(CDef.HeadRecipe.YAxis_Picker1_Pick_First_Position, YAxisVelocity);
                            break;
                        case "Picker1_UnderVisionPosition":
                            XX_Axis.MoveAbs(CDef.HeadRecipe.XXAxis_Picker1_UnderVision_Position, XXAxisVelocity);
                            Y_Axis.MoveAbs(CDef.HeadRecipe.YAxis_Picker1_UnderVision_Position, YAxisVelocity);
                            break;
                        case "Picker1_PlaceFirstPosition":
                            XX_Axis.MoveAbs(CDef.HeadRecipe.XXAxis_Place_First_Position, XXAxisVelocity);
                            Y_Axis.MoveAbs(CDef.HeadRecipe.YAxis_Picker1_Place_First_Position, YAxisVelocity);
                            break;

                        case "Picker2_PickFirstPosition":
                            XX_Axis.MoveAbs(CDef.HeadRecipe.XXAxis_Pick_First_Position, XXAxisVelocity);
                            Y_Axis.MoveAbs(CDef.HeadRecipe.YAxis_Picker2_Pick_First_Position, YAxisVelocity);
                            break;
                        case "Picker2_UnderVisionPosition":
                            XX_Axis.MoveAbs(CDef.HeadRecipe.XXAxis_Picker2_UnderVision_Position, XXAxisVelocity);
                            Y_Axis.MoveAbs(CDef.HeadRecipe.YAxis_Picker2_UnderVision_Position, YAxisVelocity);
                            break;
                        case "Picker2_PlaceFirstPosition":
                            XX_Axis.MoveAbs(CDef.HeadRecipe.XXAxis_Place_First_Position, XXAxisVelocity);
                            Y_Axis.MoveAbs(CDef.HeadRecipe.YAxis_Picker2_Place_First_Position, YAxisVelocity);
                            break;
                    }
                });
            }
        }
        #endregion

        #region Privates
        private double _PositionControl = 0.001;
        private double _Percent_Velocity = 10;
        private bool _IsModeJogControl = true;
        #endregion
    }
}
