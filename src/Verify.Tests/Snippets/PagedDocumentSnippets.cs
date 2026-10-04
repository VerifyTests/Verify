#if DEBUG
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
#pragma warning disable IDE0060

public class PagedDocumentSnippets
{
    static Task PageText(Stream pdf) =>

        #region PageTextPerPage

        Verify(pdf, "pdf")
            .PageText(PageTextPlacement.PerPage);

    #endregion

    static Task NoPageText(Stream pdf) =>

        #region PageTextNone

        Verify(pdf, "pdf")
            .PageText(PageTextPlacement.None);

    #endregion

    static Task PagesToIncludeCount(Stream pdf) =>

        #region PagesToIncludeCount

        Verify(pdf, "pdf")
            .PagesToInclude(2);

    #endregion

    static Task PagesToIncludeDelegate(Stream pdf) =>

        #region PagesToIncludeDelegate

        Verify(pdf, "pdf")
            .PagesToInclude(pageNumber => pageNumber is 1 or 5);

    #endregion

    static Task ExcludeDerivedTargets(Stream pdf) =>

        #region ExcludeDerivedTargets

        Verify(pdf, "pdf")
            .ExcludeDerivedTargets("png");

    #endregion

    #region SourceAndDerivedTargets

    // A converter of a workbook: the workbook is the source, and a csv of each sheet is derived
    static ConversionResult ConvertWorkbook(string? name, Stream stream, IReadOnlyDictionary<string, object> context)
    {
        var workbook = Workbook.Load(stream);

        var sheets = new List<Target>();
        foreach (var sheet in workbook.Sheets)
        {
            // Named by what it is. The name of the target being converted is added by Verify
            sheets.Add(new("csv", sheet.ToCsv(), sheet.Name));
        }

        Target? source = null;
        if (!context.IsTargetExcluded("xlsx"))
        {
            source = new("xlsx", workbook.Save());
        }

        return new(
            info: new
            {
                workbook.Author
            },
            source,
            derived: sheets);
    }

    #endregion

    class Workbook
    {
        public static Workbook Load(Stream stream) =>
            new();

        public string Author => "";
        public List<Sheet> Sheets { get; } = [];

        public Stream Save() =>
            new MemoryStream();
    }

    class Sheet
    {
        public string Name => "";

        public string ToCsv() =>
            "";
    }
}

#endif
