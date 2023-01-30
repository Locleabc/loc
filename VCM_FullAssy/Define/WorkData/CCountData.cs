using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace VCM_FullAssy.Define
{
    public class CCountData : PropertyChangedNotifier
    {
        #region Properties
        public uint Total
        {
            get { return _Total; }
            set
            {
                if (_Total == value) return;

                _Total = value;
                OnPropertyChanged();
            }
        }

        public uint OK
        {
            get { return _OK; }
            set
            {
                if (_OK == value) return;

                _OK = value;
                OnPropertyChanged();
            }
        }

        public uint PickNG
        {
            get { return _PickNG; }
            set
            {
                if (_PickNG == value) return;

                _PickNG = value;
                OnPropertyChanged();
            }
        }

        public uint PlaceNG
        {
            get { return _PlaceNG; }
            set
            {
                if (_PlaceNG == value) return;

                _PlaceNG = value;
                OnPropertyChanged();
            }
        }

        public uint LoadVisionNG
        {
            get { return _LoadVisionNG; }
            set
            {
                if (_LoadVisionNG == value) return;

                _LoadVisionNG = value;
                OnPropertyChanged();
            }
        }

        public uint UnderVisionNG
        {
            get { return _BotVisionNG; }
            set
            {
                if (_BotVisionNG == value) return;

                _BotVisionNG = value;
                OnPropertyChanged();
            }
        }

        public uint UnloadVisionNG
        {
            get { return _UnloadVisionNG; }
            set
            {
                if (_UnloadVisionNG == value) return;

                _UnloadVisionNG = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private uint _Total;
        private uint _OK;
        private uint _PickNG;
        private uint _PlaceNG;

        private uint _LoadVisionNG;
        private uint _BotVisionNG;
        private uint _UnloadVisionNG;
        #endregion
    }
}
