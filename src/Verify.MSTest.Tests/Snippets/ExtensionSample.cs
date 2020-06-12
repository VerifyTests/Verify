using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;
using VerifyMSTest;

[TestClass]
public class ExtensionSample
{
    VerifySettings classLevelSettings;

    public ExtensionSample()
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.UseExtension("json");
    }

    [TestMethod]
    public Task AtMethod()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.UseExtension("xml");
        return Verifier.Verify(
            target: @"<note>
<to>Joe</to>
<from>Kim</from>
<heading>Reminder</heading>
</note>",
            settings: settings);
    }

    [TestMethod]
    public Task SharedClassLevelSettings()
    {
        return Verifier.Verify(
            target: @"{
    ""fruit"": ""Apple"",
    ""size"": ""Large"",
    ""color"": ""Red""
}",
            settings: classLevelSettings);
    }
}