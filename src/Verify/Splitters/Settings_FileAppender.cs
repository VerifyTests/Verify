namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<FileAppender> fileAppenders = new();

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
    internal List<Target> appendedFiles = new();

    public void AppendFile(string content, string extension = "txt") =>
        appendedFiles.Add(new(extension, content));

    public void AppendFile(StringBuilder content, string extension = "txt") =>
        appendedFiles.Add(new(extension, content));

    public void AppendFile(byte[] content, string extension = "txt")
    {
        if (FileExtensions.IsText(extension))
        {
            appendedFiles.Add(new(extension, Encoding.UTF8.GetString(content)));
        }
        else
        {
            appendedFiles.Add(new(extension, new MemoryStream(content)));
        }
    }

    public void AppendFile(FileStream stream) =>
        AppendFile(stream, stream.Extension());

    public void AppendFile(Stream stream, string extension = "txt")
    {
        stream.MoveToStart();
        if (FileExtensions.IsText(extension))
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            appendedFiles.Add(new(extension, reader.ReadToEnd()));
        }
        else
        {
            appendedFiles.Add(new(extension, stream));
        }
    }
}

public partial class SettingsTask
{
    public SettingsTask AppendFile(StringBuilder content, string extension = "txt")
    {
        CurrentSettings.AppendFile(content, extension);
        return this;
    }

    public SettingsTask AppendFile(string content, string extension = "txt")
    {
        CurrentSettings.AppendFile(content, extension);
        return this;
    }

    public SettingsTask AppendFile(byte[] content, string extension = "txt")
    {
        CurrentSettings.AppendFile(content, extension);
        return this;
    }

    public SettingsTask AppendFile(FileStream stream)
    {
        CurrentSettings.AppendFile(stream);
        return this;
    }

    public SettingsTask AppendFile(Stream stream, string extension = "txt")
    {
        CurrentSettings.AppendFile(stream);
        return this;
    }
}