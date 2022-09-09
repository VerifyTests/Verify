// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    Dictionary<string, ScrubOrIgnore?> ignoredByNameMembers = new();

    public void IgnoreStackTrace() =>
        IgnoreMember("StackTrace");

    public void IgnoreMember(string name)
    {
        Guard.AgainstNullOrEmpty(name);
        ignoredByNameMembers[name] = ScrubOrIgnore.Ignore;
    }

    public void ScrubMember(string name)
    {
        Guard.AgainstNullOrEmpty(name);
        ignoredByNameMembers[name] = ScrubOrIgnore.Scrub;
    }

    public void IgnoreMembers(params string[] names)
    {
        Guard.AgainstNullOrEmpty(names);
        foreach (var name in names)
        {
            IgnoreMember(name);
        }
    }

    public void ScrubMembers(params string[] names)
    {
        Guard.AgainstNullOrEmpty(names);
        foreach (var name in names)
        {
            ScrubMember(name);
        }
    }

    internal bool TryGetScrubOrIgnoreByName(string name, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore) =>
        ignoredByNameMembers.TryGetValue(name, out scrubOrIgnore);
}