namespace VerifyMSTest.SourceGenerator;

public static class TrackingNames
{
    public static string InitialTransform => nameof(InitialTransform);
    public static string RemoveNulls => nameof(RemoveNulls);
    public static string Collect => nameof(Collect);

    public static IReadOnlyCollection<string> GetTrackingNames() =>
        typeof(TrackingNames)
        .GetProperties()
        .Where(_ => _.PropertyType == typeof(string))
        .Select(_ => _.GetValue(null))
        .OfType<string>()
        .Where(_ => !string.IsNullOrEmpty(_))
        .ToList();
}
