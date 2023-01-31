using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom;

namespace LOC.Define
{
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
}
