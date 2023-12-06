public class VerifySettingsTests
{
    [Fact]
    public void ContextIsCloned()
    {
        var originalSettings = new VerifySettings();
        originalSettings.Context.Add("cloneable", new MyCloneable());
        var newSettings = new VerifySettings(originalSettings);
        Assert.NotSame(originalSettings.Context.Single()
            .Value, newSettings.Context.Single()
            .Value);
    }

    class MyCloneable :
        ICloneable
    {
        public object Clone() =>
            new MyCloneable();
    }
}