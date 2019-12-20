using System.Collections.Generic;
using System.IO;
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
        var (receivedPath, verifiedPath) = GetFileNames(extension, settings.Namer);
        var verifyResult = await StreamVerifier.VerifyStreams(input, extension, receivedPath, verifiedPath);

        if (verifyResult == VerifyResult.MissingVerified)
        {
            throw VerificationException(verifiedPath);
        }

        if (verifyResult == VerifyResult.NotEqual)
        {
            throw VerificationException(notEqual:receivedPath);
        }
    }

    async Task VerifyMultipleBinary(IEnumerable<Stream> streams, VerifySettings settings)
    {
        var extension = settings.ExtensionOrBin();
        var missingVerified = new List<string>();
        var notEquals = new List<string>();
        var index = 0;
        foreach (var stream in streams)
        {
            try
            {
                stream.MoveToStart();
                var suffix = $"{index:D2}";
                var (receivedPath, verifiedPath) = GetFileNames(extension, settings.Namer, suffix);
                var verifyResult = await StreamVerifier.VerifyStreams(stream, extension, receivedPath, verifiedPath);

                if (verifyResult == VerifyResult.MissingVerified)
                {
                    missingVerified.Add(verifiedPath);
                }

                if (verifyResult == VerifyResult.NotEqual)
                {
                    notEquals.Add(receivedPath);
                }

                index++;
            }
            finally
            {
                stream.Dispose();
            }
        }

        if (missingVerified.Count == 0 && notEquals.Count == 0)
        {
            return;
        }

        throw VerificationException(missingVerified, notEquals);
    }
}