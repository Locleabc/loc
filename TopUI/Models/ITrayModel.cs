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
    public delegate void CellClickHandler(ITrayModel tray, int clickedCellIndex, ModifierKeys key, bool isDoubleClicked = false);

    public interface ITrayModel
    {
        [JsonIgnore]
        CellClickHandler CellClickCallback { get; set; }

        bool IsEnable { get; set; }

        string Name { get; set; }
        int RowCount { get; set; }
        int ColumnCount { get; set; }
        ObservableCollection<TrayCell> Cells { get; set; }

        ECellShape Shape { get; set; }
        EStartPosition StartPosition { get; set; }
        int HeadCount { get; }
        /// <summary>
        /// Index number of first cell, should be 0 or 1 ONLY
        /// </summary>
        int FirstIndex { get; }
        int WorkStartIndex { get; set; }
        /// <summary>
        /// Start with 0 -> (ColumnCount / HeadCount - 1)
        /// </summary>
        int WorkStartIndex_X { get; }
        /// <summary>
        /// Start with 0 -> RowCount
        /// </summary>
        int WorkStartIndex_Y { get; }

        [JsonIgnore]
        bool WorkIndexInRage { get; }

        void InitCells();
        void SetAllCell(ECellStatus status);
        /// <summary>
        /// Set all cell before cellIndex to passed status (include the cellIndex)
        /// </summary>
        /// <param name="status">Status to set</param>
        /// <param name="cellIndex">Cell Index</param>
        void SetToCell(ECellStatus status, int cellIndex);
        void SetSingleCell(ECellStatus status, int cellIndex);
        /// <summary>
        /// Set cells with same set, it could be a set of n cell for n head
        /// </summary>
        /// <param name="status">Status to set</param>
        /// <param name="cellIndex">Any cell index of the set</param>
        void SetSetOfCell(ECellStatus status, int cellIndex);
        void ToggleCell(int cellIndex);

        ECellStatus GetCellStatus(int cellIndex);

        /// <summary>
        /// Reset all cells status, set first cell as WorkStart Cell
        /// </summary>
        RelayCommand ResetCommand { get; }

        RelayCommand ReverseEnableStatusCommand { get; }
    }
}
