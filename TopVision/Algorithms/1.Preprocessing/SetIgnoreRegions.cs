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
    /// Vision Parameter for <see cref="SetIgnoreRegions"/> process
    /// </summary>
    public class SetIgnoreRegionsParameter : VisionParameterBase
    {
        #region Properties
        public ObservableCollection<CRectangle> IgnoreRectRegions
        {
            get { return _IgnoreRectRegions; }
            set
            {
                _IgnoreRectRegions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CCircle> IgnoreCircleRegions
        {
            get { return _IgnoreCircleRegions; }
            set
            {
                _IgnoreCircleRegions = value;
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
        private ObservableCollection<CRectangle> _IgnoreRectRegions = new ObservableCollection<CRectangle>();
        private ObservableCollection<CCircle> _IgnoreCircleRegions = new ObservableCollection<CCircle>();
        #endregion
    }

    /// <summary>
    /// Vision Result for <see cref="SetIgnoreRegions"/> process
    /// </summary>
    public class SetIgnoreRegionsResult : VisionResultBase
    {
    }

    public class SetIgnoreRegions : VisionProcessBase
    {
        #region Privates
        private SetIgnoreRegionsParameter ThisParameter
        {
            get { return (SetIgnoreRegionsParameter)Parameter; }
        }

        private SetIgnoreRegionsResult ThisResult
        {
            get { return (SetIgnoreRegionsResult)Result; }
        }
        #endregion

        #region Constructor
        public SetIgnoreRegions()
            : this(new SetIgnoreRegionsParameter())
        {
        }

        public SetIgnoreRegions(SetIgnoreRegionsParameter parameter)
        {
            Parameter = parameter;
            Result = new SetIgnoreRegionsResult();
        }
        #endregion 

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new SetIgnoreRegionsResult();

            Mat mask = Mat.Zeros(InputMat.Size(), MatType.CV_8UC1);

            foreach (CRectangle rect in ThisParameter.IgnoreRectRegions)
            {
                mask.SubMat(rect.OCvSRect).SetTo(255);
            }

            foreach (CCircle circle in ThisParameter.IgnoreCircleRegions)
            {
                Cv2.Circle(mask, (Point)circle.OCvSCircle.Center, (int)circle.Radius, 255, thickness: -1);
            }

            OutputMat = InputMat.Clone();
            OutputMat.SetTo(0, mask);

            return EVisionRtnCode.OK;
        }
    }
}
