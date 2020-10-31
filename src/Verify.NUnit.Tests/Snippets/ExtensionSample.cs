using System.Threading.Tasks;
using NUnit.Framework;
using VerifyTests;
using static VerifyNUnit.Verifier;

#region NUnitExtensionSample
[TestFixture]
public class ExtensionSample
{
    VerifySettings classLevelSettings;

    public ExtensionSample()
    {
        classLevelSettings = new VerifySettings();
        classLevelSettings.UseExtension("json");
    }

    [Test]
    public async Task AtMethod()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.UseExtension("xml");
        await Verify(
            target: @"
<note>
  <to>Joe</to>
  <from>Kim</from>
  <heading>Reminder</heading>
</note>",
            settings: settings);
    }

    [Test]
    public async Task AtMethodFluent()
    {
        await Verify(
                target: @"
<note>
  <to>Joe</to>
  <from>Kim</from>
  <heading>Reminder</heading>
</note>",
                settings: classLevelSettings)
            .UseExtension("xml");
    }

    [Test]
    public async Task SharedClassLevelSettings()
    {
        await Verify(
            target: @"
{
    fruit: 'Apple',
    size: 'Large',
    color: 'Red'
}",
            settings: classLevelSettings);
    }
}
#endregion