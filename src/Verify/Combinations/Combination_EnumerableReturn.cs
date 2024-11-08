namespace VerifyTests;

public partial class Combination
{
    [Pure]
    public SettingsTask Verify<A, TReturn>(
        Func<A, IEnumerable<TReturn>> method,
        IEnumerable<A> a) =>
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
        Func<A, B, IEnumerable<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b) =>
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
        Func<A, B, C, IEnumerable<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c) =>
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
        Func<A, B, C, D, IEnumerable<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d) =>
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
        Func<A, B, C, D, E, IEnumerable<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e) =>
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
        Func<A, B, C, D, E, F, IEnumerable<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f) =>
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
        Func<A, B, C, D, E, F, G, IEnumerable<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g) =>
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
        Func<A, B, C, D, E, F, G, H, IEnumerable<TReturn>> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h) =>
        verify(
            settings,
            sourceFile,
            _ =>
            {
                var target = CombinationRunner.Run(method, captureExceptions, a, b, c, d, e, f, g, h);
                return _.Verify(target);
            });
}