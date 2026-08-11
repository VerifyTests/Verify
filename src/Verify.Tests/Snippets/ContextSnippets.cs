#if DEBUG

public class ContextSnippets
{
    #region ContextInTest

    [Fact]
    public Task ComparerWithContext()
    {
        var settings = new VerifySettings();
        settings.Context["featureEnabled"] = true;
        settings.UseStringComparer(Compare, "txt");
        return Verify("TheText", settings);
    }

    #endregion

    #region ContextInTestFluent

    [Fact]
    public Task ComparerWithContextFluent() =>
        Verify("TheText")
            .AddContext("featureEnabled", true)
            .UseStringComparer(Compare, "txt");

    #endregion

    #region ContextInComparer

    static Task<CompareResult> Compare(
        string received,
        string verified,
        IReadOnlyDictionary<string, object> context)
    {
        if (context.TryGetValue("featureEnabled", out var value) &&
            value is true)
        {
            // Drop the flagged content from both sides before comparing
            return Task.FromResult(
                new CompareResult(RemoveFlagged(received) == RemoveFlagged(verified)));
        }

        return Task.FromResult(new CompareResult(received == verified));
    }

    static string RemoveFlagged(string value) =>
        string.Join(
            '\n',
            value
                .Split('\n')
                .Where(_ => !_.Contains("FeatureFlagged")));

    #endregion
}
#endif
