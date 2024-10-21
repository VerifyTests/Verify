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