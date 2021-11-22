namespace VerifyTests;

public static partial class VerifierSettings
{
    static List<JsonAppender> jsonAppenders = new();

    internal static List<ToAppend> GetJsonAppenders(VerifySettings settings)
    {
        var list = new List<ToAppend>();
        foreach (var appender in jsonAppenders)
        {
            var data = appender(settings.Context);
            if (data is not null)
            {
                list.Add(data.Value);
            }
        }

        list.AddRange(settings.Appends);

        return list;
    }

    public static void RegisterJsonAppender(JsonAppender appender)
    {
        jsonAppenders.Add(appender);
    }
}