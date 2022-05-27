namespace VerifyTests;

public static partial class VerifierSettings
{
    public static void DontScrubGuids() =>
        ModifySerialization(_ => _.DontScrubGuids());

    public static void DontScrubDateTimes() =>
        ModifySerialization(_ => _.DontScrubDateTimes());

    public static void DontScrubNumericIds() =>
        ModifySerialization(_ => _.DontScrubNumericIds());

    public static void TreatAsNumericId(IsNumericId isNumericId) =>
        ModifySerialization(_ => _.TreatAsNumericId(isNumericId));

    public static void IncludeObsoletes() =>
        ModifySerialization(_ => _.IncludeObsoletes());

    public static void DontIgnoreEmptyCollections() =>
        ModifySerialization(_ => _.DontIgnoreEmptyCollections());

    public static void DontIgnoreFalse() =>
        ModifySerialization(_ => _.DontIgnoreFalse());

    public static void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembers(expressions));

    public static void IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembers(expression));

    public static void IgnoreMembers<T>(params string[] names)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembers<T>(names));

    public static void IgnoreMember<T>(string name)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMember<T>(name));

    public static void IgnoreMembers(Type declaringType, params string[] names) =>
        ModifySerialization(_ => _.IgnoreMembers(names));

    public static void IgnoreMember(Type declaringType, string name) =>
        ModifySerialization(_ => _.IgnoreMember(declaringType, name));

    public static void IgnoreMember(string name) =>
        ModifySerialization(_ => _.IgnoreMember( name));

    public static void IgnoreMembers(params string[] names) =>
        ModifySerialization(_ => _.IgnoreMembers( names));

    public static void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreInstance( shouldIgnore));

    public static void IgnoreInstance(Type type, Func<object, bool> shouldIgnore) =>
        ModifySerialization(_ => _.IgnoreInstance( shouldIgnore));

    public static void IgnoreMembersWithType<T>()
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembersWithType<T>());

    public static void IgnoreMembersWithType(Type type) =>
        ModifySerialization(_ => _.IgnoreMembersWithType(type));

    public static void IgnoreMembersThatThrow<T>()
        where T : Exception =>
        ModifySerialization(_ => _.IgnoreMembersThatThrow<T>());

    public static void IgnoreMembersThatThrow(Func<Exception, bool> item) =>
        ModifySerialization(_ => _.IgnoreMembersThatThrow(item));

    public static void IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception =>
        ModifySerialization(_ => _.IgnoreMembersThatThrow(item));
}