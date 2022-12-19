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

    public void AppendTextFile(string content, string extension = "txt") =>
        appendedFiles.Add(new(extension, content));

    public void AppendTextFile(StringBuilder content, string extension = "txt") =>
        appendedFiles.Add(new(extension, content));

    public void AppendTextFile(byte[] content, string extension = "txt") =>
        appendedFiles.Add(new(extension, Encoding.UTF8.GetString(content)));

    public void AppendTextFile(Stream content, string extension = "txt")
    {
        content.MoveToStart();
        using var reader = new StreamReader(content, Encoding.UTF8);
        appendedFiles.Add(new(extension, reader.ReadToEnd()));
    }

    public void AppendTextFile(FileStream content)
    {
        content.MoveToStart();
        using var reader = new StreamReader(content, Encoding.UTF8);
        AppendTextFile(content, content.Extension());
    }

    public void AppendFile(string extension, byte[] content) =>
        AppendFile(extension, new MemoryStream(content));

    public void AppendFile(FileStream content) =>
        AppendFile(content.Extension(), content);

    public void AppendFile(string extension, Stream content) =>
        appendedFiles.Add(new(extension, content));
}

public partial class SettingsTask
{
    public SettingsTask AppendTextFile(StringBuilder content, string extension = "txt")
    {
        CurrentSettings.AppendTextFile(content, extension);
        return this;
    }

    public SettingsTask AppendTextFile(string content, string extension = "txt")
    {
        CurrentSettings.AppendTextFile(content, extension);
        return this;
    }

    public SettingsTask AppendTextFile(byte[] content, string extension = "txt")
    {
        CurrentSettings.AppendTextFile(content, extension);
        return this;
    }

    public SettingsTask AppendTextFile(Stream content, string extension = "txt")
    {
        CurrentSettings.AppendTextFile(content, extension);
        return this;
    }

    public SettingsTask AppendFile(string extension, byte[] content)
    {
        CurrentSettings.AppendFile(extension, content);
        return this;
    }

    public SettingsTask AppendFile(FileStream content)
    {
        CurrentSettings.AppendFile(content);
        return this;
    }

    public SettingsTask AppendFile(string extension, Stream content)
    {
        CurrentSettings.AppendFile(extension, content);
        return this;
    }
}