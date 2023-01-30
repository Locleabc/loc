using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TopCom;
using TopCom.Command;
using TOPV_Dispenser.Define;

namespace TOPV_Dispenser.MVVM.ViewModels
{
    public class MessageWindowViewModel : PropertyChangedNotifier
    {
        #region Properties
        public event EventHandler ModalChangedEvent;

        public bool Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
                OnPropertyChanged();
            }
        }

        public bool ConfirmMode
        {
            get { return _ConfirmMode; }
            set
            {
                if (_ConfirmMode == value) return;

                _ConfirmMode = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisibility
        {
            get { return _IsVisibility; }
            set
            {
                _IsVisibility = value;
                OnPropertyChanged();
            }
        }

        public string Caption
        {
            get { return _Caption; }
            set
            {
                _Caption = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                OnPropertyChanged();

                if (string.IsNullOrEmpty(value) == false)
                {
                    IsVisibility = true;
                }
            }
        }

        public Brush CaptionBrush
        {
            get
            {
                if (!IsAlarm) return (SolidColorBrush)new BrushConverter().ConvertFromString("#24292e");
                else return Brushes.Red;
            }
        }

        private bool _IsAlarm = false;
        public bool IsAlarm
        {
            get
            {
                return _IsAlarm;
            }
            set
            {
                if (_IsAlarm == value) return;

                _IsAlarm = value;
                OnPropertyChanged();
                OnPropertyChanged("CaptionBrush");
            }
        }
        #endregion

        #region Commands
        public RelayCommand ConfirmCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    Result = true;
                    IsVisibility = false;
                });
            }
        }

        public RelayCommand CancelCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    Result = false;
                    IsVisibility = false;
                });
            }
        }
        #endregion

        #region Methods
        public void Show(string message, bool isAlarm = false, string caption = "Confirm")
        {
            Message = message;
            ConfirmMode = false;
            IsAlarm = isAlarm;
            Caption = caption;

            ModalChangedEvent?.Invoke(false, EventArgs.Empty);
        }

        public void ShowDialog(string message, bool isAlarm = false, string caption = "Confirm")
        {
            ConfirmMode = true;
            Message = message;
            IsAlarm = isAlarm;
            Caption = caption;

            Result = false;
            ModalChangedEvent?.Invoke(true, EventArgs.Empty);
        }
        #endregion

        #region Privates
        private bool _ConfirmMode = false;
        private bool _IsVisibility = false;
        private string _Caption = "Confirm";
        private string _Message = "";
        private bool _Result;
        #endregion
    }
}
