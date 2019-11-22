using System;
using Xunit;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        Func<string?, string>? instanceScrubber;
        static Func<string?, string>? globalScrubber;

        public static void SetGlobalScrubber(Func<string?, string>? scrubber)
        {
            if (globalScrubber != null)
            {
                throw new Exception("Global scrubber can only be set once.");
            }

            globalScrubber = scrubber;
        }

        string? ApplyScrubbers(Func<string?, string>? scrubber, string? target)
        {
            if (target == null)
            {
                return null;
            }

            if (scrubber != null)
            {
                target = scrubber(target);
            }

            if (instanceScrubber != null)
            {
                target = instanceScrubber(target);
            }

            if (globalScrubber != null)
            {
                target = globalScrubber(target);
            }

            return target;
        }
    }
}