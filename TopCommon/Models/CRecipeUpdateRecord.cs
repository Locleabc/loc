using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom.Models
{
    public class CRecipeUpdateRecord : PropertyChangedNotifier, IIndexer
    {
        public int Index
        {
            get { return _Index; }
            set
            {
                if (_Index == value) return;

                _Index = value;
                OnPropertyChanged();
            }
        }

        public string Date
        {
            get { return _Date; }
            set
            {
                if (_Date == value) return;

                _Date = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                _Description = value;
                OnPropertyChanged();
            }
        }

        public string AxisName
        {
            get { return _AxisName; }
            set
            {
                if (_AxisName == value) return;

                _AxisName = value;
                OnPropertyChanged();
            }
        }

        public double OldValue
        {
            get { return _OldValue; }
            set
            {
                if (_OldValue == value) return;

                _OldValue = value;
                OnPropertyChanged();
            }
        }

        public double NewValue
        {
            get { return _NewValue; }
            set { _NewValue = value; }
        }

        #region Privates
        private int _Index;
        private string _Date;

        private string _Description;
        private string _AxisName;

        private double _OldValue;
        private double _NewValue;
        #endregion
    }
}
