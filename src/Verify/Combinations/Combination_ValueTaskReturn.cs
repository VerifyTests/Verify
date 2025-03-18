namespace VerifyTests;

public partial class Combination
{
    [Pure]
    public SettingsTask Verify<A, TReturn>(
        Func<A, ValueTask<TReturn>> method,
        IEnumerable<A> a,
        [CallerArgumentExpression(nameof(a))] string? aName = null,
        [CallerArgumentExpression(nameof(a))] string? bName = null,
        [CallerArgumentExpression(nameof(a))] string? cName = null,
        [CallerArgumentExpression(nameof(a))] string? dName = null,
        [CallerArgumentExpression(nameof(a))] string? eName = null,
        [CallerArgumentExpression(nameof(a))] string? fName = null,
        [CallerArgumentExpression(nameof(a))] string? gName = null,
        [CallerArgumentExpression(nameof(a))] string? hName = null) =>
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
        Func<A, B, ValueTask<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        [CallerArgumentExpression(nameof(a))] string? aName = null,
        [CallerArgumentExpression(nameof(a))] string? bName = null,
        [CallerArgumentExpression(nameof(a))] string? cName = null,
        [CallerArgumentExpression(nameof(a))] string? dName = null,
        [CallerArgumentExpression(nameof(a))] string? eName = null,
        [CallerArgumentExpression(nameof(a))] string? fName = null,
        [CallerArgumentExpression(nameof(a))] string? gName = null,
        [CallerArgumentExpression(nameof(a))] string? hName = null) =>
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
        Func<A, B, C, ValueTask<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        [CallerArgumentExpression(nameof(a))] string? aName = null,
        [CallerArgumentExpression(nameof(a))] string? bName = null,
        [CallerArgumentExpression(nameof(a))] string? cName = null,
        [CallerArgumentExpression(nameof(a))] string? dName = null,
        [CallerArgumentExpression(nameof(a))] string? eName = null,
        [CallerArgumentExpression(nameof(a))] string? fName = null,
        [CallerArgumentExpression(nameof(a))] string? gName = null,
        [CallerArgumentExpression(nameof(a))] string? hName = null) =>
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
        Func<A, B, C, D, ValueTask<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        [CallerArgumentExpression(nameof(a))] string? aName = null,
        [CallerArgumentExpression(nameof(a))] string? bName = null,
        [CallerArgumentExpression(nameof(a))] string? cName = null,
        [CallerArgumentExpression(nameof(a))] string? dName = null,
        [CallerArgumentExpression(nameof(a))] string? eName = null,
        [CallerArgumentExpression(nameof(a))] string? fName = null,
        [CallerArgumentExpression(nameof(a))] string? gName = null,
        [CallerArgumentExpression(nameof(a))] string? hName = null) =>
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
        Func<A, B, C, D, E, ValueTask<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        [CallerArgumentExpression(nameof(a))] string? aName = null,
        [CallerArgumentExpression(nameof(a))] string? bName = null,
        [CallerArgumentExpression(nameof(a))] string? cName = null,
        [CallerArgumentExpression(nameof(a))] string? dName = null,
        [CallerArgumentExpression(nameof(a))] string? eName = null,
        [CallerArgumentExpression(nameof(a))] string? fName = null,
        [CallerArgumentExpression(nameof(a))] string? gName = null,
        [CallerArgumentExpression(nameof(a))] string? hName = null) =>
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
        Func<A, B, C, D, E, F, ValueTask<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        [CallerArgumentExpression(nameof(a))] string? aName = null,
        [CallerArgumentExpression(nameof(a))] string? bName = null,
        [CallerArgumentExpression(nameof(a))] string? cName = null,
        [CallerArgumentExpression(nameof(a))] string? dName = null,
        [CallerArgumentExpression(nameof(a))] string? eName = null,
        [CallerArgumentExpression(nameof(a))] string? fName = null,
        [CallerArgumentExpression(nameof(a))] string? gName = null,
        [CallerArgumentExpression(nameof(a))] string? hName = null) =>
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
        Func<A, B, C, D, E, F, G, ValueTask<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        [CallerArgumentExpression(nameof(a))] string? aName = null,
        [CallerArgumentExpression(nameof(a))] string? bName = null,
        [CallerArgumentExpression(nameof(a))] string? cName = null,
        [CallerArgumentExpression(nameof(a))] string? dName = null,
        [CallerArgumentExpression(nameof(a))] string? eName = null,
        [CallerArgumentExpression(nameof(a))] string? fName = null,
        [CallerArgumentExpression(nameof(a))] string? gName = null,
        [CallerArgumentExpression(nameof(a))] string? hName = null) =>
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
        Func<A, B, C, D, E, F, G, H, ValueTask<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        [CallerArgumentExpression(nameof(a))] string? aName = null,
        [CallerArgumentExpression(nameof(a))] string? bName = null,
        [CallerArgumentExpression(nameof(a))] string? cName = null,
        [CallerArgumentExpression(nameof(a))] string? dName = null,
        [CallerArgumentExpression(nameof(a))] string? eName = null,
        [CallerArgumentExpression(nameof(a))] string? fName = null,
        [CallerArgumentExpression(nameof(a))] string? gName = null,
        [CallerArgumentExpression(nameof(a))] string? hName = null) =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, a, b, c, d, e, f, g, h);
                return _.Verify(target);
            });
}