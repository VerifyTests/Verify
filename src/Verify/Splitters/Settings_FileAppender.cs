namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<FileAppender> fileAppenders = new();

    internal static IEnumerable<Target> GetFileAppenders(VerifySettings settings)
    {
        foreach (var appender in fileAppenders)
        {
            var stream = appender(settings.Context);
            if (stream is not null)
            {
                yield return (Target)stream;
            }
        }
    }

    public static void RegisterFileAppender(FileAppender appender)
    {
        fileAppenders.Add(appender);
    }
}