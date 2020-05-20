using NUnit.Framework.Internal;

static class ModuleInitializer
{
    public static void Initialize()
    {
        InnerVerifier.Init(
            message => new NUnitException(message),
            input => CounterContext.Current.IntOrNext(input),
            input => CounterContext.Current.IntOrNext(input),
            input => CounterContext.Current.IntOrNext(input));
    }
}