static class VerifyExceptionMessageBuilder
{
    public static string Build(
        string directory,
        List<NewResult> @new,
        List<NotEqualResult> notEquals,
        IReadOnlyList<string> delete,
        IReadOnlyList<FilePair> equal)
    {
        var builder = new StringBuilder($"Directory: {directory}");
        builder.AppendLine();

        if (@new.Any())
        {
            builder.AppendLine("New:");
            foreach (var file in @new)
            {
                AppendFile(builder, file.File);
            }
        }

        if (notEquals.Any())
        {
            builder.AppendLine("NotEqual:");
            foreach (var file in notEquals)
            {
                AppendFile(builder, file.File);
            }
        }

        if (delete.Any())
        {
            builder.AppendLine("Delete:");
            foreach (var file in delete)
            {
                builder.AppendLine($"  - {Path.GetFileName(file)}");
            }
        }

        if (equal.Any())
        {
            builder.AppendLine("Equal:");
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
        builder.AppendLine($"  - Received: {file.ReceivedName}");
        builder.AppendLine($"    Verified: {file.VerifiedName}");
    }

    static void AppendContent(IReadOnlyList<NewResult> @new, List<NotEqualResult> notEquals, StringBuilder builder)
    {
        if (VerifierSettings.omitContentFromException)
        {
            return;
        }

        var newContentFiles = @new.Where(x => x.File.IsText).ToList();
        var notEqualContentFiles = notEquals.Where(x => x.File.IsText || x.Message != null).ToList();
        if (newContentFiles.IsEmpty() && notEqualContentFiles.IsEmpty())
        {
            return;
        }

        builder.AppendLine();
        builder.AppendLine("FileContent:");
        builder.AppendLine();

        if (newContentFiles.Any())
        {
            builder.AppendLine("New:");
            builder.AppendLine();
            foreach (var item in newContentFiles)
            {
                builder.AppendLine($"Received: {item.File.ReceivedName}");
                builder.AppendLine(item.ReceivedText);
                builder.AppendLine();
            }
        }

        if (notEqualContentFiles.Any())
        {
            builder.AppendLine("NotEqual:");
            builder.AppendLine();
            foreach (var notEqual in notEqualContentFiles)
            {
                if (notEqual.File.IsText || notEqual.Message != null)
                {
                    AppendNotEqualContent(builder, notEqual);
                    builder.AppendLine();
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
            builder.AppendLine($"Received: {item.ReceivedName}");
            builder.AppendLine(notEqual.ReceivedText);
            builder.AppendLine($"Verified: {item.VerifiedName}");
            builder.AppendLine(notEqual.VerifiedText);
        }
        else
        {
            builder.AppendLine($"Received: {item.ReceivedName}");
            builder.AppendLine($"Verified: {item.VerifiedName}");
            builder.AppendLine($"Compare Result: {message}");
        }
    }
}