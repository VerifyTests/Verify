namespace VerifyTests;

public readonly struct ResolvedTarget
{
    readonly StringBuilder? stringBuilderData;
    readonly Stream? streamData;
    public string Extension { get; }
    public string? Name { get; }

    public string NameOrTarget => Name ?? "target";

    public Stream StreamData
    {
        get
        {
            if (streamData is null)
            {
                throw new("Use StringData or StringBuilderData.");
            }

            return streamData;
        }
    }

    public bool IsStream => streamData is not null;
    public bool IsString => stringBuilderData is not null;

    internal bool TryGetStringBuilder([NotNullWhen(true)] out StringBuilder? value)
    {
        if (stringBuilderData is { } builder)
        {
            value = builder;
            return true;
        }

        value = null;
        return false;
    }

    public ResolvedTarget(string extension, Stream data, string? name = null)
    {
        Guards.AgainstBadExtension(extension);

        if (FileExtensions.IsTextExtension(extension))
        {
            throw new(
                $"""
                 Don't pass a stream for text.
                 If {extension} is not a text extension then use `FileExtensions.RemoveTextExtensions("{extension}")` at initialization;
                 Otherwise use `ResolvedTarget(string extension, string data)` or `ResolvedTarget(string extension, StringBuilder data, string? name)`.
                 """);
        }

        Extension = extension;
        Name = FileNameCleaner.SanitizeFilePath(name);
        streamData = data;
        stringBuilderData = null;
    }

    public ResolvedTarget(string extension, StringBuilder data, string? name = null)
    {
        ValidateExtension(extension);

        Extension = extension;
        Name = FileNameCleaner.SanitizeFilePath(name);
        streamData = null;
        stringBuilderData = data;
    }

    static void ValidateExtension(string extension)
    {
        Guards.AgainstBadExtension(extension);
        if (extension == "noextension" ||
            FileExtensions.IsTextExtension(extension))
        {
            return;
        }

        throw new(
            $"""
             Don't pass text for a binary extension.
             If {extension} is a text extension then use `FileExtensions.AddTextExtension("{extension}")` at initialization;
             Otherwise use `ResolvedTarget(string extension, Stream data, string? name)`.
             """);
    }

    public ResolvedTarget(string extension, string data, string? name = null)
    {
        ValidateExtension(extension);

        Extension = extension;
        Name = FileNameCleaner.SanitizeFilePath(name);
        stringBuilderData = new(data);
        streamData = null;
    }
}
