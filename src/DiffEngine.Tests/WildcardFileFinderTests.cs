using System.IO;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class WildcardFileFinderTests :
    VerifyBase
{
    [Fact]
    public void FullFilePath()
    {
        Assert.True(WildcardFileFinder.TryFind(SourceFile, out var result));
        Assert.True(File.Exists(result));
    }

    [Fact]
    public void FullFilePath_missing()
    {
        Assert.False(WildcardFileFinder.TryFind(SourceFile.Replace(".cs", ".foo"), out var result));
        Assert.Null(result);
    }

    [Fact]
    public void WildCardInFile()
    {
        var path = Path.Combine(SourceDirectory, "WildcardFileFinder*.cs");
        Assert.True(WildcardFileFinder.TryFind(path, out var result));
        Assert.True(File.Exists(result));
    }

    [Fact]
    public void WildCardInFile_missing()
    {
        var path = Path.Combine(SourceDirectory, "WildcardFileFinder*.foo");
        Assert.False(WildcardFileFinder.TryFind(path, out var result));
        Assert.Null(result);
    }

    [Fact]
    public void WildCardInDir()
    {
        var directory = SourceDirectory.Replace("DiffEngine.Tests", "Diff*.Tests");
        var path = Path.Combine(directory, "WildcardFileFinderTests.cs");
        Assert.True(WildcardFileFinder.TryFind(path, out var result));
        Assert.True(File.Exists(result));
    }

    [Fact]
    public void WildCardInDir_missing()
    {
        var directory = SourceDirectory.Replace("DiffEngine.Tests", "Diff*.Foo");
        var path = Path.Combine(directory, "WildcardFileFinderTests.cs");
        Assert.False(WildcardFileFinder.TryFind(path, out var result));
        Assert.Null(result);
    }

    public WildcardFileFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}