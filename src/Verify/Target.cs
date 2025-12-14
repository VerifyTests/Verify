namespace VerifyTests;

public readonly struct Target
{
    public object? Data { get; }
    public string? Extension { get; }
    public string? Name { get; } = null;
    public bool PerformConversion { get; } = true;
    public string NameOrTarget => Name ?? "target";

    public Stream StreamData
    {
        get
        {
            if (Data is not Stream stream)
            {
                throw new("Data is not a Stream. Use Data property to access the underlying data.");
            }

            return stream;
        }
    }

    public bool IsStream => Data is Stream;
    public bool IsStringBuilder => Data is StringBuilder;
    public bool IsString => Data is string;
    public bool IsObject => Data is not null && !IsStream && !IsStringBuilder && !IsString;

    internal bool TryGetStringBuilder([NotNullWhen(true)] out StringBuilder? value)
    {
        if (Data is StringBuilder builder)
        {
            value = builder;
            return true;
        }

        value = null;
        return false;
    }

    internal bool TryGetStream([NotNullWhen(true)] out Stream? value)
    {
        if (Data is Stream stream)
        {
            value = stream;
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
                 If {extension} is not a text extension then use `FileExtensions.RemoveTextExtensions("{extension}")` at initialization;
                 Otherwise use `Target(string extension, string data)` or `Target(string extension, StringBuilder data, string? name)`.
                 """);
        }

        Extension = extension;
        Name = FileNameCleaner.SanitizeFilePath(name);
        PerformConversion = performConversion;
        Data = data;
    }

    public Target(string extension, StringBuilder data, string? name = null)
    {
        ValidateTextExtension(extension);

        Extension = extension;
        Name = FileNameCleaner.SanitizeFilePath(name);
        Data = data;
    }

    static void ValidateTextExtension(string extension)
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
        ValidateTextExtension(extension);

        Extension = extension;
        Name = FileNameCleaner.SanitizeFilePath(name);
        Data = new StringBuilder(data);
    }

    /// <summary>
    /// Creates a Target wrapping an arbitrary object.
    /// The object will be resolved to a stream or string later in the verification pipeline.
    /// </summary>
    public Target(object? data, string? name = null)
    {
        Data = data;
        Extension = null;
        Name = FileNameCleaner.SanitizeFilePath(name);
        PerformConversion = true;
    }
}
