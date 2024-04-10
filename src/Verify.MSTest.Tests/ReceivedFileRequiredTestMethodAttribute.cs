sealed class ReceivedFileRequiredTestMethodAttribute :
    TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var results = base.Execute(testMethod);

        foreach (var result in results)
        {
            var hasAttachment = result.ResultFiles?.Any(file => Path.GetFileNameWithoutExtension(file).EndsWith("received")) ?? false;
            if (!hasAttachment)
            {
                result.Outcome = UnitTestOutcome.Failed;

                var message = "Expected to find *.received.* file attached to test result but did not.";

                if (result.TestFailureException != null)
                {
                    message += $"{Environment.NewLine}{Environment.NewLine}{result.TestFailureException.Message}";
                }

                result.TestFailureException = new Exception(message, result.TestFailureException);
            }
        }

        return results;
    }
}