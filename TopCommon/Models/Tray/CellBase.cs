using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TopCom;

namespace TopCom
{
    public class CellBase<T> : PropertyChangedNotifier, ICell<T> 
    {
        #region Properties
        public int Index
        {
            get { return _Index; }
            set
            {
                _Index = value;
                OnPropertyChanged();
            }
        }

        public T Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Privates
        private int _Index;
        private T _Status;
        #endregion
    }
}
