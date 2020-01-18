using System.IO;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class DiffRunnerTests :
    VerifyBase
{
    [Fact(Skip = "Explicit")]
    public void Launch()
    {
        var diffTool = DiffTools.Find("txt");
        if (diffTool != null)
        {
            DiffRunner.Launch(diffTool, Path.Combine(SourceDirectory, "DiffRunnerFile1.txt"), Path.Combine(SourceDirectory, "DiffRunnerFile2.txt"));
        }
    }

    public DiffRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}