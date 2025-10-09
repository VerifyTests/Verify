namespace VerifyTests;

public readonly struct Target
{
    readonly StringBuilder? stringBuilderData;
    readonly Stream? streamData;
    public string Extension { get; }
    public string? Name { get; } = null;
    public bool PerformConversion { get; } = true;
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

    [OverloadResolutionPriority(-1)]
    public Target(string extension, Stream data, string? name = null) :
        this(extension, data, name, true)
    {
    }

    public Target(string extension, Stream data, string? name = null, bool performConversion = true)
    {
        Guards.AgainstBadExtension(extension);

        if (FileExtensions.IsTextExtension(extension))
        {
            throw new(
                $"""
                 Don't pass a stream for text.
                 If {extension} is not a text extension then use `FileExtensions.RemoveTextExtensions(\"{extension}\")` at initialization;
                 Otherwise use `Target(string extension, string data)` or `Target(string extension, StringBuilder data, string? name)`.
                 """);
        }

        Extension = extension;
        Name = FileNameCleaner.SanitizeFilePath(name);
        PerformConversion = performConversion;
        streamData = data;
        stringBuilderData = null;
    }

    public Target(string extension, StringBuilder data, string? name = null)
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
             Otherwise use `Target(string extension, Stream data, string? name)`.
             """);
    }

    public Target(string extension, string data, string? name = null)
    {
        ValidateExtension(extension);

        Extension = extension;
        Name = FileNameCleaner.SanitizeFilePath(name);
        stringBuilderData = new(data);
        streamData = null;
    }
}