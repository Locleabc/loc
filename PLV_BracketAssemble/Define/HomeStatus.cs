using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLV_BracketAssemble.Define
{
    public static class HomeStatus
    {
        public static bool IsAllAxisHomeDone { get; set; }

        public static void Clear()
        {
            IsAllAxisHomeDone = false;
        }
    }
}
