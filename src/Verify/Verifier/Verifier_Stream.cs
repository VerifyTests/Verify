using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Verify;

partial class Verifier
{
    async Task VerifyBinary(Stream input, VerifySettings settings)
    {
        Guard.AgainstNull(input, nameof(input));
        try
        {
            await VerifyBinaryInner(input, settings);
        }
        finally
        {
            input.Dispose();
        }
    }

    async Task VerifyBinaryInner(Stream input, VerifySettings settings)
    {
        input.MoveToStart();

        var extension = settings.ExtensionOrBin();
        var file = GetFileNames(extension, settings.Namer);
        var verifyResult = await StreamVerifier.VerifyStreams(input, extension, file);

        if (verifyResult == VerifyResult.MissingVerified)
        {
            throw VerificationException(file.Verified);
        }

        if (verifyResult == VerifyResult.NotEqual)
        {
            throw VerificationException(notEqual: file.Received);
        }
    }

    async Task VerifyMultipleBinary(IEnumerable<Stream> streams, VerifySettings settings)
    {
        var extension = settings.ExtensionOrBin();
        var missingVerified = new List<string>();
        var notEquals = new List<string>();
        var index = 0;
        var verifiedPattern = GetVerifiedPattern(extension, settings.Namer);
        var verifiedFiles = Directory.EnumerateFiles(directory, verifiedPattern).ToList();
        foreach (var stream in streams)
        {
            try
            {
                stream.MoveToStart();
                var suffix = $"{index:D2}";
                var file = GetFileNames(extension, settings.Namer, suffix);
                var verifyResult = await StreamVerifier.VerifyStreams(stream, extension, file);

                verifiedFiles.Remove(file.Verified);
                if (verifyResult == VerifyResult.MissingVerified)
                {
                    missingVerified.Add(file.Verified);
                }

                if (verifyResult == VerifyResult.NotEqual)
                {
                    notEquals.Add(file.Received);
                }

                index++;
            }
            finally
            {
                stream.Dispose();
            }
        }

        foreach (var verifiedFile in verifiedFiles)
        {
            await ClipboardCapture.AppendDelete(verifiedFile);
        }

        if (missingVerified.Count == 0 &&
            notEquals.Count == 0 &&
            verifiedFiles.Count == 0)
        {
            return;
        }

        throw VerificationException(missingVerified, notEquals, verifiedFiles);
    }
}