using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task Verify(byte[] target)
        {
            Guard.AgainstNull(target, nameof(target));
            var stream = new MemoryStream(target);
            return VerifyStream(stream, settings.extension);
        }

        async Task VerifyStream(Stream stream, string? extension)
        {
#if NETSTANDARD2_0 || NETFRAMEWORK
            using (stream)
#else
            await using (stream)
#endif
            {
                if (extension != null)
                {
                    if (VerifierSettings.TryGetExtensionConverter(extension, out var conversion))
                    {
                        var result = await conversion(stream, settings.Context);
                        await VerifyBinary(result.Streams, extension, result.Info, result.Cleanup);
                        return;
                    }
                }

                extension ??= "bin";

                var streams = new List<ConversionStream>
                {
                    new(extension, stream)
                };
                await VerifyBinary(streams, extension, null, null);
            }
        }

        async Task VerifyBinary(IEnumerable<ConversionStream> streams, string infoExtension, object? info, Func<Task>? cleanup)
        {
            var engine = new VerifyEngine(infoExtension, settings, directory, testName, assembly);

            var builders = streams
                .Concat(VerifierSettings.GetFileAppenders(settings))
                .Select(appender =>
                {
                    return new ResultBuilder(
                        appender.Extension,
                        file => GetResult(settings, file, appender));
                })
                .ToList();

            await VerifyInfo(engine, info);

            await HandleResults(builders, engine);

            if (cleanup != null)
            {
                await cleanup.Invoke();
            }

            await engine.ThrowIfRequired();
        }

        static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair filePair, ConversionStream conversionStream)
        {
            var stream = conversionStream.Stream;
#if NETSTANDARD2_0 || NETFRAMEWORK
            using (stream)
#else
            await using (stream)
#endif
            {
                stream.MoveToStart();
                if (!EmptyFiles.Extensions.IsText(conversionStream.Extension))
                {
                    return await Comparer.Streams(settings, stream, filePair);
                }

                var builder = await stream.ReadAsString();
                ApplyScrubbers.Apply(builder, settings.instanceScrubbers);
                return await Comparer.Text(filePair, builder, settings);
            }
        }

        async Task VerifyInfo(VerifyEngine engine, object? info)
        {
            var appends = VerifierSettings.GetJsonAppenders(settings);
            if (info == null && !appends.Any())
            {
                return;
            }

            var file = GetFileNames("txt", settings.Namer, "info");

            var builder = JsonFormatter.AsJson(
                info,
                settings.serialization.currentSettings,
                appends,
                settings);

            ApplyScrubbers.Apply(builder, settings.instanceScrubbers);

            var result = await Comparer.Text(file, builder, settings);
            engine.HandleCompareResult(result, file);
        }
    }
}