using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class SerializationTests :
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

    [Fact]
    public Task NewLineEscapedInProperty()
    {
        #region NewLineEscapedInProperty
        return Verify(new {Property ="a\r\nb"});
        #endregion
    }

    [Fact]
    public async Task NewLineNotEscapedInProperty()
    {
        #region DisableNewLineEscaping
        var settings = new VerifySettings();
        settings.DisableNewLineEscaping();
        await Verify(new {Property = "a\r\nb"}, settings);
        #endregion
    }

    public SerializationTests(ITestOutputHelper output) :
        base(output)
    {
    }
}