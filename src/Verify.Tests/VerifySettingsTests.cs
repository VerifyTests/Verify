using System;
using System.Linq;
using VerifyTests;
using Xunit;

public class VerifySettingsTests
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
}