using System.Diagnostics;
using System.Linq;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ProcessCleanupTests :
    VerifyBase
{
    [Fact]
    public void Find()
    {
        var enumerable = ProcessCleanup.FindAll().ToList();
        foreach (var x in enumerable)
        {
            Debug.WriteLine($"{x.Process} {x.Command}");
        }
    }

    public ProcessCleanupTests(ITestOutputHelper output) :
        base(output)
    {
    }
}