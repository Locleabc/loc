using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopVision.Helpers
{
    internal static class TaskHelpers
    {
        public static bool ExecuteDone(this Task<EVisionRtnCode> task)
        {
            if (task == null) return true;

            return (task.IsCompleted);
        }

        public static bool ExecuteDone(this Task task)
        {
            if (task == null) return true;

            return (task.IsCompleted);
        }

        public static bool ExecuteDone(this List<Task> tasks)
        {
            if (tasks == null) return true;
            if (tasks.Count <= 0) return true;

            foreach (var task in tasks)
            {
                if (task.IsCompleted == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
