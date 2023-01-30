using System.Windows;
using System.Windows.Controls;
using TopCom.Command;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for AutoOperationButtons.xaml
    /// </summary>
    public partial class AutoOperationButtons : UserControl
    {
        #region Dependency Properties
        public RelayCommand<object> ButtonCommand
        {
            get { return (RelayCommand<object>)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for AutoOperationButtonCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(RelayCommand<object>), typeof(AutoOperationButtons), new PropertyMetadata(null));

        public RelayCommand HomeCommand
        {
            get { return (RelayCommand)GetValue(HomeCommandProperty); }
            set { SetValue(HomeCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for HomeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HomeCommandProperty =
            DependencyProperty.Register("HomeCommand", typeof(RelayCommand), typeof(AutoOperationButtons), new PropertyMetadata(null));

        public RelayCommand ChangeCommand
        {
            get { return (RelayCommand)GetValue(ChangeCommandProperty); }
            set { SetValue(ChangeCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for HomeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeCommandProperty =
            DependencyProperty.Register("ChangeCommand", typeof(RelayCommand), typeof(AutoOperationButtons), new PropertyMetadata(null));

        public RelayCommand StartCommand
        {
            get { return (RelayCommand)GetValue(StartCommandProperty); }
            set { SetValue(StartCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for StartCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartCommandProperty =
            DependencyProperty.Register("StartCommand", typeof(RelayCommand), typeof(AutoOperationButtons), new PropertyMetadata(null));

        public RelayCommand StopCommand
        {
            get { return (RelayCommand)GetValue(StopCommandProperty); }
            set { SetValue(StopCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for StopCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StopCommandProperty =
            DependencyProperty.Register("StopCommand", typeof(RelayCommand), typeof(AutoOperationButtons), new PropertyMetadata(null));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AutoOperationButtons), new PropertyMetadata(Orientation.Horizontal));

        public bool ContainChangeButton
        {
            get { return (bool)GetValue(ContainChangeButtonProperty); }
            set { SetValue(ContainChangeButtonProperty, value); }
        }
        // Using a DependencyProperty as the backing store for StopCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContainChangeButtonProperty =
            DependencyProperty.Register("ContainChangeButton", typeof(bool), typeof(AutoOperationButtons), new PropertyMetadata(false));

        public int ButtonWidth
        {
            get { return (int)GetValue(ButtonWidthProperty); }
            set { SetValue(ButtonWidthProperty, value); }
        }
        // Using a DependencyProperty as the backing store for StopCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register("ButtonWidth", typeof(int), typeof(AutoOperationButtons), new PropertyMetadata(120));

        public int ButtonHeight
        {
            get { return (int)GetValue(ButtonHeightProperty); }
            set { SetValue(ButtonHeightProperty, value); }
        }
        // Using a DependencyProperty as the backing store for StopCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonHeightProperty =
            DependencyProperty.Register("ButtonHeight", typeof(int), typeof(AutoOperationButtons), new PropertyMetadata(65));
        #endregion

        public AutoOperationButtons()
        {
            InitializeComponent();
        }
    }
}
