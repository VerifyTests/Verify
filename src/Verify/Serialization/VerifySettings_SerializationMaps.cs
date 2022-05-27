namespace VerifyTests;

public partial class VerifySettings
{
    public void DontScrubGuids() =>
        ModifySerialization(_ => _.DontScrubGuids());

    public void DontScrubDateTimes() =>
        ModifySerialization(_ => _.DontScrubDateTimes());

    public void DontScrubNumericIds() =>
        ModifySerialization(_ => _.DontScrubNumericIds());

    public void TreatAsNumericId(IsNumericId isNumericId) =>
        ModifySerialization(_ => _.TreatAsNumericId(isNumericId));

    public void IncludeObsoletes() =>
        ModifySerialization(_ => _.IncludeObsoletes());

    public void DontIgnoreEmptyCollections() =>
        ModifySerialization(_ => _.DontIgnoreEmptyCollections());

    public void DontIgnoreFalse() =>
        ModifySerialization(_ => _.DontIgnoreFalse());

    public void IgnoreMembers<T>(params Expression<Func<T, object?>>[] expressions)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembers(expressions));

    public void IgnoreMember<T>(Expression<Func<T, object?>> expression)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembers(expression));

    public void IgnoreMembers<T>(params string[] names)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembers<T>(names));

    public void IgnoreMember<T>(string name)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMember<T>(name));

    public void IgnoreMembers(Type declaringType, params string[] names) =>
        ModifySerialization(_ => _.IgnoreMembers(names));

    public void IgnoreMember(Type declaringType, string name) =>
        ModifySerialization(_ => _.IgnoreMember(declaringType, name));

    public void IgnoreMember(string name) =>
        ModifySerialization(_ => _.IgnoreMember( name));

    public void IgnoreMembers(params string[] names) =>
        ModifySerialization(_ => _.IgnoreMembers( names));

    public void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreInstance( shouldIgnore));

    public void IgnoreInstance(Type type, Func<object, bool> shouldIgnore) =>
        ModifySerialization(_ => _.IgnoreInstance( shouldIgnore));

    public void IgnoreMembersWithType<T>()
        where T : notnull =>
        ModifySerialization(_ => _.IgnoreMembersWithType<T>());

    public void IgnoreMembersWithType(Type type) =>
        ModifySerialization(_ => _.IgnoreMembersWithType(type));

    public void IgnoreMembersThatThrow<T>()
        where T : Exception =>
        ModifySerialization(_ => _.IgnoreMembersThatThrow<T>());

    public void IgnoreMembersThatThrow(Func<Exception, bool> item) =>
        ModifySerialization(_ => _.IgnoreMembersThatThrow(item));

    public void IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception =>
        ModifySerialization(_ => _.IgnoreMembersThatThrow(item));
}