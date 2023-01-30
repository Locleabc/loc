using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TopCom;
using TopCom.Command;
using TopCom.Define;
using TopVision.Adorners;
using TopVision.Models;
using TopVision.Helpers;
using System.Windows.Media;
using TopVision.Algorithms;

namespace TopVision.ViewModels
{
    public class VisionProcessViewModel : PropertyChangedNotifier
    {
        public EventHandler OnMainProcessChange { get; set; }

        public int ImageHeight
        {
            get
            {
                if (MainProcess.InputMat.IsNullOrEmpty()) return Default.ImageHeight;

                return MainProcess.InputMat.Height > 0 ? MainProcess.InputMat.Height : Default.ImageHeight;
            }
        }

        public int ImageWidth
        {
            get
            {
                if (MainProcess.InputMat.IsNullOrEmpty()) return Default.ImageWidth;

                return MainProcess.InputMat.Width > 0 ? MainProcess.InputMat.Width : Default.ImageWidth;
            }
        }

        #region Properties
        public IVisionProcess MainProcess
        {
            get { return _MainProcess; }
            set
            {
                if (_MainProcess == value) return;

                _MainProcess = value;
                SelectedVisionProcess = value;
                DisplayImage = value.InputMat;

                OnPropertyChanged();
                OnPropertyChanged("MainProcessDumbList");
            }
        }

        public IVisionProcess SelectedVisionProcess
        {
            get { return _SelectedVisionProcess; }
            set
            {
                _SelectedVisionProcess = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IVisionProcess> MainProcessDumbList
        {
            get { return new ObservableCollection<IVisionProcess> { MainProcess }; }
        }

        public Mat DisplayImage
        {
            get { return _DisplayImage; }
            set
            {
                _DisplayImage = value;
                OnPropertyChanged();
            }
        }

        public UIElement UIElement { get; set; }
        public Panel UIPanel { get; set; }
        #endregion

        #region Commands
        public RelayCommand ProcessMoveUpCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
#if SHAREWARE
                    // In Shareware mode, user have no right access this function
                    return;
#endif
                    if (o is IVisionProcess)
                    {
                        if (MainProcess.PreProcessors.Contains((IVisionProcess)o))
                        {
                            int index = MainProcess.PreProcessors.IndexOf((IVisionProcess)o);

                            if (index > 0)
                            {
                                MainProcess.PreProcessors.Move(index, index - 1);
                            }
                        }
                        if (MainProcess == (IVisionProcess)o)
                        {
                            return;
                        }
                        if (MainProcess.SiblingProcessors.Contains((IVisionProcess)o))
                        {
                            int index = MainProcess.SiblingProcessors.IndexOf((IVisionProcess)o);

                            if (index > 0)
                            {
                                MainProcess.SiblingProcessors.Move(index, index - 1);
                            }
                        }
                    }
                });
            }
        }

        public RelayCommand ProcessMoveDownCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
#if SHAREWARE
                    // In Shareware mode, user have no right access this function
                    return;
#endif
                    if (o is IVisionProcess)
                    {
                        if (MainProcess.PreProcessors.Contains((IVisionProcess)o))
                        {
                            int index = MainProcess.PreProcessors.IndexOf((IVisionProcess)o);

                            if (index < MainProcess.PreProcessors.Count - 1)
                            {
                                MainProcess.PreProcessors.Move(index, index + 1);
                            }
                        }
                        if (MainProcess == (IVisionProcess)o)
                        {
                            return;
                        }
                        if (MainProcess.SiblingProcessors.Contains((IVisionProcess)o))
                        {
                            int index = MainProcess.SiblingProcessors.IndexOf((IVisionProcess)o);

                            if (index < MainProcess.PreProcessors.Count - 1)
                            {
                                MainProcess.SiblingProcessors.Move(index, index + 1);
                            }
                        }
                    }
                });
            }
        }

        public RelayCommand ProcessSelectCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o is IVisionProcess)
                    {
                        SelectedVisionProcess = (IVisionProcess)o;

                        DisplayImage = MainProcess.InputMat;

                        if (DisplayImage != null)
                        {
                            DrawAdorners();
                        }
                    }
                });
            }
        }

        public RelayCommand ProcessDeleteCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
#if SHAREWARE
                    // In Shareware mode, user have no right access this function
                    return;
#endif
                    if (o is IVisionProcess)
                    {
                        if (MainProcess.PreProcessors.Contains((IVisionProcess)o))
                        {
                            MainProcess.PreProcessors.Remove((IVisionProcess)o);
                        }
                        if (MainProcess == (IVisionProcess)o)
                        {
                            return;
                        }
                        if (MainProcess.SiblingProcessors.Contains((IVisionProcess)o))
                        {
                            MainProcess.SiblingProcessors.Remove((IVisionProcess)o);
                        }
                    }
                });
            }
        }

        public RelayCommand ProcessChangeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
#if SHAREWARE
                    // In Shareware mode, user have no right access this function
                    return;
#endif
                    if (o is IVisionProcess)
                    {
                        if (MainProcess.PreProcessors.Contains((IVisionProcess)o))
                        {
                            ContextMenu cm = Application.Current.FindResource("PreProcessContextMenu") as ContextMenu;
                            cm.DataContext = this;
                            foreach (object menu in cm.Items)
                            {
                                if (menu.GetType() != typeof(MenuItem)) continue;
                                ((MenuItem)menu).Command = ProcessChangeCommand;
                            }
                            cm.PlacementTarget = o as Button;
                            cm.IsOpen = true;
                        }
                        if (MainProcess == (IVisionProcess)o || MainProcess.SiblingProcessors.Contains((IVisionProcess)o))
                        {
                            ContextMenu cm = Application.Current.FindResource("MainsProcessContextMenu") as ContextMenu;
                            cm.DataContext = this;
                            foreach (object menu in cm.Items)
                            {
                                if (menu.GetType() != typeof(MenuItem)) continue;
                                ((MenuItem)menu).Command = ProcessChangeCommand;
                            }
                            cm.PlacementTarget = o as Button;
                            cm.IsOpen = true;
                        }

                        ToChangeProcess = o as IVisionProcess;
                    }
                    else
                    {
                        if (o is Type)
                        {
                            object instance = (IVisionProcess)Activator.CreateInstance((Type)o);

                            if (MainProcess.PreProcessors.Contains(ToChangeProcess))
                            {
                                int index = MainProcess.PreProcessors.IndexOf(ToChangeProcess);

                                MainProcess.PreProcessors[index] = (IVisionProcess)instance;
                            }
                            if (MainProcess == ToChangeProcess)
                            {
                                //MainProcess = (IVisionProcess)instance;
                                if (OnMainProcessChange != null)
                                {
                                    OnMainProcessChange.Invoke((IVisionProcess)instance, EventArgs.Empty);
                                }
                            }
                            if (MainProcess.SiblingProcessors.Contains(ToChangeProcess))
                            {
                                int index = MainProcess.SiblingProcessors.IndexOf(ToChangeProcess);

                                MainProcess.SiblingProcessors[index] = (IVisionProcess)instance;
                            }
                        }
                    }
                });
            }
        }

        public RelayCommand AddPreProcessCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
#if SHAREWARE
                    // In Shareware mode, user have no right access this function
                    return;
#endif

                    if (o is Button)
                    {
                        ContextMenu cm = Application.Current.FindResource("PreProcessContextMenu") as ContextMenu;
                        cm.DataContext = this;
                        foreach (object menu in cm.Items)
                        {
                            if (menu.GetType() != typeof(MenuItem)) continue;
                            ((MenuItem)menu).Command = AddPreProcessCommand;
                        }
                        cm.PlacementTarget = o as Button;
                        cm.IsOpen = true;
                    }
                    else
                    {
                        if (o is Type)
                        {
                            object instance = Activator.CreateInstance((Type)o);
                            ((IVisionProcess)instance).IsPreProcess = true;
                            MainProcess.PreProcessors.Add((IVisionProcess)instance);
                        }
                    }
                });
            }
        }

        public RelayCommand AddSiblingProcessCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
#if SHAREWARE
                    // In Shareware mode, user have no right access this function
                    return;
#endif
                    if (o is Button)
                    {
                        ContextMenu cm = Application.Current.FindResource("MainsProcessContextMenu") as ContextMenu;
                        cm.DataContext = this;
                        foreach (object menu in cm.Items)
                        {
                            if (menu.GetType() != typeof(MenuItem)) continue;
                            ((MenuItem)menu).Command = AddSiblingProcessCommand;
                        }
                        cm.PlacementTarget = o as Button;
                        cm.IsOpen = true;
                    }
                    else
                    {
                        if (o is Type)
                        {
                            object instance = Activator.CreateInstance((Type)o);
                            ((IVisionProcess)instance).IsSiblingProcess = true;
                            MainProcess.SiblingProcessors.Add((IVisionProcess)instance);
                        }
                    }
                });
            }
        }

        public RelayCommand InputImageShowCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    ClearAdorners();

                    DisplayImage = MainProcess.InputMat;
                });
            }
        }

        public RelayCommand PreProcessedImageShowCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    ClearAdorners();

                    MainProcess.ExcutePreProcessors(isManual: true);
                    DisplayImage = MainProcess.PreProcessedMat;
                });
            }
        }

        public RelayCommand OutputImageShowCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    ClearAdorners();

                    MainProcess.Run();
                    DisplayImage = MainProcess.OutputMat;
                });
            }
        }

        public RelayCommand TeachingCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    ClearAdorners();

                    MainProcess.Run(isTeachingMode: true);
                    DisplayImage = MainProcess.OutputMat;
                });
            }
        }
        #endregion

        #region Constructor(s)
        public VisionProcessViewModel()
        {
        }
        #endregion

        public void ClearAdorners()
        {
            UIPanel.Children.Clear();
        }

        public void DrawAdorners()
        {
            ClearAdorners();

            if (SelectedVisionProcess.IsPreProcess)
            {
                return;
            }

            double sizeFactorX = UIElement.RenderSize.Width / ImageWidth;
            double sizeFactorY = UIElement.RenderSize.Height / ImageHeight;

            foreach (CRectangle rect in SelectedVisionProcess.Parameter.ROIs)
            {
                Border border = new Border()
                {
                    Width = rect.Width * sizeFactorX,
                    Height = rect.Height * sizeFactorY,
                    BorderBrush = Brushes.Red,
                    BorderThickness = new Thickness(2),
                };
                ((Canvas)UIPanel).Children.Add(border);
                Canvas.SetLeft(border, rect.X * sizeFactorX);
                Canvas.SetTop(border, rect.Y * sizeFactorY);

                RectangleResizeDragAdorner resizeAdorner = new RectangleResizeDragAdorner(border) { Description = "ROI" };
                resizeAdorner.AdornerArranged += (obj, agr) =>
                {
                    rect.Width = (int)(((RectangleResizeDragAdorner)obj).AdornedElement.DesiredSize.Width / sizeFactorX);
                    rect.Height = (int)(((RectangleResizeDragAdorner)obj).AdornedElement.DesiredSize.Height / sizeFactorY);
                    rect.X = (int)(Canvas.GetLeft(((RectangleResizeDragAdorner)obj).AdornedElement) / sizeFactorX);
                    rect.Y = (int)(Canvas.GetTop(((RectangleResizeDragAdorner)obj).AdornedElement) / sizeFactorY);
                };

                AdornerLayer.GetAdornerLayer(UIPanel).Add(resizeAdorner);
            }

            if (SelectedVisionProcess is CircularObjectDetection)
            {
                var process = (CircularObjectDetection)SelectedVisionProcess;

                int teachingOffsetX = (int)((CircularObjectDetectionParameter)(process.Parameter)).TeachingFixtureOffset.FromMillimeterToPixel(process.PixelSize / 1000).X;
                int teachingOffsetY = (int)((CircularObjectDetectionParameter)(process.Parameter)).TeachingFixtureOffset.FromMillimeterToPixel(process.PixelSize / 1000).Y;

                Border border = new Border()
                {
                    Width = ((CircularObjectDetectionParameter)(process.Parameter)).Radius * 2 * sizeFactorX,
                    Height = ((CircularObjectDetectionParameter)(process.Parameter)).Radius * 2 * sizeFactorY,
                    BorderBrush = Brushes.Blue,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(9999),
                };

                Canvas.SetLeft(border, UIElement.RenderSize.Width / 2 - border.Width / 2 + teachingOffsetX * sizeFactorX);
                Canvas.SetTop(border, UIElement.RenderSize.Height / 2 - border.Height / 2 + teachingOffsetY * sizeFactorX);

                ((Canvas)UIPanel).Children.Add(border);

                CircleResizeDragAdorner resizeAdorner = new CircleResizeDragAdorner(border) { Description = "Circle" };
                resizeAdorner.AdornerArranged += (obj, agr) =>
                {
                    ((CircularObjectDetectionParameter)(process.Parameter)).Radius = (((CircleResizeDragAdorner)obj).AdornedElement.DesiredSize.Width / sizeFactorX) / 2;
                };

                AdornerLayer.GetAdornerLayer(UIPanel).Add(resizeAdorner);
            }

            if (SelectedVisionProcess is SingleTemplateMatching)
            {
                var process = (SingleTemplateMatching)SelectedVisionProcess;
                CRectangle rect = ((SingleTemplateMatchingParameter)(process.Parameter)).TemplateTechingRectangle;

                Border border = new Border()
                {
                    Width = rect.Width * sizeFactorX,
                    Height = rect.Height * sizeFactorY,
                    BorderBrush = Brushes.Blue,
                    BorderThickness = new Thickness(2),
                };

                Canvas.SetLeft(border, (rect.X) * sizeFactorX);
                Canvas.SetTop(border, (rect.Y) * sizeFactorY);

                ((Canvas)UIPanel).Children.Add(border);

                RectangleResizeDragAdorner resizeAdorner = new RectangleResizeDragAdorner(border) { Description = "Template" };
                resizeAdorner.AdornerArranged += (obj, agr) =>
                {
                    rect.Width = (int)(((RectangleResizeDragAdorner)obj).AdornedElement.DesiredSize.Width / sizeFactorX);
                    rect.Height = (int)(((RectangleResizeDragAdorner)obj).AdornedElement.DesiredSize.Height / sizeFactorY);
                    rect.X = (int)(Canvas.GetLeft(((RectangleResizeDragAdorner)obj).AdornedElement) / sizeFactorX);
                    rect.Y = (int)(Canvas.GetTop(((RectangleResizeDragAdorner)obj).AdornedElement) / sizeFactorY);
                };

                AdornerLayer.GetAdornerLayer(UIPanel).Add(resizeAdorner);
            }
        }

        #region Privates
        private IVisionProcess _MainProcess;
        private IVisionProcess _SelectedVisionProcess;
        private IVisionProcess ToChangeProcess { get; set; }
        private Mat _DisplayImage;
        #endregion
    }
}
