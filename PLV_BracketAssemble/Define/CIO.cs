using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using TopCom;

namespace PLV_BracketAssemble.Define
{
    public class CInput : PropertyChangedNotifier
    {
        public CInput()
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) =>
            {
                OnPropertyChanged("StartSW");
                OnPropertyChanged("ChangeSW");

                OnPropertyChanged("InputTrayDetect");
                OnPropertyChanged("OutputTrayDetect");

                OnPropertyChanged("Picker1_DownDetect");
                OnPropertyChanged("Picker1_UpDetect");
                OnPropertyChanged("Picker1_VacDetect");

                OnPropertyChanged("Picker2_DownDetect");
                OnPropertyChanged("Picker2_UpDetect");
                OnPropertyChanged("Picker2_VacDetect");

                OnPropertyChanged("PreAlgin1_CMDetect");
                OnPropertyChanged("PreAlgin2_CMDetect");

                OnPropertyChanged("TrayReadyPositionDetect");
                OnPropertyChanged("HeadReadyPositionDetect");

                OnPropertyChanged("MainPowerSW");
                OnPropertyChanged("EmergencySW");
            };
            timer.Start();
        }

        public bool PreAlgin1_CMDetect
        {
            // TODO: Update IO Map
            get { return false; }
        }

        public bool PreAlgin2_CMDetect
        {
            // TODO: Update IO Map
            get { return false; }
        }

        public bool StartSW
        {
            get { return CDef.IO_X.Input[0]; }
        }

        public bool ChangeSW
        {
            get { return CDef.IO_X.Input[1]; }
        }

        public bool TrayReadyPositionDetect
        {
            get
            {
#if !SIMULATION
                return CDef.IO_X.Input[2];
#else
                return CDef.AllAxis.XAxis.IsOnPosition(CDef.TrayRecipe.XAxis_Change_Position);
#endif
            }
        }

        public bool HeadReadyPositionDetect
        {
            get
            {
#if !SIMULATION
                return CDef.IO_X.Input[3];
#else
                return CDef.AllAxis.YAxis.IsOnPosition(CDef.HeadRecipe.YAxis_Change_Position);
#endif
            }
        }

        public bool InputTrayDetect
        {
            get
            {
#if !SIMULATION
                return CDef.IO_Y.Input[0];
#else
                return true;
#endif
            }
        }

        public bool OutputTrayDetect
        {
            get
            {
#if !SIMULATION
                return CDef.IO_Y.Input[1];
#else
                return true;
#endif
            }
        }

        public bool Picker2_UpDetect
        {
            get
            {
#if !SIMULATION
                return CDef.In_DIO3[8];
#else
                return CDef.IO.Output.Picker2_Up;
#endif
            }
        }

        public bool Picker2_DownDetect
        {
            get
            {
#if !SIMULATION
                return CDef.In_DIO3[9];
#else
                return CDef.IO.Output.Picker2_Down;
#endif
            }
        }

        public bool Picker1_UpDetect
        {
            get
            {
#if !SIMULATION
                return CDef.In_DIO3[12];
#else
                return CDef.IO.Output.Picker1_Up;
#endif
            }
        }

        public bool Picker1_DownDetect
        {
            get
            {
#if !SIMULATION
                return CDef.In_DIO3[13];
#else
                return CDef.IO.Output.Picker1_Down;
#endif
            }
        }

        public bool Picker1_VacDetect
        {
            get
            {
#if !SIMULATION
                return CDef.In_DIO3[14];
#else
                return CDef.IO.Output.Picker1_Vac;
#endif
            }
        }

        public bool Picker2_VacDetect
        {
            get
            {
#if !SIMULATION
                return CDef.In_DIO3[10];
#else
                return CDef.IO.Output.Picker2_Vac;
#endif
            }
        }

        public bool MainPowerSW
        {
            get
            {
                return CDef.IO_Tester.Input[7];
            }
        }

        public bool EmergencySW
        {
            get
            {
                return CDef.IO_Tester.Input[8];
            }
        }
    }

    public class COutput : PropertyChangedNotifier
    {
#region Native Buttons
        public bool StartLamp
        {
            get { return CDef.IO_X.Output[0]; }
            set
            {
                CDef.IO_X.Output[0] = value;
                OnPropertyChanged();
            }
        }

        public bool ChangeLamp
        {
            get { return CDef.IO_X.Output[1]; }
            set
            {
                CDef.IO_X.Output[1] = value;
                OnPropertyChanged();
            }
        }

        public bool TowerLampRed
        {
            get { return CDef.IO_Y.Output[0]; }
            set
            {
                CDef.IO_Y.Output[0] = value;
                OnPropertyChanged();
            }
        }

        public bool TowerLampYellow
        {
            get { return CDef.IO_Y.Output[1]; }
            set
            {
                CDef.IO_Y.Output[1] = value;
                OnPropertyChanged();
            }
        }

        public bool TowerLampGreen
        {
            get { return CDef.IO_Y.Output[2]; }
            set
            {
                CDef.IO_Y.Output[2] = value;
                OnPropertyChanged();
            }
        }

        public bool Buzzer1
        {
            get { return CDef.IO_XX.Output[0]; }
            set
            {
                CDef.IO_XX.Output[0] = value;
                OnPropertyChanged();
            }
        }

        public bool Buzzer2
        {
            get { return CDef.IO_XX.Output[1]; }
            set
            {
                CDef.IO_XX.Output[1] = value;
                OnPropertyChanged();
            }
        }

        public bool Picker2_Up
        {
            get { return CDef.Out_DIO4[0]; }
            set
            {
                CDef.Out_DIO4[0] = value;
                OnPropertyChanged();
            }
        }

        public bool Picker2_Down
        {
            get { return CDef.Out_DIO4[1]; }
            set
            {
                CDef.Out_DIO4[1] = value;
                OnPropertyChanged();
            }
        }

        public bool Picker2_Vac
        {
            get { return CDef.Out_DIO4[2]; }
            set
            {
                CDef.Out_DIO4[2] = value;
                OnPropertyChanged();
            }
        }

        public bool Picker2_Purge
        {
            get { return CDef.Out_DIO4[3]; }
            set
            {
                CDef.Out_DIO4[3] = value;
                OnPropertyChanged();
            }
        }

        public bool Picker1_Up
        {
            get { return CDef.Out_DIO4[4]; }
            set
            {
                CDef.Out_DIO4[4] = value;
                OnPropertyChanged();
            }
        }

        public bool Picker1_Down
        {
            get { return CDef.Out_DIO4[5]; }
            set
            {
                CDef.Out_DIO4[5] = value;
                OnPropertyChanged();
            }
        }

        public bool Picker1_Vac
        {
            get { return CDef.Out_DIO4[6]; }
            set
            {
                CDef.Out_DIO4[6] = value;
                OnPropertyChanged();
            }
        }

        public bool Picker1_Purge
        {
            get { return CDef.Out_DIO4[7]; }
            set
            {
                CDef.Out_DIO4[7] = value;
                OnPropertyChanged();
            }
        }
#endregion

#region Complex Buttons
        public bool TowerLamp_Home
        {
            set
            {
                TowerLamp_Clear();

                TowerLampYellow_Blink = true;
            }
        }

        public bool TowerLamp_Run
        {
            set
            {
                TowerLamp_Clear();

                TowerLampGreen = true;
            }
        }

        public bool TowerLamp_Warning
        {
            set
            {
                TowerLamp_Clear();

                TowerLampRed = true;
            }
        }

        public bool TowerLamp_Alarm
        {
            set
            {
                TowerLamp_Clear();

                TowerLampRed = true;
            }
        }

        public bool TowerLamp_Stop
        {
            set
            {
                TowerLamp_Clear();

                TowerLampYellow = true;
            }
        }

        public bool TowerLamp_Idle
        {
            set
            {
                TowerLamp_Clear();

                TowerLampYellow = true;
            }
        }
#endregion

#region Methods
        public void AllLightOff()
        {
        }

        public void VacPurgeOff()
        {
            Picker1_Vac = false;
            Picker2_Vac = false;

            Picker1_Purge = false;
            Picker2_Purge = false;
        }
#endregion

#region Privates
        private void TowerLamp_Clear()
        {
            TowerLampGreen = false;
            TowerLampYellow = false;
            TowerLampRed = false;

            TowerLampGreen_Blink = false;
            TowerLampYellow_Blink = false;
            TowerLampRed_Blink = false;
        }

        private Timer TowerLampRed_BlinkTimer;
        private bool TowerLampRed_Blink
        {
            set
            {
                if (value == true)
                {
                    if (TowerLampRed_BlinkTimer != null)
                    {
                        TowerLampRed_BlinkTimer.Dispose();
                    }
                    TowerLampRed_BlinkTimer = new Timer((sender) =>
                    {
                        TowerLampRed = !TowerLampRed;
                    }, null, 0, 500);
                }
                else
                {
                    TowerLampRed = false;
                    if (TowerLampRed_BlinkTimer != null)
                    {
                        TowerLampRed_BlinkTimer.Dispose();
                    }
                }
            }
        }

        private Timer TowerLampYellow_BlinkTimer;
        private bool TowerLampYellow_Blink
        {
            set
            {
                if (value == true)
                {
                    if (TowerLampYellow_BlinkTimer != null)
                    {
                        TowerLampYellow_BlinkTimer.Dispose();
                    }
                    TowerLampYellow_BlinkTimer = new Timer((sender) =>
                    {
                        TowerLampYellow = !TowerLampYellow;
                    }, null, 0, 500);
                }
                else
                {
                    TowerLampYellow = false;
                    if (TowerLampYellow_BlinkTimer != null)
                    {
                        TowerLampYellow_BlinkTimer.Dispose();
                    }
                }
            }
        }

        private Timer TowerLampGreen_BlinkTimer;
        private bool TowerLampGreen_Blink
        {
            set
            {
                if (value == true)
                {
                    if (TowerLampGreen_BlinkTimer != null)
                    {
                        TowerLampGreen_BlinkTimer.Dispose();
                    }
                    TowerLampGreen_BlinkTimer = new Timer((sender) =>
                    {
                        TowerLampGreen = !TowerLampGreen;
                    }, null, 0, 500);
                }
                else
                {
                    TowerLampYellow = false;
                    if (TowerLampGreen_BlinkTimer != null)
                    {
                        TowerLampGreen_BlinkTimer.Dispose();
                    }
                }
            }
        }
#endregion
    }

    public class CHeadStatus
    {
        public bool HeadOccupied
        {
            get
            {
                return CDef.IO.Input.Picker1_VacDetect | CDef.IO.Input.Picker2_VacDetect;
            }
        }
    }

    public class CIO
    {
        public CInput Input { get; set; } = new CInput();
        public COutput Output { get; set; } = new COutput();
        public CHeadStatus HeadStatus { get; set; } = new CHeadStatus();
    }
}
