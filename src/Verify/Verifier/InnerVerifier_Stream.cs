using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task Verify(byte[] target)
        {
            Guard.AgainstNull(target, nameof(target));
            MemoryStream stream = new(target);
            return VerifyStream(stream);
        }

        async Task VerifyStream(Stream stream)
        {
            var extension = settings.extension;
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
                        await VerifyInner(result.Info, result.Cleanup, result.Streams);
                        return;
                    }
                }

                extension ??= "bin";

                List<ConversionStream> streams = new()
                {
                    new(extension, stream)
                };
                await VerifyInner(null, null, streams);
            }
        }

        async Task VerifyInner(object? target, Func<Task>? cleanup, IEnumerable<ConversionStream> streams)
        {
            VerifyEngine engine = new(settings, fileNameBuilder);

            var streamsList = streams.ToList();

            if (TryGetTargetBuilder(target, out var builder, out var extension))
            {
                ApplyScrubbers.Apply(builder, settings.instanceScrubbers);

                var received = builder.ToString();
                var stream = new ConversionStream(extension, received);
                streamsList.Insert(0, stream);
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

        bool TryGetTargetBuilder(object? target, [NotNullWhen(true)] out StringBuilder? builder, [NotNullWhen(true)] out string? extension)
        {
            var appends = VerifierSettings.GetJsonAppenders(settings);

            var hasAppends = appends.Any();

            if (target == null)
            {
                if (!hasAppends)
                {
                    builder = null;
                    extension = null;
                    return false;
                }

                extension = "txt";
                if (VerifierSettings.StrictJson)
                {
                    extension = "json";
                }

                builder = JsonFormatter.AsJson(
                    null,
                    settings.serialization.currentSettings,
                    appends,
                    settings);
                return true;
            }

            if (!hasAppends && target is string stringTarget)
            {
                builder = new StringBuilder(stringTarget);
                builder.FixNewlines();
                extension = settings.ExtensionOrTxt();
                return true;
            }

            extension = "txt";

            if (VerifierSettings.StrictJson)
            {
                extension = "json";
            }

            builder = JsonFormatter.AsJson(
                target,
                settings.serialization.currentSettings,
                appends,
                settings);

            return true;
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