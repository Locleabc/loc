using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using TopVision.Models;

namespace TopVision.Helpers
{
    internal static class ConvertHelpers
    {
        public static string ToBase64String(this Mat img, string ext = ".jpg")
        {
            byte[] buffer;
            bool result = Cv2.ImEncode(".jpg", img, out buffer);
            string bufferString = Convert.ToBase64String(buffer);
            if (result)
            {
                return bufferString;
            }
            else
            {
                return null;
            }
        }

        public static Mat FromBase64String(string bufferString)
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(bufferString);
                return Cv2.ImDecode(buffer, ImreadModes.Grayscale);
            }
            catch
            {
                return null;
            }
        }

        public static string ToBase64String(this StrokeCollection strokes)
        {
            var ms = new MemoryStream();
            strokes.Save(ms, false);

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Convert offset from pixel to millimeter
        /// </summary>
        /// <param name="offsetInPixel">offset in pixel</param>
        /// <param name="pixelSize">Unit [mm]</param>
        /// <returns></returns>
        public static XYTOffset FromPixelToMillimeter(this XYTOffset offsetInPixel, double pixelSize)
        {
            return new XYTOffset
            {
                Theta = offsetInPixel.Theta,
                X = offsetInPixel.X * pixelSize,
                Y = offsetInPixel.Y * pixelSize
            };
        }

        /// <summary>
        /// Convert offset from millimeter to pixel
        /// </summary>
        /// <param name="offsetInPixel">offset in pixel</param>
        /// <param name="pixelSize">Unit [mm]</param>
        /// <returns></returns>
        public static XYTOffset FromMillimeterToPixel(this XYTOffset offsetInMillimeter, double pixelSize)
        {
            return new XYTOffset
            {
                Theta = offsetInMillimeter.Theta,
                X = offsetInMillimeter.X / pixelSize,
                Y = offsetInMillimeter.Y / pixelSize
            };
        }

        public static StrokeCollection Base64StringToStrokeCollection(string bufferString)
        {
            if (bufferString == null) return new StrokeCollection();

            byte[] baStrokes = Convert.FromBase64String(bufferString);
            return new StrokeCollection(new MemoryStream(baStrokes));
        }
    }
}
