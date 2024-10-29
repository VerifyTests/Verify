partial class CombinationRunner
{
    CombinationResults Run<TReturn>(Func<object?[], TReturn> method)
    {
        var items = new List<CombinationResult>();
        while (true)
        {
            var keys = BuildParameters();
            try
            {
                var value = method(keys);
                items.Add(new(keys, value));
            }
            catch (Exception exception)
                when (captureExceptions)
            {
                items.Add(new(keys, exception));
            }

            if (Increment())
            {
                break;
            }
        }

        return new(items, keyTypes);
    }

    public static CombinationResults Run<A, TReturn>(
        Func<A, TReturn> method,
        bool? captureExceptions,
        IEnumerable<A> a)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            [a.Cast<object?>()],
            [typeof(A)]);
        return generator.Run(_ => method((A)_[0]!));
    }

    public static CombinationResults Run<A, B, TReturn>(
        Func<A, B, TReturn> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B)
            ]);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!));
    }

    public static CombinationResults Run<A, B, C, TReturn>(
        Func<A, B, C, TReturn> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C)
            ]);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!));
    }

    public static CombinationResults Run<A, B, C, D, TReturn>(
        Func<A, B, C, D, TReturn> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D)
            ]);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!));
    }

    public static CombinationResults Run<A, B, C, D, E, TReturn>(
        Func<A, B, C, D, E, TReturn> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D),
                typeof(E)
            ]);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!,
                (E)_[4]!));
    }

    public static CombinationResults Run<A, B, C, D, E, F, TReturn>(
        Func<A, B, C, D, E, F, TReturn> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D),
                typeof(E),
                typeof(F)
            ]);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!,
                (E)_[4]!,
                (F)_[5]!));
    }

    public static CombinationResults Run<A, B, C, D, E, F, G, TReturn>(
        Func<A, B, C, D, E, F, G, TReturn> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>(),
                g.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D),
                typeof(E),
                typeof(F),
                typeof(G)
            ]);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!,
                (E)_[4]!,
                (F)_[5]!,
                (G)_[6]!));
    }

    public static CombinationResults Run<A, B, C, D, E, F, G, H, TReturn>(
        Func<A, B, C, D, E, F, G, H, TReturn> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>(),
                g.Cast<object?>(),
                h.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D),
                typeof(E),
                typeof(F),
                typeof(G),
                typeof(H)
            ]);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!,
                (E)_[4]!,
                (F)_[5]!,
                (G)_[6]!,
                (H)_[7]!));
    }
}