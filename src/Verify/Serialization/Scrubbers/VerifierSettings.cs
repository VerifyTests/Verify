using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static List<Action<StringBuilder>> GlobalScrubbers = new List<Action<StringBuilder>>();

        public static void AddScrubber(Action<StringBuilder> scrubber)
        {
            Guard.AgainstNull(scrubber, nameof(scrubber));

            GlobalScrubbers.Insert(0, scrubber);
        }

        internal static bool newLineEscapingDisabled;

        public static void DisableNewLineEscaping()
        {
            newLineEscapingDisabled = true;
        }

        public static void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
        {
            GlobalScrubbers.Insert(0, s => s.RemoveLinesContaining(comparison, stringToMatch));
        }

        public static void ScrubLines(Func<string, bool> removeLine)
        {
            GlobalScrubbers.Insert(0, s => s.FilterLines(removeLine));
        }

        public static void ScrubLinesWithReplace(Func<string, string> replaceLine)
        {
            GlobalScrubbers.Insert(0, s => s.ReplaceLines(replaceLine));
        }

        public static void ScrubLinesContaining(params string[] stringToMatch)
        {
            ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);
        }

        public static void ScrubMachineName()
        {
            AddScrubber(Scrubbers.ScrubMachineName);
        }
    }
}