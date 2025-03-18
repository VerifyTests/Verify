partial class CombinationRunner
{
    public static Task<CombinationResults> Run<A, TReturn>(
        Func<A, Task<TReturn>> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        List<string> columns)
    {
        var generator = new CombinationRunner(
            captureExceptions,
            [a.Cast<object?>()],
            [typeof(A)],
            columns);
        return generator.RunWithReturn(_ => method((A)_[0]!));
    }

    public static Task<CombinationResults> Run<A, B, TReturn>(
        Func<A, B, Task<TReturn>> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        List<string> columns)
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
            ],
            columns);
        return generator.RunWithReturn(
            _ => method(
                (A)_[0]!,
                (B)_[1]!));
    }

    public static Task<CombinationResults> Run<A, B, C, TReturn>(
        Func<A, B, C, Task<TReturn>> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        List<string> columns)
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
            ],
            columns);
        return generator.RunWithReturn(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!));
    }

    public static Task<CombinationResults> Run<A, B, C, D, TReturn>(
        Func<A, B, C, D, Task<TReturn>> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        List<string> columns)
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
            ],
            columns);
        return generator.RunWithReturn(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!));
    }

    public static Task<CombinationResults> Run<A, B, C, D, E, TReturn>(
        Func<A, B, C, D, E, Task<TReturn>> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        List<string> columns)
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
            ],
            columns);
        return generator.RunWithReturn(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!,
                (E)_[4]!));
    }

    public static Task<CombinationResults> Run<A, B, C, D, E, F, TReturn>(
        Func<A, B, C, D, E, F, Task<TReturn>> method,
        bool? captureExceptions,
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
        return generator.RunWithReturn(
            _ => method(
                (A)_[0]!,
                (B)_[1]!,
                (C)_[2]!,
                (D)_[3]!,
                (E)_[4]!,
                (F)_[5]!));
    }

    public static Task<CombinationResults> Run<A, B, C, D, E, F, G, TReturn>(
        Func<A, B, C, D, E, F, G, Task<TReturn>> method,
        bool? captureExceptions,
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
        return generator.RunWithReturn(
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
        Func<A, B, C, D, E, F, G, H, Task<TReturn>> method,
        bool? captureExceptions,
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
        return generator.RunWithReturn(
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