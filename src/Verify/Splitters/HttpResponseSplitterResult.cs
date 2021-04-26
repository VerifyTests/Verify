using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using VerifyTests;

static class HttpResponseSplitterResult
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.RegisterFileConverter<HttpResponseMessage>(
            (instance, _) => Convert(instance));
    }

    static ConversionResult Convert(HttpResponseMessage instance)
    {
        var targets = new List<Target>();
        var content = instance.Content;
        if (content.TryGetExtension(out var extension))
        {
            if (EmptyFiles.Extensions.IsText(extension))
            {
                return new ConversionResult(instance, targets);
            }

            targets.Add(new Target(extension, content.ReadAsStream()));
        }

        return new ConversionResult(
            new
            {
                instance.Version,
                instance.StatusCode,
                instance.IsSuccessStatusCode,
                instance.ReasonPhrase,
                instance.Headers,
#if NET5_0 || NETSTANDARD2_1
                instance.TrailingHeaders,
#endif
                instance.RequestMessage
            },
            targets);
    }
}