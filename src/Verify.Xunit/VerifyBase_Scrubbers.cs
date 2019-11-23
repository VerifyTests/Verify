using System;
using System.Collections.Generic;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        List<Func<string, string>> instanceScrubbers = new List<Func<string, string>>();
        public void AddScrubber(Func<string, string> scrubber)
        {
            Guard.AgainstNull(scrubber, nameof(scrubber));

            instanceScrubbers.Insert(0, scrubber);
        }

        string? ApplyScrubbers(string? target)
        {
            if (target == null)
            {
                return null;
            }

            foreach (var scrubber in instanceScrubbers)
            {
                target = scrubber(target);
            }

            foreach (var scrubber in Global.GlobalScrubbers)
            {
                target = scrubber(target);
            }

            return target;
        }
    }
}