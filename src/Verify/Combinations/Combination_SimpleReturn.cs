namespace VerifyTests;

public partial class Combination
{
    [Pure]
    public SettingsTask Verify<A, TReturn>(
        Func<A, TReturn> method,
        IEnumerable<A> a,
        [CallerArgumentExpression(nameof(a))] string aName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, a);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, TReturn>(
        Func<A, B, TReturn> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        [CallerArgumentExpression(nameof(a))] string aName = "",
        [CallerArgumentExpression(nameof(b))] string bName = "") =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, a, b);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, TReturn>(
        Func<A, B, C, TReturn> method,
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
                var target = CombinationRunner.Run(method, captureExceptions, a, b, c);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, TReturn>(
        Func<A, B, C, D, TReturn> method,
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
                var target = CombinationRunner.Run(method, captureExceptions, a, b, c, d);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, TReturn>(
        Func<A, B, C, D, E, TReturn> method,
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
                var target = CombinationRunner.Run(method, captureExceptions, a, b, c, d, e);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, F, TReturn>(
        Func<A, B, C, D, E, F, TReturn> method,
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
                var target = CombinationRunner.Run(method, captureExceptions, a, b, c, d, e, f);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, F, G, TReturn>(
        Func<A, B, C, D, E, F, G, TReturn> method,
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
                var target = CombinationRunner.Run(method, captureExceptions, a, b, c, d, e, f, g);
                return _.Verify(target);
            });

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, F, G, H, TReturn>(
        Func<A, B, C, D, E, F, G, H, TReturn> method,
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
                var target = CombinationRunner.Run(method, captureExceptions, a, b, c, d, e, f, g, h);
                return _.Verify(target);
            });
}