partial class SerializationSettings
{
    [Obsolete("Use ScrubGuids = false")]
    public void DontScrubGuids() =>
        ScrubGuids = false;

    [Obsolete("Use ScrubDateTimes = false")]
    public void DontScrubDateTimes() =>
        ScrubDateTimes = false;
}
