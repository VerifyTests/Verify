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
        if (!DiffTools.TryFind("txt", out var diffTool))
        {
            return;
        }
        var pair = new FilePair(
            "txt",
            received: Path.Combine(SourceDirectory, "DiffRunnerFile1.txt"),
            verified: Path.Combine(SourceDirectory, "DiffRunnerFile2.txt"));
        DiffRunner.Launch(diffTool, pair);
    }

    public DiffRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}