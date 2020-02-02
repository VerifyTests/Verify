using System.IO;
using DiffEngine;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class DiffRunnerTests :
    VerifyBase
{
    [Fact(Skip = "Explicit")]
    public void Launch()
    {
        var path1 = Path.Combine(SourceDirectory, "DiffRunner.file1.txt");
        var path2 = Path.Combine(SourceDirectory, "DiffRunner.file2.txt");
        #region DiffRunnerLaunch
        DiffRunner.Launch("txt", path1, path2);
        #endregion
    }

    [Fact(Skip = "Explicit")]
    public void Kill()
    {
        var path1 = Path.Combine(SourceDirectory, "DiffRunner.file1.txt");
        var path2 = Path.Combine(SourceDirectory, "DiffRunner.file2.txt");
        DiffRunner.Launch("txt", path1, path2);
        #region DiffRunnerKill
        DiffRunner.Kill("txt", path1, path2);
        #endregion
    }

    public DiffRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}