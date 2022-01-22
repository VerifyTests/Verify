using VerifyTests;

static class VerifyExceptionMessageBuilder
{
    public static async Task<string> Build(
        string directory,
        IReadOnlyList<string> delete,
        IReadOnlyList<(FilePair filePair, string? message)> notEqual,
        IReadOnlyList<FilePair> @new,
        List<FilePair> equal)
    {
        var builder = new StringBuilder($"Directory: {directory}");
        builder.AppendLine();
        if (delete.Any())
        {
            builder.AppendLine("Delete:");
            foreach (var file in delete)
            {
                builder.AppendLine($"  - {Path.GetFileName(file)}");
            }
        }

        if (@new.Any())
        {
            builder.AppendLine("New:");
            foreach (var file in @new)
            {
                AppendFile(builder, file);
            }
        }

        if (notEqual.Any())
        {
            builder.AppendLine("NotEqual:");
            foreach (var file in notEqual)
            {
                AppendFile(builder, file.filePair);
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

        if (!VerifierSettings.omitContentFromException)
        {
            builder.AppendLine("FileContent:");
            await AppendNewContent(builder, @new);
            await AppendNotEqualContent(builder, notEqual);
        }

        return builder.ToString();
    }

    static void AppendFile(StringBuilder builder, FilePair file)
    {
        builder.AppendLine($"  - Received: {file.ReceivedName}");
        builder.AppendLine($"    Verified: {file.VerifiedName}");
    }

    static async Task AppendNotEqualContent(StringBuilder builder, IReadOnlyList<(FilePair filePair, string? message)> notEqual)
    {
        var textFiles = notEqual.Where(x => x.filePair.IsText).ToList();
        if (textFiles.IsEmpty())
        {
            return;
        }

        builder.AppendLine("Differences:");
        foreach (var (filePair, message) in textFiles)
        {
            await AppendNotEqualContent(builder, filePair, message);
        }
    }

    static async Task AppendNewContent(StringBuilder builder, IReadOnlyList<FilePair> @new)
    {
        var textFiles = @new.Where(x => x.IsText).ToList();
        if (textFiles.IsEmpty())
        {
            return;
        }

        builder.AppendLine("NewFiles:");
        foreach (var item in textFiles)
        {
            builder.AppendLine($"Received: {item.ReceivedName}");
            builder.AppendLine($"{await FileHelpers.ReadText(item.ReceivedPath)}");
        }
    }

    static async Task AppendNotEqualContent(StringBuilder builder, FilePair item, string? message)
    {
        if (message is null)
        {
            builder.AppendLine($"Received: {item.ReceivedName}");
            builder.AppendLine($"{await FileHelpers.ReadText(item.ReceivedPath)}");
            builder.AppendLine($"Verified: {item.VerifiedName}");
            builder.AppendLine($"{await FileHelpers.ReadText(item.VerifiedPath)}");
        }
        else
        {
            builder.AppendLine($"Received: {item.ReceivedName}");
            builder.AppendLine($"Verified: {item.VerifiedName}");
            builder.AppendLine($"Compare Result: {message}");
        }
    }
}