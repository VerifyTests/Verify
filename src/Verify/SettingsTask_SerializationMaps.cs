namespace VerifyTests;

public partial class SettingsTask
{
    public SettingsTask DontScrubGuids() =>
        ModifySerialization(_ => _.DontScrubGuids());

    public SettingsTask DontScrubDateTimes() =>
        ModifySerialization(_ => _.DontScrubDateTimes());

    public SettingsTask DontScrubNumericIds() =>
        ModifySerialization(_ => _.DontScrubNumericIds());

    public SettingsTask TreatAsNumericId(IsNumericId isNumericId) =>
        ModifySerialization(_ => _.TreatAsNumericId(isNumericId));

    public SettingsTask IncludeObsoletes() =>
        ModifySerialization(_ => _.IncludeObsoletes());

    public SettingsTask DontIgnoreEmptyCollections() =>
        ModifySerialization(_ => _.DontIgnoreEmptyCollections());

    public SettingsTask DontIgnoreFalse() =>
        ModifySerialization(_ => _.DontIgnoreFalse());

    public SettingsTask IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembers(expressions));

    public SettingsTask IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembers(expression));

    public SettingsTask IgnoreMembers<T>(params string[] names)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembers<T>(names));

    public SettingsTask IgnoreMember<T>(string name)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMember<T>(name));

    public SettingsTask IgnoreMembers(Type declaringType, params string[] names) =>
        ModifySerialization(_ => _.IgnoreMembers(names));

    public SettingsTask IgnoreMember(Type declaringType, string name) =>
        ModifySerialization(_ => _.IgnoreMember(declaringType, name));

    public SettingsTask IgnoreMember(string name) =>
        ModifySerialization(_ => _.IgnoreMember( name));

    public SettingsTask IgnoreMembers(params string[] names) =>
        ModifySerialization(_ => _.IgnoreMembers( names));

    public SettingsTask IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreInstance( shouldIgnore));

    public SettingsTask IgnoreInstance(Type type, Func<object, bool> shouldIgnore) =>
        ModifySerialization(_ => _.IgnoreInstance( shouldIgnore));

    public SettingsTask IgnoreMembersWithType<T>()
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembersWithType<T>());

    public SettingsTask IgnoreMembersWithType(Type type) =>
        ModifySerialization(_ => _.IgnoreMembersWithType(type));

    public SettingsTask IgnoreMembersThatThrow<T>()
        where T : Exception =>
        ModifySerialization(_ => _.IgnoreMembersThatThrow<T>());

    public SettingsTask IgnoreMembersThatThrow(Func<Exception, bool> item) =>
        ModifySerialization(_ => _.IgnoreMembersThatThrow(item));

    public SettingsTask IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception =>
        ModifySerialization(_ => _.IgnoreMembersThatThrow(item));
}