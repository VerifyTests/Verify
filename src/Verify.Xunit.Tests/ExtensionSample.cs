using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ExtensionSample :
    VerifyBase
{
    [Fact]
    public async Task AtMethod()
    {
        UseExtensionForText(".xml");
        await VerifyText(@"<note>
<to>Joe</to>
<from>Kim</from>
<heading>Reminder</heading>
</note>");
    }

    [Fact]
    public async Task InheritedFromClass()
    {
        await VerifyText(@"{
    ""fruit"": ""Apple"",
    ""size"": ""Large"",
    ""color"": ""Red""
}");
    }

    public ExtensionSample(ITestOutputHelper output) :
        base(output)
    {
        UseExtensionForText(".json");
    }
}