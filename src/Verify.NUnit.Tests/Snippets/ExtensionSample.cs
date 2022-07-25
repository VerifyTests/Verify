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
    public Task AtMethod()
    {
        var settings = new VerifySettings(classLevelSettings);
        settings.UseExtension("xml");
        return Verify(
            target: @"
<note>
  <to>Joe</to>
  <from>Kim</from>
  <heading>Reminder</heading>
</note>",
            settings: settings);
    }

    [Test]
    public Task AtMethodFluent() =>
        Verify(
                target: @"
<note>
  <to>Joe</to>
  <from>Kim</from>
  <heading>Reminder</heading>
</note>",
                settings: classLevelSettings)
            .UseExtension("xml");

    [Test]
    public Task SharedClassLevelSettings() =>
        Verify(
            target: @"
{
  fruit: 'Apple',
  size: 'Large',
  color: 'Red'
}",
            settings: classLevelSettings);
}

#endregion