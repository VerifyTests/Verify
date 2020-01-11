using System;
using System.Linq;
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
        originalSettings.Data.Add("cloneable", new MyCloneable());
        var newSettings = new VerifySettings(originalSettings);
        Assert.NotSame(originalSettings.Data.Single().Value, newSettings.Data.Single().Value);
    }

    class MyCloneable :
        ICloneable
    {
        public object Clone()
        {
            return new MyCloneable();
        }
    }

    public VerifySettingsTests(ITestOutputHelper output) :
        base(output)
    {
    }
}