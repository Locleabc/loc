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
using TopCom.Models;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for ValueBox.xaml
    /// </summary>
    public partial class ValueBox : UserControl
    {
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ValueBox), new PropertyMetadata("Header"));


        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ValueBox), new PropertyMetadata(""));


        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ValueBox), new PropertyMetadata(0.0));


        public ValueBox()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PositionData positionData = new PositionData()
            {
                AxisName = "",
                OldValue = Value,
                Value = Value,
                PositionName = $"Change {Header}"
            };

            ValueEditor valueEditor = new ValueEditor(positionData);
            if (valueEditor.ShowDialog() == true)
            {
                Value = positionData.Value;
            }
        }
    }
}
