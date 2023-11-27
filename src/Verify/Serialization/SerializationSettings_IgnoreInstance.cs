partial class SerializationSettings
{
    Dictionary<Type, List<Func<object, ScrubOrIgnore?>>> ignoredInstances = [];

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
            ignoredInstances[type] = list = [];
        }

        list.Add(_ =>
        {
            if (shouldIgnore(_))
            {
                return ScrubOrIgnore.Ignore;
            }

            return null;
        });
    }

    public void ScrubInstance<T>(Func<T, bool> shouldScrub)
        where T : notnull
    {
        var type = typeof(T);
        ScrubInstance(
            type,
            target =>
            {
                var arg = (T) target;
                return shouldScrub(arg);
            });
    }

    public void ScrubInstance(Type type, ShouldScrub shouldScrub)
    {
        if (!ignoredInstances.TryGetValue(type, out var list))
        {
            ignoredInstances[type] = list = [];
        }

        list.Add(_ =>
        {
            if (shouldScrub(_))
            {
                return ScrubOrIgnore.Scrub;
            }

            return null;
        });
    }

    internal bool GetShouldIgnoreInstance(Type memberType, [NotNullWhen(true)] out List<Func<object, ScrubOrIgnore?>>? funcs) =>
        ignoredInstances.TryGetValue(memberType, out funcs);
}