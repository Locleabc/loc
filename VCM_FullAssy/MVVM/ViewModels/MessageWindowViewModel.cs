using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;
using TopCom.Command;
using VCM_FullAssy.Define;

namespace VCM_FullAssy.MVVM.ViewModels
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
        public void Show(string message)
        {
            Message = message;
            ConfirmMode = false;

            ModalChangedEvent?.Invoke(false, EventArgs.Empty);
        }

        public void ShowDialog(string message)
        {
            ConfirmMode = true;
            Message = message;

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
