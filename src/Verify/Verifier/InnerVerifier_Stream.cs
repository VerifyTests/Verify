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
        public Task Verify(byte[] target, VerifySettings? settings = null)
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
                    if (VerifierSettings.TryGetConverter(settings.extension!, out var converter))
                    {
                        var converterSettings = new VerifySettings(settings);
                        converterSettings.UseExtension(converter.ToExtension);
                        var result = await converter.Conversion(stream, converterSettings);
                        await VerifyBinary(result.Streams, converterSettings, result.Info, result.Cleanup);
                        return;
                    }
                }

                await VerifyBinary(new List<Stream> {stream}, settings, null, null);
            }
        }

        async Task VerifyBinary(IEnumerable<Stream> streams, VerifySettings settings, object? info, Func<Task>? cleanup)
        {
            var extension = settings.ExtensionOrBin();
            var engine = new VerifyEngine(
                extension,
                settings,
                directory,
                testName,
                assembly);
            var list = streams.ToList();
            await VerifyInfo(engine, settings, info);
            for (var index = 0; index < list.Count; index++)
            {
                var file = GetFileForIndex(settings, list, index, extension);

                var result = await GetResult(settings, file, list[index]);

                engine.HandleCompareResult(result, file);
            }

            if (cleanup != null)
            {
                await cleanup.Invoke();
            }
            await engine.ThrowIfRequired();
        }

        static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair file, Stream stream)
        {
#if NETSTANDARD2_1
        await using (stream)
#else
            using (stream)
#endif
            {
                stream.MoveToStart();
                if (!Extensions.IsText(file.Extension))
                {
                    return await Comparer.Streams(settings, stream, file);
                }

                var builder = await stream.ReadAsString();
                ApplyScrubbers.Apply(builder, settings.instanceScrubbers);
                return await Comparer.Text(file, builder, settings);
            }
        }

        FilePair GetFileForIndex(VerifySettings settings, List<Stream> list, int index, string extension)
        {
            if (list.Count > 1)
            {
                return GetFileNames(extension, settings.Namer, $"{index:D2}");
            }

            return GetFileNames(extension, settings.Namer);
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