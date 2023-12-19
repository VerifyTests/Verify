// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    Dictionary<Type, Dictionary<string, ScrubOrIgnore>> ignoredMembers = [];

    public void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        foreach (var expression in expressions)
        {
            IgnoreMember(expression);
        }
    }

    public void ScrubMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        foreach (var expression in expressions)
        {
            ScrubMember(expression);
        }
    }

    public void IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull =>
        IgnoreMember(expression, ScrubOrIgnore.Ignore);

    public void ScrubMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull =>
        IgnoreMember(expression, ScrubOrIgnore.Scrub);

    void IgnoreMember<T>(Expression<Func<T, object?>> expression, ScrubOrIgnore scrubOrIgnore)
        where T : notnull
    {
        var member = expression.FindMember();
        var declaringType = member.DeclaringType!;
        if (typeof(T) != declaringType)
        {
            throw new(
                """
                IgnoreMember<T> can only be used on the type that defines the member.
                To ignore specific members for T, create a custom converter.
                """);
        }

        IgnoreMember(declaringType, member.Name, scrubOrIgnore);
    }

    public void IgnoreMembers<T>(params string[] names)
        where T : notnull =>
        IgnoreMembers(typeof(T), names);

    public void ScrubMembers<T>(params string[] names)
        where T : notnull =>
        ScrubMembers(typeof(T), names);

    public void IgnoreMember<T>(string name)
        where T : notnull =>
        IgnoreMember(typeof(T), name);

    public void ScrubMember<T>(string name)
        where T : notnull =>
        ScrubMember(typeof(T), name);

    public void IgnoreMembers(Type declaringType, params string[] names)
    {
        Guard.AgainstNullOrEmpty(names);
        foreach (var name in names)
        {
            IgnoreMember(declaringType, name);
        }
    }

    public void ScrubMembers(Type declaringType, params string[] names)
    {
        Guard.AgainstNullOrEmpty(names);
        foreach (var name in names)
        {
            ScrubMember(declaringType, name);
        }
    }

    public void IgnoreMember(Type declaringType, string name) =>
        IgnoreMember(declaringType, name, ScrubOrIgnore.Ignore);

    public void ScrubMember(Type declaringType, string name) =>
        IgnoreMember(declaringType, name, ScrubOrIgnore.Scrub);

    void IgnoreMember(Type declaringType, string name, ScrubOrIgnore scrubOrIgnore)
    {
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstNullable(declaringType);
        if (!ignoredMembers.TryGetValue(declaringType, out var list))
        {
            ignoredMembers[declaringType] = list = [];
        }

        list[name] = scrubOrIgnore;
    }

    internal bool TryGetScrubOrIgnoreByMemberOfType(Type declaringType, string name, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore)
    {
        if (ignoredMembers.TryGetValue(declaringType, out var ignores))
        {
            if (ignores.TryGetValue(name, out var innerScrubOrIgnore))
            {
                scrubOrIgnore = innerScrubOrIgnore;
                return true;
            }
        }

        foreach (var (key, value) in ignoredMembers)
        {
            if (!key.IsAssignableFrom(declaringType))
            {
                continue;
            }

            if (!value.TryGetValue(name, out var innerScrubOrIgnore))
            {
                continue;
            }

            scrubOrIgnore = innerScrubOrIgnore;
            return true;
        }

        scrubOrIgnore = null;
        return false;
    }
}