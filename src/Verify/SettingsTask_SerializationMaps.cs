namespace VerifyTests;

public partial class SettingsTask
{
    public SettingsTask DontScrubGuids()
    {
        CurrentSettings.DontScrubGuids();
        return this;
    }

    public SettingsTask DontScrubDateTimes()
    {
        CurrentSettings.DontScrubDateTimes();
        return this;
    }

    public SettingsTask IncludeObsoletes()
    {
        CurrentSettings.IncludeObsoletes();
        return this;
    }

    public SettingsTask DontIgnoreEmptyCollections()
    {
        CurrentSettings.DontIgnoreEmptyCollections();
        return this;
    }

    public SettingsTask DontIgnoreFalse()
    {
        CurrentSettings.DontIgnoreFalse();
        return this;
    }

    public SettingsTask IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        CurrentSettings.IgnoreMembers(expressions);
        return this;
    }

    public SettingsTask IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        CurrentSettings.IgnoreMembers(expression);
        return this;
    }

    public SettingsTask IgnoreMembers<T>(params string[] names)
        where T : notnull
    {
        CurrentSettings.IgnoreMembers<T>(names);
        return this;
    }

    public SettingsTask IgnoreMember<T>(string name)
        where T : notnull
    {
        CurrentSettings.IgnoreMember<T>(name);
        return this;
    }

    public SettingsTask IgnoreMembers(Type declaringType, params string[] names)
    {
        CurrentSettings.IgnoreMembers(declaringType, names);
        return this;
    }

    public SettingsTask IgnoreMember(Type declaringType, string name)
    {
        CurrentSettings.IgnoreMember(declaringType, name);
        return this;
    }

    public SettingsTask IgnoreMember(string name)
    {
        CurrentSettings.IgnoreMember(name);
        return this;
    }

    public SettingsTask IgnoreMembers(params string[] names)
    {
        CurrentSettings.IgnoreMembers(names);
        return this;
    }

    public SettingsTask IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull
    {
        CurrentSettings.IgnoreInstance(shouldIgnore);
        return this;
    }

    public SettingsTask IgnoreInstance(Type type, ShouldIgnore shouldIgnore)
    {
        CurrentSettings.IgnoreInstance(type, shouldIgnore);
        return this;
    }

    public SettingsTask IgnoreMembersWithType<T>()
        where T : notnull
    {
        CurrentSettings.IgnoreMembersWithType<T>();
        return this;
    }

    public SettingsTask IgnoreMembersWithType(Type type)
    {
        CurrentSettings.IgnoreMembersWithType(type);
        return this;
    }

    public SettingsTask IgnoreMembersThatThrow<T>()
        where T : Exception
    {
        CurrentSettings.IgnoreMembersThatThrow<T>();
        return this;
    }

    public SettingsTask IgnoreMembersThatThrow(Func<Exception, bool> item)
    {
        CurrentSettings.IgnoreMembersThatThrow(item);
        return this;
    }

    public SettingsTask IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception
    {
        CurrentSettings.IgnoreMembersThatThrow(item);
        return this;
    }
}