static class KeyTypes
{
    public static (List<IEnumerable<object?>> lists, Type[] keyTypes) Build<A>(
        IEnumerable<A> a)
    {
        List<IEnumerable<object?>> lists =
            [a.Cast<object?>()];
        Type[] keyTypes =
            [typeof(A)];
        return (lists, keyTypes);
    }

    public static (List<IEnumerable<object?>> lists, Type[] keyTypes) Build<A, B>(
        IEnumerable<A> a,
        IEnumerable<B> b)
    {
        List<IEnumerable<object?>> lists =
        [
            a.Cast<object?>(),
            b.Cast<object?>(),
        ];
        Type[] keyTypes =
        [
            typeof(A),
            typeof(B),
        ];
        return (lists, keyTypes);
    }

    public static (List<IEnumerable<object?>> lists, Type[] keyTypes) Build<A, B, C>(
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c)
    {
        List<IEnumerable<object?>> lists =
        [
            a.Cast<object?>(),
            b.Cast<object?>(),
            c.Cast<object?>(),
        ];
        Type[] keyTypes =
        [
            typeof(A),
            typeof(B),
            typeof(C),
        ];
        return (lists, keyTypes);
    }

    public static (List<IEnumerable<object?>> lists, Type[] keyTypes) Build<A, B, C, D>(
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d)
    {
        List<IEnumerable<object?>> lists =
        [
            a.Cast<object?>(),
            b.Cast<object?>(),
            c.Cast<object?>(),
            d.Cast<object?>(),
        ];
        Type[] keyTypes =
        [
            typeof(A),
            typeof(B),
            typeof(C),
            typeof(D),
        ];
        return (lists, keyTypes);
    }

    public static (List<IEnumerable<object?>> lists, Type[] keyTypes) Build<A, B, C, D, E>(
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e)
    {
        List<IEnumerable<object?>> lists =
        [
            a.Cast<object?>(),
            b.Cast<object?>(),
            c.Cast<object?>(),
            d.Cast<object?>(),
            e.Cast<object?>(),
        ];
        Type[] keyTypes =
        [
            typeof(A),
            typeof(B),
            typeof(C),
            typeof(D),
            typeof(E),
        ];
        return (lists, keyTypes);
    }

    public static (List<IEnumerable<object?>> lists, Type[] keyTypes) Build<A, B, C, D, E, F>(
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f)
    {
        List<IEnumerable<object?>> lists =
        [
            a.Cast<object?>(),
            b.Cast<object?>(),
            c.Cast<object?>(),
            d.Cast<object?>(),
            e.Cast<object?>(),
            f.Cast<object?>(),
        ];
        Type[] keyTypes =
        [
            typeof(A),
            typeof(B),
            typeof(C),
            typeof(D),
            typeof(E),
            typeof(F),
        ];
        return (lists, keyTypes);
    }

    public static (List<IEnumerable<object?>> lists, Type[] keyTypes) Build<A, B, C, D, E, F, G>(
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g)
    {
        List<IEnumerable<object?>> lists =
        [
            a.Cast<object?>(),
            b.Cast<object?>(),
            c.Cast<object?>(),
            d.Cast<object?>(),
            e.Cast<object?>(),
            f.Cast<object?>(),
            g.Cast<object?>(),
        ];
        Type[] keyTypes =
        [
            typeof(A),
            typeof(B),
            typeof(C),
            typeof(D),
            typeof(E),
            typeof(F),
            typeof(G),
        ];
        return (lists, keyTypes);
    }

    public static (List<IEnumerable<object?>> lists, Type[] keyTypes) Build<A, B, C, D, E, F, G, H>(
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h)
    {
        List<IEnumerable<object?>> lists =
        [
            a.Cast<object?>(),
            b.Cast<object?>(),
            c.Cast<object?>(),
            d.Cast<object?>(),
            e.Cast<object?>(),
            f.Cast<object?>(),
            g.Cast<object?>(),
            h.Cast<object?>()
        ];
        Type[] keyTypes =
        [
            typeof(A),
            typeof(B),
            typeof(C),
            typeof(D),
            typeof(E),
            typeof(F),
            typeof(G),
            typeof(H)
        ];
        return (lists, keyTypes);
    }

    public static Type[] Build(List<IEnumerable<object?>> lists)
    {
        var types = new Type[lists.Count];
        for (var index = 0; index < lists.Count; index++)
        {
            var keys = lists[index];
            Type? type = null;
            foreach (var key in keys)
            {
                if (key == null)
                {
                    continue;
                }

                var current = key.GetType();
                if (type == null)
                {
                    type = current;
                    continue;
                }

                if (type != current)
                {
                    type = null;
                    break;
                }

                type = current;
            }

            types[index] = type ?? typeof(object);
        }

        return types;
    }
}