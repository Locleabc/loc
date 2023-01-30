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
    /// Vision Parameter for <see cref="SetInspectRegions"/> process
    /// </summary>
    public class SetInspectRegionsParameter : VisionParameterBase
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
        public RelayCommand CCircleAddCommand
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

        public RelayCommand CCircleDeleteCommand
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
    /// Vision Result for <see cref="SetInspectRegions"/> process
    /// </summary>
    public class SetInspectRegionsResult : VisionResultBase
    {
    }

    public class SetInspectRegions : VisionProcessBase
    {
        #region Privates
        private SetInspectRegionsParameter ThisParameter
        {
            get { return (SetInspectRegionsParameter)Parameter; }
        }

        private SetInspectRegionsResult ThisResult
        {
            get { return (SetInspectRegionsResult)Result; }
        }
        #endregion

        #region Constructors
        public SetInspectRegions()
            : this(new SetInspectRegionsParameter())
        {
        }

        public SetInspectRegions(SetInspectRegionsParameter parameter)
        {
            Parameter = parameter;
            Result = new SetInspectRegionsResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new SetInspectRegionsResult();

            Mat mask = Mat.Ones(InputMat.Size(), MatType.CV_8UC1);

            foreach (CRectangle rect in ThisParameter.InspectRectRegions)
            {
                mask.SubMat(rect.OCvSRect).SetTo(0);
            }

            foreach (CCircle circle in ThisParameter.InspectCircleRegions)
            {
                Cv2.Circle(mask, (Point)circle.OCvSCircle.Center, (int)circle.Radius, 0, thickness: -1);
            }

            OutputMat = InputMat.Clone();
            OutputMat.SetTo(0, mask);

            return EVisionRtnCode.OK;
        }
    }
}
