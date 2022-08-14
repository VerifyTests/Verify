// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    List<Type> ignoredTypes = new();

    public void IgnoreMembersWithType<T>()
        where T : notnull =>
        ignoredTypes.Add(typeof(T));

    public void IgnoreMembersWithType(Type type) =>
        ignoredTypes.Add(type);

    bool ShouldIgnoreType(Type memberType)
    {
        if (ignoredTypes.Any(memberType.InheritsFrom))
        {
            return true;
        }

        var typeFromNullable = Nullable.GetUnderlyingType(memberType);

        if (typeFromNullable != null)
        {
            if (ignoredTypes.Any(_ => _.IsAssignableFrom(typeFromNullable)))
            {
                return true;
            }
        }

        return false;
    }
}