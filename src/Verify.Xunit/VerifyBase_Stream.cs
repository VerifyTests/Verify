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
            return Verify(input, "bin");
        }

        public async Task Verify(Stream input, string extension)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            Guard.AgainstNull(input, nameof(input));

            var verifyResult = await InnerVerify(input, extension);

            if (verifyResult == VerifyResult.MissingVerified)
            {
                throw VerificationNotFoundException(extension);
            }

            if (verifyResult == VerifyResult.NotEqual)
            {
                var builder = new StringBuilder("Streams do not match.");
                builder.AppendLine();
                if (!BuildServerDetector.Detected)
                {
                    builder.AppendLine(ExceptionHelpers.CommandHasBeenCopiedToTheClipboard);
                }

                throw new XunitException(builder.ToString());
            }
        }

        public Task Verify(IEnumerable<Stream> streams)
        {
            return Verify(streams, "bin");
        }

        VerifyResult DoCompare(string receivedPath, string verifiedPath, string extension)
        {
            if (!File.Exists(verifiedPath))
            {
                return VerifyResult.MissingVerified;
            }

            if (EmptyFiles.IsEmptyFile(extension, verifiedPath))
            {
                return VerifyResult.NotEqual;
            }

            if (!FileHelpers.FilesEqual(receivedPath, verifiedPath))
            {
                return VerifyResult.NotEqual;
            }

            return VerifyResult.Equal;

        }

        async Task<VerifyResult> InnerVerify(Stream stream, string extension, string? suffix = null)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            try
            {
                var (receivedPath, verifiedPath) = GetFileNames(extension, suffix);
                await FileHelpers.WriteStream(receivedPath, stream);

                var verifyResult = DoCompare(receivedPath, verifiedPath, extension);

                if (verifyResult == VerifyResult.Equal)
                {
                    File.Delete(receivedPath);
                    return verifyResult;
                }

                if (!BuildServerDetector.Detected)
                {
                    if (DiffTools.TryFindForExtension(extension, out var diffTool))
                    {
                        if (EmptyFiles.TryWriteEmptyFile(extension, verifiedPath))
                        {
                            DiffRunner.Launch(diffTool, receivedPath, verifiedPath);
                        }
                    }

                    await ClipboardCapture.Append(receivedPath, verifiedPath);
                }

                return verifyResult;
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

        public async Task Verify(IEnumerable<Stream> streams, string extension)
        {
            var missingVerified = new List<int>();
            var notEquals = new List<int>();
            var index = 0;
            foreach (var stream in streams)
            {
                var verifyResult = await InnerVerify(stream, extension, $"{index:D2}");

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

            var builder = new StringBuilder("Streams do not match.");
            builder.AppendLine();
            if (!BuildServerDetector.Detected)
            {
                builder.AppendLine(ExceptionHelpers.CommandHasBeenCopiedToTheClipboard);
            }

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
            return new XunitException($"First verification. {Context.UniqueTestName}.verified.{extension} not found. Verification command has been copied to the clipboard.");
        }
    }
}