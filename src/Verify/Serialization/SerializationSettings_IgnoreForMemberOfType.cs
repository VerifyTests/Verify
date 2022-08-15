// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    Dictionary<Type, List<string>> ignoredMembers = new();

    public void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        foreach (var expression in expressions)
        {
            IgnoreMember(expression);
        }
    }

    public void IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        var member = expression.FindMember();
        var declaringType = member.DeclaringType!;
        if (typeof(T) != declaringType)
        {
            throw new(@"IgnoreMember<T> can only be used on the type that defines the member.
To ignore specific members for T, create a custom converter.");
        }

        IgnoreMember(declaringType, member.Name);
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

    public void IgnoreMember(Type declaringType, string name)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstNullable(declaringType, nameof(name));
        if (!ignoredMembers.TryGetValue(declaringType, out var list))
        {
            ignoredMembers[declaringType] = list = new();
        }

        list.Add(name);
    }

    internal bool ShouldIgnoreForMemberOfType(Type declaringType, string name) =>
        ignoredMembers.TryGetValue(declaringType, out var names)
        && names.Contains(name);
}