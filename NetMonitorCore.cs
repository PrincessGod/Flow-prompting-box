using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NetMonitor
{
    public static class NetMonitorCore
    {
        private static PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
        private static int interfaceLength = performanceCounterCategory.GetInstanceNames().Count();
        private static int _testNum;
        private static string instance = performanceCounterCategory.GetInstanceNames()[1];
        private static PerformanceCounter performanceCounterRecv = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);
        private static PerformanceCounter performanceCounterSend = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
        private static int TestNum
        {
            get
            {
                return _testNum;
            }
            set
            {
                _testNum = value;
                string instance = performanceCounterCategory.GetInstanceNames()[_testNum];
                performanceCounterRecv = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);
                performanceCounterSend = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
            }
        }

        /// <summary>
        /// Change the connect of PerformanceCounter
        /// </summary>
        public static void TryNextConnect()
        {
            if (TestNum + 1 < interfaceLength)
            {
                TestNum = TestNum + 1;
                return;
            }

            TestNum = 0;
        }

        /// <summary>
        /// Get the ReciveSpeed (b/s)
        /// </summary>
        /// <returns></returns>
        public static float GetNetRecv()
        {
            return performanceCounterRecv.NextValue();
        }

        /// <summary>
        /// Get the SendSpeed (b/s)
        /// </summary>
        /// <returns></returns>
        public static float GetNetSend()
        {
            return performanceCounterSend.NextValue();
        }
    }
}
