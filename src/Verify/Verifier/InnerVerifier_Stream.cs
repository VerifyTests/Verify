using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Verify;

partial class InnerVerifier
{
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
            var suffix = GetSuffix(list, index);

            var stream = list[index];
            var file = GetFileNames(extension, settings.Namer, suffix);
            var result = await Comparer.Streams(stream, file);

            engine.HandleCompareResult(result, file);
        }

        await engine.ThrowIfRequired();
    }

    async Task VerifyInfo(VerifyEngine engine, VerifySettings settings, object? info)
    {
        if (info == null)
        {
            return;
        }

        var file = GetFileNames("txt", settings.Namer, "info");

        var formatJson = JsonFormatter.AsJson(info, settings.serialization.currentSettings);

        var scrubbedInput = ScrubInput(formatJson, settings);

        var result = await Comparer.Text(file, scrubbedInput);
        engine.HandleCompareResult(result, file);
    }

    static string? GetSuffix(List<Stream> list, int index)
    {
        if (list.Count > 1)
        {
            return $"{index:D2}";
        }

        return null;
    }
}