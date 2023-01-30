using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for LogDisplayer.xaml
    /// </summary>
    public partial class LogDisplayer : UserControl
    {


        public ObservableCollection<string> LogSource
        {
            get { return (ObservableCollection<string>)GetValue(LogSourceProperty); }
            set { SetValue(LogSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LogSourceProperty =
            DependencyProperty.Register("LogSource", typeof(ObservableCollection<string>), typeof(LogDisplayer), new PropertyMetadata(new ObservableCollection<string>()));



        public LogDisplayer()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount((sender as ListBox)) > 0)
            {
                Decorator border = VisualTreeHelper.GetChild((sender as ListBox), 0) as Decorator;
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                scrollViewer.ScrollToBottom();
            }
        }
    }
}
