sealed class ResultFilesCallback :
    TestMethodAttribute
{
    public static Action<List<string>>? Callback;

    public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
    {
        try
        {
            var results = await base.ExecuteAsync(testMethod);

            if (Callback == null)
            {
                throw new("Expected Callback");
            }

            Callback(
                results
                    .Where(_ => _.ResultFiles != null)
                    .SelectMany(_ => _.ResultFiles!)
                    .ToList());

            return results;
        }
        finally
        {
            Callback = null;
        }
    }
}