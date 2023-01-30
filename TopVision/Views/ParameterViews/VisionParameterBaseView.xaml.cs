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

namespace TopVision.Views.ParameterViews
{
    /// <summary>
    /// Interaction logic for VisionParameterBaseView.xaml
    /// </summary>
    public partial class VisionParameterBaseView : UserControl
    {
        #region DependencyProperties
        public Visibility GlobalVisibility
        {
            get { return (Visibility)GetValue(GlobalVisibilityProperty); }
            set { SetValue(GlobalVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GlobalVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GlobalVisibilityProperty =
            DependencyProperty.Register("GlobalVisibility", typeof(Visibility), typeof(VisionParameterBaseView), new PropertyMetadata(Visibility.Visible));



        public Visibility ThresholdVisibility
        {
            get { return (Visibility)GetValue(ThresholdVisibilityProperty); }
            set { SetValue(ThresholdVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThresholdVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThresholdVisibilityProperty =
            DependencyProperty.Register("ThresholdVisibility", typeof(Visibility), typeof(VisionParameterBaseView), new PropertyMetadata(Visibility.Visible));



        public Visibility ROIVisibility
        {
            get { return (Visibility)GetValue(ROIVisibilityProperty); }
            set { SetValue(ROIVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ROIVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ROIVisibilityProperty =
            DependencyProperty.Register("ROIVisibility", typeof(Visibility), typeof(VisionParameterBaseView), new PropertyMetadata(Visibility.Visible));



        public Visibility OutputDisplayOptionVisibility
        {
            get { return (Visibility)GetValue(OutputDisplayOptionVisibilityProperty); }
            set { SetValue(OutputDisplayOptionVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutputDisplayOptionVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutputDisplayOptionVisibilityProperty =
            DependencyProperty.Register("OutputDisplayOptionVisibility", typeof(Visibility), typeof(VisionParameterBaseView), new PropertyMetadata(Visibility.Visible));



        public Visibility ThetaAdjustVisibility
        {
            get { return (Visibility)GetValue(ThetaAdjustVisibilityProperty); }
            set { SetValue(ThetaAdjustVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThetaAdjustVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThetaAdjustVisibilityProperty =
            DependencyProperty.Register("ThetaAdjustVisibility", typeof(Visibility), typeof(VisionParameterBaseView), new PropertyMetadata(Visibility.Visible));



        public Visibility UseInputImageVisibility
        {
            get { return (Visibility)GetValue(UseInputImageVisibilityProperty); }
            set { SetValue(UseInputImageVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseInputImageVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseInputImageVisibilityProperty =
            DependencyProperty.Register("UseInputImageVisibility", typeof(Visibility), typeof(VisionParameterBaseView), new PropertyMetadata(Visibility.Collapsed));




        public Visibility UseFixtureAlignVisibility
        {
            get { return (Visibility)GetValue(UseFixtureAlignVisibilityProperty); }
            set { SetValue(UseFixtureAlignVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseFixtureAlignVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseFixtureAlignVisibilityProperty =
            DependencyProperty.Register("UseFixtureAlignVisibility", typeof(Visibility), typeof(VisionParameterBaseView), new PropertyMetadata(Visibility.Collapsed));




        public Visibility OffsetLimitVisibility
        {
            get { return (Visibility)GetValue(OffsetLimitVisibilityProperty); }
            set { SetValue(OffsetLimitVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffsetLimitVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetLimitVisibilityProperty =
            DependencyProperty.Register("OffsetLimitVisibility", typeof(Visibility), typeof(VisionParameterBaseView), new PropertyMetadata(Visibility.Collapsed));


        #endregion

        public VisionParameterBaseView()
        {
            InitializeComponent();
        }
    }
}
