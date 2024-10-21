namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyCombinations<A>(
        Func<A, object> processCall,
        IEnumerable<A> a)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            null,
            [a.Cast<object?>()]);
        return Verify(target.ToString());
    }

    static StringBuilder GetCombinationString(
        Func<object?[], object?> processCall,
        Func<object, string>? resultFormatter,
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

                object? result;

                try
                {
                    result = processCall(combo);
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

                if (resultFormatter == null)
                {
                    builder.AppendLineN(result.ToString());
                    return;
                }

                builder.AppendLineN(resultFormatter(result));
            });
        combinationGenerator.Run();
        return builder;
    }
}