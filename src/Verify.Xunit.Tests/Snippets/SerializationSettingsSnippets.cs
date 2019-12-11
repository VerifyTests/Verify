using Verify;
using VerifyXunit;
using Xunit.Abstractions;

public class SerializationSettingsSnippets :
    VerifyBase
{
    void DontIgnoreEmptyCollections()
    {
        #region DontIgnoreEmptyCollections
        Global.ModifySerialization(settings => settings.DontIgnoreEmptyCollections());
        #endregion
    }

    void DontScrubGuids()
    {
        #region DontScrubGuids
        Global.ModifySerialization(settings => settings.DontScrubGuids());
        #endregion
    }

    void DontScrubDateTimes()
    {
        #region DontScrubDateTimes
        Global.ModifySerialization(settings => settings.DontScrubDateTimes());
        #endregion
    }

    void DontIgnoreFalse()
    {
        #region DontIgnoreFalse
        Global.ModifySerialization(settings => settings.DontIgnoreFalse());
        #endregion
    }

    public SerializationSettingsSnippets(ITestOutputHelper output) :
        base(output)
    {
    }
}