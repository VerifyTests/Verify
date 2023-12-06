[UsesVerify]
public class SortedJsonTests :
    BaseTest
{
    public SortedJsonTests() =>
        VerifierSettings.SortJsonObjects();

    #region SortJsonObjectsUsage

    [Fact]
    public Task Alphabetically()
    {
        var json = "{'b': 1, 'a': 2}";
        return VerifyJson(json);
    }

    #endregion
}