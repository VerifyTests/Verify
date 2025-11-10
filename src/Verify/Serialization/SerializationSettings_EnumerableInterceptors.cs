partial class SerializationSettings
{
    Dictionary<Type, Func<IEnumerable, IEnumerable>>? enumerableInterceptors;

    public void OrderEnumerableBy<T>(Func<T, object?>? keySelector = null)
    {
        enumerableInterceptors ??= [];
        Func<IEnumerable, IEnumerable> interceptor;
        if (keySelector == null)
        {
            interceptor = _ => _
                .Cast<T>()
                .OrderBy(_=>_);
        }
        else
        {
            interceptor = _ => _
                .Cast<T>()
                .OrderBy(keySelector);
        }

        enumerableInterceptors[typeof(T)] = interceptor;
    }

    public void OrderEnumerableByDescending<T>(Func<T, object?>? keySelector = null)
    {
        enumerableInterceptors ??= [];
        Func<IEnumerable, IEnumerable> interceptor;
        if (keySelector == null)
        {
            interceptor = _ => _
                .Cast<T>()
                .OrderByDescending(_=>_);
        }
        else
        {
            interceptor = _ => _
                .Cast<T>()
                .OrderByDescending(keySelector);
        }

        enumerableInterceptors[typeof(T)] = interceptor;
    }

    internal bool TryGetEnumerableInterceptors(Type memberType, [NotNullWhen(true)] out Func<IEnumerable, IEnumerable>? order)
    {
        if (enumerableInterceptors == null)
        {
            order = null;
            return false;
        }

        return enumerableInterceptors.TryGetValue(memberType, out order);
    }
}