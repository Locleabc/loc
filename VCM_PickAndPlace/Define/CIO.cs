using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using TopCom;

namespace VCM_PickAndPlace.Define
{
    public class CInput : PropertyChangedNotifier
    {
        public CInput()
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) =>
            {
                OnPropertyChanged("Y1LoadingSensor");
                OnPropertyChanged("Y2LoadingSensor");
                OnPropertyChanged("Door1");
                OnPropertyChanged("Door2");
                OnPropertyChanged("EMS_SW");
                OnPropertyChanged("MainPower");
                OnPropertyChanged("StartSW");
                OnPropertyChanged("StopSW");
                OnPropertyChanged("OKSW");
                OnPropertyChanged("HomeSW");
                OnPropertyChanged("MainCDA");
                OnPropertyChanged("Head1_VAC");
                OnPropertyChanged("Head2_VAC");
            };
            timer.Start();
        }

        public bool Y1LoadingSensor
        {
            get { return CDef.IO1.Input[0]; }
        }

        public bool Y2LoadingSensor
        {
            get { return CDef.IO1.Input[1]; }
        }

        public bool Door1
        {
            get { return CDef.IO1.Input[2]; }
        }

        public bool Door2
        {
            get { return CDef.IO1.Input[3]; }
        }

        public bool EMS_SW
        {
            get { return CDef.IO1.Input[6]; }
        }

        public bool MainPower
        {
            get { return CDef.IO1.Input[7]; }
        }

        public bool StartSW
        {
            get { return CDef.IO1.Input[8]; }
        }

        public bool StopSW
        {
            get { return CDef.IO1.Input[9]; }
        }

        public bool OKSW
        {
            get { return CDef.IO1.Input[10]; }
        }

        public bool HomeSW
        {
            get { return CDef.IO1.Input[11]; }
        }

        public bool MainCDA
        {
            get { return CDef.IO1.Input[12]; }
        }

        public bool Head1_VAC
        {
            get
            {
#if SIMULATION
                return CDef.IO.Output.Head1VAC;
#else
                return CDef.IO1.Input[13];
#endif
            }
        }

        public bool Head2_VAC
        {
            get
            {
#if SIMULATION
                return CDef.IO.Output.Head2VAC;
#else
                return CDef.IO1.Input[14];
#endif
            }
        }
    }

    public class COutput : PropertyChangedNotifier
    {
        #region Native Buttons
        public bool TowerLampRed
        {
            get { return CDef.IO1.Output[0]; }
            set
            {
                CDef.IO1.Output[0] = value;
                OnPropertyChanged();
            }
        }

        public bool TowerLampYellow
        {
            get { return CDef.IO1.Output[1]; }
            set
            {
                CDef.IO1.Output[1] = value;
                OnPropertyChanged();
            }
        }

        public bool TowerLampGreen
        {
            get { return CDef.IO1.Output[2]; }
            set
            {
                CDef.IO1.Output[2] = value;
                OnPropertyChanged();
            }
        }

        public bool Buzzer1
        {
            get { return CDef.IO1.Output[3]; }
            set
            {
                CDef.IO1.Output[3] = value;
                OnPropertyChanged();
            }
        }

        public bool Buzzer2
        {
            get { return CDef.IO1.Output[4]; }
            set
            {
                CDef.IO1.Output[4] = value;
                OnPropertyChanged();
            }
        }

        public bool IonizerSol
        {
            get { return CDef.IO1.Output[5]; }
            set
            {
                CDef.IO1.Output[5] = value;
                OnPropertyChanged();
            }
        }

        public bool LightUpper
        {
            get { return CDef.IO1.Output[6]; }
            set
            {
                CDef.IO1.Output[6] = value;
                OnPropertyChanged();
            }
        }

        public bool LightUnder
        {
            get { return CDef.IO1.Output[7]; }
            set
            {
                CDef.IO1.Output[7] = value;
                OnPropertyChanged();
            }
        }

        public bool LightInspect
        {
            get { return CDef.IO2.Output[0]; }
            set
            {
                CDef.IO2.Output[0] = value;
                OnPropertyChanged();
            }
        }

        public bool StartLamp
        {
            get { return CDef.IO1.Output[8]; }
            set
            {
                CDef.IO1.Output[8] = value;
                OnPropertyChanged();
            }
        }

        public bool StopLamp
        {
            get { return CDef.IO1.Output[9]; }
            set
            {
                CDef.IO1.Output[9] = value;
                OnPropertyChanged();
            }
        }

        public bool OKLamp
        {
            get { return CDef.IO1.Output[10]; }
            set
            {
                CDef.IO1.Output[10] = value;
                OnPropertyChanged();
            }
        }

        public bool HomeLamp
        {
            get { return CDef.IO1.Output[11]; }
            set
            {
                CDef.IO1.Output[11] = value;
                OnPropertyChanged();
            }
        }

        public bool Head1VAC
        {
            get { return CDef.IO1.Output[12]; }
            set
            {
                CDef.IO1.Output[12] = value;
                OnPropertyChanged();
            }
        }

        public bool Head1Purge
        {
            get { return CDef.IO1.Output[13]; }
            set
            {
                CDef.IO1.Output[13] = value;
                OnPropertyChanged();
            }
        }

        public bool Head2VAC
        {
            get { return CDef.IO1.Output[14]; }
            set
            {
                CDef.IO1.Output[14] = value;
                OnPropertyChanged();
            }
        }

        public bool Head2Purge
        {
            get { return CDef.IO1.Output[15]; }
            set
            {
                CDef.IO1.Output[15] = value;
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
                HomeLamp_Blink = true;
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
            LightUpper = false;
            LightUnder = false;
            LightInspect = false;
        }

        public void VacPurgeOff()
        {
            Head1VAC = false;
            Head2VAC = false;

            Head1Purge = false;
            Head2Purge = false;
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

            HomeLamp_Blink = false;
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

        private Timer HomeLamp_BlinkTimer;
        private bool HomeLamp_Blink
        {
            set
            {
                if (value == true)
                {
                    if (HomeLamp_BlinkTimer != null)
                    {
                        HomeLamp_BlinkTimer.Dispose();
                    }
                    HomeLamp_BlinkTimer = new Timer((sender) =>
                    {
                        HomeLamp = !HomeLamp;
                    }, null, 0, 500);
                }
                else
                {
                    HomeLamp = false;
                    if (HomeLamp_BlinkTimer != null)
                    {
                        HomeLamp_BlinkTimer.Dispose();
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
                return CDef.IO.Input.Head1_VAC | CDef.IO.Input.Head2_VAC;
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
