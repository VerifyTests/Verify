using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;
using VerifyTests;
#region NUnitExtensionSample
[TestFixture]
public class ExtensionSample
{
    VerifySettings classLevelSettings;

    public ExtensionSample()
    {
        classLevelSettings = new();
        classLevelSettings.UseExtension("json");
    }

    [Test]
    public async Task AtMethod()
    {
        VerifySettings settings = new(classLevelSettings);
        settings.UseExtension("xml");
        await Verifier.Verify(
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
        await Verifier.Verify(
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
        await Verifier.Verify(
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