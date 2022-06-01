namespace VerifyTests;

public partial class VerifySettings
{
    public void DontScrubGuids()
    {
        CloneSettings();
        serialization.DontScrubGuids();
    }

    public void DontScrubDateTimes()
    {
        CloneSettings();
        serialization.DontScrubDateTimes();
    }

    public void IgnoreStackTrack()
    {
        CloneSettings();
        serialization.IgnoreMember("StackTrace");
    }

    public void IncludeObsoletes()
    {
        CloneSettings();
        serialization.IncludeObsoletes();
    }

    public void DontIgnoreEmptyCollections()
    {
        CloneSettings();
        serialization.DontIgnoreEmptyCollections();
    }

    public void DontIgnoreFalse()
    {
        CloneSettings();
        serialization.DontIgnoreFalse();
    }

    public void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMembers(expressions);
    }

    public void IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMembers(expression);
    }

    public void IgnoreMembers<T>(params string[] names)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMembers<T>(names);
    }

    public void IgnoreMember<T>(string name)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMember<T>(name);
    }

    public void IgnoreMembers(Type declaringType, params string[] names)
    {
        CloneSettings();
        serialization.IgnoreMembers(declaringType, names);
    }

    public void IgnoreMember(Type declaringType, string name)
    {
        CloneSettings();
        serialization.IgnoreMember(declaringType, name);
    }

    public void IgnoreMember(string name)
    {
        CloneSettings();
        serialization.IgnoreMember(name);
    }

    public void IgnoreMembers(params string[] names)
    {
        CloneSettings();
        serialization.IgnoreMembers(names);
    }

    public void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreInstance(shouldIgnore);
    }

    public void IgnoreInstance(Type type, ShouldIgnore shouldIgnore)
    {
        CloneSettings();
        serialization.IgnoreInstance(type, shouldIgnore);
    }

    public void IgnoreMembersWithType<T>()
        where T : notnull
    {
        CloneSettings();
        serialization.IgnoreMembersWithType<T>();
    }

    public void IgnoreMembersWithType(Type type)
    {
        CloneSettings();
        serialization.IgnoreMembersWithType(type);
    }

    public void IgnoreMembersThatThrow<T>()
        where T : Exception
    {
        CloneSettings();
        serialization.IgnoreMembersThatThrow<T>();
    }

    public void IgnoreMembersThatThrow(Func<Exception, bool> item)
    {
        CloneSettings();
        serialization.IgnoreMembersThatThrow(item);
    }

    public void IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception
    {
        CloneSettings();
        serialization.IgnoreMembersThatThrow(item);
    }
}