using System;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class LinesScrubberTests
{
    [Fact]
    public Task ScrubLinesContaining()
    {
        VerifySettings settings = new();
        settings.ScrubLinesContaining("c", "D", "F");
        return Verifier.Verify(
            settings: settings,
            target: @"a
b
c
D
e
f");
    }

    [Fact]
    public Task DontScrubTrailingNewline()
    {
        VerifySettings settings = new();
        settings.ScrubLines(removeLine: x => x.Contains("D"));
        return Verifier.Verify(
            settings: settings,
            target:  @"b
");
    }

    [Fact]
    public Task DontScrubMultiNewline()
    {
        VerifySettings settings = new();
        settings.ScrubLines(removeLine: x => x.Contains("D"));
        return Verifier.Verify(
            settings: settings,
            target:  @"b

c");
    }

    [Fact]
    public Task FilterLines()
    {
        VerifySettings settings = new();
        settings.ScrubLines(removeLine: x => x.Contains("D"));
        return Verifier.Verify(
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
        VerifySettings settings = new();
        settings.ScrubLinesContaining(StringComparison.Ordinal, "c", "D", "F");
        return Verifier.Verify(
            settings: settings,
            target: @"a
b
c
D
e
f");
    }
}