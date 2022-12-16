namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static Dictionary<string, List<Action<StringBuilder>>> ExtensionMappedGlobalScrubbers = new();

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public static void AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        if (!ExtensionMappedGlobalScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedGlobalScrubbers[extension] = values = new();
        }

        switch (location)
        {
            case ScrubberLocation.First:
                values.Insert(0, scrubber);
                break;
            case ScrubberLocation.Last:
                values.Add(scrubber);
                break;
        }
    }
}