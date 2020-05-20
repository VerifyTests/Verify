using Bunit;
using Verify;
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
        SharedVerifySettings.RegisterFileConverter<IRenderedFragment>("html", FragmentToStream.Convert);
        SharedVerifySettings.ModifySerialization(settings =>
        {
            settings.AddExtraSettings(serializerSettings =>
            {
                var converters = serializerSettings.Converters;
                converters.Add(new RenderedFragmentConverter());
            });
        });
    }
}