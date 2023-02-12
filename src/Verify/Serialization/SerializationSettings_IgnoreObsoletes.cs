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

        var attributes = Attribute.GetCustomAttributes(member, typeof(ObsoleteAttribute), true);
        return attributes.Length > 0;
    }
}