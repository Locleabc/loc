﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TopCom;
using TopCom.Command;
using TopCom.Processing;
using TopCom.LOG;
using TopUI.Controls;
using TopUI.Models;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class MainWindowViewModel : PropertyChangedNotifier
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

        public MainContentViewModel MainContentVM
        {
            get
            {
                return _MainContentVM ?? (_MainContentVM = new MainContentViewModel());
            }
        }

        public LicenseActiveViewModel LicenseActiveVM
        {
            get
            {
                return _LicenseActiveVM ?? (_LicenseActiveVM = new LicenseActiveViewModel());
            }
        }

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
                    if (LicenseActiveVM.IsValid)
                    {
                        SwitchToInit();
                    }
                    else
                    {
                        SwitchToLicenseActive();
                    }
                });
            }
        }
        #endregion

        #region Methods
        public void SwitchToLicenseActive()
        {
            ShowVM = LicenseActiveVM;
        }

        public void SwitchToInit()
        {
            ShowVM = InitVM;
            InitVM.Initialize();
        }

        public void SwitchToMainContent()
        {
            ShowVM = MainContentVM;
        }

        public void SwitchToTerminate()
        {
            ShowVM = TerminateVM;
            TerminateVM.Terminate();
        }
        #endregion

        #region Privates
        private MainContentViewModel _MainContentVM;

        private HeaderViewModel _HeaderVM;
        private LicenseActiveViewModel _LicenseActiveVM;
        private InitViewModel _InitVM;
        private TerminateViewModel _TerminateVM;
        private FooterViewModel _FooterVM;
        #endregion
    }
}
