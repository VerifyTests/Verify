namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<JsonAppender>? jsonAppenders = [];

    internal static List<ToAppend> GetJsonAppenders(VerifySettings settings)
    {
        var list = new List<ToAppend>();

        if (Recording.TryStop(out var recorded))
        {
            list.AddRange(recorded);
        }

        if (jsonAppenders != null)
        {
            foreach (var appender in jsonAppenders)
            {
                var data = appender(settings.Context);
                if (data is not null)
                {
                    var append = data.Value;
                    list.Add(append);
                }
            }
        }

        if (settings.Appends != null)
        {
            list.AddRange(settings.Appends);
        }

        return list;
    }

    public static void RegisterJsonAppender(JsonAppender appender)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        jsonAppenders ??= [];
        jsonAppenders.Add(appender);
    }
}