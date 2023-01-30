using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TopVision.Adorners
{
    public class CircleResizeDragAdorner : Adorner
    {
        #region Constants
        readonly Size ThumbSize = new Size(10, 10);
        readonly Size MinSize = new Size(20, 20);
        readonly int ThumbMargin = -1;
        #endregion

        VisualCollection AdornerVisuals;

        /// <summary>
        /// Circle resize thumb
        /// </summary>
        Thumb thumb_Resize;

        /// <summary>
        /// Drag center thumb
        /// </summary>
        Thumb thumb_Center;

        /// <summary>
        /// Description of the adorned element
        /// </summary>
        TextBlock label_Name;

        public string Description { get; set; }

        public event EventHandler AdornerArranged;

        #region Constructor(s)
        public CircleResizeDragAdorner(UIElement adornedElement) : base(adornedElement)
        {
            ThumbInit();
            LabelInit();

            // Make sure to init all thump before add it to the VisualCollection
            AdornerVisuals = new VisualCollection(this)
            {
                label_Name,
                thumb_Center,

                thumb_Resize,
            };
        }

        void ThumbInit()
        {
            thumb_Resize = new Thumb()
            {
                Tag = "Resize",
                Cursor = Cursors.SizeNWSE,
                Visibility = Visibility.Collapsed,
            };

            thumb_Center = new Thumb()
            {
                Style = (Style)FindResource("CircleBorderThumbStyle"),
                Tag = "Center",
                Cursor = Cursors.SizeAll,
                Focusable = true,
            };

            thumb_Center.GotFocus += (s, e) =>
            {
                thumb_Resize.Visibility = Visibility.Visible;
            };

            thumb_Center.LostFocus += (s, e) =>
            {
                thumb_Resize.Visibility = Visibility.Collapsed;
            };

            thumb_Resize.DragDelta += Thumb_DragDelta;
            thumb_Center.DragDelta += Thumb_DragDelta;
        }

        void LabelInit()
        {
            label_Name = new TextBlock();

            label_Name.FontSize = 10;
            label_Name.Foreground = Brushes.Black;
        }
        #endregion

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = (FrameworkElement)AdornedElement;

            int sizeFactor_Horizontal;
            int sizeFactor_Vertical;
            int positionFactor_Horizontal;
            int positionFactor_Vertical;

            double horizontalChange = e.HorizontalChange;
            double vericalChange = e.VerticalChange;

            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);

            switch ((string)(((Thumb)sender).Tag))
            {
                case "Resize":
                    sizeFactor_Horizontal = 1;
                    sizeFactor_Vertical = 1;

                    // Moving the circle, make the circle's NorthWest point fixed while resizing
                    positionFactor_Horizontal = 1;
                    positionFactor_Vertical = 1;

                    horizontalChange = Math.Max(e.HorizontalChange, e.VerticalChange);
                    vericalChange = Math.Max(e.HorizontalChange, e.VerticalChange);

                    double newWidth = element.Width + horizontalChange * sizeFactor_Horizontal;
                    double newHeight = element.Height + vericalChange * sizeFactor_Vertical;

                    if (newWidth >= MinSize.Width)
                    {
                        Canvas.SetLeft(element, left - (newWidth - element.Width) * ((2 - Math.Sqrt(2)) / 4));
                        element.Width = newWidth;
                    }
                    else
                    {
                        element.Width = MinSize.Width;
                    }

                    if (newHeight >= MinSize.Height)
                    {
                        Canvas.SetTop(element, top - (newHeight - element.Height) * ((2 - Math.Sqrt(2)) / 4));
                        element.Height = newHeight;
                    }
                    else
                    {
                        element.Height = MinSize.Height;
                    }
                    break;
                case "Center":
                    sizeFactor_Horizontal = 0;
                    sizeFactor_Vertical = 0;

                    positionFactor_Horizontal = 1;
                    positionFactor_Vertical = 1;

                    Canvas.SetLeft(element, left + horizontalChange * positionFactor_Horizontal);
                    Canvas.SetTop(element, top + vericalChange * positionFactor_Vertical);
                    break;
                default:
                    return;
            }


        }

        #region Override(s)
        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override int VisualChildrenCount => AdornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            thumb_Center.Arrange(
                new Rect(
                    x: -(ThumbMargin),
                    y: -(ThumbMargin),
                    width: AdornedElement.DesiredSize.Width + ThumbMargin * 2,
                    height: AdornedElement.DesiredSize.Height + ThumbMargin * 2)
                );

            thumb_Resize.Arrange(
                new Rect(
                    x: AdornedElement.DesiredSize.Width * ((2 + Math.Sqrt(2)) / 4) - ThumbSize.Width / 2 + ThumbMargin,
                    y: AdornedElement.DesiredSize.Height * ((2 + Math.Sqrt(2)) / 4) - ThumbSize.Height / 2 + ThumbMargin,
                    width: ThumbSize.Width,
                    height: ThumbSize.Height)
                );

            label_Name.Text = Description;
            label_Name.Arrange(
                new Rect(
                    x: AdornedElement.DesiredSize.Width / 2,
                    y: -13,
                    width: AdornedElement.DesiredSize.Width / 2,
                    height: 50)
                );

            OnAdornerArranged();

            return base.ArrangeOverride(finalSize);
        }

        private void OnAdornerArranged()
        {
            AdornerArranged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
