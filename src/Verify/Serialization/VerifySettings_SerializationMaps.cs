namespace VerifyTests;

public partial class VerifySettings
{
    public VerifySettings DontScrubGuids()
    {
        CloneSettings();
        serialization.DontScrubGuids();
        return this;
    }

    public VerifySettings DontScrubDateTimes()
    {
        CloneSettings();
        serialization.DontScrubDateTimes();
        return this;
    }

    public VerifySettings DontSortDictionaries()
    {
        CloneSettings();
        serialization.DontSortDictionaries();
        return this;
    }

    public VerifySettings IgnoreStackTrace()
    {
        CloneSettings();
        serialization.IgnoreMember("StackTrace");
        return this;
    }

    public VerifySettings IncludeObsoletes()
    {
        CloneSettings();
        serialization.IncludeObsoletes();
        return this;
    }

    public VerifySettings DontIgnoreEmptyCollections()
    {
        CloneSettings();
        serialization.DontIgnoreEmptyCollections();
        return this;
    }

    public VerifySettings IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMembers(expressions);
        return this;
    }

    public VerifySettings ScrubMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        CloneSettings();
        serialization.ScrubMembers(expressions);
        return this;
    }

    public VerifySettings IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMembers(expression);
        return this;
    }

    public VerifySettings ScrubMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        CloneSettings();
        serialization.ScrubMembers(expression);
        return this;
    }

    public VerifySettings IgnoreMembers<T>(params string[] names)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMembers<T>(names);
        return this;
    }

    public VerifySettings ScrubMembers<T>(params string[] names)
        where T : notnull
    {
        CloneSettings();
        serialization.ScrubMembers<T>(names);
        return this;
    }

    public VerifySettings IgnoreMember<T>(string name)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMember<T>(name);
        return this;
    }

    public VerifySettings ScrubMember<T>(string name)
        where T : notnull
    {
        CloneSettings();
        serialization.ScrubMember<T>(name);
        return this;
    }

    public VerifySettings IgnoreMembers(Type declaringType, params string[] names)
    {
        CloneSettings();
        serialization.IgnoreMembers(declaringType, names);
        return this;
    }

    public VerifySettings ScrubMembers(Type declaringType, params string[] names)
    {
        CloneSettings();
        serialization.ScrubMembers(declaringType, names);
        return this;
    }

    public VerifySettings IgnoreMember(Type declaringType, string name)
    {
        CloneSettings();
        serialization.IgnoreMember(declaringType, name);
        return this;
    }

    public VerifySettings ScrubMember(Type declaringType, string name)
    {
        CloneSettings();
        serialization.ScrubMember(declaringType, name);
        return this;
    }

    public VerifySettings IgnoreMember(string name)
    {
        CloneSettings();
        serialization.IgnoreMember(name);
        return this;
    }

    public VerifySettings ScrubMember(string name)
    {
        CloneSettings();
        serialization.ScrubMember(name);
        return this;
    }

    public VerifySettings IgnoreMembers(params string[] names)
    {
        CloneSettings();
        serialization.IgnoreMembers(names);
        return this;
    }

    public VerifySettings ScrubMembers(params string[] names)
    {
        CloneSettings();
        serialization.ScrubMembers(names);
        return this;
    }

    public VerifySettings IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreInstance(shouldIgnore);
        return this;
    }

    public VerifySettings ScrubInstance<T>(Func<T, bool> shouldScrub)
        where T : notnull
    {
        CloneSettings();
        serialization.ScrubInstance(shouldScrub);
        return this;
    }

    public VerifySettings OrderEnumerableBy<T>(Func<T, object?> keySelector)
    {
        CloneSettings();
        serialization.OrderEnumerableBy(keySelector);
        return this;
    }

    public VerifySettings OrderEnumerableByDescending<T>(Func<T, object?> keySelector)
    {
        CloneSettings();
        serialization.OrderEnumerableByDescending(keySelector);
        return this;
    }

    public VerifySettings IgnoreInstance(Type type, ShouldIgnore shouldIgnore)
    {
        CloneSettings();
        serialization.IgnoreInstance(type, shouldIgnore);
        return this;
    }

    public VerifySettings ScrubInstance(Type type, ShouldScrub shouldScrub)
    {
        CloneSettings();
        serialization.ScrubInstance(type, shouldScrub);
        return this;
    }

    public VerifySettings IgnoreMembersWithType<T>()
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMembersWithType<T>();
        return this;
    }

    public VerifySettings ScrubMembersWithType<T>()
        where T : notnull
    {
        CloneSettings();
        serialization.ScrubMembersWithType<T>();
        return this;
    }

    public VerifySettings AlwaysIncludeMembersWithType<T>()
        where T : notnull
    {
        CloneSettings();
        serialization.AlwaysIncludeMembersWithType<T>();
        return this;
    }

    public VerifySettings IgnoreMembersWithType(Type type)
    {
        CloneSettings();
        serialization.IgnoreMembersWithType(type);
        return this;
    }

    public VerifySettings ScrubMembersWithType(Type type)
    {
        CloneSettings();
        serialization.ScrubMembersWithType(type);
        return this;
    }

    public VerifySettings AlwaysIncludeMembersWithType(Type type)
    {
        CloneSettings();
        serialization.AlwaysIncludeMembersWithType(type);
        return this;
    }

    public VerifySettings IgnoreMembersThatThrow<T>()
        where T : Exception
    {
        CloneSettings();
        serialization.IgnoreMembersThatThrow<T>();
        return this;
    }

    public VerifySettings IgnoreMembersThatThrow(Func<Exception, bool> item)
    {
        CloneSettings();
        serialization.IgnoreMembersThatThrow(item);
        return this;
    }

    public VerifySettings IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception
    {
        CloneSettings();
        serialization.IgnoreMembersThatThrow(item);
        return this;
    }
}