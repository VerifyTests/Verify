using System;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class LinesScrubberTests :
    VerifyBase
{
    [Fact]
    public Task ScrubLinesContaining()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesContaining("c", "D", "F");
        return Verify(
            settings: settings,
            target: @"a
b
cs
D
e
f");
    }

    [Fact]
    public Task FilterLines()
    {
        var settings = new VerifySettings();
        settings.ScrubLines(removeLine: x => x.Contains("D"));
        return Verify(
            settings: settings,
            target:  @"a
b
c
D
e
f");
    }

    [Fact]
    public Task ScrubLinesContaining_case_sensitive()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesContaining(StringComparison.Ordinal, "c", "D", "F");
        return Verify(
            settings: settings,
            target: @"a
b
c
D
e
f");
    }

    public LinesScrubberTests(ITestOutputHelper output) :
        base(output)
    {
    }
}