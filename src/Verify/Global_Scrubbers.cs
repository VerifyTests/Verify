using System;
using System.Collections.Generic;

namespace Verify
{
    public static partial class Global
    {
        internal static List<Func<string, string>> GlobalScrubbers = new List<Func<string, string>>();

        public static void AddScrubber(Func<string, string> scrubber)
        {
            Guard.AgainstNull(scrubber, nameof(scrubber));

            GlobalScrubbers.Insert(0, scrubber);
        }

        public static void ScrubMachineName()
        {
            AddScrubber(Scrubbers.ScrubMachineName);
        }
    }
}