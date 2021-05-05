using System;
using System.Collections.Generic;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static List<(Action start, Action stop)> testCallbacks = new();

        internal static void RunStartCallbacks()
        {
            foreach (var (start, _) in testCallbacks)
            {
                start();
            }
        }

        public static void AddTestCallback(Action start, Action stop)
        {
            testCallbacks.Add((start, stop));
        }

        internal static void RunStopCallbacks()
        {
            foreach (var (_, stop) in testCallbacks)
            {
                stop();
            }
        }
    }
}