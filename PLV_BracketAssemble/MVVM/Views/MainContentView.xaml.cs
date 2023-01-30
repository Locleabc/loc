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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PLV_BracketAssemble.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainContentView : UserControl
    {
        public MainContentView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is MainContentViewModel)
            {
                ((MainContentViewModel)DataContext).View = this;
            }
        }

        public void ShowVisionTeachingTab()
        {
            MainTabControl.SelectedItem = VisionTeachingTabItem;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem == VisionTeachingTabItem)
            {
                CDef.MainViewModel.MainContentVM.VisionAutoVM.TeachCommand.Execute(null);
            }
        }
    }
}
