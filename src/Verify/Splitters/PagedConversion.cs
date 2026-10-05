namespace VerifyTests;

/// <summary>
/// Builds the <see cref="ConversionResult" /> of a converter for a paged document: the document
/// as the source, and an image and the text of each page as what was derived from it.
/// </summary>
/// <remarks>
/// What every such converter would otherwise decide for itself is decided here, from the settings
/// of the verification: how the page files are named, where the text goes
/// (<see cref="PageTextPlacement" />), which pages are verified (<c>PagesToInclude</c>), and which
/// derived targets are left out (<c>ExcludeDerivedTargets</c>).
/// <para>
/// A page is the unit whatever the document calls it: a page of a pdf, a slide of a
/// presentation, a frame of an image. Page numbers are 1 based.
/// </para>
/// </remarks>
public sealed class PagedConversion(IReadOnlyDictionary<string, object> context)
{
    Target? source;
    string? text;
    List<Page> pages = [];
    List<Target> derived = [];

    /// <summary>
    /// The extension of the image of a page. <c>png</c> by default.
    /// </summary>
    public string ImageExtension { get; set; } = "png";

    /// <summary>
    /// The extension of the text files written under <see cref="PageTextPlacement.PerPage" />.
    /// <c>txt</c> by default. A converter that reads a document as markdown sets <c>md</c>.
    /// </summary>
    public string TextExtension { get; set; } = "txt";

    /// <summary>
    /// What the converter says about the document as a whole. Written to the info file.
    /// </summary>
    public object? Info { get; set; }

    /// <summary>
    /// The number of pages in the document, counting those that are not verified. Written to the
    /// info file. Set by <see cref="Pages" />, and by <see cref="AddImages(IEnumerable{Stream})" />
    /// when nothing else has set it.
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// Whether the images of pages are verified. False when their extension is excluded, in which
    /// case a converter can skip rendering.
    /// </summary>
    public bool IncludeImages => !context.IsDerivedTargetExcluded(ImageExtension);

    /// <summary>
    /// Whether the text of the document is verified. False under
    /// <see cref="PageTextPlacement.None" />, and under <see cref="PageTextPlacement.PerPage" />
    /// when the extension of the text files is excluded, in which case a converter can skip
    /// reading it.
    /// </summary>
    public bool IncludeText
    {
        get
        {
            var placement = context.PageTextPlacement();
            if (placement == PageTextPlacement.None)
            {
                return false;
            }

            if (placement == PageTextPlacement.PerPage)
            {
                return !context.IsDerivedTargetExcluded(TextExtension);
            }

            return true;
        }
    }

    /// <summary>
    /// Whether the page with the 1 based <paramref name="number" /> is verified.
    /// </summary>
    public bool IsPageIncluded(int number) =>
        context.IsPageIncluded(number);

    /// <summary>
    /// The 1 based numbers of the pages to verify, of a document with
    /// <paramref name="pageCount" /> pages. Records <see cref="PageCount" />.
    /// </summary>
    public IEnumerable<int> Pages(int pageCount)
    {
        // Recorded here rather than in the iterator, which runs nothing until it is enumerated
        PageCount = pageCount;
        return Included(pageCount);
    }

    IEnumerable<int> Included(int pageCount)
    {
        for (var number = 1; number <= pageCount; number++)
        {
            if (IsPageIncluded(number))
            {
                yield return number;
            }
        }
    }

    /// <summary>
    /// The document itself. Leave it out when <c>context.IsTargetExcluded</c> says its extension
    /// is excluded, so that it does not have to be produced.
    /// </summary>
    public void Source(Target document) =>
        source = document;

    /// <summary>
    /// A page of the document.
    /// </summary>
    /// <param name="number">The 1 based number of the page.</param>
    /// <param name="image">The page rendered. The stream is owned by the verification.</param>
    /// <param name="text">The text read from the page.</param>
    /// <param name="info">What the converter says about the page. Written to the info file.</param>
    public void AddPage(int number, Stream? image = null, string? text = null, object? info = null)
    {
        if (number < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(number), number, "Page numbers are 1 based.");
        }

        pages.Add(new(number, image, NullIfEmpty(text), info));
    }

    // A page with no text has none, however the library reading it says so. Kept as empty it
    // would be an empty member in the info file, or a file for the page with nothing in it
    static string? NullIfEmpty(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return text;
    }

    /// <summary>
    /// The image of every page of the document, in page order, from a renderer that produces them
    /// all at once and so cannot skip a page that is not verified. Those are dropped here.
    /// </summary>
    public void AddImages(IEnumerable<Stream> images)
    {
        var number = 0;
        foreach (var image in images)
        {
            number++;
            if (IsPageIncluded(number))
            {
                AddPage(number, image);
            }
            else
            {
                image.Dispose();
            }
        }

        PageCount ??= number;
    }

    /// <inheritdoc cref="AddImages(IEnumerable{Stream})" />
    public void AddImages(IEnumerable<byte[]> images) =>
        AddImages(images.Select(Stream (_) => new MemoryStream(_)));

    /// <summary>
    /// The text of the whole document, from a converter that cannot say which page each part of
    /// it is on.
    /// </summary>
    public void Text(string? text) =>
        this.text = NullIfEmpty(text);

    /// <summary>
    /// Any other target computed from the document: a csv for each sheet, the styles of a
    /// workbook. It has to be named, since the name is what tells it from the other targets.
    /// </summary>
    public void AddDerived(Target target)
    {
        if (target.Name is null)
        {
            throw new ArgumentException("A derived target requires a name.", nameof(target));
        }

        derived.Add(target);
    }

    /// <summary>
    /// The name of the files for the page with the 1 based <paramref name="number" />.
    /// </summary>
    public static string PageName(int number) =>
        $"page_{number:0000}";

    /// <summary>
    /// The result to return from the converter.
    /// </summary>
    /// <param name="cleanup">Run once the verification no longer needs the targets.</param>
    public ConversionResult Build(Func<Task>? cleanup = null)
    {
        var placement = context.PageTextPlacement();
        var includeImages = IncludeImages;
        var includeText = IncludeText;
        var targets = new List<Target>();
        var infos = new List<PagedInfo.Page>();

        if (includeText &&
            placement == PageTextPlacement.PerPage &&
            text is not null)
        {
            targets.Add(new(TextExtension, text, "text"));
        }

        foreach (var page in pages.OrderBy(_ => _.Number))
        {
            var name = PageName(page.Number);
            if (page.Image is { } image)
            {
                if (includeImages)
                {
                    targets.Add(new(ImageExtension, image, name));
                }
                else
                {
                    // Rendered by a converter that did not ask first. The verification owns the
                    // stream of a target it is given, and this one never becomes a target.
                    image.Dispose();
                }
            }

            string? inInfo = null;
            if (includeText &&
                page.Text is not null)
            {
                if (placement == PageTextPlacement.PerPage)
                {
                    targets.Add(new(TextExtension, page.Text, name));
                }
                else
                {
                    inInfo = page.Text;
                }
            }

            if (page.Info is not null ||
                inInfo is not null)
            {
                infos.Add(new(page.Number, page.Info, inInfo));
            }
        }

        targets.AddRange(derived);

        string? textInInfo = null;
        if (includeText &&
            placement == PageTextPlacement.InInfo)
        {
            textInInfo = text;
        }

        return new(
            PagedInfo.Build(Info, PageCount, textInInfo, infos),
            source,
            targets,
            cleanup);
    }

    record Page(int Number, Stream? Image, string? Text, object? Info);
}
