using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public void WriteFoundTools()
    {
        var md = Path.Combine(SourceDirectory, "diffTools.include.md");
        File.Delete(md);
        using var writer = File.CreateText(md);
        foreach (var tool in DiffTools.Tools())
        {
            writer.WriteLine($@"
## [{tool.Name}]({tool.Url})");

            writer.WriteLine($@"
  * IsMdi: {tool.IsMdi}
  * SupportsAutoRefresh: {tool.SupportsAutoRefresh}");

            if (tool.Notes != null)
            {
                writer.WriteLine(@"
### Notes:
");
                writer.WriteLine(tool.Notes);
            }

            if (tool.WindowsExePaths.Any())
            {
                writer.WriteLine(@"
### Windows settings:
");
                writer.WriteLine($@"
**Example arguments:** `{tool.BuildWindowsArguments("tempFile", "targetFile")}`");

                writer.WriteLine(@"
**Scanned paths:**
");
                foreach (var path in tool.WindowsExePaths)
                {
                    writer.WriteLine($@" * `{path}`");
                }
            }

            if (tool.OsxExePaths.Any())
            {
                writer.WriteLine(@"
### Osx settings:
");
                writer.WriteLine($@"
**Example arguments:** `{tool.BuildOsxArguments("tempFile", "targetFile")}`");

                writer.WriteLine(@"
**Scanned paths:**
");
                foreach (var path in tool.OsxExePaths)
                {
                    writer.WriteLine($@" * `{path}`");
                }
            }

            if (tool.LinuxExePaths.Any())
            {
                writer.WriteLine(@"
### Linux settings:
");
                writer.WriteLine($@"
**Example arguments:** `{tool.BuildLinuxArguments("tempFile", "targetFile")}`");

                writer.WriteLine(@"
**Scanned paths:**
");
                foreach (var path in tool.LinuxExePaths)
                {
                    writer.WriteLine($@" * `{path}`");
                }
            }

            writer.WriteLine($@"
### Supported Text files: {tool.SupportsText}
");

            if (tool.BinaryExtensions.Any())
            {
                writer.WriteLine(@"
### Supported binary extensions:
");
                foreach (var extension in tool.BinaryExtensions)
                {
                    writer.WriteLine($@" * {extension}");
                }
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

#if DEBUG
    [Fact]
    public void TryFind()
    {
        Assert.True(DiffTools.TryFind("txt", out var resolved));
        Assert.NotNull(resolved);

        Assert.True(DiffTools.TryFind(DiffTool.VisualStudio, "txt", out resolved));
        Assert.NotNull(resolved);

        Assert.False(DiffTools.TryFind("notFound", out resolved));
        Assert.Null(resolved);

        Assert.False(DiffTools.TryFind(DiffTool.VisualStudio, "notFound", out resolved));
        Assert.Null(resolved);

        Assert.False(DiffTools.TryFind(DiffTool.Kaleidoscope, "txt", out resolved));
        Assert.Null(resolved);
    }
#endif

    public DiffToolsTest(ITestOutputHelper output) :
        base(output)
    {
    }
}