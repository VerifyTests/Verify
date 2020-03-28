using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiffEngine;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class DiffToolsTest :
    VerifyBase
{
    //[Fact]
    //public void IsDetected()
    //{
    //    Assert.False(DiffTools.IsDetectedFor(DiffTool.Kaleidoscope, "png"));
    //    Assert.False(DiffTools.IsDetectedFor(DiffTool.Kaleidoscope, "txt"));
    //    Assert.True(DiffTools.IsDetectedFor(DiffTool.VisualStudio, "txt"));
    //    Assert.True(DiffTools.IsDetectedFor(DiffTool.BeyondCompare, "png"));
    //}

    [Fact]
    public async Task WriteFoundTools()
    {
        var md = Path.Combine(SourceDirectory, "diffTools.include.md");
        File.Delete(md);
        using var writer = File.CreateText(md);
        foreach (var tool in DiffTools.Tools())
        {
            await writer.WriteLineAsync($@"
## [{tool.Name}]({tool.Url})");

            await writer.WriteLineAsync($@"
  * IsMdi: {tool.IsMdi}
  * SupportsAutoRefresh: {tool.SupportsAutoRefresh}");

            if (tool.WindowsExePaths.Any())
            {
                await writer.WriteLineAsync(@"
### Windows scanned paths:
");
                foreach (var path in tool.WindowsExePaths)
                {
                    await writer.WriteLineAsync($@" * `{path}`");
                }
            }

            if (tool.OsxExePaths.Any())
            {
                await writer.WriteLineAsync(@"
### OSX scanned paths:
");
                foreach (var path in tool.OsxExePaths)
                {
                    await writer.WriteLineAsync($@" * `{path}`");
                }
            }

            if (tool.LinuxExePaths.Any())
            {
                await writer.WriteLineAsync(@"
### Linux scanned paths:
");
                foreach (var path in tool.LinuxExePaths)
                {
                    await writer.WriteLineAsync($@" * `{path}`");
                }
            }
            if (!tool.BinaryExtensions.Any())
            {
                continue;
            }

            await writer.WriteLineAsync($@"
### Supported Text files: {tool.SupportsText}
");

            await writer.WriteLineAsync(@"
### Supported binary extensions:
");
            foreach (var extension in tool.BinaryExtensions)
            {
                await writer.WriteLineAsync($@" * {extension}");
            }
        }
    }

    //[Fact]
    //public void LaunchImageDiff()
    //{
    //    foreach (var tool in DiffTools.ResolvedDiffTools)
    //    {
    //        DiffRunner.Launch(tool,
    //            Path.Combine(SourceDirectory, "input.file1.png"),
    //            Path.Combine(SourceDirectory, "input.file2.png"));
    //    }
    //}

    //[Fact]
    //public void LaunchTextDiff()
    //{
    //    foreach (var tool in DiffTools.ResolvedDiffTools)
    //    {
    //        DiffRunner.Launch(tool,
    //            Path.Combine(SourceDirectory, "input.file1.txt"),
    //            Path.Combine(SourceDirectory, "input.file2.txt"));
    //    }
    //}

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