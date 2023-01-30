using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TopCom;
using TopCom.Command;
using TopVision.Algorithms;
using TopVision.Models;
using TopVision.ViewModels;

namespace TopVision.Views
{
    /// <summary>
    /// Interaction logic for VisionProcessView.xaml
    /// </summary>
    public partial class VisionProcessView : UserControl
    {
        public VisionProcessView()
        {
            InitializeComponent();
        }

        private void Root_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (this.DataContext as VisionProcessViewModel).UIElement = DisplayImage;
            (this.DataContext as VisionProcessViewModel).UIPanel = AdornerCanvas;
        }

        private void DisplayImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (this.DataContext as VisionProcessViewModel).DrawAdorners();
        }
    }
}
