using Verify;
using VerifyXunit;
using Xunit.Abstractions;

public class SerializationSettingsSnippets :
    VerifyBase
{
    void DontIgnoreEmptyCollections()
    {
        #region DontIgnoreEmptyCollections
        SharedVerifySettings.ModifySerialization(settings => settings.DontIgnoreEmptyCollections());
        #endregion
    }

    void DontScrubGuids()
    {
        #region DontScrubGuids
        SharedVerifySettings.ModifySerialization(settings => settings.DontScrubGuids());
        #endregion
    }

    void DontScrubDateTimes()
    {
        #region DontScrubDateTimes
        SharedVerifySettings.ModifySerialization(settings => settings.DontScrubDateTimes());
        #endregion
    }

    void DontIgnoreFalse()
    {
        #region DontIgnoreFalse
        SharedVerifySettings.ModifySerialization(settings => settings.DontIgnoreFalse());
        #endregion
    }

    public SerializationSettingsSnippets(ITestOutputHelper output) :
        base(output)
    {
    }
}