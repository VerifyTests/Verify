namespace VerifyTests;

public partial class Combination
{
    [Pure]
    public SettingsTask Verify<A>(
        Func<A, ValueTask> method,
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
    public SettingsTask Verify<A, B>(
        Func<A, B, ValueTask> method,
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
    public SettingsTask Verify<A, B, C>(
        Func<A, B, C, ValueTask> method,
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
    public SettingsTask Verify<A, B, C, D>(
        Func<A, B, C, D, ValueTask> method,
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
    public SettingsTask Verify<A, B, C, D, E>(
        Func<A, B, C, D, E, ValueTask> method,
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
    public SettingsTask Verify<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, ValueTask> method,
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
    public SettingsTask Verify<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G, ValueTask> method,
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
    public SettingsTask Verify<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H, ValueTask> method,
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