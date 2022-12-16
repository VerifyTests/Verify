namespace VerifyTests;

public partial class VerifySettings
{
    internal Dictionary<string, List<Action<StringBuilder>>> ExtensionMappedInstanceScrubbers = new();

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public void AddScrubber(string extension, Action<StringBuilder> scrubber, ScrubberLocation location = ScrubberLocation.First)
    {
        if (!ExtensionMappedInstanceScrubbers.TryGetValue(extension, out var values))
        {
            ExtensionMappedInstanceScrubbers[extension] = values = new();
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