using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCM_CoilLoading.Define
{
    public static class HomeStatus
    {
        public static bool IsAllAxisHomeDone { get; set; }

        public static bool Z1Done { get; set; }
        public static bool Z2Done { get; set; }
        public static bool Z3Done { get; set; }

        public static void Clear()
        {
            IsAllAxisHomeDone = false;

            Z1Done = false;
            Z2Done = false;
            Z3Done = false;
        }

        public static bool IsAllZAxisHomeDone()
        {
            return Z1Done & Z2Done & Z3Done;
        }
    }
}
