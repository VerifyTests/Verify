#region SortJsonObjects

static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.SortJsonObjects();
}

#endregion

[UsesVerify]
public class SortedJsonTests
{
    #region SortJsonObjectsUsage

    [Fact]
    public Task Alphabetically()
    {
        var json = "{'b': 1, 'a': 2}";
        return VerifyJson(json);
    }

    #endregion
}
