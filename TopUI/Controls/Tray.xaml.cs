using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TopUI.Models;
using TopUI.Define;
using TopCom;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for Tray.xaml
    /// </summary>
    public partial class Tray : UserControl, INotifyPropertyChanged
    {
        #region Dependence Properties

        public EStartPosition StartPosition
        {
            get { return (EStartPosition)GetValue(StartPositionProperty); }
            set { SetValue(StartPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartPositionProperty =
            DependencyProperty.Register("StartPosition", typeof(EStartPosition), typeof(Tray), new PropertyMetadata(EStartPosition.TopLeft));



        public TrayModelBase TrayInfo
        {
            get { return (TrayModelBase)GetValue(TrayInfoProperty); }
            set { SetValue(TrayInfoProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TrayInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrayInfoProperty =
            DependencyProperty.Register("TrayInfo", typeof(TrayModelBase), typeof(Tray), new PropertyMetadata(new TrayModelBase()));

        public Thickness TrayPadding
        {
            get { return (Thickness)GetValue(TrayPaddingProperty); }
            set { SetValue(TrayPaddingProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TrayPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrayPaddingProperty =
            DependencyProperty.Register("TrayPadding", typeof(Thickness), typeof(Tray), new PropertyMetadata(new Thickness(3)));

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public Tray()
        {
            InitializeComponent();
        }
    }
}
