// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    Dictionary<string, ScrubOrIgnore?> ignoredByNameMembers = [];

    public SerializationSettings IgnoreStackTrace() =>
        IgnoreMember("StackTrace");

    public SerializationSettings IgnoreMember(string name)
    {
        Guard.NotNullOrEmpty(name);
        ignoredByNameMembers[name] = ScrubOrIgnore.Ignore;
        return this;
    }

    public SerializationSettings ScrubMember(string name)
    {
        Guard.NotNullOrEmpty(name);
        ignoredByNameMembers[name] = ScrubOrIgnore.Scrub;
        return this;
    }

    public SerializationSettings IgnoreMembers(params string[] names)
    {
        Guard.NotNullOrEmpty(names);
        foreach (var name in names)
        {
            IgnoreMember(name);
        }
        return this;
    }

    public SerializationSettings ScrubMembers(params string[] names)
    {
        Guard.NotNullOrEmpty(names);
        foreach (var name in names)
        {
            ScrubMember(name);
        }
        return this;
    }

    internal bool ShouldIgnoreByName(string name)
    {
        if (ignoredByNameMembers.TryGetValue(name, out var scrubOrIgnore))
        {
            return scrubOrIgnore == ScrubOrIgnore.Ignore;
        }

        return false;
    }

    internal bool ShouldScrubByName(string name)
    {
        if (ignoredByNameMembers.TryGetValue(name, out var scrubOrIgnore))
        {
            return scrubOrIgnore == ScrubOrIgnore.Scrub;
        }

        return false;
    }

    internal bool TryGetScrubOrIgnoreByName(string name, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore) =>
        ignoredByNameMembers.TryGetValue(name, out scrubOrIgnore);
}