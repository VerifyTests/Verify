// ReSharper disable InconsistentNaming
namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyCombinations<A>(
        Func<A, object?> method,
        bool captureExceptions,
        IEnumerable<A> a)
    {
        var target = GetCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [a.Cast<object?>()],
            [
                typeof(A)
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B>(
        Func<A, B, object?> method,
        bool captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b)
    {
        var target = GetCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B)
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C>(
        Func<A, B, C, object?> method,
        bool captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c)
    {
        var target = GetCombinations(
            method.DynamicInvoke,
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
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D>(
        Func<A, B, C, D, object?> method,
        bool captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d)
    {
        var target = GetCombinations(
            method.DynamicInvoke,
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
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E>(
        Func<A, B, C, D, E, object?> method,
        bool captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e)
    {
        var target = GetCombinations(
            method.DynamicInvoke,
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
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, object?> method,
        bool captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f)
    {
        var target = GetCombinations(
            method.DynamicInvoke,
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
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G, object?> method,
        bool captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g)
    {
        var target = GetCombinations(
            method.DynamicInvoke,
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
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H, object?> method,
        bool captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h)
    {
        var target = GetCombinations(
            method.DynamicInvoke,
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
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations(
        Func<object?[], object?> method,
        bool captureExceptions,
        List<IEnumerable<object?>> lists)
    {
        var target = GetCombinations(method, captureExceptions, lists, null);
        return Verify(target);
    }

    static CombinationResults GetCombinations(
        Func<object?[], object?> method,
        bool captureExceptions,
        List<IEnumerable<object?>> lists,
        Type[]? keyTypes)
    {
        var items = new List<CombinationResult>();
        var listCopy = lists.Select(_ => _.ToList()).ToList();
        var combinationGenerator = new CombinationGenerator(
            listCopy,
            combo =>
            {
                var keys = combo.ToArray();
                object? value;
                try
                {
                    value = method(combo);
                }
                catch (TargetInvocationException exception)
                    when (captureExceptions)
                {
                    items.Add(new(keys, exception.InnerException!));
                    return;
                }
                catch (Exception exception)
                    when (captureExceptions)
                {
                    items.Add(new(keys, exception));
                    return;
                }

                items.Add(new(keys, value));
            });
        combinationGenerator.Run();
        return new(items, keyTypes);
    }
}