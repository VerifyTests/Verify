using System;
using System.Linq;
using VerifyTests;
using Xunit;

public class VerifySettingsTests
{
    [Fact]
    public void ContextIsCloned()
    {
        VerifySettings originalSettings = new();
        originalSettings.Context.Add("cloneable", new MyCloneable());
        VerifySettings newSettings = new(originalSettings);
        Assert.NotSame(originalSettings.Context.Single().Value, newSettings.Context.Single().Value);
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