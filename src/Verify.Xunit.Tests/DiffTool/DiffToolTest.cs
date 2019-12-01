using System.Diagnostics;
using System.IO;
using System.Linq;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class DiffToolsTest :
    VerifyBase
{
    [Fact]
    public void WriteFoundTools()
    {
        foreach (var tool in DiffTools.Tools().Where(x => x.Exists))
        {
            Debug.WriteLine(tool.Name);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void LaunchImageDiff()
    {
        foreach (var tool in DiffTools.Tools().Where(x => x.Exists))
        {
            if (!tool.BinaryExtensions.Contains("png"))
            {
                continue;
            }

            DiffRunner.Launch(
                new ResolvedDiffTool(tool.Name, tool.ExePath!, tool.ArgumentPrefix),
                receivedPath: Path.Combine(SourceDirectory, "input_received.png"),
                verifiedPath: Path.Combine(SourceDirectory, "input_verified.png"));
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