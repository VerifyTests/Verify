partial class SerializationSettings
{
    Dictionary<Type, Func<IEnumerable, IEnumerable>> enumerableInterceptors = [];

    public void OrderEnumerableBy<T>(Func<T, object?> keySelector) =>
        enumerableInterceptors[typeof(T)] = _ => _
            .Cast<T>()
            .OrderBy(keySelector);

    public void OrderEnumerableByDescending<T>(Func<T, object?> keySelector) =>
        enumerableInterceptors[typeof(T)] = _ => _
            .Cast<T>()
            .OrderByDescending(keySelector);

    internal bool TryGetEnumerableInterceptors(Type memberType, [NotNullWhen(true)] out Func<IEnumerable, IEnumerable>? order) =>
        enumerableInterceptors.TryGetValue(memberType, out order);
}