namespace VerifyTests;

public partial class Combination
{
    [Pure]
    public SettingsTask Verify<A, TReturn>(
        Func<A, Task<TReturn>> method,
        IEnumerable<A> a,
        [CallerArgumentExpression(nameof(a))] string aName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, header, a, [aName]);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, TReturn>(
        Func<A, B, Task<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        [CallerArgumentExpression(nameof(a))] string aName = "",
        [CallerArgumentExpression(nameof(b))] string bName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, header, a, b, [aName, bName]);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, TReturn>(
        Func<A, B, C, Task<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        [CallerArgumentExpression(nameof(a))] string aName = "",
        [CallerArgumentExpression(nameof(b))] string bName = "",
        [CallerArgumentExpression(nameof(c))] string cName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, header, a, b, c, [aName, bName, cName]);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, TReturn>(
        Func<A, B, C, D, Task<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        [CallerArgumentExpression(nameof(a))] string aName = "",
        [CallerArgumentExpression(nameof(b))] string bName = "",
        [CallerArgumentExpression(nameof(c))] string cName = "",
        [CallerArgumentExpression(nameof(d))] string dName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, header, a, b, c, d, [aName, bName, cName, dName]);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, TReturn>(
        Func<A, B, C, D, E, Task<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        [CallerArgumentExpression(nameof(a))] string aName = "",
        [CallerArgumentExpression(nameof(b))] string bName = "",
        [CallerArgumentExpression(nameof(c))] string cName = "",
        [CallerArgumentExpression(nameof(d))] string dName = "",
        [CallerArgumentExpression(nameof(e))] string eName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, header, a, b, c, d, e, [aName, bName, cName, dName, eName]);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, F, TReturn>(
        Func<A, B, C, D, E, F, Task<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        [CallerArgumentExpression(nameof(a))] string aName = "",
        [CallerArgumentExpression(nameof(b))] string bName = "",
        [CallerArgumentExpression(nameof(c))] string cName = "",
        [CallerArgumentExpression(nameof(d))] string dName = "",
        [CallerArgumentExpression(nameof(e))] string eName = "",
        [CallerArgumentExpression(nameof(f))] string fName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, header, a, b, c, d, e, f, [aName, bName, cName, dName, eName, fName]);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, F, G, TReturn>(
        Func<A, B, C, D, E, F, G, Task<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        [CallerArgumentExpression(nameof(a))] string aName = "",
        [CallerArgumentExpression(nameof(b))] string bName = "",
        [CallerArgumentExpression(nameof(c))] string cName = "",
        [CallerArgumentExpression(nameof(d))] string dName = "",
        [CallerArgumentExpression(nameof(e))] string eName = "",
        [CallerArgumentExpression(nameof(f))] string fName = "",
        [CallerArgumentExpression(nameof(g))] string gName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, header, a, b, c, d, e, f, g, [aName, bName, cName, dName, eName, fName, gName]);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, F, G, H, TReturn>(
        Func<A, B, C, D, E, F, G, H, Task<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        [CallerArgumentExpression(nameof(a))] string aName = "",
        [CallerArgumentExpression(nameof(b))] string bName = "",
        [CallerArgumentExpression(nameof(c))] string cName = "",
        [CallerArgumentExpression(nameof(d))] string dName = "",
        [CallerArgumentExpression(nameof(e))] string eName = "",
        [CallerArgumentExpression(nameof(f))] string fName = "",
        [CallerArgumentExpression(nameof(g))] string gName = "",
        [CallerArgumentExpression(nameof(h))] string hName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, header, a, b, c, d, e, f, g, h, [aName, bName, cName, dName, eName, fName, gName, hName]);
                return _.Verify(target);
            });
}