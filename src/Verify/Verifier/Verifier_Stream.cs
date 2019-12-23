using System;
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
        var verifyResult = await StreamVerifier.VerifyStreams(input, file);

        Func<FilePair, Task> diff = pair =>
        {
            if (DiffTools.TryFindForExtension(file.Extension, out var diffTool))
            {
                if (EmptyFiles.TryWriteEmptyFile(file.Extension, file.Verified))
                {
                    DiffRunner.Launch(diffTool, file.Received, file.Verified);
                }
            }

            return Task.CompletedTask;
        };

        if (verifyResult == VerifyResult.MissingVerified)
        {
            throw await VerificationException(diff,file);
        }

        if (verifyResult == VerifyResult.NotEqual)
        {
            throw await VerificationException(diff, notEqual: file);
        }
    }

    async Task VerifyMultipleBinary(IEnumerable<Stream> streams, VerifySettings settings)
    {
        var extension = settings.ExtensionOrBin();
        var missingVerified = new List<FilePair>();
        var notEquals = new List<FilePair>();
        var index = 0;
        var verifiedPattern = GetVerifiedPattern(extension, settings.Namer);
        var verifiedFiles = Directory.EnumerateFiles(directory, verifiedPattern).ToList();

        var action = GetDiffAction(extension);

        foreach (var stream in streams)
        {
            try
            {
                stream.MoveToStart();
                var suffix = $"{index:D2}";
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

                index++;
            }
            finally
            {
                stream.Dispose();
            }
        }

        if (missingVerified.Count == 0 &&
            notEquals.Count == 0 &&
            verifiedFiles.Count == 0)
        {
            return;
        }

        throw await VerificationException(action,missingVerified, notEquals, verifiedFiles);
    }

    static Func<FilePair, Task> GetDiffAction(string extension)
    {
        if (!DiffTools.TryFindForExtension(extension, out var diffTool))
        {
            return pair => Task.CompletedTask;
        }

        return pair =>
        {
            if (EmptyFiles.TryWriteEmptyFile(extension, pair.Verified))
            {
                DiffRunner.Launch(diffTool, pair.Received, pair.Verified);
            }

            return Task.CompletedTask;
        };
    }
}