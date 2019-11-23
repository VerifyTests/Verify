using System;
using System.Collections.Generic;

namespace VerifyXunit
{
    public static class Global{

        internal static List<Func<string, string>> GlobalScrubbers = new List<Func<string, string>>();

        public static void AddScrubber(Func<string, string> scrubber)
        {
            Guard.AgainstNull(scrubber, nameof(scrubber));

            GlobalScrubbers.Insert(0, scrubber);
        }
    }
}