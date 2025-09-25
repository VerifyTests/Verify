namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<FileAppender>? fileAppenders = [];

    internal static IEnumerable<Target> GetFileAppenders(VerifySettings settings)
    {
        if (fileAppenders != null)
        {
            foreach (var appender in fileAppenders)
            {
                var target = appender(settings.Context);
                if (target.HasValue)
                {
                    yield return target.Value;
                }
            }
        }

        if (settings.appendedFiles != null)
        {
            foreach (var target in settings.appendedFiles)
            {
                yield return target;
            }
        }
    }

    public static void RegisterFileAppender(FileAppender appender)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        fileAppenders ??= [];
        fileAppenders.Add(appender);
    }
}

public partial class VerifySettings
{
    internal List<Target>? appendedFiles = [];

    public void AppendContentAsFile(string content, string extension = "txt", string? name = null)
    {
        appendedFiles ??= [];
        appendedFiles.Add(new(extension, content, name));
    }

    public void AppendContentAsFile(StringBuilder content, string extension = "txt", string? name = null)
    {
        appendedFiles ??= [];
        appendedFiles.Add(new(extension, content, name));
    }

    public void AppendContentAsFile(byte[] content, string extension = "txt", string? name = null)
    {
        appendedFiles ??= [];
        if (FileExtensions.IsTextExtension(extension))
        {
            appendedFiles.Add(new(extension, Encoding.UTF8.GetString(content), name));
        }
        else
        {
            appendedFiles.Add(new(extension, new MemoryStream(content), name));
        }
    }

    public void AppendFile(string file, string? name = null) =>
        AppendFile(IoHelpers.OpenRead(file), name);

    public void AppendFile(FileInfo file, string? name = null) =>
        AppendFile(file.FullName, name);

    public void AppendFile(FileStream stream, string? name = null) =>
        AppendFile(stream, stream.Extension(), name ?? Path.GetFileNameWithoutExtension(stream.Name));

    public void AppendFile(Stream stream, string extension = "txt", string? name = null)
    {
        stream.MoveToStart();
        appendedFiles ??= [];
        if (FileExtensions.IsTextExtension(extension))
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            appendedFiles.Add(new(extension, reader.ReadToEnd(), name));
        }
        else
        {
            appendedFiles.Add(new(extension, stream, name));
        }
    }
}

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.AppendContentAsFile(StringBuilder,string,string?)"/>
    [Pure]
    public SettingsTask AppendContentAsFile(StringBuilder content, string extension = "txt", string? name = null)
    {
        CurrentSettings.AppendContentAsFile(content, extension, name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendContentAsFile(string,string,string?)"/>
    [Pure]
    public SettingsTask AppendContentAsFile(string content, string extension = "txt", string? name = null)
    {
        CurrentSettings.AppendContentAsFile(content, extension, name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendContentAsFile(string,string,string?)"/>
    [Pure]
    public SettingsTask AppendContentAsFile(byte[] content, string extension = "txt", string? name = null)
    {
        CurrentSettings.AppendContentAsFile(content, extension, name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendFile(FileStream,string?)"/>
    [Pure]
    public SettingsTask AppendFile(FileStream stream, string? name = null)
    {
        CurrentSettings.AppendFile(stream, name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendFile(Stream,string,string?)"/>
    [Pure]
    public SettingsTask AppendFile(Stream stream, string extension = "txt", string? name = null)
    {
        CurrentSettings.AppendFile(stream, extension, name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendFile(string,string?)"/>
    [Pure]
    public SettingsTask AppendFile(string file, string? name = null)
    {
        CurrentSettings.AppendFile(file, name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendFile(FileInfo,string?)"/>
    [Pure]
    public SettingsTask AppendFile(FileInfo file, string? name = null)
    {
        CurrentSettings.AppendFile(file, name);
        return this;
    }
}