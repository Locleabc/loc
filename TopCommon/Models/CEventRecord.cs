using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopCom.Models
{
    public class CEventRecord : PropertyChangedNotifier, IIndexer
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

        #region Privates
        private int _Index;
        private string _Date;

        private string _Description;
        #endregion
    }
}
