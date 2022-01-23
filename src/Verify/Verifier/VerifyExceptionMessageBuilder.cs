using VerifyTests;

static class VerifyExceptionMessageBuilder
{
    public static async Task<string> Build(
        string directory,
        IReadOnlyList<FilePair> @new,
        IReadOnlyList<(FilePair filePair, string? message)> notEqual,
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

        if (notEqual.Any())
        {
            builder.AppendLine("NotEqual:");
            foreach (var file in notEqual)
            {
                AppendFile(builder, file.filePair);
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

        await AppendContent(@new, notEqual, builder);

        return builder.ToString();
    }

    static void AppendFile(StringBuilder builder, FilePair file)
    {
        builder.AppendLine($"  - Received: {file.ReceivedName}");
        builder.AppendLine($"    Verified: {file.VerifiedName}");
    }

    static async Task AppendContent(IReadOnlyList<FilePair> @new, IReadOnlyList<(FilePair filePair, string? message)> notEqual, StringBuilder builder)
    {
        if (VerifierSettings.omitContentFromException)
        {
            return;
        }

        var newTextFiles = @new.Where(x => x.IsText).ToList();
        var notEqualTextFiles = notEqual.Where(x => x.filePair.IsText).ToList();
        if (newTextFiles.IsEmpty() && notEqualTextFiles.IsEmpty())
        {
            return;
        }

        builder.AppendLine("FileContent:");
        builder.AppendLine();

        if (newTextFiles.Any())
        {
            builder.AppendLine("New:");
            builder.AppendLine();
            foreach (var item in newTextFiles)
            {
                builder.AppendLine($"Received: {item.ReceivedName}");
                builder.AppendLine($"{await FileHelpers.ReadText(item.ReceivedPath)}");
                builder.AppendLine();
            }
        }

        if (notEqualTextFiles.Any())
        {
            builder.AppendLine("NotEqual:");
            builder.AppendLine();
            foreach (var (filePair, message) in notEqualTextFiles)
            {
                await AppendNotEqualContent(builder, filePair, message);
                builder.AppendLine();
            }
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