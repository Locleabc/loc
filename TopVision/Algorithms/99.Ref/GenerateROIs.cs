using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopVision.Models;

namespace TopVision.Algorithms
{
    public enum ROIGenerateMode
    {
        /// <summary>
        /// Using NumberOfROIToGenerate to generate
        /// </summary>
        RotateByCenterOfImage,
        /// <summary>
        /// x2 ROIs
        /// </summary>
        FlipX,
        /// <summary>
        /// x2 ROIs
        /// </summary>
        FlipY,
        /// <summary>
        /// x4 ROIs
        /// </summary>
        FlipXY
    }

    public class GenerateROIsParameter : VisionParameterBase
    {
        /// <summary>
        /// ROI Generation Mode
        /// </summary>
        public ROIGenerateMode Mode { get; set; }
        /// <summary>
        /// Count of ROIs before execute the algorithm
        /// </summary>
        public int NumberOfInputROI { get; internal set; }
        /// <summary>
        /// Some mode will use this parameter for generating ROIs </b>
        /// Some mode, like Flip will auto generating matching number of parameter
        /// </summary>
        public int TotalOfROIAfterGenerate { get; set; }
    }

    public class GenerateROIsResult : VisionResultBase
    {
    }

    /// <summary>
    /// Generate ROIs base on first user input ROI
    /// </summary>
    public class GenerateROIs : VisionProcessBase
    {
        private GenerateROIsParameter ThisParameter;

        public GenerateROIs(GenerateROIsParameter parameter)
        {
            Parameter = parameter;
            Result = new GenerateROIsResult();

            ThisParameter = Parameter as GenerateROIsParameter;
        }

        public override EVisionRtnCode DIPFunction(bool isTeachingMode = false)
        {
            Result = new GenerateROIsResult();
            EVisionRtnCode rtnCode = EVisionRtnCode.OK;

            ThisParameter.NumberOfInputROI = ThisParameter.ROIs.Count();

            Point imageCenter = new Point(PreProcessedMat.Width / 2, PreProcessedMat.Height / 2);
            switch (ThisParameter.Mode)
            {
                case ROIGenerateMode.RotateByCenterOfImage:
                    for (int i = 1; i < ThisParameter.TotalOfROIAfterGenerate; i++)
                    {
                        Point newPoint = RotatePoint(new Point(ThisParameter.ROIs[0].X + ThisParameter.ROIs[0].Width / 2,
                                                                ThisParameter.ROIs[0].Y + ThisParameter.ROIs[0].Height / 2),
                                                     imageCenter,
                                                     (360 / ThisParameter.TotalOfROIAfterGenerate) * i);

                        newPoint.X -= ThisParameter.ROIs[0].Width / 2;
                        newPoint.Y -= ThisParameter.ROIs[0].Height / 2;

                        CRectangle newROI = new CRectangle(newPoint, ThisParameter.ROIs[0].OCvSRect.Size);
                        ThisParameter.ROIs.Add(newROI);
                    }
                    break;
                case ROIGenerateMode.FlipX:
                    for (int i = 0; i < ThisParameter.NumberOfInputROI; i++)
                    {
                        ThisParameter.ROIs.Add(
                            new CRectangle(
                                new Point(
                                    2 * imageCenter.X - (ThisParameter.ROIs[i].OCvSRect.Left - ThisParameter.ROIs[i].Width / 2)
                                    , ThisParameter.ROIs[i].OCvSRect.Top)
                                , ThisParameter.ROIs[i].OCvSRect.Size)
                        );
                    }
                    break;
                case ROIGenerateMode.FlipY:
                    for (int i = 0; i < ThisParameter.NumberOfInputROI; i++)
                    {
                        ThisParameter.ROIs.Add(
                            new CRectangle(
                                new Point(
                                    ThisParameter.ROIs[i].OCvSRect.Left
                                    , 2 * imageCenter.Y - ThisParameter.ROIs[i].OCvSRect.Top)
                                , ThisParameter.ROIs[i].OCvSRect.Size)
                        );
                    }
                    break;
                case ROIGenerateMode.FlipXY:
                    for (int i = 0; i < ThisParameter.NumberOfInputROI; i++)
                    {
                        ThisParameter.ROIs.Add(
                            new CRectangle(
                                new Point(
                                    2 * imageCenter.X - ThisParameter.ROIs[i].OCvSRect.Left
                                    , ThisParameter.ROIs[i].OCvSRect.Top)
                                , ThisParameter.ROIs[i].OCvSRect.Size)
                        );
                        ThisParameter.ROIs.Add(
                            new CRectangle(
                                new Point(
                                    ThisParameter.ROIs[i].OCvSRect.Left
                                    , 2 * imageCenter.Y - ThisParameter.ROIs[i].OCvSRect.Top)
                                , ThisParameter.ROIs[i].OCvSRect.Size)
                        );
                    }
                    break;
            }

            OutputMat = PreProcessedMat;
            return rtnCode;
        }

        private Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X = (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y = (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
}
