namespace ApprovalTests.Set;

public static class SetApprovals
{
    [Obsolete("Run the formatter then pass the result to Verifier.Verify(enumerable)", true)]
    public static void VerifySet<T>(IEnumerable<T> enumerable, Func<T, string> formatter)
    {
    }

    [Obsolete("Use Verifier.Verify<T>(T)", true)]
    public static void VerifySet<T>(IEnumerable<T> enumerable, string label)
    {
    }

    [Obsolete("Run the formatter then pass the result to Verifier.Verify(enumerable)", true)]
    public static void VerifySet<T>(IEnumerable<T> enumerable, string label, Func<T, string> formatter)
    {
    }

    [Obsolete("Use Verifier.Verify<T>(T)", true)]
    public static void VerifySet<T>(string header, IEnumerable<T> enumerable, string label)
    {
    }

    [Obsolete("Run the formatter then pass the result to Verifier.Verify(enumerable)", true)]
    public static void VerifySet<T>(string header, IEnumerable<T> enumerable, Func<T, string> formatter)
    {
    }

    [Obsolete("Read all lines of the file, then pass the result to Verifier.Verify(lines).AddScrubber(scrubber)", true)]
    public static void VerifyFileAsSet(string filename, Func<string, string> scrubber)
    {
    }

    [Obsolete("Read all lines of the file, then pass the result to Verifier.Verify(lines)", true)]
    public static void VerifyFileAsSet(string filename)
    {
    }
}