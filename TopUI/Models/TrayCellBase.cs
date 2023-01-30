using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopCom;
using TopUI.Define;

namespace TopUI.Models
{
    public class TrayCellBase : PropertyChangedNotifier, ITrayCellModel
    {
        private ECellStatus cellStatus;
        public ECellStatus CellStatus
        {
            get { return cellStatus; }
            set
            {
                cellStatus = value;
                OnPropertyChanged("CellStatus");
            }
        }

        private ECellShape cellShape;
        public ECellShape CellShape
        {
            get { return cellShape; }
            set
            {
                cellShape = value;
                OnPropertyChanged("CellShape");
            }
        }

        private int cellIndex;
        public int CellIndex
        {
            get { return cellIndex; }
            set
            {
                cellIndex = value;
                OnPropertyChanged("CellIndex");
            }
        }

        private int cellID;
        public int CellID
        {
            get { return cellID; }
            set
            {
                cellID = value;
            }
        }

        public TrayCellBase()
        {
        }
    }
}
