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
using System.Windows.Threading;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for Footer.xaml
    /// </summary>
    public partial class Footer : UserControl
    {
        #region DependencyProperty
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(Footer), new PropertyMetadata(DateTime.Now));

        public string MachineName
        {
            get { return (string)GetValue(MachineNameProperty); }
            set { SetValue(MachineNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MachineName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MachineNameProperty =
            DependencyProperty.Register("MachineName", typeof(string), typeof(Footer), new PropertyMetadata("EQPlatform"));

        public string SoftwareVersion
        {
            get { return (string)GetValue(SoftwareVersionProperty); }
            set { SetValue(SoftwareVersionProperty, value); }
        }
        // Using a DependencyProperty as the backing store for SoftwareVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SoftwareVersionProperty =
            DependencyProperty.Register("SoftwareVersion", typeof(string), typeof(Footer), new PropertyMetadata("0.0.0.0"));

        public string VersionDescription
        {
            get { return (string)GetValue(VersionDescriptionProperty); }
            set { SetValue(VersionDescriptionProperty, value); }
        }
        // Using a DependencyProperty as the backing store for VersionDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionDescriptionProperty =
            DependencyProperty.Register("VersionDescription", typeof(string), typeof(Footer), new PropertyMetadata(""));
        #endregion

        public Footer()
        {
            InitializeComponent();
            InitFooter();
        }

        public void InitFooter()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

            timer.Tick += (sender, args) =>
            {
                CurrentTime = DateTime.Now;
            };

            timer.Start();
        }
    }
}
