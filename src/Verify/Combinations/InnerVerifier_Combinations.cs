// ReSharper disable InconsistentNaming
namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyCombinations<A>(
        Func<A, string?> processCall,
        IEnumerable<A> a)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [a.Cast<object?>()]);
        return Verify(target.ToString());
    }

    public Task<VerifyResult> VerifyCombinations<A, B>(
        Func<A, B, string?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>()
            ]);
        return Verify(target.ToString());
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C>(
        Func<A, B, C, string?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>()
            ]);
        return Verify(target.ToString());
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D>(
        Func<A, B, C, D, string?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>()
            ]);
        return Verify(target.ToString());
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E>(
        Func<A, B, C, D, E, string?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>()
            ]);
        return Verify(target.ToString());
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, string?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>()
            ]);
        return Verify(target.ToString());
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G, string?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>(),
                g.Cast<object?>()
            ]);
        return Verify(target.ToString());
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H, string?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h)
    {
        var target = GetCombinationString(
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
            ]);
        return Verify(target.ToString());
    }

    public Task<VerifyResult> VerifyCombinations(
        Func<object?[], string?> processCall,
        List<IEnumerable<object?>> lists)
    {
        var target = GetCombinationString(processCall, lists);
        return Verify(target.ToString());
    }

    static StringBuilder GetCombinationString(
        Func<object?[], object?> processCall,
        List<IEnumerable<object?>> lists)
    {
        var builder = new StringBuilder();
        var listCopy = lists.Select(_=>_.ToList()).ToList();
        var combinationGenerator = new CombinationGenerator(
            listCopy,
            combo =>
            {
                builder.Append('[');

                for (var index = 0; index < combo.Length; index++)
                {
                    var item = combo[index];
                    VerifierSettings.AppendParameter(item, builder, true);
                    if (index + 1 != combo.Length)
                    {
                        builder.Append(", ");
                    }
                }

                builder.Append("] => ");

                string? result;

                try
                {
                    result = (string?) processCall(combo);
                }
                catch (Exception exception)
                {
                    builder.AppendLineN($"Exception: {exception.Message}");
                    return;
                }

                if (result == null)
                {
                    builder.AppendLineN("null");
                    return;
                }

                builder.AppendLineN(result);
            });
        combinationGenerator.Run();
        return builder;
    }
}