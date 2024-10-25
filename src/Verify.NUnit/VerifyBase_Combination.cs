// ReSharper disable InconsistentNaming
namespace VerifyNUnit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask VerifyCombinations<A>(
        Func<A, object?> method,
        IEnumerable<A> a,
        bool? captureExceptions = null,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, captureExceptions, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B>(
        Func<A, B, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        bool? captureExceptions = null,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, captureExceptions, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C>(
        Func<A, B, C, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        bool? captureExceptions = null,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, captureExceptions, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D>(
        Func<A, B, C, D, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        bool? captureExceptions = null,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, captureExceptions, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E>(
        Func<A, B, C, D, E, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        bool? captureExceptions = null,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, captureExceptions, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        bool? captureExceptions = null,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, f, captureExceptions, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        bool? captureExceptions = null,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, f, g, captureExceptions, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        bool? captureExceptions = null,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, f, g, h, captureExceptions, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations(
        Func<object?[], object?> method,
        List<IEnumerable<object?>> lists,
        bool? captureExceptions = null,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, lists, captureExceptions, settings ?? this.settings, sourceFile);
}