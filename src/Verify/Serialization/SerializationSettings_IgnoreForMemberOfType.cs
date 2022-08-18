// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    Dictionary<Type, Dictionary<string, ScrubOrIgnore>> ignoredMembers = new();

    public void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        foreach (var expression in expressions)
        {
            IgnoreMember(expression);
        }
    }

    public void IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull =>
        IgnoreMember(expression, ScrubOrIgnore.Ignore);

    public void IgnoreMember<T>(Expression<Func<T, object?>> expression,ScrubOrIgnore scrubOrIgnore)
        where T : notnull
    {
        var member = expression.FindMember();
        var declaringType = member.DeclaringType!;
        if (typeof(T) != declaringType)
        {
            throw new(@"IgnoreMember<T> can only be used on the type that defines the member.
To ignore specific members for T, create a custom converter.");
        }

        IgnoreMember(declaringType, member.Name,scrubOrIgnore);
    }

    public void IgnoreMembers<T>(params string[] names)
        where T : notnull =>
        IgnoreMembers(typeof(T), names);

    public void IgnoreMember<T>(string name)
        where T : notnull =>
        IgnoreMember(typeof(T), name);

    public void IgnoreMembers(Type declaringType, params string[] names)
    {
        Guard.AgainstNullOrEmpty(names, nameof(names));
        foreach (var name in names)
        {
            IgnoreMember(declaringType, name);
        }
    }

    public void IgnoreMember(Type declaringType, string name) =>
        IgnoreMember(declaringType, name, ScrubOrIgnore.Ignore);

    void IgnoreMember(Type declaringType, string name, ScrubOrIgnore scrubOrIgnore)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstNullable(declaringType, nameof(name));
        if (!ignoredMembers.TryGetValue(declaringType, out var list))
        {
            ignoredMembers[declaringType] = list = new();
        }

        list[name] = scrubOrIgnore;
    }

    internal bool ShouldIgnoreForMemberOfType(Type declaringType, string name, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore)
    {
        foreach (var typeIgnores in ignoredMembers)
        {
            if (typeIgnores.Key.IsAssignableFrom(declaringType))
            {
                if (typeIgnores.Value.TryGetValue(name, out var innerScrubOrIgnore))
                {
                    scrubOrIgnore = innerScrubOrIgnore;
                    return true;
                }
            }
        }

        scrubOrIgnore = null;
        return false;
    }
}