using System.Collections.Generic;

namespace VCM_FullAssy.Define
{
    public static class TaktTimeHelper
    {
        public static void AddTT(this Queue<double> TaktTimeQueue, double lastValue, ref double totalTakt)
        {
            if (TaktTimeQueue.Count >= 30)
            {
                TaktTimeQueue.Dequeue();
            }

            TaktTimeQueue.Enqueue(lastValue);

            totalTakt = 0;
            foreach (double takt in TaktTimeQueue.ToArray())
            {
                totalTakt += takt;
            }

            totalTakt = totalTakt * 30 / TaktTimeQueue.Count;
        }
    }
}
