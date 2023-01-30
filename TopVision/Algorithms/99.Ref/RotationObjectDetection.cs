using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Algorithms
{
    public class RotationObjectDetection
    {
        public void Run()
        {
        }

        Vector<Point> ImageComparation(Mat obj, Mat scene, TemplateMatchModes match_method, float peek_percent)
        {
            int result_cols = scene.Cols - obj.Cols + 1;
            int result_rows = scene.Rows - obj.Rows + 1;

            Mat result = new Mat(result_cols, result_rows, MatType.CV_32FC1);

            // match scene with template
            Cv2.MatchTemplate(scene, obj, result, TemplateMatchModes.SqDiffNormed);
            Cv2.ImShow("matched_template", result);

            //normalize(result, result, 0, 1, NORM_MINMAX, -1, Mat());
            Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax, -1, new Mat());
            Cv2.ImShow("normalized", result);

            // Localizing the best match with minMaxLoc
            double minVal; double maxVal;
            Point minLoc = new Point();
            Point maxLoc = new Point();
            Point matchLoc = new Point();

            // For SQDIFF and SQDIFF_NORMED, the best matches are lower values. For all the other methods, the higher the better
            if (match_method == TemplateMatchModes.SqDiff || match_method == TemplateMatchModes.SqDiffNormed)
            {
                matchLoc = minLoc;
                //threshold(result, result, 0.1, 1, CV_THRESH_BINARY_INV);
                Cv2.Threshold(result, result, 0.1, 1, ThresholdTypes.BinaryInv);
                Cv2.ImShow("threshold_1", result);
            }
            else
            {
                matchLoc = maxLoc;
                Cv2.Threshold(result, result, 0.9, 1, ThresholdTypes.Tozero);
                Cv2.ImShow("threshold_2", result);
            }

            Vector<Point> res = new Vector<Point>();
            maxVal = 1.0;
            while (maxVal > peek_percent)
            {
                Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc, new Mat());
                if (maxVal > peek_percent)
                {
                    Cv2.Rectangle(
                        result
                        , new Point(maxLoc.X - obj.Cols / 2, maxLoc.Y - obj.Rows / 2)
                        , new Point(maxLoc.X + obj.Cols / 2, maxLoc.Y + obj.Rows / 2)
                        , new Scalar(0), -1);

                    //res.push_back(maxLoc);
                }
            }

            return res;
        }

        public void Display()
        {

        }
    }
}
