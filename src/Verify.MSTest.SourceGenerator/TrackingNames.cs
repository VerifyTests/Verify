namespace VerifyMSTest.SourceGenerator;

public static class TrackingNames
{
    public static string InitialTransform => nameof(InitialTransform);
    public static string RemoveNulls => nameof(RemoveNulls);
    public static string Collect => nameof(Collect);

    // Keep this list in-sync with the tracking name properties so that tests can verify cachability
    public static IReadOnlyCollection<string> AllNames { get; } = [InitialTransform, RemoveNulls, Collect];
}
