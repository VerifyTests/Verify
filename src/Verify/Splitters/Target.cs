namespace VerifyTests;

public readonly struct Target
{
    readonly string? stringData;
    readonly StringBuilder? stringBuilderData;
    readonly Stream? streamData;
    public string Extension { get; }
    public string? Name { get; } = null;

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

    public string StringData
    {
        get
        {
            if (stringData is null)
            {
                throw new("Use StreamData or StringBuilderData.");
            }

            return stringData;
        }
    }

    public bool IsString => stringData is not null;

    public StringBuilder StringBuilderData
    {
        get
        {
            if (stringBuilderData is null)
            {
                throw new("Use StreamData or StringData.");
            }

            return stringBuilderData;
        }
    }

    public bool IsStringBuilder => stringBuilderData is not null;

    public Target(string extension, Stream streamData, string? name = null)
    {
        Guard.AgainstBadExtension(extension, nameof(extension));

        if (EmptyFiles.Extensions.IsText(extension))
        {
            throw new("Dont pass a stream for text. Instead use `Target(string extension, string stringData)` or `Target(string extension, StringBuilder stringBuilderData)`.");
        }

        Extension = extension;
        Name = name;
        this.streamData = streamData;
        stringData = null;
        stringBuilderData = null;
    }

    public Target(string extension, StringBuilder stringBuilderData, string? name = null)
    {
        Guard.AgainstBadExtension(extension, nameof(extension));
        if (!EmptyFiles.Extensions.IsText(extension))
        {
            throw new("Dont pass a text for a binary extension. Instead use `Target(string extension, Stream streamData)`.");
        }

        Extension = extension;
        Name = name;
        stringData = null;
        streamData = null;
        this.stringBuilderData = stringBuilderData;
    }

    public Target(string extension, string stringData, string? name = null)
    {
        Guard.AgainstBadExtension(extension, nameof(extension));
        Guard.AgainstBadTargetName(name, nameof(name));
        if (!EmptyFiles.Extensions.IsText(extension))
        {
            throw new("Dont pass a text for a binary extension. Instead use `Target(string extension, Stream streamData)`.");
        }

        Extension = extension;
        Name = name;
        this.stringData = stringData;
        streamData = null;
        stringBuilderData = null;
    }
}