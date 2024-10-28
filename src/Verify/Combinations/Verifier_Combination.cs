// ReSharper disable InconsistentNaming
namespace VerifyXunit;

public class Combination(
    bool? captureExceptions,
    VerifySettings? settings,
    string sourceFile,
    bool useUniqueDirectory,
    Func<VerifySettings?, string, Func<InnerVerifier, Task<VerifyResult>>, bool, SettingsTask> verify)
{
    [Pure]
    public SettingsTask Verify<A>(
        Func<A, object?> method,
        IEnumerable<A> a) =>
        verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(method, captureExceptions, a),
            useUniqueDirectory);

    [Pure]
    public SettingsTask Verify<A, B>(
        Func<A, B, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b) =>
        verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(method, captureExceptions, a, b),
            useUniqueDirectory);

    [Pure]
    public SettingsTask Verify<A, B, C>(
        Func<A, B, C, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c) =>
        verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c),
            useUniqueDirectory);

    [Pure]
    public SettingsTask Verify<A, B, C, D>(
        Func<A, B, C, D, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d) =>
        verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d),
            useUniqueDirectory);

    [Pure]
    public SettingsTask Verify<A, B, C, D, E>(
        Func<A, B, C, D, E, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e) =>
        verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d, e),
            useUniqueDirectory);

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f) =>
        verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d, e, f),
            useUniqueDirectory);

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G, object?> method,
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
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d, e, f, g),
            useUniqueDirectory);

    [Pure]
    public SettingsTask Verify<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H, object?> method,
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
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d, e, f, g, h),
            useUniqueDirectory);

    [Pure]
    public SettingsTask Verify(
        Func<object?[], object?> method,
        List<IEnumerable<object?>> lists) =>
        verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(method, captureExceptions, lists),
            useUniqueDirectory);
}