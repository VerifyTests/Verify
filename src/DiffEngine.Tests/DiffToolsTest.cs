using System;
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
    public Task ParseEnvironmentVariable()
    {
        return Verify(DiffTools.ParseEnvironmentVariable("VisualStudio,Meld"));
    }

    [Fact]
    public Task BadEnvironmentVariable()
    {
        var exception = Assert.Throws<Exception>(() => DiffTools.ParseEnvironmentVariable("Foo").ToList());
        return Verify(exception.Message);
    }

    [Fact]
    public void WriteFoundTools()
    {
        var md = Path.Combine(SourceDirectory, "diffTools.include.md");
        File.Delete(md);
        using var writer = File.CreateText(md);
        var tools = DiffTools
            .Tools();

        foreach (var tool in tools
            .OrderBy(x => x.Name.ToString()))
        {
            writer.WriteLine($@"
## [{tool.Name}]({tool.Url})");

            writer.WriteLine($@"
  * Is MDI: {tool.IsMdi}
  * Supports auto-refresh: {tool.SupportsAutoRefresh}
  * Supports text files: {tool.SupportsText}");

            if (tool.BinaryExtensions.Any())
            {
                writer.WriteLine(@" * Supported binaries: " + string.Join(", ", tool.BinaryExtensions));
            }

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
 * Example arguments: `{tool.BuildWindowsArguments!("tempFile", "targetFile")}`");
                WritePaths(writer, tool.WindowsExePaths);
            }

            if (tool.OsxExePaths.Any())
            {
                writer.WriteLine(@"
### OSX settings:
");
                writer.WriteLine($@"
 * Example arguments: `{tool.BuildOsxArguments!("tempFile", "targetFile")}`");
                WritePaths(writer, tool.OsxExePaths);
            }

            if (tool.LinuxExePaths.Any())
            {
                writer.WriteLine(@"
### Linux settings:
");
                writer.WriteLine($@"
 * Example arguments: `{tool.BuildLinuxArguments!("tempFile", "targetFile")}`");

                WritePaths(writer, tool.LinuxExePaths);
            }
        }
    }

    static void WritePaths(TextWriter writer, string[] paths)
    {
        if (paths.Length > 1)
        {
            writer.WriteLine(@" * Scanned paths:
");
            foreach (var path in paths)
            {
                writer.WriteLine($@"   * `{path}`");
            }
        }
        else
        {
            writer.WriteLine($@" * Scanned path: `{paths.Single()}`");
        }
    }

    [Fact]
    public void WriteDefaultDiffToolOrder()
    {
        var md = Path.Combine(SourceDirectory, "defaultDiffToolOrder.include.md");
        File.Delete(md);
        using var writer = File.CreateText(md);

        foreach (var tool in DiffTools.Tools())
        {
            writer.WriteLine($@" * {tool.Name}");
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