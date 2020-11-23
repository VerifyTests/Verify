using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyTests;
using VerifyMSTest;

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
        VerifySettings settings = new(classLevelSettings);
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

    [TestMethod]
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