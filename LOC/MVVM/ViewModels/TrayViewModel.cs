using Newtonsoft.Json;
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

namespace LOC.MVVM.ViewModels
{
    public class TrayViewModel : PropertyChangedNotifier
    {
        public TrayBase<ECellSimpleStatus> Tray { get; set; }
        public ObservableCollection<Button> OnOffButtons
        {
            get
            {
                return _OnOffButtons;
            }
            set
            {
                _OnOffButtons = value;
                OnPropertyChanged();
            }
        }
        public Brush BackgroundColor { get; set; } = Brushes.NavajoWhite;



        public TrayViewModel(string trayName)
        {
            LoadTrayFromFile(trayName);
            RefreshTray();
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

        private void RefreshTray()
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
                        Content = $"Ohh",
                        Margin = new System.Windows.Thickness(3),
                        Command = RowOnOffCommand(),
                        CommandParameter = rowIndex,
                        FontSize = 12,
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
        private ObservableCollection<Button> _OnOffButtons;
    }
}
