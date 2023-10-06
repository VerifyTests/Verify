namespace VerifyTests;

public static partial class VerifierSettings
{
    public static void AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.AddExtraSettings(action);
    }

    public static void AddExtraDateFormat(string format)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SerializationSettings.dateFormats.Add(format);
    }

    public static void AddExtraTimeFormat(string format)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SerializationSettings.timeFormats.Add(format);
    }

    public static void AddExtraDatetimeFormat(string format)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SerializationSettings.datetimeFormats.Add(format);
    }

    public static void AddExtraDatetimeOffsetFormat(string format)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SerializationSettings.datetimeOffsetFormats.Add(format);
    }

    public static void DontScrubGuids()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.DontScrubGuids();
    }

    public static void DontScrubDateTimes()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.DontScrubDateTimes();
    }

    public static void DontSortDictionaries()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.DontSortDictionaries();
    }

    public static void IncludeObsoletes()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IncludeObsoletes();
    }

    public static void DontIgnoreEmptyCollections()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.DontIgnoreEmptyCollections();
    }

    public static void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembers(expressions);
    }

    public static void ScrubMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.ScrubMembers(expressions);
    }

    public static void IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembers(expression);
    }

    public static void ScrubMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.ScrubMember(expression);
    }

    public static void IgnoreMembers<T>(params string[] names)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembers<T>(names);
    }

    public static void ScrubMembers<T>(params string[] names)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembers<T>(names);
    }

    public static void IgnoreMember<T>(string name)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMember<T>(name);
    }

    public static void ScrubMember<T>(string name)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMember<T>(name);
    }

    public static void IgnoreMembers(Type declaringType, params string[] names)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembers(declaringType, names);
    }

    public static void ScrubMembers(Type declaringType, params string[] names)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembers(declaringType, names);
    }

    public static void IgnoreMember(Type declaringType, string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMember(declaringType, name);
    }

    public static void ScrubMember(Type declaringType, string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.ScrubMember(declaringType, name);
    }

    public static void IgnoreStackTrace()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreStackTrace();
    }

    public static void IgnoreMember(string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMember(name);
    }

    public static void ScrubMember(string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.ScrubMember(name);
    }

    public static void IgnoreMembers(params string[] names)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembers(names);
    }

    public static void ScrubMembers(params string[] names)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.ScrubMembers(names);
    }

    public static void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreInstance(shouldIgnore);
    }

    public static void ScrubInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.ScrubInstance(shouldIgnore);
    }

    public static void OrderEnumerableBy<T>(Func<T, object?> keySelector)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.OrderEnumerableBy(keySelector);
    }

    public static void OrderEnumerableByDescending<T>(Func<T, object?> keySelector)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.OrderEnumerableByDescending(keySelector);
    }

    public static void IgnoreInstance(Type type, ShouldIgnore shouldIgnore)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreInstance(type, shouldIgnore);
    }

    public static void ScrubInstance(Type type, ShouldScrub shouldScrub)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.ScrubInstance(type, shouldScrub);
    }

    public static void IgnoreMembersWithType<T>()
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembersWithType<T>();
    }

    public static void ScrubMembersWithType<T>()
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.ScrubMembersWithType<T>();
    }

    public static void IgnoreMembersWithType(Type type)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembersWithType(type);
    }

    public static void ScrubMembersWithType(Type type)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.ScrubMembersWithType(type);
    }

    public static void IgnoreMembersThatThrow<T>()
        where T : Exception
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembersThatThrow<T>();
    }

    public static void IgnoreMembersThatThrow(Func<Exception, bool> item)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembersThatThrow(item);
    }

    public static void IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        serialization.IgnoreMembersThatThrow(item);
    }
}