using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmptyFiles;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task Verify(byte[] target, VerifySettings settings)
        {
            Guard.AgainstNull(target, nameof(target));
            var stream = new MemoryStream(target);
            return VerifyStream(settings, stream);
        }

        async Task VerifyStream(VerifySettings settings, Stream stream)
        {
#if NETSTANDARD2_1
            await using (stream)
#else
            using (stream)
#endif
            {
                if (settings.HasExtension())
                {
                    if (VerifierSettings.TryGetExtensionConverter(settings.extension!, out var conversion))
                    {
                        var result = await conversion(stream, settings);
                        await VerifyBinary(result.Streams, settings.ExtensionOrTxt(), settings, result.Info, result.Cleanup);
                        return;
                    }
                }

                var extension = settings.ExtensionOrBin();
                var streams = new List<ConversionStream>
                {
                    new ConversionStream(extension, stream)
                };
                await VerifyBinary(streams, extension, settings, null, null);
            }
        }

        async Task VerifyBinary(IEnumerable<ConversionStream> streams, string infoExtension, VerifySettings settings, object? info, Func<Task>? cleanup)
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

            await VerifyInfo(engine, settings, info);

            await HandleResults(settings, builders, engine);

            if (cleanup != null)
            {
                await cleanup.Invoke();
            }

            await engine.ThrowIfRequired();
        }

        static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair file, ConversionStream conversionStream)
        {
            var stream = conversionStream.Stream;
#if NETSTANDARD2_1
            await using (stream)
#else
            using (stream)
#endif
            {
                stream.MoveToStart();
                if (!Extensions.IsText(conversionStream.Extension))
                {
                    return await Comparer.Streams(settings, stream, file);
                }

                var builder = await stream.ReadAsString();
                ApplyScrubbers.Apply(builder, settings.instanceScrubbers);
                return await Comparer.Text(file, builder, settings);
            }
        }

        async Task VerifyInfo(VerifyEngine engine, VerifySettings settings, object? info)
        {
            if (info == null)
            {
                return;
            }

            var file = GetFileNames("txt", settings.Namer, "info");

            var builder = JsonFormatter.AsJson(
                info,
                settings.serialization.currentSettings,
                settings.IsNewLineEscapingDisabled);

            ApplyScrubbers.Apply(builder, settings.instanceScrubbers);

            var result = await Comparer.Text(file, builder, settings);
            engine.HandleCompareResult(result, file);
        }
    }
}