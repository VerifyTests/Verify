using System;
using System.Text;

namespace VerifyTesting
{
    public static class Scrubbers
    {
        public static void ScrubMachineName(StringBuilder builder)
        {
            builder.Replace(Environment.MachineName, "TheMachineName");
        }
    }
}