//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using OpenCvSharp;

//namespace TopVision.Models
//{
//    /// <summary>
//    /// https://stackoverflow.com/questions/51606215/how-to-draw-bounding-box-on-best-matches/51607041#51607041
//    /// </summary>
//    class DrawBestMatchRectangle : CInspectBase
//    {
//        private Point RoiSize = new Point(1400, 1400);
//        Rect ROI { get; set; }
//        string PatternImagePath = @"D:\TOP\TOPVEQ\Recipe\Default\Pattern.jpg";

//        public override void Run()
//        {
//            ROI = new Rect((2592 - RoiSize.X) / 2, (1944 - RoiSize.Y) / 2, RoiSize.X, RoiSize.Y);

//            using var imgFeature = new Mat(PatternImagePath, ImreadModes.Grayscale);

//            using var imgROI = InputImage.SubMat(ROI);

//            using var orb = ORB.Create(1000);
//            using var descriptors1 = new Mat();
//            using var descriptors2 = new Mat();

//            KeyPoint[] keyPoints1 = null;
//            KeyPoint[] keyPoints2 = null;

//            var Watch = Stopwatch.StartNew();

//            var bf = new BFMatcher(NormTypes.Hamming, crossCheck: true);

//            var task1 = new Task(() => {
//                orb.DetectAndCompute(imgFeature, null, out keyPoints1, descriptors1);
//            });
//            var task2 = new Task(() => {
//                orb.DetectAndCompute(imgROI, null, out keyPoints2, descriptors2);
//            });

//            task1.Start();
//            task2.Start();

//            Task.WaitAll(task1, task2);

//            var matches = bf.Match(descriptors1, descriptors2);

//            Watch.Stop();
//            Console.WriteLine("Cost = {0} ms", Watch.ElapsedMilliseconds);

//            var goodMatches = matches
//                .OrderBy(x => x.Distance)
//                .Take(10)
//                .ToArray();

//            var srcPts = goodMatches.Select(m => keyPoints1[m.QueryIdx].Pt).Select(p => new Point2d(p.X, p.Y));
//            var dstPts = goodMatches.Select(m => keyPoints2[m.TrainIdx].Pt).Select(p => new Point2d(p.X, p.Y));

//            using var homography = Cv2.FindHomography(srcPts, dstPts, HomographyMethods.Ransac, 5, null);

//            int h = imgFeature.Height, w = imgFeature.Width;
//            var img2Bounds = new[]
//            {
//                new Point2d(0, 0),
//                new Point2d(0, h-1),
//                new Point2d(w-1, h-1),
//                new Point2d(w-1, 0),
//            };
//            var img2BoundsTransformed = Cv2.PerspectiveTransform(img2Bounds, homography);

//            using var view = InputImage.Clone();
//            var drawingPoints = img2BoundsTransformed.Select(p => (Point)p).ToArray();

//            for (int i = 0; i < drawingPoints.Length; i++)
//            {
//                drawingPoints[i].X += ROI.X;
//                drawingPoints[i].Y += ROI.Y;
//            }

//            Cv2.Polylines(view, new[] { drawingPoints }, true, Scalar.Red, 3);

//            InputImage = view;

//            //using (new Window("view", WindowMode.KeepRatio, view))
//            //{
//            //    Cv2.WaitKey();
//            //}
//        }
//    }
//}