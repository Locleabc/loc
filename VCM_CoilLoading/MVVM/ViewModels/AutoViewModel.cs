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
using VCM_CoilLoading.Define;
using VCM_CoilLoading.Processing;

namespace VCM_CoilLoading.MVVM.ViewModels
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
                    CDef.LoadingTray.ResetCommand.Execute(null);
                });
            }
        }

        public RelayCommand ResetUnloadingTrayCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.UnloadingTray1.ResetCommand.Execute(null);
                    CDef.UnloadingTray2.ResetCommand.Execute(null);
                });
            }
        }

        public RelayCommand LoadingTrayChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CDef.RootProcess.RunMode = ERunMode.Manual_LoadingTray_Change;
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
                    CDef.RootProcess.RunMode = ERunMode.Manual_UnloadingTray_Change;
                    CDef.RootProcess.OperationCommand = OperatingMode.Run;
                });
            }
        }
        #endregion

        #region Constructors
        public AutoViewModel()
        {
            Init();
        }

        public void Init()
        {
            InitAllTray();
        }

        private ITrayModel LoadTray(string JsonFile)
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
                CDef.LoadingTray = LoadTray("TrayLoading.json");
            }
            catch
            {
                if (CDef.MainViewModel.InitVM.InitCompleted == true)
                {
                    CDef.LoadingTray = new TrayModelBase
                    {
                        RowCount = (int)CDef.CommonRecipe.LoadTray_YCount,
                        ColumnCount = (int)CDef.CommonRecipe.LoadTray_XCount,
                        Shape = TopUI.Define.ECellShape.Circle,
                        Name = "LoadingTray"
                    };
                }
                else
                {
                    CDef.LoadingTray = new TrayModelBase
                    {
                        RowCount = 10,
                        ColumnCount = 10,
                        Shape = TopUI.Define.ECellShape.Circle,
                        Name = "LoadingTray"
                    };
                }
                CDef.LoadingTray.InitCells();
            }

            try
            {
                CDef.UnloadingTray1 = LoadTray("TrayUnloading1.json");
            }
            catch
            {
                if (CDef.MainViewModel.InitVM.InitCompleted == true)
                {
                    CDef.UnloadingTray1 = new TrayModelBase
                    {
                        RowCount = (int)CDef.CommonRecipe.UnloadTray_YCount,
                        ColumnCount = (int)CDef.CommonRecipe.UnloadTray_XCount,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "UnloadingTray1"
                    };
                }
                else
                {
                    CDef.UnloadingTray1 = new TrayModelBase
                    {
                        RowCount = 5,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "UnloadingTray1"
                    };
                }
                CDef.UnloadingTray1.InitCells();
            }

            try
            {
                CDef.UnloadingTray2 = LoadTray("TrayUnloading2.json");
            }
            catch
            {
                if (CDef.MainViewModel.InitVM.InitCompleted == true)
                {
                    CDef.UnloadingTray2 = new TrayModelBase
                    {
                        RowCount = (int)CDef.CommonRecipe.UnloadTray_YCount,
                        ColumnCount = (int)CDef.CommonRecipe.UnloadTray_XCount,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "UnloadingTray2"
                    };
                }
                else
                {
                    CDef.UnloadingTray2 = new TrayModelBase
                    {
                        RowCount = 5,
                        ColumnCount = 6,
                        Shape = TopUI.Define.ECellShape.Rectangle,
                        Name = "UnloadingTray2"
                    };
                }
                CDef.UnloadingTray2.InitCells();
            }

            (CDef.LoadingTray as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;
            (CDef.UnloadingTray1 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback;
            (CDef.UnloadingTray2 as TrayModelBase).CellClickCallback += OnTrayCellClickCallback_Tray2;
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

        private void OnTrayCellClickCallback_Tray2(ITrayModel tray, int clickedCellIndex, ModifierKeys key, bool isDoubleClicked)
        {
            if (isDoubleClicked)
            {
                CDef.UnloadingTray1.SetToCell(TopUI.Define.ECellStatus.OK, CDef.UnloadingTray1.Cells.Count);
                CDef.UnloadingTray1.WorkStartIndex = CDef.UnloadingTray1.Cells.Count + 2;

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
