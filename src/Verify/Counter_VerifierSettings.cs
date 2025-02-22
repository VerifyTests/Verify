namespace VerifyTests;

public partial class VerifierSettings
{
#if NET6_0_OR_GREATER

    public static void AddNamedDate(Date value, string name) =>
        Counter.AddNamed(value, name);

    public static void AddNamedTime(Time value, string name) =>
        Counter.AddNamed(value, name);

#endif

    public static void AddNamedDateTime(DateTime value, string name) =>
        Counter.AddNamed(value, name);

    public static void AddNamedGuid(Guid value, string name) =>
        Counter.AddNamed(value, name);

    public static void AddNamedDateTimeOffset(DateTimeOffset value, string name) =>
        Counter.AddNamed(value, name);
}