static class VerifyExceptionMessageBuilder
{
    public static string Build(
        string directory,
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        IReadOnlyCollection<string> delete,
        IReadOnlyCollection<FilePair> equal)
    {
        var directoryLength = directory.Length;
        var builder = new StringBuilder($"Directory: {directory}");
        builder.AppendLineN();

        if (@new.Any())
        {
            builder.AppendLineN("New:");
            foreach (var file in @new)
            {
                AppendFile(directoryLength, builder, file.File);
            }
        }

        if (notEquals.Any())
        {
            builder.AppendLineN("NotEqual:");
            foreach (var file in notEquals)
            {
                AppendFile(directoryLength, builder, file.File);
            }
        }

        if (delete.Any())
        {
            builder.AppendLineN("Delete:");
            foreach (var file in delete)
            {
                builder.AppendLineN($"  - {file[directoryLength..]}");
            }
        }

        if (equal.Any())
        {
            builder.AppendLineN("Equal:");
            foreach (var file in equal)
            {
                AppendFile(directoryLength, builder, file);
            }
        }

        AppendContent(directoryLength, @new, notEquals, builder);

        return builder.ToString();
    }

    static void AppendFile(int directoryLength, StringBuilder builder, FilePair file)
    {
        builder.AppendLineN($"  - Received: {file.ReceivedPath[directoryLength..]}");
        builder.AppendLineN($"    Verified: {file.VerifiedPath[directoryLength..]}");
    }

    static void AppendContent(int directoryLength, IReadOnlyCollection<NewResult> @new, IReadOnlyCollection<NotEqualResult> notEquals, StringBuilder builder)
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
        if (newContentFiles.IsEmpty() &&
            notEqualContentFiles.IsEmpty())
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
                builder.AppendLineN($"Received: {item.File.ReceivedPath[directoryLength..]}");
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
                    AppendNotEqualContent(directoryLength, builder, notEqual);
                    builder.AppendLineN();
                }
            }
        }
    }

    static void AppendNotEqualContent(int directoryLength, StringBuilder builder, NotEqualResult notEqual)
    {
        var item = notEqual.File;
        var message = notEqual.Message;
        if (message is null)
        {
            builder.AppendLineN(
                $"""
                Received: {item.ReceivedPath[directoryLength..]}
                {notEqual.ReceivedText}
                Verified: {item.VerifiedPath[directoryLength..]}
                {notEqual.VerifiedText}
                """);
        }
        else
        {
            builder.AppendLineN(
                $"""
                Received: {item.ReceivedPath[directoryLength..]}
                Verified: {item.VerifiedPath[directoryLength..]}
                Compare Result:
                {message}
                """);
        }
    }
}