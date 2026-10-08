namespace VerifyTests;

// The inline snapshot's contribution to the exception message. Sits alongside the file sections
// rather than replacing them, because only the first target is inlined.
record InlineSection(
    string SourceFile,
    int Line,
    bool IsNew,
    string ReceivedText,
    string? ExpectedText,
    StagedInline? Staged)
{
    public string Header => IsNew ? "InlineNew" : "InlineNotEqual";
}

static class VerifyExceptionMessageBuilder
{
    public static string Build(
        string directory,
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        IReadOnlyCollection<string> delete,
        IReadOnlyCollection<FilePair> equal,
        InlineSection? inline = null,
        string? hint = null) =>
        Build(directory, @new, notEquals, delete, equal, inline, hint, VerifierSettings.textDiffFormat);

    /// <summary>
    /// <paramref name="textDiffFormat"/> is null to show the full received and verified text of a
    /// mismatched text file rather than a diff. Passed rather than read from the settings so tests
    /// can vary it without changing global state.
    /// </summary>
    public static string Build(
        string directory,
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        IReadOnlyCollection<string> delete,
        IReadOnlyCollection<FilePair> equal,
        InlineSection? inline,
        string? hint,
        TextDiffFormat? textDiffFormat)
    {
        var builder = new StringBuilder($"Directory: {directory}\n");

        if (inline is not null)
        {
            builder.AppendLineN($"{inline.Header}:");
            builder.AppendLineN($"  - Source: {inline.SourceFile}:{inline.Line}");
            if (inline.Staged is { } staged)
            {
                builder.AppendLineN($"    Received: {staged.Received}");
                builder.AppendLineN($"    Expected: {staged.Expected}");
                builder.AppendLineN($"    Patch: {staged.Patch}");
            }
        }

        if (@new.Count > 0)
        {
            builder.AppendLineN("New:");
            foreach (var file in @new)
            {
                AppendFile(directory, builder, file.File);
            }
        }

        if (notEquals.Count > 0)
        {
            builder.AppendLineN("NotEqual:");
            foreach (var file in notEquals)
            {
                AppendFile(directory, builder, file.File);
            }
        }

        if (delete.Count > 0)
        {
            builder.AppendLineN("Delete:");
            foreach (var file in delete)
            {
                // directory relative, like the other sections, so the parser can
                // rebuild the path. UseUniqueDirectory and VerifyDirectory put the
                // verified files in a subdirectory, which a file name would drop.
                builder.AppendLineN($"  - {IoHelpers.GetRelativePath(directory, file)}");
            }
        }

        if (equal.Count > 0)
        {
            builder.AppendLineN("Equal:");
            foreach (var file in equal)
            {
                AppendFile(directory, builder, file);
            }
        }

        AppendContent(directory, @new, notEquals, inline, hint, textDiffFormat, builder);

        return builder.ToString();
    }

    static void AppendFile(string directory, StringBuilder builder, FilePair file)
    {
        var receivedPath = IoHelpers.GetRelativePath(directory, file.ReceivedPath);
        var verifiedPath = IoHelpers.GetRelativePath(directory, file.VerifiedPath);
        builder.AppendLineN($"  - Received: {receivedPath}");
        builder.AppendLineN($"    Verified: {verifiedPath}");
    }

    static void AppendContent(
        string directory,
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        InlineSection? inline,
        string? hint,
        TextDiffFormat? textDiffFormat,
        StringBuilder builder)
    {
        var omit = VerifierSettings.omitContentFromException;

        var newContentFiles = omit
            ? []
            : @new
                .Where(_ => _.File.IsText)
                .ToList();
        var notEqualContentFiles = omit
            ? []
            : notEquals
                .Where(_ => _.File.IsText ||
                            _.Message is not null)
                .ToList();
        var inlineContent = omit ? null : inline;

        if (hint is null &&
            inlineContent is null &&
            newContentFiles.Count == 0 &&
            notEqualContentFiles.Count == 0)
        {
            return;
        }

        // Everything below the FileContent: marker is ignored by the exception parser,
        // so the hint must not be emitted before it
        builder.AppendLineN();
        builder.AppendLineN("FileContent:");
        builder.AppendLineN();

        if (hint is not null)
        {
            builder.AppendLineN(hint);
            builder.AppendLineN();
        }

        if (inlineContent is not null)
        {
            builder.AppendLineN($"{inlineContent.Header}:");
            builder.AppendLineN();
            builder.AppendLineN($"Source: {inlineContent.SourceFile}:{inlineContent.Line}");
            builder.AppendLineN("Received:");
            builder.AppendLineN(inlineContent.ReceivedText);
            if (!inlineContent.IsNew)
            {
                builder.AppendLineN("Expected:");
                builder.AppendLineN(inlineContent.ExpectedText);
            }

            if (newContentFiles.Count > 0 ||
                notEqualContentFiles.Count > 0)
            {
                builder.AppendLineN();
            }
        }

        if (newContentFiles.Count > 0)
        {
            builder.AppendLineN("New:");
            builder.AppendLineN();
            foreach (var item in newContentFiles)
            {
                var receivedPath = IoHelpers.GetRelativePath(directory, item.File.ReceivedPath);
                builder.AppendLineN($"Received: {receivedPath}");
                builder.AppendLineN(item.ReceivedText);
                builder.AppendLineN();
            }
        }

        if (notEqualContentFiles.Count > 0)
        {
            builder.AppendLineN("NotEqual:");
            builder.AppendLineN();
            foreach (var notEqual in notEqualContentFiles)
            {
                AppendNotEqualContent(directory, builder, notEqual, textDiffFormat);
                builder.AppendLineN();
            }
        }
    }

    static void AppendNotEqualContent(string directory, StringBuilder builder, NotEqualResult notEqual, TextDiffFormat? textDiffFormat)
    {
        var item = notEqual.File;
        var message = notEqual.Message;
        var receivedPath = IoHelpers.GetRelativePath(directory, item.ReceivedPath);
        var verifiedPath = IoHelpers.GetRelativePath(directory, item.VerifiedPath);
        if (message is null)
        {
            var diff = Diff(notEqual, textDiffFormat);
            if (diff is not null)
            {
                builder.AppendLineN(
                    $"""
                     Received: {receivedPath}
                     Verified: {verifiedPath}
                     Diff:
                     {diff}
                     """);
                return;
            }

            builder.AppendLineN(
                $"""
                 Received: {receivedPath}
                 {notEqual.ReceivedText}
                 Verified: {verifiedPath}
                 {notEqual.VerifiedText}
                 """);
        }
        else
        {
            builder.AppendLineN(
                $"""
                 Received: {receivedPath}
                 Verified: {verifiedPath}
                 Compare Result:
                 {message}
                 """);
        }
    }

    /// <summary>
    /// A line diff of the verified text against the received text, or null to fall back to showing
    /// both in full: when diffs are disabled, when either text is missing, and when the diff comes
    /// back empty because the texts differ only in something a line diff does not see, such as line
    /// endings.
    /// </summary>
    static string? Diff(NotEqualResult notEqual, TextDiffFormat? textDiffFormat)
    {
        if (textDiffFormat is not { } format ||
            notEqual.ReceivedText is not { } received ||
            notEqual.VerifiedText is not { } verified)
        {
            return null;
        }

        var diff = TextDiff.Format(verified, received.ToString(), format);
        if (diff.Length == 0)
        {
            return null;
        }

        return diff;
    }
}
