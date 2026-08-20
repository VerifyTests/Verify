#if DEBUG

public class BypassComparerSnippets
{
    #region BypassComparersForSubsequentOnDifference

    // A converter that emits a canonical source document alongside derived targets (eg rendered pages).
    // The source is flagged so that, when it differs, the derived targets skip their (potentially lenient)
    // comparers and fall back to exact comparison, ensuring a real change in the source is never masked.
    public static ConversionResult ConvertDocument(Stream document, IReadOnlyDictionary<string, object> context)
    {
        Target[] targets =
        [
            new("docx", document)
            {
                BypassComparersForSubsequentOnDifference = true
            },
            new("png", RenderPage(document))
        ];
        return new(info: null, targets);
    }

    #endregion

    // ReSharper disable once UnusedParameter.Local
#pragma warning disable IDE0060
    static Stream RenderPage(Stream document) =>
#pragma warning restore IDE0060
        new MemoryStream();
}

#endif
