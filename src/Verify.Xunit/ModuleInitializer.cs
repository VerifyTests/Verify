using Xunit;
using Xunit.Sdk;

static class ModuleInitializer
{
    public static void Initialize()
    {
        InnerVerifier.Init(
            message => new XunitException(message),
            input => XunitContext.Context.IntOrNext(input),
            input => XunitContext.Context.IntOrNext(input),
            input => XunitContext.Context.IntOrNext(input));
    }
}