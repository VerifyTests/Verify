namespace VerifyTests;

/// <summary>
/// Where the converter of a paged document puts the text it reads from the document.
/// </summary>
public enum PageTextPlacement
{
    /// <summary>
    /// In the info file, with whatever else the converter says about the document. The default.
    /// </summary>
    InInfo,

    /// <summary>
    /// In a text file for each page, named as the image of that page is.
    /// </summary>
    PerPage,

    /// <summary>
    /// Nowhere. The text is not verified, and a converter can skip reading it.
    /// </summary>
    None
}

/// <summary>
/// Whether the page with the 1 based <paramref name="pageNumber" /> is verified.
/// </summary>
public delegate bool IncludePage(int pageNumber);

public static partial class VerifierSettings
{
    internal static PageTextPlacement? pageText;
    internal static IncludePage? includePage;

    /// <summary>
    /// Where converters of paged documents put the text of a document, for all verifications.
    /// </summary>
    public static void PageText(PageTextPlacement placement)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        pageText = placement;
    }

    /// <summary>
    /// Limits what converters of paged documents verify to the first <paramref name="count" />
    /// pages, for all verifications. The document itself is still verified whole.
    /// </summary>
    public static void PagesToInclude(int count)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        includePage = FirstPages(count);
    }

    /// <summary>
    /// Limits what converters of paged documents verify to the pages <paramref name="include" />
    /// accepts, for all verifications. The document itself is still verified whole.
    /// </summary>
    public static void PagesToInclude(IncludePage include)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        includePage = include;
    }

    internal static IncludePage FirstPages(int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "At least one page is required.");
        }

        return _ => _ <= count;
    }

    /// <summary>
    /// Where the text of a paged document goes in the current verification, via either the global
    /// or the per-verification <c>PageText</c>. Read by a converter from its <c>context</c>.
    /// </summary>
    public static PageTextPlacement PageTextPlacement(this IReadOnlyDictionary<string, object> context)
    {
        if (context.TryGetValue(VerifySettings.pageTextKey, out var value))
        {
            return (PageTextPlacement) value;
        }

        return pageText.GetValueOrDefault();
    }

    /// <summary>
    /// Whether the page with the 1 based <paramref name="pageNumber" /> is verified in the current
    /// verification, via either the global or the per-verification <c>PagesToInclude</c>. A
    /// converter can call this on its <c>context</c> to skip rendering a page that would otherwise
    /// be dropped.
    /// </summary>
    public static bool IsPageIncluded(this IReadOnlyDictionary<string, object> context, int pageNumber)
    {
        if (context.TryGetValue(VerifySettings.includePageKey, out var value))
        {
            return ((IncludePage) value)(pageNumber);
        }

        if (includePage is null)
        {
            return true;
        }

        return includePage(pageNumber);
    }
}

public partial class VerifySettings
{
    // In Context, as the excluded targets are, so that a converter, which receives Context, can
    // read them.
    internal const string pageTextKey = "Verify.PageText";
    internal const string includePageKey = "Verify.PagesToInclude";

    /// <summary>
    /// Where converters of paged documents put the text of the document, for the current
    /// verification.
    /// </summary>
    public void PageText(PageTextPlacement placement) =>
        Context[pageTextKey] = placement;

    /// <summary>
    /// Limits what converters of paged documents verify to the first <paramref name="count" />
    /// pages, for the current verification. The document itself is still verified whole.
    /// </summary>
    public void PagesToInclude(int count) =>
        Context[includePageKey] = VerifierSettings.FirstPages(count);

    /// <summary>
    /// Limits what converters of paged documents verify to the pages <paramref name="include" />
    /// accepts, for the current verification. The document itself is still verified whole.
    /// </summary>
    public void PagesToInclude(IncludePage include) =>
        Context[includePageKey] = include;
}

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.PageText" />
    [Pure]
    public SettingsTask PageText(PageTextPlacement placement)
    {
        CurrentSettings.PageText(placement);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.PagesToInclude(int)" />
    [Pure]
    public SettingsTask PagesToInclude(int count)
    {
        CurrentSettings.PagesToInclude(count);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.PagesToInclude(IncludePage)" />
    [Pure]
    public SettingsTask PagesToInclude(IncludePage include)
    {
        CurrentSettings.PagesToInclude(include);
        return this;
    }
}
