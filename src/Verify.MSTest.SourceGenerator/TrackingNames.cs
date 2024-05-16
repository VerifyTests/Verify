namespace VerifyMSTest.SourceGenerator;

public static class TrackingNames
{
    public static string InitialTransform => nameof(InitialTransform);
    public static string RemoveNulls => nameof(RemoveNulls);
    public static string Collect => nameof(Collect);

    public static IReadOnlyCollection<string> GetTrackingNames() =>
        typeof(TrackingNames)
        .GetProperties()
        .Where(p => p.PropertyType == typeof(string))
        .Select(p => p.GetValue(null))
        .OfType<string>()
        .Where(x => !string.IsNullOrEmpty(x))
        .ToList();
}
