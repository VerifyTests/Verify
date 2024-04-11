sealed class ReceivedFileRequiredTestMethodAttribute :
    TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var results = base.Execute(testMethod);

        foreach (var result in results)
        {
            var files = result.ResultFiles;
            if(files == null)
            {
                result.Outcome = UnitTestOutcome.Failed;
                result.TestFailureException = new("No result files attached to test result.");
                continue;
            }
            var hasAttachment = files.Any(_ => Path.GetFileNameWithoutExtension(_).EndsWith("received"));
            if (!hasAttachment)
            {
                result.Outcome = UnitTestOutcome.Failed;

                var message = "Expected to find *.received.* file attached to test result but did not.";

                if (result.TestFailureException != null)
                {
                    message += $"{Environment.NewLine}{Environment.NewLine}{result.TestFailureException.Message}";
                }

                result.TestFailureException = new(message, result.TestFailureException);
            }
        }

        return results;
    }
}