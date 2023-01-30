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
using TopCom;
using TopCom.Models;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for SpeedControlSlider.xaml
    /// </summary>
    public partial class ValueSlider : UserControl
    {
        public ValueSlider()
        {
            InitializeComponent();
        }

        #region Dependence Properties
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(ValueSlider), new PropertyMetadata(0));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(ValueSlider), new PropertyMetadata("Header"));

        public bool UseSelectValue
        {
            get { return (bool)GetValue(UseSelectValueProperty); }
            set { SetValue(UseSelectValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseSelectValueProperty =
            DependencyProperty.Register(nameof(UseSelectValue), typeof(bool), typeof(ValueSlider), new PropertyMetadata(true));

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(string), typeof(ValueSlider), new PropertyMetadata(""));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(ValueSlider), new PropertyMetadata(0));

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(ValueSlider), new PropertyMetadata(100));

        public int TickFrequency
        {
            get { return (int)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register(nameof(TickFrequency), typeof(int), typeof(ValueSlider), new PropertyMetadata(1));

        #endregion

        #region Clicks
        private void ChangeValue_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch ((sender as Label).Name)
            {
                case "Value0":
                    Value = MinValue;
                    break;
                case "Value25":
                    Value = (int)(0.25 * (MaxValue - MinValue));
                    break;
                case "Value50":
                    Value = (int)(0.5 * (MaxValue - MinValue));
                    break;
                case "Value75":
                    Value = (int)(0.75 * (MaxValue - MinValue));
                    break;
                case "Value100":
                    Value = MaxValue;
                    break;
            }
        }
        #endregion

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
                Value = (int)positionData.Value;
            }
        }
    }
}
