static class VerifyExceptionMessageBuilder
{
    public static string Build(
        string directory,
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        IReadOnlyCollection<string> delete,
        IReadOnlyCollection<FilePair> equal)
    {
        var builder = new StringBuilder($"Directory: {directory}");

        builder.AppendLineN();

        if (@new.Any())
        {
            builder.AppendLineN("New:");
            foreach (var file in @new)
            {
                AppendFile(directory, builder, file.File);
            }
        }

        if (notEquals.Any())
        {
            builder.AppendLineN("NotEqual:");
            foreach (var file in notEquals)
            {
                AppendFile(directory, builder, file.File);
            }
        }

        if (delete.Any())
        {
            builder.AppendLineN("Delete:");
            foreach (var file in delete)
            {
                builder.AppendLineN($"  - {Path.GetFileName(file)}");
            }
        }

        if (equal.Any())
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

        var newContentFiles = @new.Where(_ => _.File.IsText).ToList();
        var notEqualContentFiles = notEquals
            .Where(_ => _.File.IsText ||
                        _.Message is not null)
            .ToList();
        if (newContentFiles.IsEmpty() && notEqualContentFiles.IsEmpty())
        {
            return;
        }

        builder.AppendLineN();
        builder.AppendLineN("FileContent:");
        builder.AppendLineN();

        if (newContentFiles.Any())
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

        if (notEqualContentFiles.Any())
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