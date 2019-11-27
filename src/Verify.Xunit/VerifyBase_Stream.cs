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

            try
            {
                await InnerVerifyStream(input, extension);
            }
            finally
            {
                #if NETSTANDARD2_1
                await input.DisposeAsync();
                #else
                input.Dispose();
                #endif
            }
        }

        public Task Verify(IEnumerable<Stream> streams)
        {
            return Verify(streams, ".bin");
        }

        public async Task Verify(IEnumerable<Stream> streams, string extension)
        {
            var missingVerified = new List<int>();
            var notEquals = new List<int>();
            var index = 0;
            foreach (var stream in streams)
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
                try
                {
                    var (receivedPath, verifiedPath) = GetFileNames(extension, $"{index:D2}");
                    FileHelpers.DeleteIfEmpty(verifiedPath);
                    await FileHelpers.WriteStream(receivedPath, stream);
                    if (!File.Exists(verifiedPath))
                    {
                        ClipboardCapture.Append(receivedPath, verifiedPath);
                        missingVerified.Add(index);
                        if (DiffRunner.FoundDiff)
                        {
                            FileHelpers.WriteEmpty(verifiedPath);
                            DiffRunner.Launch(receivedPath, verifiedPath);
                        }

                        continue;
                    }

                    if (!FileHelpers.FilesEqual(receivedPath, verifiedPath))
                    {
                        notEquals.Add(index);
                        ClipboardCapture.Append(receivedPath, verifiedPath);
                    }
                }
                finally
                {
                    index++;
                    stream.Dispose();
                }
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

        async Task InnerVerifyStream(Stream stream, string extension)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
            var (receivedPath, verifiedPath) = GetFileNames(extension);
            FileHelpers.DeleteIfEmpty(verifiedPath);
            await FileHelpers.WriteStream(receivedPath, stream);
            FileHelpers.DeleteIfEmpty(verifiedPath);
            if (!File.Exists(verifiedPath))
            {
                ClipboardCapture.Append(receivedPath, verifiedPath);
                if (DiffRunner.FoundDiff)
                {
                    FileHelpers.WriteEmpty(verifiedPath);
                    DiffRunner.Launch(receivedPath, verifiedPath);
                }

                throw VerificationNotFoundException(extension);
            }

            if (FileHelpers.FilesEqual(receivedPath, verifiedPath))
            {
                return;
            }

            ClipboardCapture.Append(receivedPath, verifiedPath);
            throw new XunitException($"Streams not equal. {ExceptionHelpers.CommandHasBeenCopiedToTheClipboard}");
        }

        private Exception VerificationNotFoundException(string extension)
        {
            return new XunitException($"First verification. {Context.UniqueTestName}.verified{extension} not found. Verification command has been copied to the clipboard.");
        }
    }
}