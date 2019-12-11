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

            var (receivedPath, verifiedPath) = GetFileNames(extension);
            var verifyResult = await StreamVerifier.VerifyStreams(input, extension, receivedPath, verifiedPath);

            if (verifyResult == VerifyResult.MissingVerified)
            {
                throw VerificationNotFoundException(verifiedPath);
            }

            if (verifyResult == VerifyResult.NotEqual)
            {
                var builder = new StringBuilder("Streams do not match.");
                builder.AppendLine();
                if (!BuildServerDetector.Detected)
                {
                    builder.AppendLine("Verification command has been copied to the clipboard.");
                }

                throw new XunitException(builder.ToString());
            }
        }

        public async Task VerifyBinary(IEnumerable<Stream> streams, string extension = "bin")
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            var missingVerified = new List<int>();
            var notEquals = new List<int>();
            var index = 0;
            foreach (var stream in streams)
            {
                var suffix = $"{index:D2}";
                var (receivedPath, verifiedPath) = GetFileNames(extension, suffix);
                var verifyResult = await StreamVerifier.VerifyStreams(stream, extension, receivedPath, verifiedPath);

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
                builder.AppendLine("Verification command has been copied to the clipboard.");
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

        Exception VerificationNotFoundException(string verifiedPath)
        {
            var verifiedFile = Path.GetFileName(verifiedPath);
            if (BuildServerDetector.Detected)
            {
                return new XunitException($"First verification. {verifiedFile} not found.");
            }

            return new XunitException($"First verification. {verifiedFile} not found. Verification command has been copied to the clipboard.");
        }
    }
}