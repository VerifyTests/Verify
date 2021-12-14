#region XunitExtensionSample

[UsesVerify]
public class ExtensionSample
{
    VerifySettings classLevelSettings;

    public ExtensionSample()
    {
        classLevelSettings = new();
        classLevelSettings.UseExtension("json");
    }

    [Fact]
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

    [Fact]
    public Task AtMethodFluent()
    {
        return Verify(
                target: @"
<note>
  <to>Joe</to>
  <from>Kim</from>
  <heading>Reminder</heading>
</note>",
                settings: classLevelSettings)
            .UseExtension("xml");
    }

    [Fact]
    public Task SharedClassLevelSettings()
    {
        return Verify(
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