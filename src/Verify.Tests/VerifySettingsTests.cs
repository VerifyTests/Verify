using System;
using System.Linq;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class VerifySettingsTests :
    VerifyBase
{
    [Fact]
    public void DataIsCloned()
    {
        var originalSettings = new VerifySettings();
        originalSettings.Data.Add("clonable", new MyClonable());
        var newSettings = new VerifySettings(originalSettings);
        Assert.NotSame(originalSettings.Data.Single().Value, newSettings.Data.Single().Value);
    }

    public VerifySettingsTests(ITestOutputHelper output) :
        base(output)
    {
    }

    public class MyClonable : ICloneable
    {
        public object Clone()
        {
            return new MyClonable();
        }
    }
}