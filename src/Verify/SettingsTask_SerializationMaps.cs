namespace VerifyTests;

public partial class SettingsTask
{
    [Pure]
    public SettingsTask DontScrubGuids()
    {
        CurrentSettings.DontScrubGuids();
        return this;
    }

    [Pure]
    public SettingsTask DontScrubDateTimes()
    {
        CurrentSettings.DontScrubDateTimes();
        return this;
    }

    [Pure]
    public SettingsTask DontSortDictionaries()
    {
        CurrentSettings.DontSortDictionaries();
        return this;
    }

    [Pure]
    public SettingsTask OrderEnumerableBy<T>(Func<T, object?> keySelector)
    {
        CurrentSettings.OrderEnumerableBy(keySelector);
        return this;
    }

    [Pure]
    public SettingsTask OrderEnumerableByDescending<T>(Func<T, object?> keySelector)
    {
        CurrentSettings.OrderEnumerableByDescending(keySelector);
        return this;
    }

    [Pure]
    public SettingsTask IncludeObsoletes()
    {
        CurrentSettings.IncludeObsoletes();
        return this;
    }

    [Pure]
    public SettingsTask DontIgnoreEmptyCollections()
    {
        CurrentSettings.DontIgnoreEmptyCollections();
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        CurrentSettings.IgnoreMembers(expressions);
        return this;
    }

    [Pure]
    public SettingsTask ScrubMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        CurrentSettings.ScrubMembers(expressions);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        CurrentSettings.IgnoreMembers(expression);
        return this;
    }

    [Pure]
    public SettingsTask ScrubMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        CurrentSettings.ScrubMembers(expression);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMembers<T>(params string[] names)
        where T : notnull
    {
        CurrentSettings.IgnoreMembers<T>(names);
        return this;
    }

    [Pure]
    public SettingsTask ScrubMembers<T>(params string[] names)
        where T : notnull
    {
        CurrentSettings.ScrubMembers<T>(names);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMember<T>(string name)
        where T : notnull
    {
        CurrentSettings.IgnoreMember<T>(name);
        return this;
    }

    [Pure]
    public SettingsTask ScrubMember<T>(string name)
        where T : notnull
    {
        CurrentSettings.ScrubMember<T>(name);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMembers(Type declaringType, params string[] names)
    {
        CurrentSettings.IgnoreMembers(declaringType, names);
        return this;
    }

    [Pure]
    public SettingsTask ScrubMembers(Type declaringType, params string[] names)
    {
        CurrentSettings.ScrubMembers(declaringType, names);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMember(Type declaringType, string name)
    {
        CurrentSettings.IgnoreMember(declaringType, name);
        return this;
    }

    [Pure]
    public SettingsTask ScrubMember(Type declaringType, string name)
    {
        CurrentSettings.ScrubMember(declaringType, name);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMember(string name)
    {
        CurrentSettings.IgnoreMember(name);
        return this;
    }

    [Pure]
    public SettingsTask ScrubMember(string name)
    {
        CurrentSettings.ScrubMember(name);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMembers(params string[] names)
    {
        CurrentSettings.IgnoreMembers(names);
        return this;
    }

    [Pure]
    public SettingsTask ScrubMembers(params string[] names)
    {
        CurrentSettings.ScrubMembers(names);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull
    {
        CurrentSettings.IgnoreInstance(shouldIgnore);
        return this;
    }

    [Pure]
    public SettingsTask ScrubInstance<T>(Func<T, bool> shouldScrub)
        where T : notnull
    {
        CurrentSettings.ScrubInstance(shouldScrub);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreInstance(Type type, ShouldIgnore shouldIgnore)
    {
        CurrentSettings.IgnoreInstance(type, shouldIgnore);
        return this;
    }

    [Pure]
    public SettingsTask ScrubInstance(Type type, ShouldScrub shouldScrub)
    {
        CurrentSettings.ScrubInstance(type, shouldScrub);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMembersWithType<T>()
        where T : notnull
    {
        CurrentSettings.IgnoreMembersWithType<T>();
        return this;
    }

    [Pure]
    public SettingsTask ScrubMembersWithType<T>()
        where T : notnull
    {
        CurrentSettings.ScrubMembersWithType<T>();
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMembersWithType(Type type)
    {
        CurrentSettings.IgnoreMembersWithType(type);
        return this;
    }

    [Pure]
    public SettingsTask ScrubMembersWithType(Type type)
    {
        CurrentSettings.ScrubMembersWithType(type);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMembersThatThrow<T>()
        where T : Exception
    {
        CurrentSettings.IgnoreMembersThatThrow<T>();
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMembersThatThrow(Func<Exception, bool> item)
    {
        CurrentSettings.IgnoreMembersThatThrow(item);
        return this;
    }

    [Pure]
    public SettingsTask IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception
    {
        CurrentSettings.IgnoreMembersThatThrow(item);
        return this;
    }
}