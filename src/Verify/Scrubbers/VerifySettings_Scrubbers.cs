using System;
using System.Collections.Generic;

namespace Verify
{
    public partial class VerifySettings
    {
        internal List<Func<string, string>> instanceScrubbers = new List<Func<string, string>>();

        public void ScrubMachineName()
        {
            AddScrubber(Scrubbers.ScrubMachineName);
        }

        public void AddScrubber(Func<string, string> scrubber)
        {
            Guard.AgainstNull(scrubber, nameof(scrubber));

            instanceScrubbers.Insert(0, scrubber);
        }

        public void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
        {
            instanceScrubbers.Insert(0, s => s.RemoveLinesContaining(comparison, stringToMatch));
        }
        
        public void ScrubLines(Func<string, bool> removeLine)
        {
            instanceScrubbers.Insert(0, s => s.FilterLines(removeLine));
        }
        
        public void ScrubLinesWithReplace(Func<string, string> replaceLine)
        {
            instanceScrubbers.Insert(0, s => s.ReplaceLines(replaceLine));
        }

        public void ScrubLinesContaining(params string[] stringToMatch)
        {
            ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);
        }
    }
}