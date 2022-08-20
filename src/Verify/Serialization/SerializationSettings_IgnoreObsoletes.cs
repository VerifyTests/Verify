// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    bool includeObsoletes;

    public void IncludeObsoletes() =>
        includeObsoletes = true;

    bool ShouldIgnoreIfObsolete(MemberInfo member)
    {
        if (includeObsoletes)
        {
            return false;
        }

        return member.GetCustomAttribute<ObsoleteAttribute>(true) is not null;
    }
}