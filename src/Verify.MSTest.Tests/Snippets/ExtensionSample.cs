﻿namespace TheTests;

#region MSTestExtensionSample

[TestClass]
public class ExtensionSample :
    VerifyBase
{
    VerifySettings classLevelSettings;

    public ExtensionSample()
    {
        classLevelSettings = new();
        classLevelSettings.UseExtension("json");
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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