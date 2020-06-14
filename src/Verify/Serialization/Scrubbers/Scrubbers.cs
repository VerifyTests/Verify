using System;
using System.Text;

namespace VerifyTests
{
    public static class Scrubbers
    {
        public static void ScrubMachineName(StringBuilder builder)
        {
            builder.Replace(Environment.MachineName, "TheMachineName");
        }
    }
}