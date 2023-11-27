namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<JsonAppender> jsonAppenders = [];

    internal static List<ToAppend> GetJsonAppenders(VerifySettings settings)
    {
        var list = new List<ToAppend>();

        if (Recording.TryStop(out var recorded))
        {
            foreach (var append in recorded)
            {
                if (!settings.serialization.ShouldIgnoreByName(append.Name))
                {
                    list.Add(append);
                }
            }
        }

        foreach (var appender in jsonAppenders)
        {
            var data = appender(settings.Context);
            if (data is not null)
            {
                var append = data.Value;
                if (!settings.serialization.ShouldIgnoreByName(append.Name))
                {
                    list.Add(append);
                }
            }
        }

        foreach (var append in settings.Appends)
        {
            if (!settings.serialization.ShouldIgnoreByName(append.Name))
            {
                list.Add(append);
            }
        }

        return list;
    }

    public static void RegisterJsonAppender(JsonAppender appender)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        jsonAppenders.Add(appender);
    }
}