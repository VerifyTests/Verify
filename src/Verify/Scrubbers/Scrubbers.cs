using System;

namespace Verify
{
    public static class Scrubbers
    {
        public static string ScrubMachineName(string s)
        {
            return s.Replace(Environment.MachineName, "TheMachineName");
        }
    }
}