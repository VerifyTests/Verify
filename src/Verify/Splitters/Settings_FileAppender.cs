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
            foreach (var buildTarget in settings.appendedFiles)
            {
                yield return buildTarget();
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
    /// <summary>
    /// Built once per verification rather than held as Targets. The engine disposes the
    /// stream of every target it writes, so a stored stream is dead after the first
    /// verification, and settings are reused: SettingsTask copies them per Verify call.
    /// </summary>
    internal List<Func<Target>>? appendedFiles;

    public void AppendContentAsFile(string content, string extension = "txt", string? name = null)
    {
        appendedFiles ??= [];
        appendedFiles.Add(() => new(extension, content, name));
    }

    public void AppendContentAsFile(StringBuilder content, string extension = "txt", string? name = null)
    {
        appendedFiles ??= [];
        appendedFiles.Add(() => new(extension, content, name));
    }

    public void AppendContentAsFile(byte[] content, string extension = "txt", string? name = null)
    {
        appendedFiles ??= [];
        if (FileExtensions.IsTextExtension(extension))
        {
            var text = Encoding.UTF8.GetString(content);
            appendedFiles.Add(() => new(extension, text, name));
        }
        else
        {
            // A fresh stream per verification: the bytes stay re-readable, so reusing the
            // settings for a second Verify works.
            appendedFiles.Add(() => new(extension, new MemoryStream(content), name));
        }
    }

    public void AppendFile(string file, string? name = null)
    {
        // Opened per verification rather than held open from here, for the same reason,
        // and so the handle is not held for the lifetime of the settings.
        Guards.FileExists(file);
        var extension = Path.GetExtension(file);
        extension = extension.Length == 0 ? "noextension" : extension[1..];
        AppendFile(() => IoHelpers.OpenRead(file), extension, name ?? Path.GetFileNameWithoutExtension(file));
    }

    public void AppendFile(FileInfo file, string? name = null) =>
        AppendFile(file.FullName, name);

    public void AppendFile(FileStream stream, string? name = null) =>
        AppendFile(stream, stream.Extension(), name ?? Path.GetFileNameWithoutExtension(stream.Name));

    /// <remarks>
    /// The stream is owned by the caller and can only be read once, so unlike the other
    /// overloads this one cannot be replayed for a second verification with the same
    /// settings. Use <see cref="AppendFile(string,string?)" /> or
    /// <see cref="AppendContentAsFile(byte[],string,string?)" /> where that matters.
    /// </remarks>
    public void AppendFile(Stream stream, string extension = "txt", string? name = null)
    {
        stream.MoveToStart();
        appendedFiles ??= [];
        if (FileExtensions.IsTextExtension(extension))
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var text = reader.ReadToEnd();
            appendedFiles.Add(() => new(extension, text, name));
        }
        else
        {
            appendedFiles.Add(() => new(extension, stream, name));
        }
    }

    void AppendFile(Func<Stream> openStream, string extension, string? name)
    {
        appendedFiles ??= [];
        if (FileExtensions.IsTextExtension(extension))
        {
            using var stream = openStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var text = reader.ReadToEnd();
            appendedFiles.Add(() => new(extension, text, name));
        }
        else
        {
            appendedFiles.Add(() => new(extension, openStream(), name));
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