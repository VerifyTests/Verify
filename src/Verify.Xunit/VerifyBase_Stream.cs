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
        #region VerifyBinary
        public async Task VerifyBinary(Stream input, string extension = "bin")
        #endregion
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

        Task<VerifyResult> InnerVerify(Stream stream, string extension, string? suffix = null)
        {
            var (receivedPath, verifiedPath) = GetFileNames(extension, suffix);
            return StreamVerifier.VerifyStreams(stream, extension, receivedPath, verifiedPath);
        }

        public async Task VerifyBinary(IEnumerable<Stream> streams, string extension = "bin")
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
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
            if (BuildServerDetector.Detected)
            {
                return new XunitException($"First verification. {Context.UniqueTestName}.verified.{extension} not found.");
            }

            return new XunitException($"First verification. {Context.UniqueTestName}.verified.{extension} not found. Verification command has been copied to the clipboard.");
        }
    }
}