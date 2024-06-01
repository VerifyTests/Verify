public static class TrackingNames
{
    public static string MarkerAttributeInitialTransform => nameof(MarkerAttributeInitialTransform);
    public static string AssemblyAttributeInitialTransform => nameof(AssemblyAttributeInitialTransform);
    public static string Merge => nameof(Merge);
    public static string Complete => nameof(Complete);

    // Keep this list in-sync with the tracking name properties so that tests can verify cachability
    public static IReadOnlyCollection<string> AllNames { get; } =
        [
        MarkerAttributeInitialTransform,
        AssemblyAttributeInitialTransform,
        Merge,
        Complete,
        ];
}
