namespace VerifyTests;

public readonly struct Target
{
    readonly StringBuilder? stringBuilderData;
    readonly Stream? streamData;
    public string Extension { get; }
    public string? Name { get; } = null;
    public bool PerformConversion { get; } = true;

    /// <summary>
    /// When <c>true</c> and this target differs from its verified file, all subsequent targets in the
    /// same verification skip their registered comparers and fall back to exact (binary or string) comparison.
    /// Intended for converters that emit a canonical source target (eg a document) alongside derived targets
    /// (eg rendered images or extracted text) whose comparers may otherwise mask a real difference in the source.
    /// Set this on the source target, and ensure it precedes the derived targets in the conversion result.
    /// </summary>
    public bool BypassComparersForSubsequentOnDifference { get; init; }

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

        Extension = extension;
        Name = FileNameCleaner.SanitizeFilePath(name);
        PerformConversion = performConversion;

        // text is always stored as text, so for a text extension the stream is read here.
        // eg a converter registered against a text extension re-emitting its source stream.
        if (FileExtensions.IsTextExtension(extension))
        {
            streamData = null;
            stringBuilderData = ReadText(data);
            return;
        }

        streamData = data;
        stringBuilderData = null;
    }

    // the stream is owned by the target, so it is consumed here
    static StringBuilder ReadText(Stream stream)
    {
        stream.MoveToStart();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd().ToStringBuilderWithFixedLines();
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