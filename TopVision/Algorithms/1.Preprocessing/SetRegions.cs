using TopVision.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using TopCom.Command;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision Parameter for <see cref="SetRegions"/> process
    /// </summary>
    public class SetRegionsParameter : SetIgnoreRegionsParameter
    {
        #region Properties
        public ObservableCollection<CRectangle> InspectRectRegions
        {
            get { return _InspectRectRegions; }
            set
            {
                _InspectRectRegions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CCircle> InspectCircleRegions
        {
            get { return _InspectCircleRegions; }
            set
            {
                _InspectCircleRegions = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public new RelayCommand CCircleAddCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o is ObservableCollection<CCircle>)
                    {
                        (o as ObservableCollection<CCircle>).Add(new CCircle(new CPoint2f(0, 0), 0));
                    }
                });
            }
        }

        public new RelayCommand CCircleDeleteCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o is CCircle)
                    {
                        CCircle rect = o as CCircle;

                        if (InspectCircleRegions.Contains(rect))
                        {
                            InspectCircleRegions.Remove(rect);
                        }
                        if (IgnoreCircleRegions.Contains(rect))
                        {
                            IgnoreCircleRegions.Remove(rect);
                        }
                    }
                });
            }
        }

        public override RelayCommand CRectangleDeleteCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o is CRectangle)
                    {
                        CRectangle rect = o as CRectangle;

                        if (ROIs.Contains(rect))
                        {
                            ROIs.Remove(rect);
                        }
                        if (InspectRectRegions.Contains(rect))
                        {
                            InspectRectRegions.Remove(rect);
                        }
                        if (IgnoreRectRegions.Contains(rect))
                        {
                            IgnoreRectRegions.Remove(rect);
                        }
                    }
                });
            }
        }
        #endregion

        #region Privates
        private ObservableCollection<CRectangle> _InspectRectRegions = new ObservableCollection<CRectangle>();
        private ObservableCollection<CCircle> _InspectCircleRegions = new ObservableCollection<CCircle>();
        #endregion
    }

    /// <summary>
    /// Vision Result for <see cref="SetRegions"/> process
    /// </summary>
    public class SetRegionsResult : VisionResultBase
    {
    }

    public class SetRegions : VisionProcessBase
    {
        #region Privates
        private SetRegionsParameter ThisParameter
        {
            get { return (SetRegionsParameter)Parameter; }
        }

        private SetRegionsResult ThisResult
        {
            get { return (SetRegionsResult)Result; }
        }
        #endregion

        #region Constructors
        public SetRegions()
            : this(new SetRegionsParameter())
        {
        }

        public SetRegions(SetRegionsParameter parameter)
        {
            Parameter = parameter;
            Result = new SetRegionsResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new SetRegionsResult();

            OutputMat = InputMat.Clone();

            Mat inspectMask = Mat.Ones(OutputMat.Size(), MatType.CV_8UC1);
            foreach (CRectangle rect in ThisParameter.InspectRectRegions)
            {
                if (rect.OCvSRect.Width * rect.OCvSRect.Height > 0)
                {
                    inspectMask.SubMat(rect.OCvSRect).SetTo(0);
                }
            }
            foreach (CCircle circle in ThisParameter.InspectCircleRegions)
            {
                if (circle.OCvSCircle.Radius > 0)
                {
                    Cv2.Circle(inspectMask, (Point)circle.OCvSCircle.Center, (int)circle.Radius, 0, thickness: -1);
                }
            }
            OutputMat.SetTo(0, inspectMask);

            Mat ignoreMask = Mat.Zeros(OutputMat.Size(), MatType.CV_8UC1);
            foreach (CRectangle rect in ThisParameter.IgnoreRectRegions)
            {
                if (rect.OCvSRect.Width * rect.OCvSRect.Height > 0)
                {
                    ignoreMask.SubMat(rect.OCvSRect).SetTo(255);
                }
            }
            foreach (CCircle circle in ThisParameter.IgnoreCircleRegions)
            {
                if (circle.OCvSCircle.Radius > 0)
                {
                    Cv2.Circle(ignoreMask, (Point)circle.OCvSCircle.Center, (int)circle.Radius, 255, thickness: -1);
                }
            }
            OutputMat.SetTo(0, ignoreMask);

            return EVisionRtnCode.OK;
        }
    }
}
