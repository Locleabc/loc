using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace VCM_CoilLoading.Define
{
    public class CTaktTime : PropertyChangedNotifier
    {
        #region Properties
        public double Total
        {
            get { return _Total; }
            set
            {
                if (_Total == value) return;

                _Total = value;
                OnPropertyChanged();
            }
        }

        public double Pick
        {
            get { return _Pick; }
            set
            {
                if (_Pick == value) return;

                _Pick = value;
                OnPropertyChanged();
            }
        }

        // Under vision moving + process takt time
        public double Vision
        {
            get { return _Vision; }
            set
            {
                if (_Vision == value) return;

                _Vision = value;
                OnPropertyChanged();
            }
        }

        public double Place
        {
            get { return _Place; }
            set
            {
                if (_Place == value) return;

                _Place = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CVisionTakt> LoadVision
        {
            get { return _LoadVision; }
            set
            {
                if (_LoadVision == value) return;

                _LoadVision = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CVisionTakt> BotVision
        {
            get { return _BotVision; }
            set
            {
                if (_BotVision == value) return;

                _BotVision = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CVisionTakt> UnloadVision
        {
            get { return _UnloadVision; }
            set
            {
                if (_UnloadVision == value) return;

                _UnloadVision = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties

        #region Privates
        private double _Total;
        private double _Pick;
        private double _Vision;
        private double _Place;

        private ObservableCollection<CVisionTakt> _LoadVision = new ObservableCollection<CVisionTakt> { new CVisionTakt(), new CVisionTakt() };
        private ObservableCollection<CVisionTakt> _BotVision = new ObservableCollection<CVisionTakt> { new CVisionTakt(), new CVisionTakt() };
        private ObservableCollection<CVisionTakt> _UnloadVision = new ObservableCollection<CVisionTakt> { new CVisionTakt(), new CVisionTakt() };
        #endregion
    }
}
