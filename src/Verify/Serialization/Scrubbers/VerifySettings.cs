using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyTests
{
    public partial class VerifySettings
    {
        internal List<Action<StringBuilder>> instanceScrubbers = new();

        /// <summary>
        /// Remove the <see cref="Environment.MachineName"/> from the test results.
        /// </summary>
        public void ScrubMachineName()
        {
            AddScrubber(Scrubbers.ScrubMachineName);
        }

        /// <summary>
        /// Modify the resulting test using custom code.
        /// </summary>
        public void AddScrubber(Action<StringBuilder> scrubber)
        {
            Guard.AgainstNull(scrubber, nameof(scrubber));
            instanceScrubbers.Insert(0, scrubber);
        }

        /// <summary>
        /// Remove any lines containing any of <paramref name="stringToMatch"/> from the test results.
        /// </summary>
        public void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
        {
            instanceScrubbers.Insert(0, s => s.RemoveLinesContaining(comparison, stringToMatch));
        }

        //TODO: should only do this when it is a string.
        //and instead pass a bool to the json serializer for the object scenario
        //Same for the static
        /// <summary>
        /// Replace inline <see cref="Guid"/>s with a placeholder.
        /// </summary>
        public void ScrubInlineGuids()
        {
            instanceScrubbers.Insert(0, GuidScrubber.ReplaceGuids);
        }

        /// <summary>
        /// Remove any lines containing matching <paramref name="removeLine"/> from the test results.
        /// </summary>
        public void ScrubLines(Func<string, bool> removeLine)
        {
            instanceScrubbers.Insert(0, s => s.FilterLines(removeLine));
        }

        public void ScrubLinesWithReplace(Func<string, string> replaceLine)
        {
            instanceScrubbers.Insert(0, s => s.ReplaceLines(replaceLine));
        }

        /// <summary>
        /// Remove any lines containing any of <paramref name="stringToMatch"/> from the test results.
        /// </summary>
        public void ScrubLinesContaining(params string[] stringToMatch)
        {
            ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);
        }
    }
}