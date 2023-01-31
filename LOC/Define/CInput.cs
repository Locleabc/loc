using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace LOC.Define
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
}
