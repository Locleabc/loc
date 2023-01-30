using System;
using System.Collections.Generic;
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
using TopCom.Command;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for OptionSelector.xaml
    /// </summary>
    public partial class OptionSelector : UserControl
    {
        #region Dependency Property
        public bool IsInputOnly
        {
            get { return (bool)GetValue(IsInputOnlyProperty); }
            set { SetValue(IsInputOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInputOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInputOnlyProperty =
            DependencyProperty.Register("IsInputOnly", typeof(bool), typeof(OptionSelector), new PropertyMetadata(false));

        /// <summary>
        /// Disable change IsEnable property by UI
        /// </summary>
        public bool IsHoldingButton
        {
            get { return (bool)GetValue(IsHoldingButtonProperty); }
            set { SetValue(IsHoldingButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHoldingButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHoldingButtonProperty =
            DependencyProperty.Register("IsHoldingButton", typeof(bool), typeof(OptionSelector), new PropertyMetadata(false));

        /// <summary>
        /// Is this option/ IO currently enable
        /// </summary>
        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool), typeof(OptionSelector), new PropertyMetadata(false));

        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(OptionSelector), new PropertyMetadata("Default Description"));

        public string CommandParameter
        {
            get { return (string)GetValue(CommandParameterProperty); }
            set { SetValue(ExtraButtonClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(string), typeof(OptionSelector), new PropertyMetadata(null));
        
        public RelayCommand ExtraButtonClickCommand
        {
            get { return (RelayCommand)GetValue(ExtraButtonClickCommandProperty); }
            set { SetValue(ExtraButtonClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtraButtonClickCommandProperty =
            DependencyProperty.Register("ExtraButtonClickCommand", typeof(RelayCommand), typeof(OptionSelector), new PropertyMetadata(null));
        #endregion

        public OptionSelector()
        {
            InitializeComponent();
        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ExtraButtonClickCommand != null)
            {
                return;
            }

            if (IsInputOnly)
            {
                return;
            }

            if (IsHoldingButton)
            {
                IsOn = true;
            }
        }

        private void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ExtraButtonClickCommand != null)
            {
                return;
            }

            if (IsInputOnly)
            {
                return;
            }

            if (IsHoldingButton)
            {
                IsOn = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ExtraButtonClickCommand != null)
            {
                return;
            }

            if (IsInputOnly)
            {
                return;
            }

            if (IsHoldingButton == false)
            {
                IsOn = !IsOn;
            }
        }
    }
}
