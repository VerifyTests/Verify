namespace VerifyTests;

public partial class Counter
{
    static AsyncLocal<Counter?> local = new();

    public static Counter Current
    {
        get
        {
            var context = local.Value;
            if (context is null)
            {
                throw new("No current context");
            }

            return context;
        }
    }

    internal static Counter Start()
    {
        var context = new Counter();
        local.Value = context;
        return context;
    }

    internal static void Stop() =>
        local.Value = null;
}