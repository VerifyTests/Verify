using Verify;
using VerifyXunit;
using Xunit.Abstractions;

public class SerializationSettingsSnippets :
    VerifyBase
{
    void DontIgnoreEmptyCollections()
    {
        #region DontIgnoreEmptyCollections
        SharedVerifySettings.ModifySerialization(_ => _.DontIgnoreEmptyCollections());
        #endregion
    }

    void DontScrubGuids()
    {
        #region DontScrubGuids
        SharedVerifySettings.ModifySerialization(_ => _.DontScrubGuids());
        #endregion
    }

    void DontScrubDateTimes()
    {
        #region DontScrubDateTimes
        SharedVerifySettings.ModifySerialization(_ => _.DontScrubDateTimes());
        #endregion
    }

    void DontIgnoreFalse()
    {
        #region DontIgnoreFalse
        SharedVerifySettings.ModifySerialization(_ => _.DontIgnoreFalse());
        #endregion
    }

    public SerializationSettingsSnippets(ITestOutputHelper output) :
        base(output)
    {
    }
}