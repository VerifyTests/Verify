using System;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
// Non-nullable field is uninitialized.
#pragma warning disable CS8618

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
        return Verify(new {Property = "a\r\nb"});
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

    [Fact]
    public Task ScrubTempPath()
    {
        return Verify(Path.GetTempPath().TrimEnd('/', '\\'));
    }

    [Fact]
    public Task ScrubCurrentDirectory()
    {
        return Verify(Environment.CurrentDirectory.TrimEnd('/', '\\'));
    }

    [Fact]
    public Task ScrubCodeBaseLocation()
    {
        return Verify(CodeBaseLocation.CurrentDirectory.TrimEnd('/', '\\'));
    }

    [Fact]
    public Task ScrubBaseDirectory()
    {
        return Verify(AppDomain.CurrentDomain.BaseDirectory!.TrimEnd('/', '\\'));
    }

    public class TypeTarget
    {
        public Type Type;
        public Type Dynamic;
    }

    [Fact]
    public async Task ShouldUseShortTypeName()
    {
        #region type

        var foo = new {x = 1};
        var target = new TypeTarget
        {
            Type = GetType(),
            Dynamic = foo.GetType(),
        };

        await Verify(target);

        #endregion
    }

#if(!NETSTANDARD2_0)
    [Fact]
    public async Task NamedTuple()
    {
        #region VerifyTuple

        await Verify(() => MethodWithNamedTuple());

        #endregion
    }

    #region MethodWithNamedTuple

    static (bool Member1, string Member2, string Member3) MethodWithNamedTuple()
    {
        return (true, "A", "B");
    }

    #endregion

    [Fact]
    public async Task Tuple()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verify(() => MethodWithTuple()));
        await Verify(exception.Message);
    }

    static (bool, string, string) MethodWithTuple()
    {
        return (true, "A", "B");
    }

#endif
    public SerializationTests(ITestOutputHelper output) :
        base(output)
    {
    }
}