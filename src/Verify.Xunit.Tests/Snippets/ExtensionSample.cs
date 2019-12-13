using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ExtensionSample :
    VerifyBase
{
    VerifySettings classLevelSettings;

    [Fact]
    public async Task AtMethod()
    {
        var settings = new VerifySettings();
        settings.UseExtension("xml");
        await Verify(
            target: @"<note>
<to>Joe</to>
<from>Kim</from>
<heading>Reminder</heading>
</note>",
            settings: settings);
    }

    [Fact]
    public async Task InheritedFromClass()
    {
        await Verify(
            target: @"{
    ""fruit"": ""Apple"",
    ""size"": ""Large"",
    ""color"": ""Red""
}",
            settings: classLevelSettings);
    }

    public ExtensionSample(ITestOutputHelper output) :
        base(output)
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.UseExtension("json");
    }
}