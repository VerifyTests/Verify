﻿static class VerifyExceptionMessageBuilder
{
    public static string Build(
        string directory,
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        IReadOnlyCollection<string> delete,
        IReadOnlyCollection<FilePair> equal)
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

    static void AppendContent(IReadOnlyCollection<NewResult> @new, IReadOnlyCollection<NotEqualResult> notEquals, StringBuilder builder)
    {
        if (VerifierSettings.omitContentFromException)
        {
            return;
        }

        var newContentFiles = @new.Where(_ => _.File.IsText).ToList();
        var notEqualContentFiles = notEquals
            .Where(_ => _.File.IsText || _.Message != null)
            .ToList();
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
            builder.AppendLine(
                $"""
                Received: {item.ReceivedName}
                {notEqual.ReceivedText}
                Verified: {item.VerifiedName}
                {notEqual.VerifiedText}
                """);
        }
        else
        {
            builder.AppendLine(
                $"""
                Received: {item.ReceivedName}
                Verified: {item.VerifiedName}
                Compare Result:
                {message}
                """);
        }
    }
}