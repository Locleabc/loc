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
    /// Interaction logic for SliderValue.xaml
    /// </summary>
    public partial class SliderValue : UserControl
    {
        public SliderValue()
        {
            InitializeComponent();
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(SliderValue), new PropertyMetadata(""));

        public string MinValue
        {
            get { return (string)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(SliderValue), new PropertyMetadata(0));

        public string MaxValue
        {
            get { return (string)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(SliderValue), new PropertyMetadata(100));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(SliderValue), new PropertyMetadata("10"));

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(string), typeof(SliderValue), new PropertyMetadata(""));

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch((sender as Label).Name)
            {
                case "lb0":
                    Value = "0";
                    break;
                case "lb25":
                    Value = "25";
                    break;
                case "lb50":
                    Value = "50";
                    break;
                case "lb75":
                    Value = "75";
                    break;
                case "lb100":
                    Value = "100";
                    break;
            }
        }
    }
}
