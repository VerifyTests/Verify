public class OrderJsonTests :
    BaseTest
{
    public OrderJsonTests() =>
        VerifierSettings.SortJsonObjects();

    #region OrderJsonObjectsUsage

    [Fact]
    public Task Alphabetically()
    {
        var json = "{'b': 1, 'a': 2}";
        return VerifyJson(json);
    }

    #endregion
}