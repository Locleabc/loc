using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TopUI.Define;
using TopUI.Models;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class TrayCell : UserControl
    {
        #region Dependency Property
        [JsonProperty]
        public TrayCellBase CellInfo { get; set; }
        //public TrayCellBase CellInfo
        //{
        //    get { return (TrayCellBase)GetValue(CellInfoProperty); }
        //    set { SetValue(CellInfoProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for CellBase.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CellInfoProperty =
        //    DependencyProperty.Register("CellInfo", typeof(TrayCellBase), typeof(TrayCell), new PropertyMetadata(new TrayCellBase()));
        #endregion

        #region Event Handlers
        public event RoutedEventHandler CellClicked;
        public event MouseButtonEventHandler CellDoubleClicked;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CellClicked != null)
            {
                CellClicked.Invoke(this, e);
            }
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (CellDoubleClicked != null && e.ChangedButton == MouseButton.Left)
            {
                CellDoubleClicked.Invoke(this, e);
            }
        }
        #endregion

        public TrayCell()
        {
            InitializeComponent();

            this.DataContext = this;

            Padding = new Thickness(1.5);
        }
    }
}
