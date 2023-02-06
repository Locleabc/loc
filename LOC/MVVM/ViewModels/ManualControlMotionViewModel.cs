using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;
using TopCom.Command;

namespace LOC.MVVM.ViewModels
{
    public class ManualControlMotionViewModel : PropertyChangedNotifier
    {
        private string _ModeToString = "Jog";
        private bool _IsModeJogControl = false;
        private double _INCPosition = 0.001;
        public string ModeToString
        {
            get
            {
                //OnPropertyChanged();
                return IsModeJogControl ? "Jog": "Relative";
            }
        }
        public bool IsModeJogControl
        {
            get
            {
                return _IsModeJogControl;
            }
            set
            {
                _IsModeJogControl = value;
                OnPropertyChanged();
                OnPropertyChanged("ModeToString");
            }
        }
        public double INCPosition
        {
            get
            {
                return _INCPosition;
            }
            set
            {
                _INCPosition = value;
            }
        }




        public RelayCommand ChangeModeControl
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    IsModeJogControl = !IsModeJogControl;
                    //OnPropertyChanged("IsModeJogControl");

                });
            }
        }
    }
}
