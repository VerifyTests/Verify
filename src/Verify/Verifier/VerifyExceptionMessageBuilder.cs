static class VerifyExceptionMessageBuilder
{
    public static string Build(
        string directory,
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        IReadOnlyCollection<string> delete,
        IReadOnlyCollection<FilePair> equal)
    {
        var builder = new StringBuilder($"Directory: {directory}\n");

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
                builder.AppendLineN($"  - {Path.GetFileName(file)}");
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

        AppendContent(directory, @new, notEquals, builder);

        return builder.ToString();
    }

    public static string BuildInline(
        string directory,
        string sourceFile,
        int line,
        bool isNew,
        string receivedText,
        string? expectedText,
        string? stagedReceived,
        string? stagedExpected,
        string? stagedPatch,
        IReadOnlyCollection<string> delete,
        string? hint = null)
    {
        var section = isNew ? "InlineNew" : "InlineNotEqual";
        var builder = new StringBuilder($"Directory: {directory}\n");
        builder.AppendLineN($"{section}:");
        builder.AppendLineN($"  - Source: {sourceFile}:{line}");
        if (stagedReceived is not null)
        {
            builder.AppendLineN($"    Received: {stagedReceived}");
        }

        if (stagedExpected is not null)
        {
            builder.AppendLineN($"    Expected: {stagedExpected}");
        }

        if (stagedPatch is not null)
        {
            builder.AppendLineN($"    Patch: {stagedPatch}");
        }

        if (delete.Count > 0)
        {
            builder.AppendLineN("Delete:");
            foreach (var file in delete)
            {
                builder.AppendLineN($"  - {Path.GetFileName(file)}");
            }
        }

        // Everything below the FileContent: marker is ignored by the exception parser,
        // so the hint must not be emitted before it
        var appendContent = !VerifierSettings.omitContentFromException;
        if (appendContent || hint is not null)
        {
            builder.AppendLineN();
            builder.AppendLineN("FileContent:");
            builder.AppendLineN();
        }

        if (hint is not null)
        {
            builder.AppendLineN(hint);
            builder.AppendLineN();
        }

        if (appendContent)
        {
            builder.AppendLineN($"{section}:");
            builder.AppendLineN();
            builder.AppendLineN($"Source: {sourceFile}:{line}");
            builder.AppendLineN("Received:");
            builder.AppendLineN(receivedText);
            if (!isNew)
            {
                builder.AppendLineN("Expected:");
                builder.AppendLineN(expectedText);
            }
        }

        return builder.ToString();
    }

    static void AppendFile(string directory, StringBuilder builder, FilePair file)
    {
        var receivedPath = IoHelpers.GetRelativePath(directory, file.ReceivedPath);
        var verifiedPath = IoHelpers.GetRelativePath(directory, file.VerifiedPath);
        builder.AppendLineN($"  - Received: {receivedPath}");
        builder.AppendLineN($"    Verified: {verifiedPath}");
    }

    static void AppendContent(string directory, IReadOnlyCollection<NewResult> @new, IReadOnlyCollection<NotEqualResult> notEquals, StringBuilder builder)
    {
        if (VerifierSettings.omitContentFromException)
        {
            return;
        }

        if (@new.Count == 0 &&
            notEquals.Count == 0)
        {
            return;
        }

        var newContentFiles = @new
            .Where(_ => _.File.IsText)
            .ToList();
        var notEqualContentFiles = notEquals
            .Where(_ => _.File.IsText ||
                        _.Message is not null)
            .ToList();

        if (newContentFiles.Count == 0 &&
            notEqualContentFiles.Count == 0)
        {
            return;
        }

        builder.AppendLineN();
        builder.AppendLineN("FileContent:");
        builder.AppendLineN();

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
                if (notEqual.File.IsText ||
                    notEqual.Message is not null)
                {
                    AppendNotEqualContent(directory, builder, notEqual);
                    builder.AppendLineN();
                }
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