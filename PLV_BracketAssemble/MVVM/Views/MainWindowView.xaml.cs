using PLV_BracketAssemble.Define;
using PLV_BracketAssemble.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TopCom.Define;

namespace PLV_BracketAssemble.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        private void HeaderVM_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Environment.ExitCode != (int)EExitCode.UserTerminatedAppication)
            {
                //(this.DataContext as MainWindowViewModel).HeaderVM.ExitCommand.Execute(sender);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MainWindowViewModel).WindowLoadDone.Execute(sender);
        }
        //public void ShowVisionTeachingTab()
        //{
        //    MainTabControl.SelectedItem = VisionTeachingTabItem;
        //}

        //private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (MainTabControl.SelectedItem == VisionTeachingTabItem)
        //    {
        //        CDef.MainViewModel.VisionAutoVM.TeachCommand.Execute(this);
        //    }
        //}
    }
}
