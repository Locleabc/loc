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
using VCM_CoilLoading.MVVM.ViewModels;

namespace VCM_CoilLoading.MVVM.Views
{
    /// <summary>
    /// Interaction logic for VisionSettingWindowView.xaml
    /// </summary>
    public partial class VisionTeachWindowView : Window
    {
        public VisionTeachWindowView(VisionAutoViewModel parentVM)
        {
            InitializeComponent();

            (this.DataContext as VisionTeachWindowViewModel).ParentVM = parentVM;
        }
    }
}
