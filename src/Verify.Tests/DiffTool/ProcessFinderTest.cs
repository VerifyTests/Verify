using System.Diagnostics;
using System.Linq;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ProcessFinderTest :
    VerifyBase
{
    [Fact]
    public void Find()
    {
        var startNew = Stopwatch.StartNew();
        var enumerable = ProcessFinder.Find().ToList();
        WriteLine(startNew.ElapsedMilliseconds);
        foreach (var x in enumerable)
        {
            Debug.WriteLine(x);
        }
    }

    public ProcessFinderTest(ITestOutputHelper output) :
        base(output)
    {
    }
}