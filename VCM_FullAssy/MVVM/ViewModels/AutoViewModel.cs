using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TopCom;
using TopCom.Command;
using TopCom.Processing;
using TopUI.Controls;
using TopUI.Models;
using VCM_FullAssy.Define;
using VCM_FullAssy.Processing;

namespace VCM_FullAssy.MVVM.ViewModels
{
    public class AutoViewModel : PropertyChangedNotifier
    {
        #region Properties
        public MESViewModel MESVM
        {
            get
            {
                return _MESVM ?? (_MESVM = new MESViewModel());
            }
        }

        public WorkDataViewModel WorkDataVM
        {
            get
            {
                return _WorkDataVM ?? (_WorkDataVM = new WorkDataViewModel());
            }
        }
        #endregion

        #region Commands
        public RelayCommand ResetCountDataCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    Datas.WorkData.Reset();
                });
            }
        }

        public RelayCommand ResetLeftTrayCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CTray.LoadTray1.ResetCommand.Execute(null);
                    CTray.LoadTray2.ResetCommand.Execute(null);
                    CTray.UnloadTray1.ResetCommand.Execute(null);
                    CTray.UnloadTray2.ResetCommand.Execute(null);
                });
            }
        }

        public RelayCommand ResetRightTrayCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CTray.LoadTray3.ResetCommand.Execute(null);
                    CTray.LoadTray4.ResetCommand.Execute(null);
                    CTray.UnloadTray3.ResetCommand.Execute(null);
                    CTray.UnloadTray4.ResetCommand.Execute(null);
                });
            }
        }

        public RelayCommand LeftTrayChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.RootProcess.LeftTrayProcess.InWorking = true;
                    CDef.RootProcess.RightTrayProcess.InWorking = false;

                    CDef.RootProcess.RunMode = ERunMode.Manual_TrayChange;
                    CDef.RootProcess.OperationCommand = OperatingMode.Run;
                });
            }
        }

        public RelayCommand RightTrayChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.RootProcess.LeftTrayProcess.InWorking = false;
                    CDef.RootProcess.RightTrayProcess.InWorking = true;

                    CDef.RootProcess.RunMode = ERunMode.Manual_TrayChange;
                    CDef.RootProcess.OperationCommand = OperatingMode.Run;
                });
            }
        }
        #endregion

        #region Constructors
        public AutoViewModel()
        {
            InitAllTray();
        }

        public ITrayModel InitTray(string JsonFile)
        {
            if (File.Exists(JsonFile) == false) throw new FileNotFoundException($"{JsonFile} not found");

            string trayBackupString = File.ReadAllText(JsonFile);
            TrayModelBase tmp = JsonConvert.DeserializeObject<TrayModelBase>(trayBackupString);

            if (tmp == null) throw new FileNotFoundException($"{JsonFile} is empty or wrong format");

            ITrayModel Tray = new TrayModelBase
            {
                RowCount = tmp.RowCount,
                ColumnCount = tmp.ColumnCount,
                Shape = tmp.Cells[0].CellInfo.CellShape,
                StartPosition = tmp.StartPosition,
                Name = tmp.Name,
            };
            Tray.InitCells();
            Tray.WorkStartIndex = tmp.WorkStartIndex;

            for (int i = 0; i < Tray.Cells.Count; i++)
            {
                int cellID = Tray.Cells[i].CellInfo.CellID;

                TrayCell cell = tmp.Cells.First(c => c.CellInfo.CellID == cellID);
                if (cell != null)
                {
                    Tray.Cells[i].CellInfo.CellStatus = cell.CellInfo.CellStatus;
                }
            }

            return Tray;
        }

        public void InitAllTray()
        {
            try
            {
                CTray.LoadTray1 = InitTray("LoadTray1.json");
            }
            catch
            {
                CTray.LoadTray1 = new TrayModelBase
                {
                    RowCount = 6,
                    ColumnCount = 5,
                    Shape = TopUI.Define.ECellShape.Rectangle,
                    Name = "LoadTray1",
                };
                CTray.LoadTray1.InitCells();
            }

            try
            {
                CTray.LoadTray2 = InitTray("LoadTray2.json");
            }
            catch
            {
                CTray.LoadTray2 = new TrayModelBase
                {
                    RowCount = 6,
                    ColumnCount = 5,
                    Shape = TopUI.Define.ECellShape.Rectangle,
                    Name = "LoadTray2",
                };
                CTray.LoadTray2.InitCells();
            }

            try
            {
                CTray.LoadTray3 = InitTray("LoadTray3.json");
            }
            catch
            {
                CTray.LoadTray3 = new TrayModelBase
                {
                    RowCount = 6,
                    ColumnCount = 5,
                    Shape = TopUI.Define.ECellShape.Rectangle,
                    Name = "LoadTray3",
                };
                CTray.LoadTray3.InitCells();
            }

            try
            {
                CTray.LoadTray4 = InitTray("LoadTray4.json");
            }
            catch
            {
                CTray.LoadTray4 = new TrayModelBase
                {
                    RowCount = 6,
                    ColumnCount = 5,
                    Shape = TopUI.Define.ECellShape.Rectangle,
                    Name = "LoadTray4",
                };
                CTray.LoadTray4.InitCells();
            }

            try
            {
                CTray.UnloadTray1 = InitTray("UnloadTray1.json");
            }
            catch
            {
                CTray.UnloadTray1 = new TrayModelBase
                {
                    RowCount = 6,
                    ColumnCount = 5,
                    Shape = TopUI.Define.ECellShape.Rectangle,
                    Name = "UnloadTray1",
                };
                CTray.UnloadTray1.InitCells();
            }

            try
            {
                CTray.UnloadTray2 = InitTray("UnloadTray2.json");
            }
            catch
            {
                CTray.UnloadTray2 = new TrayModelBase
                {
                    RowCount = 6,
                    ColumnCount = 5,
                    Shape = TopUI.Define.ECellShape.Rectangle,
                    Name = "UnloadTray2",
                };
                CTray.UnloadTray2.InitCells();
            }

            try
            {
                CTray.UnloadTray3 = InitTray("UnloadTray3.json");
            }
            catch
            {
                CTray.UnloadTray3 = new TrayModelBase
                {
                    RowCount = 6,
                    ColumnCount = 5,
                    Shape = TopUI.Define.ECellShape.Rectangle,
                    Name = "UnloadTray3",
                };
                CTray.UnloadTray3.InitCells();
            }

            try
            {
                CTray.UnloadTray4 = InitTray("UnloadTray4.json");
            }
            catch
            {
                CTray.UnloadTray4 = new TrayModelBase
                {
                    RowCount = 6,
                    ColumnCount = 5,
                    Shape = TopUI.Define.ECellShape.Rectangle,
                    Name = "UnloadTray4",
                };
                CTray.UnloadTray4.InitCells();
            }

            (CTray.LoadTray1 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;
            (CTray.LoadTray2 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;
            (CTray.LoadTray3 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;
            (CTray.LoadTray4 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;

            (CTray.UnloadTray1 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;
            (CTray.UnloadTray2 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;
            (CTray.UnloadTray3 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;
            (CTray.UnloadTray4 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;
        }
        #endregion

        #region Methods
        private void OnTrayCellClickCallback(ITrayModel tray, int clickedCellIndex, ModifierKeys key, bool isDoubleClicked)
        {
            if (isDoubleClicked)
            {
                tray.SetAllCell(TopUI.Define.ECellStatus.Empty);
                tray.SetToCell(TopUI.Define.ECellStatus.OK, clickedCellIndex - 1);
                tray.WorkStartIndex = clickedCellIndex;
            }
            else
            {
                if (tray.WorkStartIndex <= clickedCellIndex)
                {
                    tray.ToggleCell(clickedCellIndex);
                }
            }
        }
        #endregion

        #region Privates
        private MESViewModel _MESVM;
        private WorkDataViewModel _WorkDataVM;
        #endregion
    }
}
