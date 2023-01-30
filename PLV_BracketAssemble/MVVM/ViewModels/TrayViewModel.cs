using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TopCom;
using TopCom.Command;
using TopUI.Define;
using TopUI.Models;

namespace PLV_BracketAssemble.MVVM.ViewModels
{
    public class TrayViewModel : PropertyChangedNotifier
    {
        public TrayBase<ECellSimpleStatus> Tray { get; set; }

        public ObservableCollection<Button> OnOffButtons
        {
            get { return _OnOffButtons; }
            set
            {
                _OnOffButtons = value;
                OnPropertyChanged();
            }
        }

        public int WorkIndex
        {
            get
            {
                int workIndex = -1;
                try
                {
                    workIndex = Tray.Cells.First(cell => cell.Status == TopCom.ECellSimpleStatus.Ready).Index;
                }
                catch
                {

                }
                
                return workIndex;
            }
        }

        public int WorkIndex_X
        {
            get
            {
                int workIndex_X = -1;

                if (WorkIndex == -1) return workIndex_X;

                if (WorkIndex % Tray.ColumnCount == 0)
                {
                    workIndex_X = Tray.ColumnCount;
                }
                else
                {
                    workIndex_X = WorkIndex % Tray.ColumnCount;
                }
                return workIndex_X;
            }
        }

        public int WorkIndex_Y
        {
            get
            {
                int workIndex_Y = -1;

                if (WorkIndex == -1) return workIndex_Y;

                int workIndex = Tray.Cells.First(cell => cell.Status == TopCom.ECellSimpleStatus.Ready).Index;

                if (workIndex % Tray.ColumnCount == 0)
                {
                    workIndex_Y = workIndex/Tray.ColumnCount;
                }
                else
                {
                    workIndex_Y = workIndex / Tray.ColumnCount + 1;
                }
                return workIndex_Y;
            }
        }

        public int CountReadySlot
        {
            get
            {
                return Tray.Cells.Count(c => c.Status == ECellSimpleStatus.Ready);
            }
        }

        public Brush BackgroundColor { get; set; } = Brushes.NavajoWhite;

        public TrayViewModel(string trayName)
        {
            LoadTrayFromFile(trayName);
            RefreshTray();
        }

        public void SetSingleCell(ECellSimpleStatus status, int cellIndex)
        {
            try
            {
                Tray.Cells.First(cell => cell.Index == cellIndex).Status = status;
            }
            catch
            {
                throw new ArgumentOutOfRangeException("Cell is not exist on tray");
            }
        }

        private void LoadTrayFromFile(string trayName)
        {
            try
            {
                string trayBackupString = File.ReadAllText($"{Regex.Replace(trayName, @"\s+", "")}.json");
                Tray = JsonConvert.DeserializeObject<TrayBase<ECellSimpleStatus>>(trayBackupString);
            }
            catch
            {
                Tray = new TrayBase<ECellSimpleStatus>()
                {
                    ColumnCount = 5,
                    RowCount = 10,
                    Name = trayName,
                    StartPosition = EStartPosition.TopLeft,
                };

                Tray.GenerateCells();
            }
        }

        public void BackupTrayToFile()
        {
            File.WriteAllText($"{Regex.Replace(Tray.Name, @"\s+", "")}.json", JsonConvert.SerializeObject(Tray, Formatting.None));
        }

        public void RefreshTray()
        {
            GenerateOnOffButtons();
        }

        private void GenerateOnOffButtons()
        {
            if (Tray == null) return;

            OnOffButtons = new ObservableCollection<Button>();
            for (int i = 1; i <= Tray.RowCount; i++)
            {
                int rowIndex;

                if (Tray.StartPosition == EStartPosition.TopLeft || Tray.StartPosition == EStartPosition.TopRight)
                {
                    rowIndex = i;
                }
                else
                {
                    rowIndex = Tray.RowCount - i + 1;
                }

                OnOffButtons.Add(
                    new Button
                    {
                        Content = $"On",
                        Margin = new System.Windows.Thickness(3),
                        Command = RowOnOffCommand(),
                        CommandParameter = rowIndex,
                        FontSize = 12,
                    });
            }
        }

        public RelayCommand CellClickedCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (((CellBase<ECellSimpleStatus>)o).Status == ECellSimpleStatus.Ready)
                    {
                        ((CellBase<ECellSimpleStatus>)o).Status = ECellSimpleStatus.Skip;
                    }
                    else
                    {
                        ((CellBase<ECellSimpleStatus>)o).Status = ECellSimpleStatus.Ready;
                    }
                });
            }
        }

        public RelayCommand RowOnOffCommand()
        {
            return new RelayCommand((o) =>
            {
                if (o.GetType() != typeof(int)) return;

                int row = (int)o;

                if (Tray.IsRowContainAnotherThan(row, ECellSimpleStatus.Skip))
                {
                    Tray.SetRow(row, ECellSimpleStatus.Skip);
                }
                else
                {
                    Tray.SetRow(row, ECellSimpleStatus.Ready);
                }
            });
        }

        
        public RelayCommand AllRowOnOffCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (CountReadySlot == 0)
                    {
                        Tray.SetAllTray(ECellSimpleStatus.Ready);
                    }
                    else
                    {
                        Tray.SetAllTray(ECellSimpleStatus.Skip);
                    }
                });
            }

        }

        private ObservableCollection<Button> _OnOffButtons;
    }
}