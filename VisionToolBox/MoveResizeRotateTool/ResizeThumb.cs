using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;

namespace VisionToolBox.MoveResizeRotateTool
{
    public class ResizeThumb : Thumb
    {
        private RotateTransform rotateTransform;
        private double angle;
        private Adorner adorner;
        private Point transformOrigin;
        private ContentControl designerItem;
        private Canvas canvas;

        public ResizeThumb()
        {
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(this.ResizeThumb_DragCompleted);
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = this.DataContext as ContentControl;

            if (this.designerItem != null)
            {
                this.canvas = VisualTreeHelper.GetParent(this.designerItem) as Canvas;

                if (this.canvas != null)
                {
                    this.transformOrigin = this.designerItem.RenderTransformOrigin;

                    this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                    if (this.rotateTransform != null)
                    {
                        this.angle = this.rotateTransform.Angle * Math.PI / 180.0;
                    }
                    else
                    {
                        this.angle = 0.0d;
                    }

                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.canvas);
                    if (adornerLayer != null)
                    {
                        this.adorner = new SizeAdorner(this.designerItem);
                        adornerLayer.Add(this.adorner);
                    }
                }
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null)
            {
                double deltaVertical = 0, deltaHorizontal = 0;
                bool verticalResized = false, horizontalResized = false;

                switch (VerticalAlignment)
                {
                    case System.Windows.VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, this.designerItem.ActualHeight - this.designerItem.MinHeight);
                        if (HorizontalAlignment == HorizontalAlignment.Stretch)
                        {
                            Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) + (this.transformOrigin.Y * deltaVertical * (1 - Math.Cos(-this.angle))));
                            Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) - deltaVertical * this.transformOrigin.Y * Math.Sin(-this.angle));
                        }
                        verticalResized = true;
                        //this.designerItem.Height -= deltaVertical;
                        break;
                    case System.Windows.VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, this.designerItem.ActualHeight - this.designerItem.MinHeight);
                        if (HorizontalAlignment == HorizontalAlignment.Stretch)
                        {
                            Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) + deltaVertical * Math.Cos(-this.angle) + (this.transformOrigin.Y * deltaVertical * (1 - Math.Cos(-this.angle))));
                            Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) + deltaVertical * Math.Sin(-this.angle) - (this.transformOrigin.Y * deltaVertical * Math.Sin(-this.angle)));
                        }
                        verticalResized = true;
                        //this.designerItem.Height -= deltaVertical;
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case System.Windows.HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, this.designerItem.ActualWidth - this.designerItem.MinWidth);
                        if (VerticalAlignment == VerticalAlignment.Stretch)
                        {
                            Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) + deltaHorizontal * Math.Sin(this.angle) - this.transformOrigin.X * deltaHorizontal * Math.Sin(this.angle));
                            Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) + deltaHorizontal * Math.Cos(this.angle) + (this.transformOrigin.X * deltaHorizontal * (1 - Math.Cos(this.angle))));
                        }
                        horizontalResized = true;
                        //this.designerItem.Width -= deltaHorizontal;
                        break;
                    case System.Windows.HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, this.designerItem.ActualWidth - this.designerItem.MinWidth);
                        if (VerticalAlignment == VerticalAlignment.Stretch)
                        {
                            Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) - this.transformOrigin.X * deltaHorizontal * Math.Sin(this.angle));
                            Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) + (deltaHorizontal * this.transformOrigin.X * (1 - Math.Cos(this.angle))));
                        }
                        horizontalResized = true;
                        //this.designerItem.Width -= deltaHorizontal;
                        break;
                    default:
                        break;
                }

                if (horizontalResized && verticalResized)
                {
                    double deltaAll = Math.Min(deltaHorizontal, deltaVertical);

                    this.designerItem.Width -= deltaAll;
                    this.designerItem.Height -= deltaAll;
                    if (VerticalAlignment == VerticalAlignment.Top && HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) + deltaAll * Math.Cos(-this.angle) + (this.transformOrigin.Y * deltaAll * (1 - Math.Cos(-this.angle))));
                        Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) + (deltaAll * this.transformOrigin.X * (1 - Math.Cos(this.angle))));
                    }
                    else if (VerticalAlignment == VerticalAlignment.Top && HorizontalAlignment == HorizontalAlignment.Left)
                    {
                        Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) + deltaAll * Math.Cos(-this.angle) + (this.transformOrigin.Y * deltaAll * (1 - Math.Cos(-this.angle))));
                        Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) + deltaAll * Math.Cos(this.angle) + (this.transformOrigin.X * deltaAll * (1 - Math.Cos(this.angle))));
                    }
                    else if (VerticalAlignment == VerticalAlignment.Bottom && HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) + (this.transformOrigin.Y * deltaAll * (1 - Math.Cos(-this.angle))));
                        Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) + (deltaAll * this.transformOrigin.X * (1 - Math.Cos(this.angle))));
                    }
                    else if (VerticalAlignment == VerticalAlignment.Bottom && HorizontalAlignment == HorizontalAlignment.Left)
                    {
                        Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) + (this.transformOrigin.Y * deltaAll * (1 - Math.Cos(-this.angle))));
                        Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) + deltaAll * Math.Cos(this.angle) + (this.transformOrigin.X * deltaAll * (1 - Math.Cos(this.angle))));
                    }
                }
                else if (horizontalResized)
                {
                    this.designerItem.Width -= deltaHorizontal;
                }
                else if (verticalResized)
                {
                    this.designerItem.Height -= deltaVertical;
                }
            }

            e.Handled = true;
        }

        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.adorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.canvas);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.adorner);
                }

                this.adorner = null;
            }
        }
    }
}
