sealed class ResultFilesCallback(
    [CallerFilePath] string callerFilePath = "",
    [CallerLineNumber] int callerLineNumber = -1) :
    TestMethodAttribute(callerFilePath, callerLineNumber)
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
            [
                .. results
                    .Where(_ => _.ResultFiles != null)
                    .SelectMany(_ => _.ResultFiles!)
            ]);

            return results;
        }
        finally
        {
            Callback = null;
        }
    }
}