namespace VerifyTests;

public readonly struct ConversionResult
{
    public object? Info { get; }

    /// <summary>
    /// Every target of the conversion. Where a <see cref="Source" /> was named it is the first.
    /// </summary>
    public IEnumerable<Target> Targets { get; }

    /// <summary>
    /// The document the other <see cref="Targets" /> were derived from, where the conversion named
    /// one. See <see cref="ConversionResult(object?, Target?, IEnumerable{Target}, Func{Task}?)" />.
    /// </summary>
    public Target? Source { get; }

    // Set by the constructor that tells a source from what was derived from it, a null source
    // included. What says the conversion follows the naming of that constructor.
    internal bool IsDerivation { get; }

    public Func<Task>? Cleanup { get; }

    public ConversionResult(object? info, IEnumerable<Target> targets, Func<Task>? cleanup = null)
    {
        Info = info;
        Targets = targets;
        Cleanup = cleanup;
    }

    /// <summary>
    /// A conversion of a document into the document itself, the <paramref name="source" />, and
    /// the targets computed from it, the <paramref name="derived" />: a rendered image or the
    /// text of each page, a csv for each sheet.
    /// </summary>
    /// <remarks>
    /// Saying which is which is what lets the rest be done once, here, rather than by each
    /// converter:
    /// <list type="bullet">
    /// <item>
    /// The source is compared first, and when it differs its derived targets skip their
    /// registered comparers and fall back to exact comparison. There is no need for
    /// <see cref="Target.BypassComparersForSubsequentOnDifference" />.
    /// </item>
    /// <item>
    /// The source is never converted again, whatever its extension.
    /// </item>
    /// <item>
    /// Names are relative to the target that was converted. A derived target named
    /// <c>page_0001</c>, from a converted target named <c>Attachment1</c>, is
    /// <c>Attachment1.page_0001</c>, and the source and the info take <c>Attachment1</c>. So a
    /// converter ignores the <c>name</c> it is passed.
    /// </item>
    /// <item>
    /// A diff tool is told that the derived files came from the source. One that shows the
    /// source as a document, with its pages, accepts them together with it.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="info">Written to the info file. Null for none.</param>
    /// <param name="source">
    /// The document. Null where it is not wanted as a target, for example when
    /// <c>context.IsTargetExcluded</c> says its extension is excluded, which leaves the derived
    /// targets standing alone.
    /// </param>
    /// <param name="derived">The targets computed from the document.</param>
    /// <param name="cleanup">Run once the verification no longer needs the targets.</param>
    public ConversionResult(object? info, Target? source, IEnumerable<Target> derived, Func<Task>? cleanup = null)
    {
        Info = info;
        Cleanup = cleanup;
        Source = source;
        IsDerivation = true;
        if (source is { } value)
        {
            Targets = [value, ..derived];
        }
        else
        {
            Targets = derived;
        }
    }

    public ConversionResult(object? info, string extension, Stream stream, Func<Task>? cleanup = null)
    {
        Ensure.NotNullOrEmpty(extension);
        Info = info;
        Cleanup = cleanup;
        Targets = [new(extension, stream)];
    }

    public ConversionResult(object? info, string extension, string data, Func<Task>? cleanup = null)
    {
        Ensure.NotNullOrEmpty(extension);
        Info = info;
        Cleanup = cleanup;
        Targets = [new(extension, data)];
    }

    public ConversionResult(object? info, string extension, StringBuilder data, Func<Task>? cleanup = null)
    {
        Ensure.NotNullOrEmpty(extension);
        Info = info;
        Cleanup = cleanup;
        Targets = [new(extension, data)];
    }
}
