namespace VerifyTests;

public static partial class VerifierSettings
{
    public static void AddExtraSettings(Action<JsonSerializerSettings> action) =>
        serialization.AddExtraSettings(action);

    public static void AddExtraDateFormat(string format) =>
        SerializationSettings.dateFormats.Add(format);

    public static void AddExtraDatetimeFormat(string format) =>
        SerializationSettings.datetimeFormats.Add(format);

    public static void AddExtraDatetimeOffsetFormat(string format) =>
        SerializationSettings.datetimeOffsetFormats.Add(format);

    public static void DontScrubGuids() =>
        serialization.DontScrubGuids();

    public static void DontScrubDateTimes() =>
        serialization.DontScrubDateTimes();

    public static void IncludeObsoletes() =>
        serialization.IncludeObsoletes();

    public static void DontIgnoreEmptyCollections() =>
        serialization.DontIgnoreEmptyCollections();

    public static void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull =>
        serialization.IgnoreMembers(expressions);

    public static void IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull =>
        serialization.IgnoreMembers(expression);

    public static void IgnoreMembers<T>(params string[] names)
        where T : notnull =>
        serialization.IgnoreMembers<T>(names);

    public static void IgnoreMember<T>(string name)
        where T : notnull =>
        serialization.IgnoreMember<T>(name);

    public static void IgnoreMembers(Type declaringType, params string[] names) =>
        serialization.IgnoreMembers(declaringType, names);

    public static void IgnoreMember(Type declaringType, string name) =>
        serialization.IgnoreMember(declaringType, name);

    public static void IgnoreStackTrace() =>
        serialization.IgnoreStackTrace();

    public static void IgnoreMember(string name) =>
        serialization.IgnoreMember(name);

    public static void IgnoreMembers(params string[] names) =>
        serialization.IgnoreMembers( names);

    public static void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull =>
        serialization.IgnoreInstance( shouldIgnore);

    public static void IgnoreInstance(Type type, ShouldIgnore shouldIgnore) =>
        serialization.IgnoreInstance(type, shouldIgnore);

    public static void IgnoreMembersWithType<T>()
        where T : notnull =>
        serialization.IgnoreMembersWithType<T>();

    public static void IgnoreMembersWithType(Type type) =>
        serialization.IgnoreMembersWithType(type);

    public static void IgnoreMembersThatThrow<T>()
        where T : Exception =>
        serialization.IgnoreMembersThatThrow<T>();

    public static void IgnoreMembersThatThrow(Func<Exception, bool> item) =>
        serialization.IgnoreMembersThatThrow(item);

    public static void IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception =>
        serialization.IgnoreMembersThatThrow(item);
}