using Microsoft.VisualStudio.TestTools.UnitTesting;

static class ModuleInitializer
{
    public static void Initialize()
    {
        InnerVerifier.Init(
            message => new AssertFailedException(message),
            input => CounterContext.Current.IntOrNext(input),
            input => CounterContext.Current.IntOrNext(input),
            input => CounterContext.Current.IntOrNext(input));
    }
}