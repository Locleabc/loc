using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TopCom;
using TopCom.Command;

namespace VCM_CoilLoading.MVVM.ViewModels
{
    public class LicenseActiveViewModel : PropertyChangedNotifier
    {
        #region Properties
        public bool IsValid
        {
            get { return /*TopLisense.CryptoService.IsValid*/ true; }
        }

        public bool IsNotValid
        {
            get { return !IsValid; }
        }

        public string ValidStatus
        {
            get
            {
                return IsValid ? "ACTIVED" : "INACTIVE";
            }
        }

        public string ValidDetail
        {
            get
            {
                return IsValid ? "Your license is successfully activated." : "Your license is not valid, contact suply vendor to get a valid license.";
            }
        }

        public Brush ValidBrush
        {
            get
            {
                return IsValid ? Brushes.Green : Brushes.Red;
            }
        }

        public string UserSerialNumber
        {
            get
            {
                var random = new Random();
                return TopLicense.CryptoService.MyIdentityList[random.Next(TopLicense.CryptoService.MyIdentityList.Count)];
            }
        }

        public string LicenseKey
        {
            get
            {
                return _LicenseKey;
            }
            set
            {
                if (_LicenseKey == value) return;

                _LicenseKey = value;
                OnPropertyChanged();
                OnPropertyChanged("IsValid");
                OnPropertyChanged("IsNotValid");
            }
        }
        #endregion

        #region Commands
        public RelayCommand SerialNumberCopyCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    Clipboard.SetText(UserSerialNumber);
                });
            }
        }

        public RelayCommand ActiveLicenseCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    TopLicense.CryptoService.RegistrationLicense(LicenseKey);

                    if (IsValid == false)
                    {
                        MessageBox.Show("Your license is not valid. Check your license and try again.");
                    }
                    else
                    {
                        MessageBox.Show("Your license is successfully actived.");
                    }

                    OnPropertyChanged("IsValid");
                    OnPropertyChanged("IsNotValid");
                    OnPropertyChanged("ValidStatus");
                    OnPropertyChanged("ValidDetail");
                    OnPropertyChanged("ValidBrush");
                });
            }
        }
        #endregion

        #region Privates
        private string _LicenseKey;
        #endregion
    }
}
