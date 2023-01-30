using OpenCvSharp;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VisionToolBox.Controls
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : UserControl
    {


        public BitmapSource DisplaySource
        {
            get { return (BitmapSource)GetValue(DisplaySourceProperty); }
            set
            {
                SetValue(DisplaySourceProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for DisplaySource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplaySourceProperty =
            DependencyProperty.Register("DisplaySource", typeof(BitmapSource), typeof(ImageViewer),
                new PropertyMetadata(new BitmapImage(new Uri(@"pack://application:,,,/VisionToolBox;component/Images/TopEngineeringBackground.bmp", UriKind.RelativeOrAbsolute))));

        #region Dependency Properties
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set
            {
                SetValue(SourceProperty, value);

                var ViewModel = this.DataContext as ViewModels.ImageViewerViewModel;

                if (ViewerImage.Stretch == System.Windows.Media.Stretch.Uniform)
                {
                    ViewModel.ImageScaleRate = System.Math.Min(ViewerImage.Width / ViewerImage.Source.Width, ViewerImage.Height / ViewerImage.Source.Height);
                }
                else if (ViewerImage.Stretch == System.Windows.Media.Stretch.UniformToFill)
                {
                    ViewModel.ImageScaleRate = System.Math.Max(ViewerImage.Width / ViewerImage.Source.Width, ViewerImage.Height / ViewerImage.Source.Height);
                }
                else if (ViewerImage.Stretch == System.Windows.Media.Stretch.None)
                {
                    ViewModel.ImageScaleRate = 1;
                }
                else
                {
                    ViewModel.ImageScaleRate = -1;
                }

                ViewerImage.Source = value;
            }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageViewer), new PropertyMetadata(null));
        #endregion

        public ImageViewer()
        {
            InitializeComponent();

            this.DataContext = new ViewModels.ImageViewerViewModel();
            //DisplaySource = new BitmapImage(new Uri(@"D:\TOP\Simulation\Images\Bot\Under_2244.jpg"));
        }

        private void ViewerThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var ViewModel = this.DataContext as ViewModels.ImageViewerViewModel;
            // Ignore draging in FixZoomCenter mode
            if (ViewModel.FixZoomCenter)
            {
                return;
            }

            if ((this.DataContext as ViewModels.ImageViewerViewModel).ViewerDragCommand.CanExecute(e))
            {
                (this.DataContext as ViewModels.ImageViewerViewModel).ViewerDragCommand.Execute(e);
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var ViewModel = this.DataContext as ViewModels.ImageViewerViewModel;
            int deltaValue = e.Delta;

            var Scale = ViewModel.Scale;

            // Zoom in when the user scrolls the mouse wheel up
            // and vice versa.
            if (deltaValue > 0)
            {
                // Limit zoom-in to 1000%
                if (Scale.ScaleX < 10)
                {
                    // Zoom-in in 10% increments
                    Scale.ScaleX += 0.5;
                    Scale.ScaleY += 0.5;

                    if (ViewModel.FixZoomCenter)
                    {
                        Scale.CenterX = this.ActualWidth / 2.0;
                        Scale.CenterY = this.ActualHeight / 2.0;
                    }
                    else
                    {
                        Scale.CenterX = e.GetPosition(ViewerImage).X;
                        Scale.CenterY = e.GetPosition(ViewerImage).Y;
                    }
                }
            }
            // When mouse wheel is scrolled down...
            else
            {
                // Limit zoom-out to 100%
                if (Scale.ScaleX > 1.0)
                {
                    // Zoom-out by 10%
                    Scale.ScaleX -= 0.5;
                    Scale.ScaleY -= 0.5;

                    if (ViewModel.FixZoomCenter)
                    {
                        Scale.CenterX = this.ActualWidth / 2.0;
                        Scale.CenterY = this.ActualHeight / 2.0;
                    }
                    else
                    {
                        Scale.CenterX = e.GetPosition(ViewerImage).X;
                        Scale.CenterY = e.GetPosition(ViewerImage).Y;
                    }
                }
            }

            if (Scale.ScaleX <= 1.0)
            {
                ViewModel.DragShiftX = 0;
                ViewModel.DragShiftY = 0;

                ViewModel.ViewerWorkMode = ViewModels.EImageViewerWorkMode.Display;
            }
            else
            {
                ViewModel.ViewerWorkMode = ViewModels.EImageViewerWorkMode.Drag;
            }
        }

        private void Root_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ViewModel = this.DataContext as ViewModels.ImageViewerViewModel;
            if (ViewModel.FixZoomCenter)
            {
                ViewModel.ScaleCenterX = this.ActualWidth / 2.0;
                ViewModel.ScaleCenterY = this.ActualHeight / 2.0;
            }
            else
            {
                ViewModel.ScaleCenterX = e.GetPosition(ViewerImage).X;
                ViewModel.ScaleCenterY = e.GetPosition(ViewerImage).Y;
            }
        }
    }
}
