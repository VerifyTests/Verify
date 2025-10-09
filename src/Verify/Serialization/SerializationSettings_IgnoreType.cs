﻿// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    Dictionary<Type, ScrubOrIgnore>? ignoredTypes;

    public void ScrubMembersWithType<T>()
        where T : notnull =>
        ScrubMembersWithType(typeof(T));

    public void ScrubMembersWithType(Type type) =>
        Add(type, ScrubOrIgnore.Scrub);

    public void IgnoreMembersWithType<T>()
        where T : notnull =>
        IgnoreMembersWithType(typeof(T));

    public void IgnoreMembersWithType(Type type) =>
        Add(type, ScrubOrIgnore.Ignore);

    public void AlwaysIncludeMembersWithType<T>()
        where T : notnull =>
        AlwaysIncludeMembersWithType(typeof(T));

    public void AlwaysIncludeMembersWithType(Type type) =>
        Add(type, ScrubOrIgnore.AlwaysInclude);

    void Add(Type type, ScrubOrIgnore scrubOrIgnore)
    {
        ignoredTypes ??= [];
        ignoredTypes[type] = scrubOrIgnore;

        if (type.IsValueType)
        {
            var genericType = typeof(Nullable<>).MakeGenericType(type);
            ignoredTypes[genericType] = scrubOrIgnore;
        }
    }

    bool TryGetScrubOrIgnoreByType(Type memberType, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore)
    {
        if (ignoredTypes != null &&
            ignoredTypes.TryGetValue(memberType, out var value))
        {
            scrubOrIgnore = value;
            return true;
        }

        if (ignoredTypes != null)
        {
            foreach (var member in ignoredTypes)
            {
                if (memberType.InheritsFrom(member.Key))
                {
                    scrubOrIgnore = member.Value;
                    return true;
                }
            }
        }

        scrubOrIgnore = null;
        return false;
    }
}