using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;
using TopCom.Command;

namespace TopVision.Models
{
    public class VisionParameterBase : PropertyChangedNotifier, IVisionParameter
    {
        #region Properties
        public double Threshold
        {
            get { return _Threshold; }
            set
            {
                if (_Threshold == value) return;

                _Threshold = value;
                OnPropertyChanged();
            }
        }

        public bool IsUseInputImageAsInput
        {
            get { return _IsUseInputImageAsInput; }
            set
            {
                _IsUseInputImageAsInput = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CRectangle> ROIs
        {
            get { return _ROIs; }
            set
            {
                if (_ROIs == value) return;

                _ROIs = value;
                OnPropertyChanged();
            }
        }

        public COutputMatOption OutputMatOption
        {
            get { return _OutputMatOption; }
            set
            {
                if (_OutputMatOption == value) return;

                _OutputMatOption = value;
                OnPropertyChanged();
            }
        }

        public double ThetaAdjust
        {
            get { return _ThetaAdjust; }
            set
            {
                if (_ThetaAdjust == value) return;

                _ThetaAdjust = value;
                OnPropertyChanged();
            }
        }

        public bool UseFixtureAlign
        {
            get { return _UseFixtureAlign; }
            set
            {
                if (_UseFixtureAlign == value) return;

                _UseFixtureAlign = value;
                OnPropertyChanged();
            }
        }

        public XYTOffset TeachingFixtureOffset
        {
            get { return _TeachingFixtureOffset; }
            set
            {
                if (_TeachingFixtureOffset == value) return;

                _TeachingFixtureOffset = value;
                OnPropertyChanged();
            }
        }

        public XYTOffset RuntimeFixtureOffset
        {
            get { return _RuntimeFixtureOffset; }
            set
            {
                if (_RuntimeFixtureOffset == value) return;

                _RuntimeFixtureOffset = value;
                OnPropertyChanged();
            }
        }

        public bool UseOffsetLimit
        {
            get { return _UseOffsetLimit; }
            set
            {
                if (_UseOffsetLimit == value) return;

                _UseOffsetLimit = value;
                OnPropertyChanged();
            }
        }

        public XYTOffset OffsetLimit
        {
            get { return _OffsetLimit; }
            set
            {
                if (_OffsetLimit == value) return;

                _OffsetLimit = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public virtual RelayCommand CRectangleAddCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o is ObservableCollection<CRectangle>)
                    {
                        (o as ObservableCollection<CRectangle>).Add(new CRectangle(0, 0, 0, 0));
                    }
                });
            }
        }

        public virtual RelayCommand CRectangleDeleteCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o is CRectangle)
                    {
                        CRectangle rect = o as CRectangle;

                        if (ROIs.Contains(rect))
                        {
                            ROIs.Remove(rect);
                        }
                    }
                });
            }
        }
        #endregion

        #region Privates
        private double _Threshold;
        private ObservableCollection<CRectangle> _ROIs = new ObservableCollection<CRectangle>();
        private COutputMatOption _OutputMatOption = new COutputMatOption();
        private double _ThetaAdjust = 1;
        private bool _IsUseInputImageAsInput = false;

        private bool _UseFixtureAlign = false;
        private XYTOffset _TeachingFixtureOffset = new XYTOffset();
        private XYTOffset _RuntimeFixtureOffset = new XYTOffset();

        private bool _UseOffsetLimit = false;
        private XYTOffset _OffsetLimit = new XYTOffset()
        {
            X = 99,
            Y = 99,
            Theta = 360,
        };
        #endregion
    }
}