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
        string? hint = null)
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

        AppendContent(directory, @new, notEquals, inline, hint, builder);

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
                AppendNotEqualContent(directory, builder, notEqual);
                builder.AppendLineN();
            }
        }
    }

    static void AppendNotEqualContent(string directory, StringBuilder builder, NotEqualResult notEqual)
    {
        var item = notEqual.File;
        var message = notEqual.Message;
        var receivedPath = IoHelpers.GetRelativePath(directory, item.ReceivedPath);
        var verifiedPath = IoHelpers.GetRelativePath(directory, item.VerifiedPath);
        if (message is null)
        {
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
}
