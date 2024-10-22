﻿// ReSharper disable InconsistentNaming
namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyCombinations<A>(
        Func<A, object?> processCall,
        IEnumerable<A> a)
    {
        var target = GetCombinations(
            processCall.DynamicInvoke,
            [a.Cast<object?>()],
            [
                typeof(A)
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B>(
        Func<A, B, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b)
    {
        var target = GetCombinations(
            processCall.DynamicInvoke,
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
        Func<A, B, C, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c)
    {
        var target = GetCombinations(
            processCall.DynamicInvoke,
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
        Func<A, B, C, D, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d)
    {
        var target = GetCombinations(
            processCall.DynamicInvoke,
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
        Func<A, B, C, D, E, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e)
    {
        var target = GetCombinations(
            processCall.DynamicInvoke,
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
        Func<A, B, C, D, E, F, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f)
    {
        var target = GetCombinations(
            processCall.DynamicInvoke,
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
        Func<A, B, C, D, E, F, G, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g)
    {
        var target = GetCombinations(
            processCall.DynamicInvoke,
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
        Func<A, B, C, D, E, F, G, H, object?> processCall,
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
            processCall.DynamicInvoke,
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
        Func<object?[], object?> processCall,
        List<IEnumerable<object?>> lists)
    {
        var target = GetCombinations(processCall, lists, null);
        return Verify(target);
    }

    static CombinationResults GetCombinations(
        Func<object?[], object?> processCall,
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
                    value = processCall(combo);
                }
                catch (TargetInvocationException exception)
                {
                    items.Add(new(keys, exception.InnerException!));
                    return;
                }
                catch (Exception exception)
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