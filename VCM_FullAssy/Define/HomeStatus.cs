using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCM_FullAssy.Define
{
    public static class HomeStatus
    {
        public static bool IsAllAxisHomeDone { get; set; }

        public static bool ZDone { get; set; }

        public static void Clear()
        {
            IsAllAxisHomeDone = false;

            ZDone = false;
        }

        public static bool IsAllZAxisHomeDone()
        {
            return ZDone;
        }
    }
}
