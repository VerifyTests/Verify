using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyTests
{
    public partial class VerifySettings
    {
        internal List<Action<StringBuilder>> instanceScrubbers = new();

        public void ScrubMachineName()
        {
            AddScrubber(Scrubbers.ScrubMachineName);
        }

        [Obsolete("Newline escaping is disabled by default", true)]
        public void DisableNewLineEscaping()
        {
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

        //TODO: should only do this when it is a string.
        //and instead pass a bool to the json serializer for the object scenario
        //Same for the static
        public void ScrubInlineGuids()
        {
            instanceScrubbers.Insert(0, GuidScrubber.ReplaceGuids);
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