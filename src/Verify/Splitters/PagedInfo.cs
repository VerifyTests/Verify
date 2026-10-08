namespace VerifyTests;

/// <summary>
/// The info file of a paged document, in the one shape every converter built on
/// <see cref="PagedConversion" /> writes: what the converter says of the document, how many pages
/// it has, its text, and what there is to say of each page.
/// </summary>
/// <remarks>
/// Serialized as any object is, so a member that is null is left out, and the settings of the
/// verification - ignored members, scrubbers - apply to it as they do to anything else.
/// </remarks>
class PagedInfo
{
    public object? Document { get; private init; }
    public int? PageCount { get; private init; }
    public string? Text { get; private init; }
    public IReadOnlyList<Page>? Pages { get; private init; }

    /// <summary>
    /// Null when there is nothing to say, so that no info file is written.
    /// </summary>
    public static PagedInfo? Build(object? document, int? pageCount, string? text, List<Page> pages)
    {
        if (document is null &&
            pageCount is null &&
            text is null &&
            pages.Count == 0)
        {
            return null;
        }

        return new()
        {
            Document = document,
            PageCount = pageCount,
            Text = text,
            Pages = pages.Count == 0 ? null : pages
        };
    }

    public class Page(int number, object? info, string? text)
    {
        public int Number { get; } = number;
        public object? Info { get; } = info;
        public string? Text { get; } = text;
    }
}
