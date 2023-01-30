using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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
using TopCom.Models;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for MotionCommandButtons.xaml
    /// </summary>
    public partial class MotionCommandButtons : UserControl
    {
        #region Dependency Properties
        public RelayCommand ButtonCommand
        {
            get { return (RelayCommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ButtonCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(RelayCommand), typeof(MotionCommandButtons), new PropertyMetadata(null));

        public double Velocity
        {
            get { return (double)GetValue(VelocityProperty); }
            set { SetValue(VelocityProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Velocity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VelocityProperty =
            DependencyProperty.Register("Velocity", typeof(double), typeof(MotionCommandButtons), new PropertyMetadata(300.0));

        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double), typeof(MotionCommandButtons), new PropertyMetadata(10.0));

        public IMotion SelectedAxis
        {
            get { return (IMotion)GetValue(SelectedAxisProperty); }
            set { SetValue(SelectedAxisProperty, value); }
        }
        // Using a DependencyProperty as the backing store for motion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedAxisProperty =
            DependencyProperty.Register("SelectedAxis", typeof(IMotion), typeof(MotionCommandButtons), new PropertyMetadata(null));
        #endregion

        public MotionCommandButtons()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonCommand != null)
            {
                if (ButtonCommand.CanExecute(sender))
                {
                    ButtonCommand.Execute(sender);
                }
            }
        }

        private void ButtonJog_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ButtonCommand != null)
            {
                if (ButtonCommand.CanExecute(sender))
                {
                    ButtonCommand.Execute(sender);
                }
            }
        }

        private void ButtonJog_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ButtonCommand != null)
            {
                if (ButtonCommand.CanExecute(StopButton))
                {
                    ButtonCommand.Execute(StopButton);
                }
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            PositionData positionData = new PositionData()
            {
                AxisName = SelectedAxis.AxisName,
                OldValue = double.Parse(textBox.Text),
                Value = double.Parse(textBox.Text),
                PositionName = $"Manual {textBox.Tag}"
            };

            ValueEditor valueEditor = new ValueEditor(positionData);
            if (valueEditor.ShowDialog() == true)
            {
                textBox.Text = positionData.Value.ToString();
            }
        }
    }
}
