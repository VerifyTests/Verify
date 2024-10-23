﻿// ReSharper disable InconsistentNaming
namespace VerifyMSTest;

public partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public SettingsTask VerifyCombinations<A>(
        Func<A, object?> method,
        IEnumerable<A> a,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(method, a, captureExceptions, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B>(
        Func<A, B, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(method, a, b, captureExceptions, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C>(
        Func<A, B, C, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(method, a, b, c, captureExceptions, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D>(
        Func<A, B, C, D, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(method, a, b, c, d, captureExceptions, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E>(
        Func<A, B, C, D, E, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, captureExceptions, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, f, captureExceptions, settings, sourceFile);

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
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, f, g, captureExceptions, settings, sourceFile);

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
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(method, a, b, c, d, e, f, g, h, captureExceptions, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations(
        Func<object?[], object?> method,
        List<IEnumerable<object?>> lists,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(method, lists, captureExceptions, settings, sourceFile);
}