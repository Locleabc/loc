using LOC.Define;
using LOC.MVVM.ViewModels;
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

namespace LOC.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MainTabControlView.xaml
    /// </summary>
    public partial class MainTabControlView : UserControl
    {
        public MainTabControlView()
        {
            InitializeComponent();
        }


        public void ShowVisionTeachingTab()
        {
            MainTabControl.SelectedItem = VisionTeachingTabItem;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem == VisionTeachingTabItem)
            {
                //Cdef.mainViewModel.MainTabControlVM.VisionAutoVM.TeachCommand.Execute(null);  chú ý
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is MainTabControlViewModel)
            {
                ((MainTabControlViewModel)DataContext).View = this;
            }
        }
    }
}
