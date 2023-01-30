using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Helpers
{
    public static class Validators
    {
        public static bool IsNullOrEmpty(this Mat mat)
        {
            if (mat == null) return true;
            if (mat.IsDisposed) return true;
            if (mat.Empty()) return true;

            return false;
        }
    }
}
