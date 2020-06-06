using Xunit.Sdk;

static class ModuleInitializer
{
    public static void Initialize()
    {
        InnerVerifier.Init(
            message => new XunitException(message),
            input => CounterContext.Current.IntOrNext(input),
            input => CounterContext.Current.IntOrNext(input),
            input => CounterContext.Current.IntOrNext(input));
    }
}