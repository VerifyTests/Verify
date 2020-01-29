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
        if (!DiffTools.TryFind("txt", out var tool))
        {
            return;
        }
        var file = new FilePair("txt", Path.Combine(SourceDirectory, "DiffRunner"));
        //TODO: remove file
        DiffRunner.Launch(tool, file.Received,file.Verified);
    }

    public DiffRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}