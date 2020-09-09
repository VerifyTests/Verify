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
            return Verify(stream, settings);
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

                await VerifyBinary(new List<ConversionStream> {new ConversionStream(settings.ExtensionOrBin(), stream)}, settings.ExtensionOrBin(), settings, null, null);
            }
        }

        async Task VerifyBinary(IEnumerable<ConversionStream> streams, string infoExtension, VerifySettings settings, object? info, Func<Task>? cleanup)
        {
            var engine = new VerifyEngine(infoExtension, settings, directory, testName, assembly);
            await VerifyInfo(engine, settings, info);

            var list = streams.Concat(VerifierSettings.GetFileAppenders(settings)).ToList();
            if (list.Count == 1)
            {
                var stream = list[0];
                var file = GetFileNames(stream.Extension, settings.Namer);
                await ProcessConversionStream(settings, file, stream, engine);
            }
            else
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var stream = list[index];
                    var file = GetFileNames(stream.Extension, settings.Namer, $"{index:D2}");
                    await ProcessConversionStream(settings, file, stream, engine);
                }
            }

            if (cleanup != null)
            {
                await cleanup.Invoke();
            }

            await engine.ThrowIfRequired();
        }

        static async Task ProcessConversionStream(VerifySettings settings, FilePair file, ConversionStream conversion, VerifyEngine engine)
        {
            EqualityResult result;
            var stream = conversion.Stream;
#if NETSTANDARD2_1
            await using (stream)
#else
            using (stream)
#endif
            {
                stream.MoveToStart();
                if (Extensions.IsText(conversion.Extension))
                {
                    var builder = await stream.ReadAsString();
                    ApplyScrubbers.Apply(builder, settings.instanceScrubbers);
                    result = await Comparer.Text(file, builder, settings);
                }
                else
                {
                    result = await Comparer.Streams(settings, stream, file);
                }
            }

            engine.HandleCompareResult(result, file);
        }

        async Task VerifyInfo(VerifyEngine engine, VerifySettings settings, object? info)
        {
            var appenders = VerifierSettings.GetJsonAppenders(settings).ToList();
            if (info == null)
            {
                if (appenders.Any())
                {
                    info = appenders.ToDictionary(x=>x.Name,x=>x.Data);
                }
            }
            else
            {
                if (appenders.Any())
                {
                    var dictionary = new Dictionary<string, object> {{"target", info}};
                    foreach (var appender in appenders)
                    {
                        dictionary.[appender.Name,appender.Data]
                    }
                    info = dictionary;
                }
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