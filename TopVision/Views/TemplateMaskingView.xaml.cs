using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TopVision.ViewModels;

namespace TopVision.Views
{
    /// <summary>
    /// Interaction logic for TemplateMaskingView.xaml
    /// </summary>
    public partial class TemplateMaskingView : UserControl
    {
        public TemplateMaskingView()
        {
            InitializeComponent();
        }

        private void root_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext.GetType() != typeof(TemplateMaskingViewModel)) return;

            (this.DataContext as TemplateMaskingViewModel).TheInkCanvas = inkCanvas;
        }
    }
}
