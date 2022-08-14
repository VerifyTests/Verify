// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    internal Dictionary<Type, List<string>> ignoredMembers = new();
    internal List<string> ignoredByNameMembers = new();

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
        where T : notnull
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        IgnoreMember(typeof(T), name);
    }

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

    public void IgnoreMember(string name)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        ignoredByNameMembers.Add(name);
    }

    public void IgnoreMembers(params string[] names)
    {
        Guard.AgainstNullOrEmpty(names, nameof(names));
        foreach (var name in names)
        {
            IgnoreMember(name);
        }
    }

    List<Type> ignoredTypes = new();

    public void IgnoreMembersWithType<T>()
        where T : notnull =>
        ignoredTypes.Add(typeof(T));

    public void IgnoreMembersWithType(Type type) =>
        ignoredTypes.Add(type);

    internal List<Func<Exception, bool>> ignoreMembersThatThrow = new();

    public void IgnoreMembersThatThrow<T>()
        where T : Exception =>
        ignoreMembersThatThrow.Add(_ => _ is T);

    public void IgnoreMembersThatThrow(Func<Exception, bool> item) =>
        IgnoreMembersThatThrow<Exception>(item);

    public void IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception =>
        ignoreMembersThatThrow.Add(
            _ =>
            {
                if (_ is T exception)
                {
                    return item(exception);
                }

                return false;
            });

    bool ignoreEmptyCollections = true;

    public void DontIgnoreEmptyCollections() =>
        ignoreEmptyCollections = false;
}