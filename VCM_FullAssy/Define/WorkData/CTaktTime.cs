using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace VCM_FullAssy.Define
{
    public class CTaktTime : PropertyChangedNotifier
    {
        #region Properties
        private Queue<double> _Last30EATaktTime = new Queue<double>();

        public Queue<double> Last30EATaktTime
        {
            get { return _Last30EATaktTime; }
            set
            {
                _Last30EATaktTime = value;
                OnPropertyChanged();
            }
        }

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
        public double SingleCycle
        {
            get { return _SingleCycle; }
            set
            {
                if (_SingleCycle == value) return;

                _SingleCycle = value;
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

        public CVisionTakt LoadVision
        {
            get { return _LoadVision; }
            set
            {
                if (_LoadVision == value) return;

                _LoadVision = value;
                OnPropertyChanged();
            }
        }

        public CVisionTakt BotVision
        {
            get { return _BotVision; }
            set
            {
                if (_BotVision == value) return;

                _BotVision = value;
                OnPropertyChanged();
            }
        }

        public CVisionTakt UnloadVision
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
        private double _SingleCycle;
        private double _Place;

        private CVisionTakt _LoadVision = new CVisionTakt();
        private CVisionTakt _BotVision = new CVisionTakt();
        private CVisionTakt _UnloadVision = new CVisionTakt();
        #endregion
    }
}
