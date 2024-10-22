// ReSharper disable InconsistentNaming
namespace VerifyXunit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask VerifyCombinations<A>(
        Func<A, object?> method,
        IEnumerable<A> a,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B>(
        Func<A, B, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C>(
        Func<A, B, C, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D>(
        Func<A, B, C, D, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E>(
        Func<A, B, C, D, E, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, f, settings ?? this.settings, sourceFile);

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
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, f, g, settings ?? this.settings, sourceFile);

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
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, f, g, h, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations(
        Func<object?[], object?> method,
        List<IEnumerable<object?>> lists,
        VerifySettings? settings = null) =>
        Verifier.VerifyCombinations(method, lists, settings ?? this.settings, sourceFile);
}