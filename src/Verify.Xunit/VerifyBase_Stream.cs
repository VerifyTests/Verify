using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(Stream input)
        {
            return Verify(input, ".bin");
        }

        public async Task Verify(Stream input, string extension)
        {
            Guard.AgainstNull(input, nameof(input));

            var verifyResult = await Verify2(input, extension);

            if (verifyResult == VerifyResult.MissingVerified)
            {
                throw VerificationNotFoundException(extension);
            }

            if (verifyResult == VerifyResult.NotEqual)
            {
                throw new XunitException($"Streams not equal. {ExceptionHelpers.CommandHasBeenCopiedToTheClipboard}");
            }
        }

        public Task Verify(IEnumerable<Stream> streams)
        {
            return Verify(streams, ".bin");
        }

        async Task<VerifyResult> Verify2(Stream stream, string extension, string? suffix = null)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            try
            {
                var (receivedPath, verifiedPath) = GetFileNames(extension, suffix);
                FileHelpers.DeleteIfEmpty(verifiedPath);
                await FileHelpers.WriteStream(receivedPath, stream);
                if (!File.Exists(verifiedPath))
                {
                    await ClipboardCapture.Append(receivedPath, verifiedPath);
                    if (DiffRunner.FoundDiff)
                    {
                        FileHelpers.WriteEmpty(verifiedPath);
                        DiffRunner.Launch(receivedPath, verifiedPath);
                    }

                    return VerifyResult.MissingVerified;
                }

                if (!FileHelpers.FilesEqual(receivedPath, verifiedPath))
                {
                    await ClipboardCapture.Append(receivedPath, verifiedPath);
                    return VerifyResult.NotEqual;
                }

                File.Delete(receivedPath);
                return VerifyResult.Equal;
            }
            finally
            {
#if NETSTANDARD2_1
                await stream.DisposeAsync();
#else
                stream.Dispose();
#endif
            }
        }

        public enum VerifyResult
        {
            Equal,
            NotEqual,
            MissingVerified
        }

        public async Task Verify(IEnumerable<Stream> streams, string extension)
        {
            var missingVerified = new List<int>();
            var notEquals = new List<int>();
            var index = 0;
            foreach (var stream in streams)
            {
                var verifyResult = await Verify2(stream, extension, $"{index:D2}");

                if (verifyResult == VerifyResult.MissingVerified)
                {
                    missingVerified.Add(index);
                }

                if (verifyResult == VerifyResult.NotEqual)
                {
                    notEquals.Add(index);
                }

                index++;
            }

            if (missingVerified.Count == 0 && notEquals.Count == 0)
            {
                return;
            }

            var builder = new StringBuilder($"Streams do not match. {ExceptionHelpers.CommandHasBeenCopiedToTheClipboard}");
            builder.AppendLine();
            if (missingVerified.Any())
            {
                builder.AppendLine($"Streams not verified: {string.Join(", ", missingVerified)}");
            }

            if (notEquals.Any())
            {
                builder.AppendLine($"Streams with differences: {string.Join(", ", notEquals)}");
            }

            throw new XunitException(builder.ToString());
        }

        Exception VerificationNotFoundException(string extension)
        {
            return new XunitException($"First verification. {Context.UniqueTestName}.verified{extension} not found. Verification command has been copied to the clipboard.");
        }
    }
}