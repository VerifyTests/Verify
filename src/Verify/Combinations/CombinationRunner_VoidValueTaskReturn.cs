partial class CombinationRunner
{
    Task<CombinationResults> Run(Func<object?[], ValueTask> method) =>
        RunWithVoid(_ => method(_).AsTask());

    public static Task<CombinationResults> Run<A>(
        Func<A, ValueTask> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        ReadOnlySpan<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            header,
            [a.Cast<object?>()],
            [typeof(A)],
            columns);
        return generator.Run(_ => method((A)_[0]!));
    }

    public static Task<CombinationResults> Run<A, B>(
        Func<A, B, ValueTask> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        ReadOnlySpan<string> columns)
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

    public static Task<CombinationResults> Run<A, B, C>(
        Func<A, B, C, ValueTask> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        ReadOnlySpan<string> columns)
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

    public static Task<CombinationResults> Run<A, B, C, D>(
        Func<A, B, C, D, ValueTask> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        ReadOnlySpan<string> columns)
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

    public static Task<CombinationResults> Run<A, B, C, D, E>(
        Func<A, B, C, D, E, ValueTask> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        ReadOnlySpan<string> columns)
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

    public static Task<CombinationResults> Run<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, ValueTask> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        ReadOnlySpan<string> columns)
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

    public static Task<CombinationResults> Run<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G, ValueTask> method,
        bool? captureExceptions,
        bool? header,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        ReadOnlySpan<string> columns)
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

    public static Task<CombinationResults> Run<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H, ValueTask> method,
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
        ReadOnlySpan<string> columns)
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