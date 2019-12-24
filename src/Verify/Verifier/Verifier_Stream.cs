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
        var verifiedFiles = Directory.EnumerateFiles(directory, verifiedPattern).ToList();

        var list = streams.ToList();
        for (var index = 0; index < list.Count; index++)
        {
            var suffix = GetSuffix(list, index);

            var stream = list[index];
            var file = GetFileNames(extension, settings.Namer, suffix);
            var verifyResult = await StreamVerifier.VerifyStreams(stream, file);

            verifiedFiles.Remove(file.Verified);
            if (verifyResult == VerifyResult.MissingVerified)
            {
                missingVerified.Add(file);
            }

            if (verifyResult == VerifyResult.NotEqual)
            {
                notEquals.Add(file);
            }
        }

        if (missingVerified.Count == 0 &&
            notEquals.Count == 0 &&
            verifiedFiles.Count == 0)
        {
            return;
        }

        throw await VerificationException(missingVerified, notEquals, verifiedFiles);
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