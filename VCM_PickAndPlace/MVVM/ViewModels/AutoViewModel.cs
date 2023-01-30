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
using TopCom.LOG;
using TopUI.Controls;
using TopUI.Models;
using VCM_PickAndPlace.Define;
using VCM_PickAndPlace.Processing;

namespace VCM_PickAndPlace.MVVM.ViewModels
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

        public RelayCommand ResetLoadingTrayCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {

                    foreach(ITrayModel tray in CDef.LoadingTrays)
                    {
                        tray.ResetCommand.Execute(null);
                    }
                });
            }
        }

        public RelayCommand ResetUnloadingTrayCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    foreach (ITrayModel tray in CDef.UnloadingTrays)
                    {
                        tray.ResetCommand.Execute(null);
                    }
                });
            }
        }

        public RelayCommand LoadingTrayChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.RootProcess.LoadTrayProcess.ManualTrayChange = true;
                    CDef.RootProcess.UnloadTrayProcess.ManualTrayChange = false;
                    CDef.RootProcess.RunMode = ERunMode.Manual_TrayChange;
                    CDef.RootProcess.OperationCommand = OperatingMode.Run;
                });
            }
        }

        public RelayCommand UnloadingTrayChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.RootProcess.LoadTrayProcess.ManualTrayChange = false;
                    CDef.RootProcess.UnloadTrayProcess.ManualTrayChange = true;
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

        public void InitAllTray()
        {
            ObservableCollection<TrayModelBase> tmpTrays = new ObservableCollection<TrayModelBase>();

            try
            {
                string trayBackupString = File.ReadAllText("TrayLoadings.json");
                CDef.LoadingTrays = JsonConvert.DeserializeObject<ObservableCollection<TrayModelBase>>(trayBackupString);
                tmpTrays = JsonConvert.DeserializeObject<ObservableCollection<TrayModelBase>>(trayBackupString);
            }
            catch
            {
                CDef.LoadingTrays = new ObservableCollection<TrayModelBase>()
                {
                    new TrayModelBase
                    {
                        RowCount = 6,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "LoadingTray1"
                    },
                    new TrayModelBase
                    {
                        RowCount = 6,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "LoadingTray2"
                    }
                };
                tmpTrays = new ObservableCollection<TrayModelBase>()
                {
                    new TrayModelBase
                    {
                        RowCount = 6,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "LoadingTray1"
                    },
                    new TrayModelBase
                    {
                        RowCount = 6,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "LoadingTray2"
                    }
                };
                tmpTrays[0].InitCells();
                tmpTrays[1].InitCells();
            }

            for (int i = 0; i < CDef.LoadingTrays.Count; i++)
            {
                int workStartIndex = CDef.LoadingTrays[i].WorkStartIndex;
                CDef.LoadingTrays[i].InitCells();
                CDef.LoadingTrays[i].WorkStartIndex = workStartIndex;
                CDef.LoadingTrays[i].CellClickCallback += OnTrayCellClickCallback;

                for (int j = 0; j < CDef.LoadingTrays[i].Cells.Count; j++)
                {
                    int cellID = CDef.LoadingTrays[i].Cells[j].CellInfo.CellID;

                    TrayCell cell = tmpTrays[i].Cells.First(c => c.CellInfo.CellID == cellID);
                    if (cell != null)
                    {
                        CDef.LoadingTrays[i].Cells[j].CellInfo.CellStatus = cell.CellInfo.CellStatus;
                    }
                }
            }

            try
            {
                string trayBackupString = File.ReadAllText("TrayUnloadings.json");
                CDef.UnloadingTrays = JsonConvert.DeserializeObject<ObservableCollection<TrayModelBase>>(trayBackupString);
                tmpTrays = JsonConvert.DeserializeObject<ObservableCollection<TrayModelBase>>(trayBackupString);
            }
            catch
            {
                CDef.UnloadingTrays = new ObservableCollection<TrayModelBase>()
                {
                    new TrayModelBase
                    {
                        RowCount = 6,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "UnloadingTray1"
                    },
                    new TrayModelBase
                    {
                        RowCount = 6,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "UnloadingTray2"
                    }
                };
                tmpTrays = new ObservableCollection<TrayModelBase>()
                {
                    new TrayModelBase
                    {
                        RowCount = 6,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "UnloadingTray1"
                    },
                    new TrayModelBase
                    {
                        RowCount = 6,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "UnloadingTray2"
                    }
                };
                tmpTrays[0].InitCells();
                tmpTrays[1].InitCells();
            }

            for (int i = 0; i < CDef.UnloadingTrays.Count; i++)
            {
                int workStartIndex = CDef.UnloadingTrays[i].WorkStartIndex;
                CDef.UnloadingTrays[i].InitCells();
                CDef.UnloadingTrays[i].WorkStartIndex = workStartIndex;
                CDef.UnloadingTrays[i].CellClickCallback += OnTrayCellClickCallback;

                for (int j = 0; j < CDef.UnloadingTrays[i].Cells.Count; j++)
                {
                    int cellID = CDef.UnloadingTrays[i].Cells[j].CellInfo.CellID;

                    TrayCell cell = tmpTrays[i].Cells.First(c => c.CellInfo.CellID == cellID);
                    if (cell != null)
                    {
                        CDef.UnloadingTrays[i].Cells[j].CellInfo.CellStatus = cell.CellInfo.CellStatus;
                    }
                }
            }
        }
        #endregion

        #region Methods
        private void OnTrayCellClickCallback(ITrayModel tray, int clickedCellIndex, ModifierKeys key, bool isDoubleClicked)
        {
            if (tray.IsEnable == false) return;

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
