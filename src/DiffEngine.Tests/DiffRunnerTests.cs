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
        if (!DiffTools.TryFind("txt", out var tool))
        {
            return;
        }

        DiffRunner.Launch(tool,
            Path.Combine(SourceDirectory, "DiffRunner.file1.txt"),
            Path.Combine(SourceDirectory, "DiffRunner.file2.txt"));
    }

    public DiffRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}