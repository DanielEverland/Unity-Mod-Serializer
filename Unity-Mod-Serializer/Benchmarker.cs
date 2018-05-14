using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace UMS
{
    public static class Benchmarker
    {
        public static double Profile(string name, int iterations, Action func)
        {
            //Run at highest priority to minimize fluctuations caused by other processes/threads
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            // warm up 
            func();

            var watch = new Stopwatch();

            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            watch.Start();
            for (int i = 0; i < iterations; i++)
            {
                func();
            }
            watch.Stop();

            double total = watch.Elapsed.TotalMilliseconds;

            UnityEngine.Debug.Log($"{name} ran {iterations.ToString("#,##")} times, elapsed total: {total.ToString("F0")}ms, average: {total / iterations}ms");

            return total;
        }
    }
}
