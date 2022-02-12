static class VerifyExceptionMessageBuilder
{
    public static async Task<string> Build(
        string directory,
        IReadOnlyList<FilePair> @new,
        List<NotEqual> notEquals,
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
                AppendFile(builder, file);
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

        await AppendContent(@new, notEquals, builder);

        return builder.ToString();
    }

    static void AppendFile(StringBuilder builder, FilePair file)
    {
        builder.AppendLine($"  - Received: {file.ReceivedName}");
        builder.AppendLine($"    Verified: {file.VerifiedName}");
    }

    static async Task AppendContent(IReadOnlyList<FilePair> @new, List<NotEqual> notEquals, StringBuilder builder)
    {
        if (VerifierSettings.omitContentFromException)
        {
            return;
        }

        var newContentFiles = @new.Where(x => x.IsText).ToList();
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
                builder.AppendLine($"Received: {item.ReceivedName}");
                //TODO:
                builder.AppendLine($"{await FileHelpers.ReadText(item.ReceivedPath)}");
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

    static void AppendNotEqualContent(StringBuilder builder, NotEqual notEqual)
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