using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Verify;

partial class Verifier
{
    async Task VerifyBinary(IEnumerable<Stream> streams, VerifySettings settings)
    {
        var extension = settings.ExtensionOrBin();
        var missingVerified = new List<FilePair>();
        var notEquals = new List<FilePair>();
        var verifiedPattern = GetVerifiedPattern(extension, settings.Namer);
        var innerVerifier = new InnerVerifier(extension,Directory.EnumerateFiles(directory, verifiedPattern));
        var list = streams.ToList();
        for (var index = 0; index < list.Count; index++)
        {
            var suffix = GetSuffix(list, index);

            var stream = list[index];
            var file = GetFileNames(extension, settings.Namer, suffix);
            var verifyResult = await StreamVerifier.VerifyStreams(stream, file);

            if (verifyResult == VerifyResult.MissingVerified)
            {
                innerVerifier.AddMissing(file);
            }

            if (verifyResult == VerifyResult.NotEqual)
            {
                notEquals.Add(file);
            }
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