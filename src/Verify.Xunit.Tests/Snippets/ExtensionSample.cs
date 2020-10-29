using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;
using static VerifyXunit.Verifier;

#region XunitExtensionSample

[UsesVerify]
public class ExtensionSample
{
    VerifySettings classLevelSettings;

    public ExtensionSample()
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.UseExtension("json");
    }

    [Fact]
    public Task AtMethod()
    {
        return Verify(@"<note>
<to>Joe</to>
<from>Kim</from>
<heading>Reminder</heading>
</note>")
            .BasedOn(classLevelSettings)
            .UseExtension("xml");
    }

    [Fact]
    public Task SharedClassLevelSettings()
    {
        return Verify(@"{
    ""fruit"": ""Apple"",
    ""size"": ""Large"",
    ""color"": ""Red""
}")
            .BasedOn(classLevelSettings);
    }
}

#endregion