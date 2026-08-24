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

    [Fact]
    public void AddContextReplacesExisting()
    {
        var task = BuildTask()
            .AddContext("key", "first")
            .AddContext("key", "second");
        Assert.Equal("second", task.CurrentSettings.Context["key"]);
    }

    [Fact]
    public async Task AddContextRejectsEmptyName()
    {
        var task = BuildTask();
        // Not an expression body, since SettingsTask is awaitable
        // and would bind to the async Assert.Throws overload
        await Assert.ThrowsAsync<ArgumentException>(
            () => task.AddContext("", "value"));
    }

    static SettingsTask BuildTask() =>
        new(new(), _ => throw new NotImplementedException());

    class MyCloneable :
        ICloneable
    {
        public object Clone() =>
            new MyCloneable();
    }
}
