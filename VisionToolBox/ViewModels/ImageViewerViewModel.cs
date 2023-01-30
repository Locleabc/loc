using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using TopCom;
using TopCom.Command;

namespace VisionToolBox.ViewModels
{
    public enum EImageViewerWorkMode
    {
        Display = 0,
        /// <summary>
        /// Moving Image Position
        /// </summary>
        Drag,
        Draw
    }

    public class ImageViewerViewModel : PropertyChangedNotifier
    {
        #region Properties
        public ObservableCollection<ContentControl> BoxCollection { get; set; } = new ObservableCollection<ContentControl>();

        private double _ImageScaleRate;

        public double ImageScaleRate
        {
            get { return _ImageScaleRate; }
            set
            {
                if (_ImageScaleRate != value)
                {
                    _ImageScaleRate = value;
                    OnPropertyChanged("ImageScaleRate");
                }
            }
        }


        private EImageViewerWorkMode _ViewerWorkMode;
        public EImageViewerWorkMode ViewerWorkMode
        {
            get { return _ViewerWorkMode; }
            set
            {
                if (_ViewerWorkMode != value)
                {
                    _ViewerWorkMode = value;
                    OnPropertyChanged("ViewerWorkMode");
                }
            }
        }

        public ScaleTransform Scale { get; set; } = new ScaleTransform(1, 1);
        public double ScaleCenterX { get; set; } = 0;
        public double ScaleCenterY { get; set; } = 0;

        private double _DragShiftX = 0;
        public double DragShiftX
        {
            get { return _DragShiftX; }
            set
            {
                _DragShiftX = value;
                OnPropertyChanged("DragShiftX");
            }
        }

        private double _DragShiftY = 0;
        public double DragShiftY
        {
            get { return _DragShiftY; }
            set
            {
                _DragShiftY = value;
                OnPropertyChanged("DragShiftY");
            }
        }

        private bool _FixZoomCenter;
        public bool FixZoomCenter
        {
            get { return _FixZoomCenter; }
            set
            {
                _FixZoomCenter = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Command
        public RelayCommand AdonerDeleteCommand
        {
            get
            {
                return new RelayCommand((o) => {
                    if (o is ContentControl == false) return;
                    
                    if (BoxCollection.Contains((ContentControl)o) == false) return;

                    BoxCollection.Remove((ContentControl)o);
                });
            }
        }

        public RelayCommand ImageViewerContextMenu_DrawCircleCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
#if SHAREWARE
                    return;
#else
                    var resourceDictionary = new ResourceDictionary();
                    resourceDictionary.Source =
                        new Uri("/VisionToolBox;component/MoveResizeRotateTool/Resources/DesignerItem.xaml",
                                UriKind.RelativeOrAbsolute);

                    ContextMenu boxContextMenu = new ContextMenu();

                    MenuItem item = new MenuItem();
                    item.Header = "Delete";

                    boxContextMenu.Items.Add(item);
                    item.Command = AdonerDeleteCommand;

                    var contentControl = new ContentControl
                    {
                        Width = 100,
                        Height = 100,
                        Style = (Style)resourceDictionary["DesignerItemStyle"],

                        ContextMenu = boxContextMenu,

                        Content = new Ellipse
                        {
                            StrokeThickness = 0.8,
                            Stroke = Brushes.Red,
                        }
                    };

                    item.CommandParameter = contentControl;

                    Canvas.SetLeft(contentControl, 100);
                    Canvas.SetTop(contentControl, 100);

                    BoxCollection.Add(contentControl);
#endif
                });
            }
        }

        public RelayCommand ImageViewerContextMenu_DrawRectangleCommand
        {
            get
            {
                return new RelayCommand((o) => {
#if SHAREWARE
                    return;
#else
                    var resourceDictionary = new ResourceDictionary();
                    resourceDictionary.Source =
                        new Uri("/VisionToolBox;component/MoveResizeRotateTool/Resources/DesignerItem.xaml",
                                UriKind.RelativeOrAbsolute);

                    ContextMenu boxContextMenu = new ContextMenu();

                    MenuItem item = new MenuItem();
                    item.Header = "Delete";

                    boxContextMenu.Items.Add(item);
                    item.Command = AdonerDeleteCommand;

                    var contentControl = new ContentControl
                    {
                        Width = 100,
                        Height = 100,
                        Style = (Style)resourceDictionary["DesignerItemStyle"],

                        ContextMenu = boxContextMenu,

                        Content = new Border
                        {
                            BorderThickness = new Thickness(0.8),
                            BorderBrush = Brushes.Red,
                        }

                        //Content = new Path
                        //{
                        //    Stroke = Brushes.Red,
                        //    StrokeThickness = 2,
                        //    Data = new PathGeometry
                        //    {
                        //        Figures = new PathFigureCollection
                        //        {
                        //            new PathFigure
                        //            {
                        //                StartPoint = new System.Windows.Point(0, 0),
                        //                Segments = new PathSegmentCollection
                        //                {
                        //                    new ArcSegment
                        //                    {
                        //                        Point = new System.Windows.Point(100, 100),
                        //                        Size = new System.Windows.Size(50, 25)
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    };

                    item.CommandParameter = contentControl;

                    Canvas.SetLeft(contentControl, 100);
                    Canvas.SetTop(contentControl, 100);

                    BoxCollection.Add(contentControl);
#endif
                });
            }
        }

        public RelayCommand ImageViewerContextMenu_ZoomInCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (Scale.ScaleX < 10)
                    {
                        // Zoom-in in 10% increments
                        Scale.ScaleX += 0.5;
                        Scale.ScaleY += 0.5;

                        Scale.CenterX = ScaleCenterX;
                        Scale.CenterY = ScaleCenterY;

                        if (Scale.ScaleX <= 1.0)
                        {
                            DragShiftX = 0;
                            DragShiftY = 0;

                            ViewerWorkMode = ViewModels.EImageViewerWorkMode.Display;
                        }
                        else
                        {
                            ViewerWorkMode = ViewModels.EImageViewerWorkMode.Drag;
                        }
                    }
                });
            }
        }

        public RelayCommand ImageViewerContextMenu_ZoomOutCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (Scale.ScaleX > 1.0)
                    {
                        // Zoom-in in 10% increments
                        Scale.ScaleX -= 0.5;
                        Scale.ScaleY -= 0.5;

                        Scale.CenterX = ScaleCenterX;
                        Scale.CenterY = ScaleCenterY;

                        if (Scale.ScaleX <= 1.0)
                        {
                            DragShiftX = 0;
                            DragShiftY = 0;

                            ViewerWorkMode = ViewModels.EImageViewerWorkMode.Display;
                        }
                        else
                        {
                            ViewerWorkMode = ViewModels.EImageViewerWorkMode.Drag;
                        }
                    }
                });
            }
        }

        public RelayCommand ImageViewerContextMenu_OriginalSizeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    Scale.ScaleX = 1;
                    Scale.ScaleY = 1;

                    DragShiftX = 0;
                    DragShiftY = 0;

                    ViewerWorkMode = ViewModels.EImageViewerWorkMode.Display;
                });
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            //BoxCollection.Remove(sender as MenuItem);
        }

        private ICommand _viewerDragCommand;

        public ICommand ViewerDragCommand
        {
            get
            {
                return _viewerDragCommand ?? (_viewerDragCommand = new RelayCommand<DragDeltaEventArgs>((e) => {
                    switch (ViewerWorkMode)
                    {
                        case EImageViewerWorkMode.Display:
                            break;
                        case EImageViewerWorkMode.Drag:
                            if (Scale.ScaleX <= 1.0)
                            {
                                return;
                            }

                            DragShiftX += (e as DragDeltaEventArgs).HorizontalChange;
                            DragShiftY += (e as DragDeltaEventArgs).VerticalChange;
                            break;
                        case EImageViewerWorkMode.Draw:
                            break;
                        default:
                            break;
                    }
                }));
            }
        }
#endregion
    }
}
