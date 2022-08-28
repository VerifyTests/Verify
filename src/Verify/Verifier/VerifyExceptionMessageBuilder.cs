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
                AppendFile(builder, file.File);
            }
        }

        if (notEquals.Any())
        {
            builder.AppendLineN("NotEqual:");
            foreach (var file in notEquals)
            {
                AppendFile(builder, file.File);
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
                AppendFile(builder, file);
            }
        }

        AppendContent(@new, notEquals, builder);

        return builder.ToString();
    }

    static void AppendFile(StringBuilder builder, FilePair file)
    {
        builder.AppendLineN($"  - Received: {file.ReceivedName}");
        builder.AppendLineN($"    Verified: {file.VerifiedName}");
    }

    static void AppendContent(IReadOnlyCollection<NewResult> @new, IReadOnlyCollection<NotEqualResult> notEquals, StringBuilder builder)
    {
        if (VerifierSettings.omitContentFromException)
        {
            return;
        }

        var newContentFiles = @new.Where(_ => _.File.IsText).ToList();
        var notEqualContentFiles = notEquals
            .Where(_ => _.File.IsText || _.Message is not null)
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
                builder.AppendLineN($"Received: {item.File.ReceivedName}");
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
                if (notEqual.File.IsText || notEqual.Message is not null)
                {
                    AppendNotEqualContent(builder, notEqual);
                    builder.AppendLineN();
                }
            }
        }
    }

    static void AppendNotEqualContent(StringBuilder builder, NotEqualResult notEqual)
    {
        var item = notEqual.File;
        var message = notEqual.Message;
        if (message is null)
        {
            builder.AppendLineN(
                $"""
                Received: {item.ReceivedName}
                {notEqual.ReceivedText}
                Verified: {item.VerifiedName}
                {notEqual.VerifiedText}
                """);
        }
        else
        {
            builder.AppendLineN(
                $"""
                Received: {item.ReceivedName}
                Verified: {item.VerifiedName}
                Compare Result:
                {message}
                """);
        }
    }
}