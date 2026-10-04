// ReSharper disable UnusedParameter.Local
public class PagedConversionTests
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.RegisterStreamConverter("slides", ConvertSlides);
        VerifierSettings.RegisterStreamConverter("report", ConvertReport);
        VerifierSettings.RegisterStreamConverter("sheets", ConvertSheets);
    }

    #region PagedConversion

    // A document that is a title, then the text of each page, with a | between them
    static ConversionResult ConvertSlides(string? name, Stream stream, IReadOnlyDictionary<string, object> context)
    {
        var bytes = ReadBytes(stream);
        var lines = Encoding.UTF8.GetString(bytes).Split('|');

        var conversion = new PagedConversion(context)
        {
            Info = new
            {
                Title = lines[0]
            }
        };

        // The document itself, unless ExcludeTargets says it is not wanted
        if (!context.IsTargetExcluded("slides"))
        {
            conversion.Source(new("slides", new MemoryStream(bytes)));
        }

        // The pages PagesToInclude asks for, which is all of them by default
        foreach (var number in conversion.Pages(pageCount: lines.Length - 1))
        {
            // Rendering and reading are the expensive parts, so ask before doing either
            Stream? image = null;
            if (conversion.IncludeImages)
            {
                image = Render(number);
            }

            string? text = null;
            if (conversion.IncludeText)
            {
                text = ReadText(lines[number]);
            }

            conversion.AddPage(number, image, text);
        }

        return conversion.Build();
    }

    #endregion

    // A renderer that draws every page at once, and reads the document as one text
    static ConversionResult ConvertReport(string? name, Stream stream, IReadOnlyDictionary<string, object> context)
    {
        var bytes = ReadBytes(stream);
        var conversion = new PagedConversion(context)
        {
            TextExtension = "md"
        };
        conversion.Source(new("report", new MemoryStream(bytes)));
        reportImages =
        [
            Render(1),
            Render(2),
            Render(3)
        ];
        conversion.AddImages(reportImages);
        conversion.Text($"# {Encoding.UTF8.GetString(bytes)}");
        return conversion.Build();
    }

    static ConversionResult ConvertSheets(string? name, Stream stream, IReadOnlyDictionary<string, object> context)
    {
        var conversion = new PagedConversion(context);
        conversion.Source(new("sheets", new MemoryStream(ReadBytes(stream))));
        conversion.AddDerived(new("csv", "a,b", "Sheet1"));
        return conversion.Build();
    }

    static List<MemoryStream> reportImages = [];
    static int rendered;
    static int read;

    static byte[] ReadBytes(Stream stream)
    {
        var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    // A whole png of one pixel, so that what is committed as an image is one
    static MemoryStream Render(int number)
    {
        Interlocked.Increment(ref rendered);
        if (number % 2 == 1)
        {
            return new(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg=="));
        }

        return new(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg=="));
    }

    static string ReadText(string line)
    {
        Interlocked.Increment(ref read);
        return line;
    }

    static MemoryStream Slides() =>
        new("The title|The first page|The second page"u8.ToArray());

    #region PagedConversionVerify

    [Fact]
    public Task Sample() =>
        Verify(Slides(), "slides");

    #endregion

    [Fact]
    public async Task TextInTheInfoByDefault()
    {
        using var temp = new TempDirectory();

        var result = await Verify(Slides(), "slides")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#page_0001.verified.png",
                "Test#page_0002.verified.png",
                "Test.verified.slides",
                "Test.verified.txt"
            ],
            Names(result, nameof(TextInTheInfoByDefault)));
        Assert.Contains("Text: The first page", Info(result));
    }

    [Fact]
    public async Task TextPerPage()
    {
        using var temp = new TempDirectory();

        var result = await Verify(Slides(), "slides")
            .PageText(PageTextPlacement.PerPage)
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#page_0001.verified.png",
                "Test#page_0001.verified.txt",
                "Test#page_0002.verified.png",
                "Test#page_0002.verified.txt",
                "Test.verified.slides",
                "Test.verified.txt"
            ],
            Names(result, nameof(TextPerPage)));
        var info = Info(result);
        Assert.DoesNotContain("Text", info);
        Assert.Contains("PageCount: 2", info);
        Assert.Equal(
            "The second page",
            File.ReadAllText(result.Files.Single(_ => _.EndsWith("#page_0002.verified.txt")), Encoding.UTF8));
    }

    /// <summary>
    /// With no text wanted the converter is told so, and does not read any.
    /// </summary>
    [Fact]
    public async Task NoText()
    {
        using var temp = new TempDirectory();

        read = 0;
        var result = await Verify(Slides(), "slides")
            .PageText(PageTextPlacement.None)
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(0, read);
        Assert.Equal(
            [
                "Test#page_0001.verified.png",
                "Test#page_0002.verified.png",
                "Test.verified.slides",
                "Test.verified.txt"
            ],
            Names(result, nameof(NoText)));
        Assert.DoesNotContain("Text", Info(result));
    }

    /// <summary>
    /// The info still says how many pages the document has, and a page keeps its own number.
    /// </summary>
    [Fact]
    public async Task TheFirstPages()
    {
        using var temp = new TempDirectory();

        rendered = 0;

        var result = await Verify(Slides(), "slides")
            .PagesToInclude(1)
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(1, rendered);
        Assert.Equal(
            [
                "Test#page_0001.verified.png",
                "Test.verified.slides",
                "Test.verified.txt"
            ],
            Names(result, nameof(TheFirstPages)));
        var info = Info(result);
        Assert.Contains("PageCount: 2", info);
        Assert.DoesNotContain("The second page", info);
    }

    [Fact]
    public async Task ThePagesAskedFor()
    {
        using var temp = new TempDirectory();

        var result = await Verify(Slides(), "slides")
            .PagesToInclude(pageNumber => pageNumber == 2)
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#page_0002.verified.png",
                "Test.verified.slides",
                "Test.verified.txt"
            ],
            Names(result, nameof(ThePagesAskedFor)));
        var info = Info(result);
        Assert.Contains("Number: 2", info);
        Assert.DoesNotContain("The first page", info);
    }

    /// <summary>
    /// With the images excluded the converter is told so, and renders nothing.
    /// </summary>
    [Fact]
    public async Task NoImages()
    {
        using var temp = new TempDirectory();

        rendered = 0;

        var result = await Verify(Slides(), "slides")
            .ExcludeDerivedTargets("png")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(0, rendered);
        Assert.Equal(
            [
                "Test.verified.slides",
                "Test.verified.txt"
            ],
            Names(result, nameof(NoImages)));
    }

    [Fact]
    public async Task NoSource()
    {
        using var temp = new TempDirectory();

        var result = await Verify(Slides(), "slides")
            .ExcludeTargets("slides")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#page_0001.verified.png",
                "Test#page_0002.verified.png",
                "Test.verified.txt"
            ],
            Names(result, nameof(NoSource)));
    }

    /// <summary>
    /// A converter that cannot say which page a part of the text is on gives the text whole. It
    /// goes where the text of a page would have: the info, or a file of its own.
    /// </summary>
    [Fact]
    public async Task TextOfTheWholeDocument()
    {
        using var temp = new TempDirectory();

        var inInfo = await Verify(new MemoryStream("The report"u8.ToArray()), "report")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();
        Assert.Equal(
            [
                "Test#page_0001.verified.png",
                "Test#page_0002.verified.png",
                "Test#page_0003.verified.png",
                "Test.verified.report",
                "Test.verified.txt"
            ],
            Names(inInfo, nameof(TextOfTheWholeDocument)));
        var info = Info(inInfo);
        Assert.Contains("Text: # The report", info);
        Assert.Contains("PageCount: 3", info);

        var perPage = await Verify(new MemoryStream("The report"u8.ToArray()), "report")
            .PageText(PageTextPlacement.PerPage)
            .UseDirectory(temp)
            .DisableRequireUniquePrefix()
            .AutoVerify()
            .DisableDiff();
        Assert.Contains(
            "Test#text.verified.md",
            Names(perPage, nameof(TextOfTheWholeDocument)));
        Assert.DoesNotContain("Text", Info(perPage));
    }

    /// <summary>
    /// A renderer that draws every page at once has drawn the ones that are not wanted too. They
    /// are dropped, and their streams closed, since nothing else will.
    /// </summary>
    [Fact]
    public async Task PagesDrawnAllAtOnceAreFilteredAfterwards()
    {
        using var temp = new TempDirectory();

        var result = await Verify(new MemoryStream("The report"u8.ToArray()), "report")
            .PagesToInclude(_ => _ != 2)
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        Assert.Equal(
            [
                "Test#page_0001.verified.png",
                "Test#page_0003.verified.png",
                "Test.verified.report",
                "Test.verified.txt"
            ],
            Names(result, nameof(PagesDrawnAllAtOnceAreFilteredAfterwards)));
        Assert.Contains("PageCount: 3", Info(result));
        Assert.False(reportImages[1].CanRead);
    }

    /// <summary>
    /// Named by the converter, even when it is the only one: a second sheet added to a workbook
    /// then adds a file rather than renaming the first.
    /// </summary>
    [Fact]
    public async Task ADerivedTargetOfAnotherKind()
    {
        using var temp = new TempDirectory();

        var result = await Verify(new MemoryStream("The workbook"u8.ToArray()), "sheets")
            .UseDirectory(temp)
            .AutoVerify()
            .DisableDiff();

        // Nothing to say of the document, so no info file
        Assert.Equal(
            [
                "Test#Sheet1.verified.csv",
                "Test.verified.sheets"
            ],
            Names(result, nameof(ADerivedTargetOfAnotherKind)));
    }

    [Fact]
    public void ADerivedTargetRequiresAName()
    {
        var conversion = new PagedConversion(new VerifySettings().Context);

        Assert.Throws<ArgumentException>(() => conversion.AddDerived(new("csv", "a,b")));
    }

    [Fact]
    public void PageNumbersAreOneBased()
    {
        var conversion = new PagedConversion(new VerifySettings().Context);

        Assert.Throws<ArgumentOutOfRangeException>(() => conversion.AddPage(0, text: "a"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new VerifySettings().PagesToInclude(0));
    }

    [Fact]
    public void PageNames()
    {
        Assert.Equal("page_0001", PagedConversion.PageName(1));
        Assert.Equal("page_0042", PagedConversion.PageName(42));
        Assert.Equal("page_12345", PagedConversion.PageName(12345));
    }

    /// <summary>
    /// Some libraries give an empty string for a page with no text. That is a page with no text:
    /// nothing in the info for it, and no file for it under PerPage.
    /// </summary>
    [Fact]
    public void EmptyTextIsNoText()
    {
        var settings = new VerifySettings();
        settings.PageText(PageTextPlacement.PerPage);
        var perPage = new PagedConversion(settings.Context);
        perPage.AddPage(1, text: "");
        perPage.Text("");
        Assert.Empty(perPage.Build().Targets);

        var inInfo = new PagedConversion(new VerifySettings().Context);
        inInfo.AddPage(1, text: "");
        inInfo.Text("");
        Assert.Null(inInfo.Build().Info);
    }

    /// <summary>
    /// A converter that asks for the pages and enumerates them later, or not at all, has still
    /// said how many the document has.
    /// </summary>
    [Fact]
    public void AskingForThePagesRecordsTheCount()
    {
        var conversion = new PagedConversion(new VerifySettings().Context);

        _ = conversion.Pages(3);

        Assert.Equal(3, conversion.PageCount);
    }

    /// <summary>
    /// Pages are written in page order however the converter came by them.
    /// </summary>
    [Fact]
    public void PagesAreOrderedByNumber()
    {
        var conversion = new PagedConversion(new VerifySettings().Context);
        conversion.AddPage(2, new MemoryStream([2]));
        conversion.AddPage(1, new MemoryStream([1]));

        var result = conversion.Build();

        Assert.Null(result.Source);
        Assert.Equal(
            ["page_0001", "page_0002"],
            result.Targets.Select(_ => _.Name));
    }

    /// <summary>
    /// What the converter is told to skip is read from the settings of the verification it is
    /// converting for.
    /// </summary>
    [Fact]
    public void TheSettingsOfTheVerificationDecideWhatIsIncluded()
    {
        var settings = new VerifySettings();
        var conversion = new PagedConversion(settings.Context);
        Assert.True(conversion.IncludeImages);
        Assert.True(conversion.IncludeText);
        Assert.True(conversion.IsPageIncluded(7));
        Assert.Equal([1, 2, 3], conversion.Pages(3));

        settings.ExcludeDerivedTargets("png");
        settings.PageText(PageTextPlacement.None);
        settings.PagesToInclude(2);
        Assert.False(conversion.IncludeImages);
        Assert.False(conversion.IncludeText);
        Assert.False(conversion.IsPageIncluded(3));
        Assert.Equal([1, 2], conversion.Pages(3));
        Assert.Equal(3, conversion.PageCount);

        // A file of text for each page is a derived target like any other, so it can be excluded
        settings.PageText(PageTextPlacement.PerPage);
        Assert.True(conversion.IncludeText);
        settings.ExcludeDerivedTargets("txt");
        Assert.False(conversion.IncludeText);

        // And an image of some other extension is not what was excluded
        conversion.ImageExtension = "jpg";
        Assert.True(conversion.IncludeImages);
    }

    static List<string> Names(VerifyResult result, string method) =>
        result.Files
            .Select(Path.GetFileName)
            .Select(_ => _!.Replace($"{nameof(PagedConversionTests)}.{method}", "Test"))
            .OrderBy(_ => _, StringComparer.Ordinal)
            .ToList();

    static string Info(VerifyResult result) =>
        File.ReadAllText(
            result.Files.Single(_ => !Path.GetFileName(_).Contains('#') && _.EndsWith(".verified.txt")),
            Encoding.UTF8);
}
