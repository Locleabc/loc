using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TopCom;
using TopCom.Command;
using TopUI.Controls;
using TopUI.Define;

namespace TopUI.Models
{
    public class TrayModelBase : PropertyChangedNotifier, ITrayModel
    {
        public CellClickHandler CellClickCallback { get; set; }

        private bool _IsEnable = true;
        public bool IsEnable
        {
            get { return _IsEnable; }
            set
            {
                if (_IsEnable == value) return;

                _IsEnable = value;
                OnPropertyChanged();
            }
        }

        public string Name { get; set; }

        private int rowCount;
        public int RowCount
        {
            get { return rowCount; }
            set
            {
                if (rowCount != value)
                {
                    rowCount = value;
                    OnPropertyChanged("RowCount");
                    //InitCells();
                }
            }
        }

        private int columnCount;
        public int ColumnCount
        {
            get { return columnCount; }
            set
            {
                if (columnCount != value)
                {
                    columnCount = value;
                    OnPropertyChanged("ColumnCount");
                    //InitCells();
                }
            }
        }

        public ObservableCollection<TrayCell> Cells { get; set; }

        private ECellShape shape;
        public ECellShape Shape
        {
            get { return shape; }
            set
            {
                if (shape != value)
                {
                    shape = value;
                    OnPropertyChanged("Shape");
                    //InitCells();
                }
            }
        }

        private EStartPosition startPosition = EStartPosition.BottomRight;
        public EStartPosition StartPosition
        {
            get { return startPosition; }
            set
            {
                startPosition = value;
            }
        }

        public int FirstIndex { get { return 1; } }

        private int workStartIndex;
        /// <summary>
        /// Index of current working cell
        /// </summary>
        public int WorkStartIndex
        {
            get { return workStartIndex; }
            set
            {
                if (Cells.Count > 0)
                {
                    int startWorkCell = value - this.FirstIndex;
                    startWorkCell /= HeadCount;
                    startWorkCell *= HeadCount;
                    startWorkCell += this.FirstIndex;

                    workStartIndex = startWorkCell;

                    if (WorkIndexInRage)
                    {
                        //this.SetAllCell(TopUI.Define.ECellStatus.Empty);
                        //this.SetToCell(TopUI.Define.ECellStatus.OK, workStartIndex);
                        this.SetSetOfCell(TopUI.Define.ECellStatus.Processing, workStartIndex);
                    }
                }
            }
        }

        public int WorkStartIndex_X
        {
            get
            {
                return (((WorkStartIndex - 1) / HeadCount)) % (ColumnCount / HeadCount);
            }
        }

        public int WorkStartIndex_Y
        {
            get
            {
                return (WorkStartIndex - FirstIndex) / ColumnCount;
            }
        }

        public bool WorkIndexInRage
        {
            get { return this.workStartIndex <= this.Cells.Count - (1 - this.FirstIndex); }
        }

        public int HeadCount { get { return 2; } }

        public void InitCells()
        {
            ObservableCollection<TrayCell> cells = new ObservableCollection<TrayCell>();

            int r = 0;
            int c = 0;

            int id = 0;

            switch (StartPosition)
            {
                case EStartPosition.TopLeft:
                    for (r = 0; r < this.RowCount; r++)
                    {
                        for (int i = 0; i < this.HeadCount; i++)
                        {
                            for (c = 0; c < this.ColumnCount / this.HeadCount; c++)
                            {
                                id++;
                                cells.Add(new TrayCell
                                {
                                    CellInfo = new TrayCellBase
                                    {
                                        CellIndex = this.FirstIndex + i
                                                + r * this.ColumnCount
                                                + c * this.HeadCount,
                                        CellID = id,
                                    }
                                });
                            }
                        }
                    }
                    break;
                case EStartPosition.TopRight:
                    for (r = 0; r < this.RowCount; r++)
                    {
                        for (int i = this.HeadCount - 1; i >= 0; i--)
                        {
                            for (c = this.ColumnCount / this.HeadCount - 1; c >= 0; c--)
                            {
                                id++;
                                cells.Add(new TrayCell
                                {
                                    CellInfo = new TrayCellBase
                                    {
                                        CellIndex = this.FirstIndex + i
                                                + r * this.ColumnCount
                                                + c * this.HeadCount,
                                        CellID = id,
                                    }
                                });
                            }
                        }
                    }
                    break;
                case EStartPosition.BottomLeft:
                    for (r = this.RowCount - 1; r >= 0; r--)
                    {
                        for (int i = 0; i < this.HeadCount; i++)
                        {
                            for (c = 0; c < this.ColumnCount / this.HeadCount; c++)
                            {
                                id++;
                                cells.Add(new TrayCell
                                {
                                    CellInfo = new TrayCellBase
                                    {
                                        CellIndex = this.FirstIndex + i
                                                + r * this.ColumnCount
                                                + c * this.HeadCount,
                                        CellID = id,
                                    }
                                });
                            }
                        }
                    }
                    break;
                case EStartPosition.BottomRight:
                    for (r = this.RowCount - 1; r >= 0; r--)
                    {
                        for (int i = this.HeadCount - 1; i >= 0; i--)
                        {
                            for (c = this.ColumnCount / this.HeadCount - 1; c >= 0; c--)
                            {
                                id++;
                                cells.Add(new TrayCell
                                {
                                    CellInfo = new TrayCellBase
                                    {
                                        CellIndex = this.FirstIndex + i
                                                + r * this.ColumnCount
                                                + c * this.HeadCount,
                                        CellID = id,
                                    }
                                });
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            foreach (TrayCell cell in cells)
            {
                cell.CellInfo.CellShape = this.Shape;
                cell.CellClicked += Cell_CellClicked;
                cell.CellDoubleClicked += Cell_CellDoubleClicked;
            }

            this.Cells = cells;

            WorkStartIndex = FirstIndex;
        }

        private void Cell_CellClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            CellClickCallback(this, (sender as TrayCell).CellInfo.CellIndex, Keyboard.Modifiers);
        }

        private void Cell_CellDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            CellClickCallback(this, (sender as TrayCell).CellInfo.CellIndex, Keyboard.Modifiers, true);
        }

        public void SetAllCell(ECellStatus status)
        {
            foreach (TrayCell cell in Cells)
            {
                cell.CellInfo.CellStatus = status;
            }
        }

        public void SetToCell(ECellStatus status, int cellIndex)
        {
            List<TrayCell> cells = Cells.Where(cell => cell.CellInfo.CellIndex <= cellIndex).ToList();

            foreach (TrayCell cell in cells)
            {
                cell.CellInfo.CellStatus = status;
            }
        }

        public void SetSingleCell(ECellStatus status, int cellIndex)
        {
            try
            {
                Cells.First(cell => cell.CellInfo.CellIndex == cellIndex).CellInfo.CellStatus = status;
            }
            catch
            {
                throw new ArgumentOutOfRangeException("Cell is not exist on tray");
            }
        }

        public void ToggleCell(int cellIndex)
        {
            TrayCell cell = Cells.First(c => c.CellInfo.CellIndex == cellIndex);

            switch (cell.CellInfo.CellStatus)
            {
                case ECellStatus.Empty:
                    cell.CellInfo.CellStatus = ECellStatus.OK;
                    break;
                default:
                    cell.CellInfo.CellStatus = ECellStatus.Empty;
                    break;
            }
        }

        public void SetSetOfCell(ECellStatus status, int cellIndex)
        {
            // DO NOT CHANGE THIS FORMULAS
            // Each formular has it's own meaning
            int startCell = cellIndex - this.FirstIndex;
            startCell /= HeadCount;
            startCell *= HeadCount;
            startCell += this.FirstIndex;

            for (int i = 0; i < HeadCount; i++)
            {
                SetSingleCell(status, startCell + i);
            }
        }

        public RelayCommand ResetCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    SetAllCell(ECellStatus.Empty);
                    WorkStartIndex = FirstIndex;
                });
            }
        }

        public RelayCommand ReverseEnableStatusCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (IsEnable)
                    {
                        IsEnable = false;
                        ResetCommand.Execute(null);
                    }
                    else
                    {
                        IsEnable = true;
                    }
                });
            }
        }

        public ECellStatus GetCellStatus(int cellIndex)
        {
            TrayCell cell = Cells.First(c => c.CellInfo.CellIndex == cellIndex);

            return cell.CellInfo.CellStatus;
        }
    }
}
