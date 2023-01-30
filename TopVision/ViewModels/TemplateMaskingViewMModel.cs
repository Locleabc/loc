using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using TopCom;
using TopCom.Command;
using TopVision.Helpers;

namespace TopVision.ViewModels
{
    public class TemplateMaskingViewModel : PropertyChangedNotifier
    {
        #region Properties
        public double PenSize
        {
            get { return _PenSize; }
            set
            {
                _PenSize = value;
                OnPropertyChanged();

                UpdateInkAttributes();
            }
        }

        public StrokeCollection Strokes
        {
            get { return _Strokes; }
            set
            {
                _Strokes = value;
                OnPropertyChanged();
            }
        }

        public System.Windows.Ink.DrawingAttributes InkAttributes
        {
            get { return _InkAttributes; }
            set
            {
                _InkAttributes = value;
                OnPropertyChanged();
            }
        }

        public InkCanvas TheInkCanvas { get; set; }

        public InkCanvasEditingMode EditMode
        {
            get { return _EditMode; }
            set
            {
                _EditMode = value;
                OnPropertyChanged();
            }
        }

        public Mat TemplateMat
        {
            get { return _TemplateMat; }
            set
            {
                _TemplateMat = value;
                OnPropertyChanged();
                OnPropertyChanged("InkWidth");
                OnPropertyChanged("InkHeight");
            }
        }

        public double ScaleRate
        {
            get
            {
                return 600.0 / TemplateMat.Width;
            }
        }

        public double InkWidth
        {
            get
            {
                return TemplateMat.Width * ScaleRate;
            }
        }

        public double InkHeight
        {
            get
            {
                return TemplateMat.Height * ScaleRate;
            }
        }

        public Mat MaskMat { get; set; }
        #endregion

        #region Commands
        public RelayCommand GenerateMaskMat
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (TemplateMat.IsNullOrEmpty())
                    {
                        MessageBox.Show("Template is null or empty");
                        return;
                    }

                    MaskMat = Mat.Zeros(TemplateMat.Size(), MatType.CV_8U);

                    foreach (var stroke in Strokes)
                    {
                        double actualStrokeRadius = stroke.DrawingAttributes.Height / ScaleRate / 2.0;

                        foreach (var point in stroke.StylusPoints)
                        {
                            OpenCvSharp.Point markPoint = new OpenCvSharp.Point(point.X / ScaleRate, point.Y / ScaleRate);
                            Cv2.Circle(MaskMat, markPoint, (int)actualStrokeRadius, 255, thickness: -1);
                        }
                    }

                    Cv2.BitwiseNot(MaskMat, MaskMat);

#if SIMULATION
                    //try
                    //{
                    //    using (OpenCvSharp.Window window = new OpenCvSharp.Window("Template image", MaskMat, flags: WindowFlags.KeepRatio))
                    //    {
                    //        int width;
                    //        int height;

                    //        if (((double)MaskMat.Width / (double)MaskMat.Height) > 600.0 / 400.0)
                    //        {
                    //            width = 600;
                    //            height = (int)((double)MaskMat.Height * width / (double)MaskMat.Width);
                    //        }
                    //        else
                    //        {
                    //            height = 400;
                    //            width = (int)((double)MaskMat.Width * height / (double)MaskMat.Height);
                    //        }

                    //        window.Resize(width, height);
                    //        //window.ShowImage(imgTemplate);
                    //        Cv2.WaitKey(10000);
                    //    }
                    //}
                    //catch { }
#endif
                });
            }
        }

        public RelayCommand UndoCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (Strokes.Count > 0)
                    {
                        int lastIndex = Strokes.Count - 1;
                        _UndoStrokes.Push(Strokes[lastIndex]);
                        Strokes.RemoveAt(lastIndex);
                    }
                });
            }
        }

        public RelayCommand RedoCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (_UndoStrokes.Count > 0)
                    {
                        Stroke stroke = _UndoStrokes.Pop();

                        Strokes.Add(stroke);
                    }
                });
            }
        }

        public RelayCommand PenCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    EditMode = InkCanvasEditingMode.Ink;
                });
            }
        }

        public RelayCommand EraseCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    EditMode = InkCanvasEditingMode.EraseByPoint;
                });
            }
        }

        public RelayCommand ShowButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (TemplateMat.IsNullOrEmpty())
                    {
                        MessageBox.Show("Template is null or empty");
                        return;
                    }

                    MaskMat = Mat.Zeros(TemplateMat.Size(), MatType.CV_8U);

                    foreach (var stroke in Strokes)
                    {
                        double actualStrokeRadius = stroke.DrawingAttributes.Height / ScaleRate / 2.0;

                        foreach (var point in stroke.StylusPoints)
                        {
                            OpenCvSharp.Point markPoint = new OpenCvSharp.Point(point.X / ScaleRate, point.Y / ScaleRate);
                            Cv2.Circle(MaskMat, markPoint, (int)actualStrokeRadius, 255, thickness: -1);
                        }
                    }

                    Cv2.BitwiseNot(MaskMat, MaskMat);
                    try
                    {
                        using (OpenCvSharp.Window window = new OpenCvSharp.Window("Template image", MaskMat, flags: WindowFlags.KeepRatio))
                        {
                            int width;
                            int height;

                            if (((double)MaskMat.Width / (double)MaskMat.Height) > 600.0 / 400.0)
                            {
                                width = 600;
                                height = (int)((double)MaskMat.Height * width / (double)MaskMat.Width);
                            }
                            else
                            {
                                height = 400;
                                width = (int)((double)MaskMat.Width * height / (double)MaskMat.Height);
                            }

                            window.Resize(width, height);
                            //window.ShowImage(imgTemplate);
                            Cv2.WaitKey(10000);
                        }
                    }
                    catch { }
                });
            }
        }
        #endregion

        #region Constructors
        public TemplateMaskingViewModel()
        {
            UpdateInkAttributes();
        }
        #endregion

        #region Methods & Events
        private void UpdateInkAttributes()
        {
            InkAttributes = new System.Windows.Ink.DrawingAttributes()
            {
                Color = Color.FromRgb(0xf0, 0x03, 0xfc),
                Width = PenSize,
                Height = PenSize,
                IsHighlighter = true
            };

            if (TheInkCanvas != null)
            {
                TheInkCanvas.EraserShape = new RectangleStylusShape(PenSize, PenSize);

                if (EditMode == InkCanvasEditingMode.EraseByPoint || EditMode == InkCanvasEditingMode.EraseByStroke)
                {
                    InkCanvasEditingMode oldMode = EditMode;
                    EditMode = InkCanvasEditingMode.None;
                    EditMode = oldMode;
                }
            }
        }
        #endregion

        #region Privates
        private double _PenSize = 20;
        private StrokeCollection _Strokes = new StrokeCollection();
        private System.Windows.Ink.DrawingAttributes _InkAttributes;

        private StylusShape _EraseShape = new RectangleStylusShape(20, 20);
        private InkCanvasEditingMode _EditMode = InkCanvasEditingMode.Ink;

        private Mat _TemplateMat;

        #region Strokes undo redo handle
        private Stack<Stroke> _UndoStrokes = new Stack<Stroke>();
        #endregion
        #endregion
    }
}
