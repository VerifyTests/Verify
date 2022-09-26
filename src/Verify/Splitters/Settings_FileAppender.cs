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
    }

    public static void RegisterFileAppender(FileAppender appender) =>
        fileAppenders.Add(appender);
}