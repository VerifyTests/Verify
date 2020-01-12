using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Verify;

partial class InnerVerifier
{
    async Task VerifyBinary(IEnumerable<Stream> streams, VerifySettings settings)
    {
        var extension = settings.ExtensionOrBin();
        var innerVerifier = new VerifyEngine(
            extension,
            settings,
            testType,
            directory,
            testName);
        var list = streams.ToList();
        for (var index = 0; index < list.Count; index++)
        {
            var suffix = GetSuffix(list, index);

            var stream = list[index];
            var file = GetFileNames(extension, settings.Namer, suffix);
            var verifyResult = await StreamVerifier.VerifyStreams(stream, file);

            innerVerifier.HandleVerifyResult(verifyResult, file);
        }

        await innerVerifier.ThrowIfRequired();
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