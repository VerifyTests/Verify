using System;
using System.Collections.Generic;
using System.Text;

namespace Verify
{
    public partial class VerifySettings
    {
        internal List<Action<StringBuilder>> instanceScrubbers = new List<Action<StringBuilder>>();

        public void ScrubMachineName()
        {
            AddScrubber(Scrubbers.ScrubMachineName);
        }

        internal bool newLineEscapingDisabled;

        public void DisableNewLineEscaping()
        {
            newLineEscapingDisabled = true;
        }

        public void AddScrubber(Action<StringBuilder> scrubber)
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