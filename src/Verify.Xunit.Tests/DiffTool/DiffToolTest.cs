using System.Diagnostics;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class DiffToolsTest :
    VerifyBase
{
    [Fact]
    public void FoundTools()
    {
        foreach (var tool in DiffTools.FoundTools())
        {
            Debug.WriteLine(tool.Name);
        }
    }

    [Fact]
    public void ExtensionLookup()
    {
        foreach (var tool in DiffTools.ExtensionLookup)
        {
            Debug.WriteLine($"{tool.Key}: {tool.Value.Name}");
        }
    }

    public DiffToolsTest(ITestOutputHelper output) :
        base(output)
    {
    }
}