using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;
using TopCom.Command;

namespace LOC.MVVM.ViewModels
{
    public class MainViewModel : PropertyChangedNotifier
    {
        #region Properties
        private object _ShowVM;

        public object ShowVM
        {
            get { return _ShowVM; }
            set
            {
                _ShowVM = value;
                OnPropertyChanged();
            }
        }

        public MainTabControlViewModel MainTabControlVM
        {
            get
            {
                return _MainTabControlVM ?? (_MainTabControlVM = new MainTabControlViewModel());
            }
        }

        //public LicenseActiveViewModel LicenseActiveVM
        //{
        //    get
        //    {
        //        return _LicenseActiveVM ?? (_LicenseActiveVM = new LicenseActiveViewModel());
        //    }
        //}

        public HeaderViewModel HeaderVM
        {
            get
            {
                return _HeaderVM ?? (_HeaderVM = new HeaderViewModel());
            }
        }

        public InitViewModel InitVM
        {
            get
            {
                return _InitVM ?? (_InitVM = new InitViewModel());
            }
        }

        public TerminateViewModel TerminateVM
        {
            get
            {
                return _TerminateVM ?? (_TerminateVM = new TerminateViewModel());
            }
        }

        public FooterViewModel FooterVM
        {
            get
            {
                return _FooterVM ?? (_FooterVM = new FooterViewModel());
            }
        }
        #endregion

        #region Commands
        public RelayCommand WindowLoadDone
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    
                        SwitchToInit();
                    
                });
            }
        }
        #endregion

        #region Methods
        //public void SwitchToLicenseActive()
        //{
        //    ShowVM = LicenseActiveVM;
        //}

        public void SwitchToInit()
        {
            ShowVM = InitVM;
            InitVM.Initialize();
        }

        public void SwitchToMainTabControl()
        {
            ShowVM = MainTabControlVM;
        }

        public void SwitchToTerminate()
        {
            ShowVM = TerminateVM;
            TerminateVM.Terminate();
        }
        #endregion

        #region Privates
        private MainTabControlViewModel _MainTabControlVM;

        private HeaderViewModel _HeaderVM;
        //private LicenseActiveViewModel _LicenseActiveVM;
        private InitViewModel _InitVM;
        private TerminateViewModel _TerminateVM;
        private FooterViewModel _FooterVM;
        #endregion
    }
}
