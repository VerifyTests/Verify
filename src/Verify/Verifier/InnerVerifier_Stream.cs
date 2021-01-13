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
            MemoryStream stream = new(target);
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
                        await VerifyBinary(result.Streams, result.Info, result.Cleanup);
                        return;
                    }
                }

                extension ??= "bin";

                List<ConversionStream> streams = new()
                {
                    new(extension, stream)
                };
                await VerifyBinary(streams, null, null);
            }
        }

        async Task VerifyBinary(IEnumerable<ConversionStream> streams, object? target, Func<Task>? cleanup)
        {
            VerifyEngine engine = new(settings, fileNameBuilder);

            var streamsList = streams.ToList();

            var appends = VerifierSettings.GetJsonAppenders(settings);
            if (target != null || appends.Any())
            {
                var extension = "txt";
                if (VerifierSettings.StrictJson)
                {
                    extension = "json";
                }

                var builder = JsonFormatter.AsJson(
                    target,
                    settings.serialization.currentSettings,
                    appends,
                    settings);

                ApplyScrubbers.Apply(builder, settings.instanceScrubbers);

                var received = builder.ToString();
                streamsList.Insert(0, new ConversionStream(extension, received));
            }

            streamsList.AddRange(VerifierSettings.GetFileAppenders(settings));
            var builders = streamsList
                .Select(
                    appender =>
                    {
                        return new ResultBuilder(
                            appender.Extension,
                            file => GetResult(settings, file, appender));
                    })
                .ToList();

            await engine.HandleResults(builders);

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
                if (EmptyFiles.Extensions.IsText(conversionStream.Extension))
                {
                    var builder = await stream.ReadAsString();
                    ApplyScrubbers.Apply(builder, settings.instanceScrubbers);
                    return await Comparer.Text(filePair, builder.ToString(), settings);
                }

                return await Comparer.Streams(settings, stream, filePair);
            }
        }
    }
}