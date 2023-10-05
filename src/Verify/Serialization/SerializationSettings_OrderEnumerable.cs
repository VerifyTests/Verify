partial class SerializationSettings
{
    Dictionary<Type, Func<IEnumerable, IEnumerable>> enumerableOrders = new();

    public void OrderEnumerableBy<TSource, TKey>(Func<TSource, TKey> keySelector) =>
        enumerableOrders[typeof(TSource)] = _ => _.Cast<TSource>().OrderBy(keySelector);

    public void OrderEnumerableByDescending<TSource, TKey>(Func<TSource, TKey> keySelector) =>
        enumerableOrders[typeof(TSource)] = _ => _.Cast<TSource>().OrderByDescending(keySelector);

    internal bool TryGetEnumerableOrders(Type memberType, [NotNullWhen(true)] out Func<IEnumerable, IEnumerable>? order) =>
        enumerableOrders.TryGetValue(memberType, out order);
}