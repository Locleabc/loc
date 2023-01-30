using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using TopCom;

namespace VCM_FullAssy.Define
{
    public class CInput
    {
        public bool EMS_SW
        {
            get { return CDef.IO_XAxis.Input[0]; }
        }

        public bool MainPower
        {
            get { return CDef.IO_XAxis.Input[1]; }
        }

        public bool StartSW
        {
            get { return CDef.IO_XAxis.Input[2]; }
        }

        public bool StopSW
        {
            get { return CDef.IO_XAxis.Input[3]; }
        }

        public bool OKSW
        {
            get { return CDef.IO_XAxis.Input[4]; }
        }

        public bool HomeSW
        {
            get { return CDef.IO_XAxis.Input[5]; }
        }

        public bool Y1LoadingSensor
        {
            get { return CDef.IO_Y1Axis.Input[0]; }
        }

        public bool Y2LoadingSensor
        {
            get { return CDef.IO_Y1Axis.Input[1]; }
        }

        public bool Door1
        {
            get { return CDef.IO_Y2Axis.Input[0]; }
        }

        public bool Door2
        {
            get { return CDef.IO_Y2Axis.Input[1]; }
        }

        public bool MainCDA
        {
            get { return CDef.IO_ZAxis.Input[0]; }
        }

        public bool Head_VAC
        {
            get { return CDef.IO_ZAxis.Input[1]; }
        }
    }

    public class COutput : PropertyChangedNotifier
    {
        #region Native Buttons
        public bool StartLamp
        {
            get { return CDef.IO_XAxis.Output[2]; }
            set
            {
                CDef.IO_XAxis.Output[2] = value;
                OnPropertyChanged();
            }
        }

        public bool StopLamp
        {
            get { return CDef.IO_XAxis.Output[3]; }
            set
            {
                CDef.IO_XAxis.Output[3] = value;
                OnPropertyChanged();
            }
        }

        public bool OKLamp
        {
            get { return CDef.IO_XAxis.Output[4]; }
            set
            {
                CDef.IO_XAxis.Output[4] = value;
                OnPropertyChanged();
            }
        }

        public bool HomeLamp
        {
            get { return CDef.IO_XAxis.Output[5]; }
            set
            {
                CDef.IO_XAxis.Output[5] = value;
                OnPropertyChanged();
            }
        }

        public bool Buzzer1
        {
            get { return CDef.IO_Y1Axis.Output[0]; }
            set
            {
                CDef.IO_Y1Axis.Output[0] = value;
                OnPropertyChanged();
            }
        }

        public bool Buzzer2
        {
            get { return CDef.IO_Y1Axis.Output[1]; }
            set
            {
                CDef.IO_Y1Axis.Output[1] = value;
                OnPropertyChanged();
            }
        }

        public bool TowerLampRed
        {
            get { return CDef.IO_Y1Axis.Output[3]; }
            set
            {
                CDef.IO_Y1Axis.Output[3] = value;
                OnPropertyChanged();
            }
        }

        public bool TowerLampYellow
        {
            get { return CDef.IO_Y1Axis.Output[4]; }
            set
            {
                CDef.IO_Y1Axis.Output[4] = value;
                OnPropertyChanged();
            }
        }

        public bool TowerLampGreen
        {
            get { return CDef.IO_Y1Axis.Output[5]; }
            set
            {
                CDef.IO_Y1Axis.Output[5] = value;
                OnPropertyChanged();
            }
        }

        public bool HeadVAC
        {
            get { return CDef.IO_ZAxis.Output[0]; }
            set
            {
                CDef.IO_ZAxis.Output[0] = value;
                OnPropertyChanged();
            }
        }

        public bool HeadPurge
        {
            get { return CDef.IO_ZAxis.Output[1]; }
            set
            {
                CDef.IO_ZAxis.Output[1] = value;
                OnPropertyChanged();
            }
        }

        public bool LightUpper
        {
            get { return CDef.IO_ZAxis.Output[2]; }
            set
            {
                CDef.IO_ZAxis.Output[2] = value;
                OnPropertyChanged();
            }
        }

        public bool LightUnder
        {
            get { return CDef.IO_ZAxis.Output[3]; }
            set
            {
                CDef.IO_ZAxis.Output[3] = value;
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
                return CDef.IO.Output.HeadVAC;
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
