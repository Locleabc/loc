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
using TopCom.Define;

namespace TopUI.Controls
{
    /// <summary>
    /// Interaction logic for CenterLines.xaml
    /// </summary>
    public partial class CenterLines : UserControl
    {
        #region Dependency Properties
        /// <summary>
        /// Pixel Size in micrometer
        /// </summary>
        public double PixelSize
        {
            get { return (double)GetValue(PixelSizeProperty); }
            set
            {
                SetValue(PixelSizeProperty, value);
                DrawCenterLines();
            }
        }

        // Using a DependencyProperty as the backing store for PixelSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PixelSizeProperty =
            DependencyProperty.Register("PixelSize", typeof(double), typeof(CenterLines), new PropertyMetadata(0.0));
        #endregion

        public CenterLines()
        {
            InitializeComponent();
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawCenterLines();
        }

        private void DrawCenterLines()
        {
            double XCenter = canvas.ActualWidth / 2;
            double YCenter = canvas.ActualHeight / 2;
            double pixelsOfOneMM = canvas.ActualWidth / Default.ImageWidth / (PixelSize / 1000);

            canvas.Children.Clear();

            // Horizontal Main Line
            canvas.Children.Add(new Line
            {
                X1 = 0,
                X2 = canvas.ActualWidth,
                Y1 = YCenter,
                Y2 = YCenter,
                Stroke = Brushes.DeepSkyBlue,
            });

            // Vertical Main Line
            canvas.Children.Add(new Line
            {
                X1 = XCenter,
                X2 = XCenter,
                Y1 = 0,
                Y2 = canvas.ActualHeight,
                Stroke = Brushes.DeepSkyBlue,
            });

            for (int i = 1; i <= canvas.ActualWidth / pixelsOfOneMM / 2; i++)
            {
                canvas.Children.Add(new Line
                {
                    X1 = XCenter + pixelsOfOneMM * i,
                    X2 = XCenter + pixelsOfOneMM * i,
                    Y1 = YCenter - mainStepSize / 2,
                    Y2 = YCenter + mainStepSize / 2,
                    Stroke = Brushes.DeepSkyBlue,
                });

                canvas.Children.Add(new Line
                {
                    X1 = XCenter - pixelsOfOneMM * i,
                    X2 = XCenter - pixelsOfOneMM * i,
                    Y1 = YCenter - mainStepSize / 2,
                    Y2 = YCenter + mainStepSize / 2,
                    Stroke = Brushes.DeepSkyBlue,
                });
            }

            for (int i = 1; i <= canvas.ActualHeight / pixelsOfOneMM / 2; i++)
            {
                canvas.Children.Add(new Line
                {
                    X1 = XCenter - mainStepSize / 2,
                    X2 = XCenter + mainStepSize / 2,
                    Y1 = YCenter + pixelsOfOneMM * i,
                    Y2 = YCenter + pixelsOfOneMM * i,
                    Stroke = Brushes.DeepSkyBlue,
                });

                canvas.Children.Add(new Line
                {
                    X1 = XCenter - mainStepSize / 2,
                    X2 = XCenter + mainStepSize / 2,
                    Y1 = YCenter - pixelsOfOneMM * i,
                    Y2 = YCenter - pixelsOfOneMM * i,
                    Stroke = Brushes.DeepSkyBlue,
                });
            }
        }

        #region Privates
        private double mainStepSize = 10;
        #endregion
    }
}
