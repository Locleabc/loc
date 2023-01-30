using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Grabbers
{
    public class CGrabResult : IDisposable
    {
        public EGrabRtnCode RtnCode { get; set; }
        /// <summary>
        /// Cost in miliseconds
        /// </summary>
        public long Cost { get; set; }

        public Mat GrabImage { get; set; }

        public CGrabResult()
        {
            GrabImage = new Mat();
        }

        public void Dispose()
        {
            GrabImage.Dispose();
        }
    }
}
