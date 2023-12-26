namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<FileAppender> fileAppenders = [];

    internal static IEnumerable<Target> GetFileAppenders(VerifySettings settings)
    {
        foreach (var appender in fileAppenders)
        {
            var target = appender(settings.Context);
            if (target.HasValue)
            {
                yield return target.Value;
            }
        }

        foreach (var target in settings.appendedFiles)
        {
            yield return target;
        }
    }

    public static void RegisterFileAppender(FileAppender appender)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        fileAppenders.Add(appender);
    }
}

public partial class VerifySettings
{
    internal List<Target> appendedFiles = [];

    public void AppendContentAsFile(string content, string extension = "txt", string? name = null) =>
        appendedFiles.Add(new(extension, content, name));

    public void AppendContentAsFile(StringBuilder content, string extension = "txt", string? name = null) =>
        appendedFiles.Add(new(extension, content, name));

    public void AppendContentAsFile(byte[] content, string extension = "txt", string? name = null)
    {
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
    [Pure]
    public SettingsTask AppendContentAsFile(StringBuilder content, string extension = "txt", string? name = null)
    {
        CurrentSettings.AppendContentAsFile(content, extension, name);
        return this;
    }

    [Pure]
    public SettingsTask AppendContentAsFile(string content, string extension = "txt", string? name = null)
    {
        CurrentSettings.AppendContentAsFile(content, extension, name);
        return this;
    }

    [Pure]
    public SettingsTask AppendContentAsFile(byte[] content, string extension = "txt", string? name = null)
    {
        CurrentSettings.AppendContentAsFile(content, extension, name);
        return this;
    }

    [Pure]
    public SettingsTask AppendFile(FileStream stream, string? name = null)
    {
        CurrentSettings.AppendFile(stream, name);
        return this;
    }

    [Pure]
    public SettingsTask AppendFile(Stream stream, string extension = "txt", string? name = null)
    {
        CurrentSettings.AppendFile(stream, extension, name);
        return this;
    }

    [Pure]
    public SettingsTask AppendFile(string file, string? name = null)
    {
        CurrentSettings.AppendFile(file, name);
        return this;
    }

    [Pure]
    public SettingsTask AppendFile(FileInfo file, string? name = null)
    {
        CurrentSettings.AppendFile(file, name);
        return this;
    }
}