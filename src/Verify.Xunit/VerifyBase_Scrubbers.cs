using System;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public void ScrubMachineName()
        {
            verifier.ScrubMachineName();
        }

        public void AddScrubber(Func<string, string> scrubber)
        {
            verifier.AddScrubber(scrubber);
        }
    }
}