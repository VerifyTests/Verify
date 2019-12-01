using System.Diagnostics;
using System.IO;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class DiffToolsTest :
    VerifyBase
{
    [Fact]
    public void FoundTools()
    {
        foreach (var tool in DiffTools.FoundTools())
        {
            Debug.WriteLine(tool.Name);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void LaunchTextDiff()
    {
        foreach (var tool in DiffTools.ResolvedDiffTools)
        {
            DiffRunner.Launch(
                tool,
                receivedPath: Path.Combine(SourceDirectory, "input_received.txt"),
                verifiedPath: Path.Combine(SourceDirectory, "input_verified.txt"));
        }
    }

    [Fact]
    public void ExtensionLookup()
    {
        foreach (var tool in DiffTools.ExtensionLookup)
        {
            Debug.WriteLine($"{tool.Key}: {tool.Value.Name}");
        }
    }

    public DiffToolsTest(ITestOutputHelper output) :
        base(output)
    {
    }
}