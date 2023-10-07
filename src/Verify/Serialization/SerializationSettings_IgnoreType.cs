// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    Dictionary<Type, ScrubOrIgnore> ignoredTypes = new();

    public void ScrubMembersWithType<T>()
        where T : notnull =>
        ScrubMembersWithType(typeof(T));

    public void ScrubMembersWithType(Type type) =>
        ignoredTypes[type]= ScrubOrIgnore.Scrub;

    public void IgnoreMembersWithType<T>()
        where T : notnull =>
        IgnoreMembersWithType(typeof(T));

    public void IgnoreMembersWithType(Type type) =>
        ignoredTypes[type] = ScrubOrIgnore.Ignore;

    bool TryGetScrubOrIgnoreByType(Type memberType, [NotNullWhen(true)]out ScrubOrIgnore? scrubOrIgnore)
    {
        foreach (var member in ignoredTypes)
        {
            if (memberType.InheritsFrom(member.Key))
            {
                scrubOrIgnore = member.Value;
                return true;
            }
        }
        var typeFromNullable = Nullable.GetUnderlyingType(memberType);
        foreach (var member in ignoredTypes)
        {
            if (member.Key.IsAssignableFrom(typeFromNullable))
            {
                scrubOrIgnore = member.Value;
                return true;
            }
        }

        scrubOrIgnore = null;
        return false;
    }
}