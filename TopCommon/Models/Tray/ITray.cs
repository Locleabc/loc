using System;
using System.Collections.ObjectModel;
using System.Linq;
using TopCom;

namespace TopCom
{
    public interface ITray<T> 
    {
        string Name { get; set; }
        int RowCount { get; set; }
        int ColumnCount { get; set; }
        EStartPosition StartPosition { get; set; }

        ObservableCollection<CellBase<T>> Cells { get; set; }

        Type GetCellType();

        void GenerateCells();
    }

    public class TrayBase<T> : PropertyChangedNotifier, ITray<T>
    {
        #region Privates
        private string _Name;
        private int _RowCount;
        private int _ColumnCount;
        private EStartPosition _StartPosition;
        private ObservableCollection<CellBase<T>> _Cells;
        #endregion

        #region Properties
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        public int RowCount
        {
            get { return _RowCount; }
            set
            {
                _RowCount = value;
                OnPropertyChanged();
            }
        }

        public int ColumnCount
        {
            get { return _ColumnCount; }
            set
            {
                _ColumnCount = value;
                OnPropertyChanged();
            }
        }

        public EStartPosition StartPosition
        {
            get { return _StartPosition; }
            set
            {
                _StartPosition = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CellBase<T>> Cells
        {
            get { return _Cells; }
            set
            {
                _Cells = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public Type GetCellType()
        {
            return typeof(T);
        }

        public ICell<T> GetCell(int index)
        {
            return _Cells[index - 1];
        }

        public void GenerateCells()
        {
            Cells = new ObservableCollection<CellBase<T>>();

            switch (StartPosition)
            {
                case EStartPosition.TopLeft:
                    for (int r = 1; r <= RowCount; r++)
                    {
                        for (int c = 1; c <= ColumnCount; c++)
                        {
                            Cells.Add(new CellBase<T> { Index = ColumnCount * (r - 1) + c });
                        }
                    }
                    break;
                case EStartPosition.TopRight:
                    for (int r = 1; r <= RowCount; r++)
                    {
                        for (int c = ColumnCount; c >= 1; c--)
                        {
                            Cells.Add(new CellBase<T> { Index = ColumnCount * (r - 1) + c });
                        }
                    }
                    break;
                case EStartPosition.BottomLeft:
                    for (int r = RowCount; r >= 1; r--)
                    {
                        for (int c = 1; c <= ColumnCount; c++)
                        {
                            Cells.Add(new CellBase<T> { Index = ColumnCount * (r - 1) + c });
                        }
                    }
                    break;
                case EStartPosition.BottomRight:
                    for (int r = RowCount; r >= 1; r--)
                    {
                        for (int c = ColumnCount; c >= 1; c--)
                        {
                            Cells.Add(new CellBase<T> { Index = ColumnCount * (r - 1) + c });
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public bool IsRowContain(int row, T _status)
        {
            return Cells.Any(c => c.Index >= (row - 1) * ColumnCount + 1
                          && c.Index <= row * ColumnCount
                          && c.Status.Equals(_status));
        }

        /// <summary>
        /// Is row contain any cell with Status is not _status
        /// </summary>
        /// <param name="row"></param>
        /// <param name="_status"></param>
        /// <returns></returns>
        public bool IsRowContainAnotherThan(int row, T _status)
        {
            return Cells.Any(c => c.Index >= (row - 1) * ColumnCount + 1
                          && c.Index <= row * ColumnCount
                          && !c.Status.Equals(_status));
        }

        public void SetRow(int row, T status)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                Cells[(row - 1) * ColumnCount + i].Status = status;
            }
        }

        public void SetAllTray(T status)
        {
            foreach(CellBase<T> cell in Cells)
            {
                cell.Status = status;
            }
        }
        #endregion
    }
}
