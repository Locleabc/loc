using TopVision.Models;
using OpenCvSharp;
using System.Collections.Generic;
using System.Linq;

namespace TopVision.Algorithms
{
    /// <summary>
    /// Vision parameter for <see cref="NoiseFilter"/> processing
    /// </summary>
    public class NoiseFilterParameter : VisionParameterBase
    {
        #region Properties
        public int RemainContourCount
        {
            get { return _RemainContourCount; }
            set
            {
                if (_RemainContourCount == value) return;
                
                _RemainContourCount = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private int _RemainContourCount = 20;
        #endregion
    }

    /// <summary>
    /// Vision result for <see cref="NoiseFilter"/> processing
    /// </summary>
    public class NoiseFilterResult : VisionResultBase
    {
    }

    public class NoiseFilter : VisionProcessBase
    {
        #region Privates
        private NoiseFilterParameter ThisParameter
        {
            get { return (NoiseFilterParameter)Parameter; }
        }

        private NoiseFilterResult ThisResult
        {
            get { return (NoiseFilterResult)Result; }
        }
        #endregion

        #region Constructors
        public NoiseFilter()
            : this(new NoiseFilterParameter())
        {
        }

        public NoiseFilter(NoiseFilterParameter parameter)
        {
            Parameter = parameter;
            Result = new NoiseFilterResult();
        }
        #endregion

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new NoiseFilterResult();

            Point[][] contours = new Point[][] { };
            HierarchyIndex[] tmpHierachyIndex = new HierarchyIndex[] { };
            Cv2.FindContours(InputMat, out contours, out tmpHierachyIndex, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            Mat mask = Mat.Ones(InputMat.Size(), MatType.CV_8UC1);

            List<Point[]> detectedCountours = new List<Point[]>();
            OutputMat = new Mat(InputMat.Height, InputMat.Width, InputMat.Type());

            //Cv2.DrawContours(OutputMat, contours.Where(c => c.Length > 300), -1, 255, -1);

            int count = 0;

            List<Point[]> sortedContours = contours.OrderBy(x => x.Length).Reverse().Take(ThisParameter.RemainContourCount).ToList();

            for (int i = 0; i < sortedContours.Count; i++)
            {
                Cv2.DrawContours(OutputMat, new List<Point[]> { sortedContours[i] }, -1, 255, -1);
                count++;
            }

            Log.Debug($"Detected Contour count = {count}");

            //OutputMat = InputMat;

            return EVisionRtnCode.OK;
        }
    }
}
