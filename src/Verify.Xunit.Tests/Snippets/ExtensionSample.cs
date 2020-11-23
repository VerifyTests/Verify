using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

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
        VerifySettings settings = new(classLevelSettings);
        settings.UseExtension("xml");
        return Verifier.Verify(
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
        return Verifier.Verify(
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
        return Verifier.Verify(
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