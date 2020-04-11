using System;
using Verify;
using VerifyXunit;
using Xunit.Abstractions;

public class ScrubberTests :
    VerifyBase
{
    void List()
    {
        var verifySettings = new VerifySettings();

        #region ScrubLines

        verifySettings.ScrubLines(line => line.Contains("text"));

        #endregion

        #region ScrubLinesContaining

        verifySettings.ScrubLinesContaining("text1", "text2");

        #endregion

        #region ScrubLinesContainingOrdinal

        verifySettings.ScrubLinesContaining(StringComparison.Ordinal, "text1", "text2");

        #endregion

        #region ScrubLinesWithReplace

        verifySettings.ScrubLinesWithReplace(line => line.ToUpper());

        #endregion

        #region ScrubMachineName

        verifySettings.ScrubMachineName();

        #endregion

        #region AddScrubber

        verifySettings.AddScrubber(fullText => fullText.Substring(0, 100));

        #endregion
    }
    public ScrubberTests(ITestOutputHelper output) :
        base(output)
    {
    }
}