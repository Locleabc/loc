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
    public class RectangleResizeDragAdorner : Adorner
    {
        #region Constants
        readonly Size ThumbSize = new Size(10, 10);
        readonly Size MinSize = new Size(20, 20);
        readonly int ThumbMargin = -1;
        #endregion

        VisualCollection AdornerVisuals;

        /// <summary>
        /// Corner Top Left thumb
        /// </summary>
        Thumb thumb_TL;
        /// <summary>
        /// Corner Bottom Right thumb
        /// </summary>
        Thumb thumb_BR;
        /// <summary>
        /// Corner Top Right thumb
        /// </summary>
        Thumb thumb_TR;
        /// <summary>
        /// Corner Bottom Left thumb
        /// </summary>
        Thumb thumb_BL;

        /// <summary>
        /// Border Center left thumb
        /// </summary>
        Thumb thumb_CenterLeft;
        /// <summary>
        /// Border Center right thumb
        /// </summary>
        Thumb thumb_CenterRight;
        /// <summary>
        /// Border Center top thumb
        /// </summary>
        Thumb thumb_CenterTop;
        /// <summary>
        /// Border Center bottom thumb
        /// </summary>
        Thumb thumb_CenterBottom;

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
        public RectangleResizeDragAdorner(UIElement adornedElement) : base(adornedElement)
        {
            CornerThumbInit();
            BorderThumbInit();
            CenterThumbInit();
            LabelInit();

            // Make sure to init all thump before add it to the VisualCollection
            AdornerVisuals = new VisualCollection(this)
            {
                label_Name,

                thumb_Center,

                thumb_CenterTop,
                thumb_CenterBottom,
                thumb_CenterLeft,
                thumb_CenterRight,

                thumb_TL,
                thumb_TR,
                thumb_BL,
                thumb_BR,
            };
        }

        void CornerThumbInit()
        {
            thumb_TL = new Thumb()
            {
                Tag = "TopLeft",
                Cursor = Cursors.SizeNWSE,
                Visibility = Visibility.Collapsed,
            };
            thumb_TR = new Thumb()
            {
                Tag = "TopRight",
                Cursor = Cursors.SizeNESW,
                Visibility = Visibility.Collapsed,
            };
            thumb_BL = new Thumb()
            {
                Tag = "BottomLeft",
                Cursor = Cursors.SizeNESW,
                Visibility = Visibility.Collapsed,
            };
            thumb_BR = new Thumb()
            {
                Tag = "BottomRight",
                Cursor = Cursors.SizeNWSE,
                Visibility = Visibility.Collapsed,
            };

            thumb_TL.DragDelta += Thumb_DragDelta;
            thumb_TR.DragDelta += Thumb_DragDelta;
            thumb_BL.DragDelta += Thumb_DragDelta;
            thumb_BR.DragDelta += Thumb_DragDelta;
        }

        void BorderThumbInit()
        {
            thumb_CenterTop = new Thumb()
            {
                Tag = "CenterTop",
                Cursor = Cursors.SizeNS,
                Visibility = Visibility.Collapsed,
            };
            thumb_CenterBottom = new Thumb()
            {
                Tag = "CenterBottom",
                Cursor = Cursors.SizeNS,
                Visibility = Visibility.Collapsed,
            };
            thumb_CenterLeft = new Thumb()
            {
                Tag = "CenterLeft",
                Cursor = Cursors.SizeWE,
                Visibility = Visibility.Collapsed,
            };
            thumb_CenterRight = new Thumb()
            {
                Tag = "CenterRight",
                Cursor = Cursors.SizeWE,
                Visibility = Visibility.Collapsed,
            };

            thumb_CenterTop.DragDelta += Thumb_DragDelta;
            thumb_CenterBottom.DragDelta += Thumb_DragDelta;
            thumb_CenterLeft.DragDelta += Thumb_DragDelta;
            thumb_CenterRight.DragDelta += Thumb_DragDelta;
        }

        void CenterThumbInit()
        {
            thumb_Center = new Thumb()
            {
                Style = (Style)FindResource("RectangleBorderThumbStyle"),
                Tag = "Center",
                Cursor = Cursors.SizeAll,
                Focusable = true,
            };

            thumb_Center.GotFocus += (s, e) =>
            {
                thumb_CenterTop.Visibility = Visibility.Visible;
                thumb_CenterRight.Visibility = Visibility.Visible;
                thumb_CenterBottom.Visibility = Visibility.Visible;
                thumb_CenterLeft.Visibility = Visibility.Visible;

                thumb_TL.Visibility = Visibility.Visible;
                thumb_TR.Visibility = Visibility.Visible;
                thumb_BL.Visibility = Visibility.Visible;
                thumb_BR.Visibility = Visibility.Visible;
            };

            thumb_Center.LostFocus += (s, e) =>
            {
                thumb_CenterTop.Visibility = Visibility.Collapsed;
                thumb_CenterRight.Visibility = Visibility.Collapsed;
                thumb_CenterBottom.Visibility = Visibility.Collapsed;
                thumb_CenterLeft.Visibility = Visibility.Collapsed;

                thumb_TL.Visibility = Visibility.Collapsed;
                thumb_TR.Visibility = Visibility.Collapsed;
                thumb_BL.Visibility = Visibility.Collapsed;
                thumb_BR.Visibility = Visibility.Collapsed;
            };

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

            switch ((string)(((Thumb)sender).Tag))
            {
                case "TopLeft":
                    sizeFactor_Horizontal = -1;
                    sizeFactor_Vertical = -1;

                    positionFactor_Horizontal = 1;
                    positionFactor_Vertical = 1;
                    break;
                case "TopRight":
                    sizeFactor_Horizontal = 1;
                    sizeFactor_Vertical = -1;

                    positionFactor_Horizontal = 0;
                    positionFactor_Vertical = 1;
                    break;
                case "BottomLeft":
                    sizeFactor_Horizontal = -1;
                    sizeFactor_Vertical = 1;

                    positionFactor_Horizontal = 1;
                    positionFactor_Vertical = 0;
                    break;
                case "BottomRight":
                    sizeFactor_Horizontal = 1;
                    sizeFactor_Vertical = 1;

                    positionFactor_Horizontal = 0;
                    positionFactor_Vertical = 0;
                    break;
                case "CenterTop":
                    sizeFactor_Horizontal = 0;
                    sizeFactor_Vertical = -1;

                    positionFactor_Horizontal = 0;
                    positionFactor_Vertical = 1;
                    break;
                case "CenterBottom":
                    sizeFactor_Horizontal = 0;
                    sizeFactor_Vertical = 1;

                    positionFactor_Horizontal = 0;
                    positionFactor_Vertical = 0;
                    break;
                case "CenterLeft":
                    sizeFactor_Horizontal = -1;
                    sizeFactor_Vertical = 0;

                    positionFactor_Horizontal = 1;
                    positionFactor_Vertical = 0;
                    break;
                case "CenterRight":
                    sizeFactor_Horizontal = 1;
                    sizeFactor_Vertical = 0;

                    positionFactor_Horizontal = 0;
                    positionFactor_Vertical = 0;
                    break;
                case "Center":
                    sizeFactor_Horizontal = 0;
                    sizeFactor_Vertical = 0;

                    positionFactor_Horizontal = 1;
                    positionFactor_Vertical = 1;
                    break;
                default:
                    return;
            }

            double newWidth = element.Width + e.HorizontalChange * sizeFactor_Horizontal;
            double newHeight = element.Height + e.VerticalChange * sizeFactor_Vertical;

            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);

            if (newWidth >= MinSize.Width)
            {
                Canvas.SetLeft(element, left + e.HorizontalChange * positionFactor_Horizontal);
                element.Width = newWidth;
            }
            else
            {
                element.Width = MinSize.Width;
            }

            if (newHeight >= MinSize.Height)
            {
                Canvas.SetTop(element, top + e.VerticalChange * positionFactor_Vertical);
                element.Height = newHeight;
            }
            else
            {
                element.Height = MinSize.Height;
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
            //thumb_Center.Width = AdornedElement.DesiredSize.Width + (ThumbSize.Width) * 2;
            //thumb_Center.Height = AdornedElement.DesiredSize.Height + (ThumbSize.Height) * 2;
            thumb_Center.Arrange(
                new Rect(
                    x: -(ThumbMargin),
                    y: -(ThumbMargin),
                    width: AdornedElement.DesiredSize.Width + ThumbMargin * 2,
                    height: AdornedElement.DesiredSize.Height + ThumbMargin * 2)
                );

            // # 1. UPDATE LOCATION
            thumb_TL.Arrange(
                new Rect(
                    x: -(ThumbSize.Width / 2 + ThumbMargin),
                    y: -(ThumbSize.Height / 2 + ThumbMargin),
                    width: ThumbSize.Width,
                    height: ThumbSize.Height)
                );
            thumb_TR.Arrange(
                new Rect(
                    x: AdornedElement.DesiredSize.Width - (ThumbSize.Width / 2 - ThumbMargin),
                    y: -(ThumbSize.Width / 2 + ThumbMargin),
                    width: ThumbSize.Width,
                    height: ThumbSize.Height)
                );
            thumb_BL.Arrange(
                new Rect(
                    x: -(ThumbSize.Width / 2 + ThumbMargin),
                    y: AdornedElement.DesiredSize.Height - (ThumbSize.Height / 2 - ThumbMargin),
                    width: ThumbSize.Width,
                    height: ThumbSize.Height)
                );
            thumb_BR.Arrange(
                new Rect(
                    x: AdornedElement.DesiredSize.Width - (ThumbSize.Width / 2 - ThumbMargin),
                    y: AdornedElement.DesiredSize.Height - (ThumbSize.Height / 2 - ThumbMargin),
                    width: ThumbSize.Width,
                    height: ThumbSize.Height)
                );

            thumb_CenterTop.Arrange(
                new Rect(
                    x: (AdornedElement.DesiredSize.Width / 2) - ThumbSize.Width / 2,
                    y: -(ThumbSize.Width / 2 + ThumbMargin),
                    width: ThumbSize.Width,
                    height: ThumbSize.Height)
                );
            thumb_CenterBottom.Arrange(
                new Rect(
                    x: (AdornedElement.DesiredSize.Width / 2) - ThumbSize.Width / 2,
                    y: AdornedElement.DesiredSize.Height - (ThumbSize.Width / 2 - ThumbMargin),
                    width: ThumbSize.Width,
                    height: ThumbSize.Height)
                );
            thumb_CenterLeft.Arrange(
                new Rect(
                    x: -(ThumbSize.Width / 2 + ThumbMargin),
                    y: (AdornedElement.DesiredSize.Height / 2) - (ThumbSize.Height / 2),
                    width: ThumbSize.Width,
                    height: ThumbSize.Height)
                );
            thumb_CenterRight.Arrange(
                new Rect(
                    x: AdornedElement.DesiredSize.Width - (ThumbSize.Width / 2 - ThumbMargin),
                    y: (AdornedElement.DesiredSize.Height / 2) - (ThumbSize.Width / 2),
                    width: ThumbSize.Width,
                    height: ThumbSize.Height)
                );

            label_Name.Text = Description;
            label_Name.Arrange(
                new Rect(
                    x: 0,
                    y: -13,
                    width: AdornedElement.DesiredSize.Width,
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
