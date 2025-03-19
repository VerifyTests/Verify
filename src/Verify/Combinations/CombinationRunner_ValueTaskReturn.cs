partial class CombinationRunner
{
    Task<CombinationResults> Run<TReturn>(Func<object?[], ValueTask<TReturn>> method) =>
        RunWithReturn(_ => method(_).AsTask());

    public static Task<CombinationResults> Run<A, TReturn>(
        Func<A, ValueTask<TReturn>> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        List<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            header,
            [a.Cast<object?>()],
            [typeof(A)],
            columns);
        return generator.Run(_ => method((A)_[0]!));
    }

    public static Task<CombinationResults> Run<A, B, TReturn>(
        Func<A, B, ValueTask<TReturn>> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        List<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            header,
            [
                a.Cast<object?>(),
                b.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B)
            ],
            columns);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!));
    }

    public static Task<CombinationResults> Run<A, B, C, TReturn>(
        Func<A, B, C, ValueTask<TReturn>> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        List<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            header,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C)
            ],
            columns);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!));
    }

    public static Task<CombinationResults> Run<A, B, C, D, TReturn>(
        Func<A, B, C, D, ValueTask<TReturn>> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        List<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            header,
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
            ],
            columns);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!));
    }

    public static Task<CombinationResults> Run<A, B, C, D, E, TReturn>(
        Func<A, B, C, D, E, ValueTask<TReturn>> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        List<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            header,
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
            ],
            columns);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!,
                (E)_[4]!));
    }

    public static Task<CombinationResults> Run<A, B, C, D, E, F, TReturn>(
        Func<A, B, C, D, E, F, ValueTask<TReturn>> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        List<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            header,
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
            ],
            columns);
        return generator.Run(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!,
                (E)_[4]!,
                (F)_[5]!));
    }

    public static Task<CombinationResults> Run<A, B, C, D, E, F, G, TReturn>(
        Func<A, B, C, D, E, F, G, ValueTask<TReturn>> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        List<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            header,
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
            ],
            columns);
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

    public static Task<CombinationResults> Run<A, B, C, D, E, F, G, H, TReturn>(
        Func<A, B, C, D, E, F, G, H, ValueTask<TReturn>> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        List<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            header,
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
            ],
            columns);
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