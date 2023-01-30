using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TOPUI.Controls;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<TrayCell> loadingTray;

        public ObservableCollection<TrayCell> LoadingTray
        {
            get { return loadingTray; }
            set { loadingTray = value; }
        }


        public MainWindow()
        {
            InitializeComponent();

            ObservableCollection<TrayCell> cells = new ObservableCollection<TrayCell>(); 

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    cells.Add(new TrayCell
                    {
                        CellIndex = i * 5 + j,
                        Margin = new Thickness(2),
                        CellShape = TOPUI.Define.ECellShape.Circle
                    });
                }
            }

            LoadingTray = cells;

            this.DataContext = this;
        }
    }
}
