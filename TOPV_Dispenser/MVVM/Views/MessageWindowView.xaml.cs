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
using TOPV_Dispenser.MVVM.ViewModels;

namespace TOPV_Dispenser.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MessageWindowView.xaml
    /// </summary>
    public partial class MessageWindowView : Window
    {
        public MessageWindowView()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null) return;

            (this.DataContext as MessageWindowViewModel).ModalChangedEvent += (_sender, _e) =>
            {
                if ((bool)_sender == true)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            this.Hide();
                        }
                        catch { }

                        this.ShowDialog();
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            this.Hide();
                        }
                        catch { }

                        this.Show();
                    });
                }
            };
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
