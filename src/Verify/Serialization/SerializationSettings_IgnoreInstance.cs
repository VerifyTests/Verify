partial class SerializationSettings
{
    Dictionary<Type, List<ShouldIgnore>> ignoredInstances = new();

    public void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        where T : notnull
    {
        var type = typeof(T);
        IgnoreInstance(
            type,
            target =>
            {
                var arg = (T) target;
                return shouldIgnore(arg);
            });
    }

    public void IgnoreInstance(Type type, ShouldIgnore shouldIgnore)
    {
        if (!ignoredInstances.TryGetValue(type, out var list))
        {
            ignoredInstances[type] = list = new();
        }

        list.Add(shouldIgnore);
    }

    internal bool GetShouldIgnoreInstance(Type memberType, [NotNullWhen(true)] out List<ShouldIgnore>? funcs) =>
        ignoredInstances.TryGetValue(memberType, out funcs);
}