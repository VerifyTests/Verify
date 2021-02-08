using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static List<Action<StringBuilder>> GlobalScrubbers = new();

        /// <summary>
        /// Modify the resulting test content using custom code.
        /// </summary>
        public static void AddScrubber(Action<StringBuilder> scrubber)
        {
            Guard.AgainstNull(scrubber, nameof(scrubber));

            GlobalScrubbers.Insert(0, scrubber);
        }

        /// <summary>
        /// Remove any lines containing any of <paramref name="stringToMatch"/> from the test results.
        /// </summary>
        public static void ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
        {
            GlobalScrubbers.Insert(0, s => s.RemoveLinesContaining(comparison, stringToMatch));
        }

        /// <summary>
        /// Remove any lines containing matching <paramref name="removeLine"/> from the test results.
        /// </summary>
        public static void ScrubLines(Func<string, bool> removeLine)
        {
            GlobalScrubbers.Insert(0, s => s.FilterLines(removeLine));
        }

        /// <summary>
        /// Remove any lines containing only whitespace from the test results.
        /// </summary>
        public static void ScrubEmptyLines()
        {
            GlobalScrubbers.Insert(0, s => s.FilterLines(string.IsNullOrWhiteSpace));
        }

        /// <summary>
        /// Replace inline <see cref="Guid"/>s with a placeholder.
        /// Uses a <see cref="Regex"/> to find <see cref="Guid"/>s inside strings.
        /// </summary>
        public static void ScrubInlineGuids()
        {
            GlobalScrubbers.Insert(0, GuidScrubber.ReplaceGuids);
        }

        /// <summary>
        /// Scrub lines with an optional replace.
        /// <paramref name="replaceLine"/> can return the input to ignore the line, or return a a different string to replace it.
        /// </summary>
        public static void ScrubLinesWithReplace(Func<string, string?> replaceLine)
        {
            GlobalScrubbers.Insert(0, s => s.ReplaceLines(replaceLine));
        }

        /// <summary>
        /// Remove any lines containing any of <paramref name="stringToMatch"/> from the test results.
        /// </summary>
        public static void ScrubLinesContaining(params string[] stringToMatch)
        {
            ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, stringToMatch);
        }

        /// <summary>
        /// Remove the <see cref="Environment.MachineName"/> from the test results.
        /// </summary>
        public static void ScrubMachineName()
        {
            AddScrubber(Scrubbers.ScrubMachineName);
        }
    }
}