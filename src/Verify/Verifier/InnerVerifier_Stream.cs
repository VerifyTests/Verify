using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmptyFiles;
using Verify;

partial class InnerVerifier
{
    public Task Verify(byte[] input, VerifySettings? settings = null)
    {
        Guard.AgainstNull(input, nameof(input));
        var stream = new MemoryStream(input);
        return Verify(stream, settings);
    }

    async Task VerifyStream(VerifySettings settings, Stream stream)
    {
        using (stream)
        {
            if (settings.HasExtension())
            {
                if (SharedVerifySettings.TryGetConverter(settings.extension!, out var converter))
                {
                    var converterSettings = new VerifySettings(settings);
                    converterSettings.UseExtension(converter.ToExtension);
                    var result = await converter.Func(stream, converterSettings);
                    await VerifyBinary(result.Streams, converterSettings, result.Info);
                    return;
                }
            }

            await VerifyBinary(new List<Stream> {stream}, settings, null);
        }
    }

    async Task VerifyBinary(IEnumerable<Stream> streams, VerifySettings settings, object? info)
    {
        var extension = settings.ExtensionOrBin();
        var engine = new VerifyEngine(
            extension,
            settings,
            testType,
            directory,
            testName);
        var list = streams.ToList();
        await VerifyInfo(engine, settings, info);
        for (var index = 0; index < list.Count; index++)
        {
            var file = GetFileForIndex(settings, list, index, extension);
            var stream = list[index];
            stream.MoveToStart();

            var result = await GetResult(settings, file, stream);

            engine.HandleCompareResult(result, file);
        }

        await engine.ThrowIfRequired();
    }

     static async Task<EqualityResult> GetResult(VerifySettings settings, FilePair file, Stream stream)
    {
        if (!Extensions.IsText(file.Extension))
        {
            return await Comparer.Streams(settings, stream, file);
        }
        var builder = await stream.ReadAsString();
        ApplyScrubbers.Apply(builder, settings.instanceScrubbers);
        return await Comparer.Text(file, builder, settings);
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

        var builder = JsonFormatter.AsJson(info, settings.serialization.currentSettings);

        ApplyScrubbers.Apply(builder, settings.instanceScrubbers);

        var result = await Comparer.Text(file, builder, settings);
        engine.HandleCompareResult(result, file);
    }
}